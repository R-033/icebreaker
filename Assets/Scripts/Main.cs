using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Common;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ELFSharp.ELF;
using ELFSharp.ELF.Sections;
using RuntimeGizmos;
using UnityEngine.EventSystems;
using World.Culling;

public class Main : MonoBehaviour
{
    public Text OpenedFileName;
    public Text HelpText;
    public Text VersionText;
    public GameObject[] HiddenElements;
    public GameObject[] HiddenElementsNoGame;
    public GameObject[] EnableOnStart;
    public GameObject[] DisableOnStart;
    public GameObject[] HiddenWhenNoEditingAllowed;
    public GameObject[] HiddenWhenNoPlayingAllowed;
    public GameObject[] HiddenWhenNoNIS;
    public GameObject[] HiddenWhenNoCamera;
    public RectTransform NisList;
    public RectTransform NisList2;
    public GameObject NisListPrefab;
    public GameObject ObjectListPrefab;
    public RectTransform Objectlist;
    public RectTransform Objectlist2;
    public ToggleGroup ObjectListGroup;
    public ToggleGroup ObjectListGroup2;
    public RectTransform SubObjectlist;
    public ToggleGroup SubObjectListGroup;
    public Toggle PreviewToggle;
    public static string GameDirectory;
    public Text SavedText;
    public Text PlayButtonText;
    public ToggleGroup tabgroup;
    public GameObject CarPrefabPlayer;
    public GameObject CarPrefabOpponent;
    public GameObject CarPrefabCop;
    public GameObject FourByThree;
    public Dictionary<string, Transform> ObjectsOnScene = new Dictionary<string, Transform>();
    public Transform SceneRoot;
    private bool updlimit;
    [HideInInspector] public int tabnum;
    public Dropdown CameraTrackSelection;
    public Text CameraTrackPropertiesName;

    public Text[] NISProps;
    public Text[] CamProps;
    public Text segmenttitle;

    public Text TimeText;

    public Text CoordText;

    public Image TabBG;
    public CanvasGroup group;

    public GameObject FloorGrid;

    public InputField NewCameraTrackName;
    public InputField NewCameraTrackBytes;

    private bool _playing;
    [HideInInspector] public int curcam;

    public MaxCamera editorCameraMovement;

    public Texture2D[] Cursors;

    public TransformGizmo gizmo;
    public TransformGizmo gizmo2;

    public Transform LineRenderers;
    public Transform FocusSphere;

    public Text OverlayDebug;

    public GameObject AboutPage;

    private static int curs;

    public static int CursorType
    {
        get => curs;
        set
        {
            curs = value;
            try
            {
                Cursor.SetCursor(FindObjectOfType<Main>().Cursors[value], new Vector2(10f, 10f), CursorMode.Auto);
            }
            catch
            {
            }

            ;
        }
    }

    public bool playing
    {
        get { return _playing; }
        set
        {
            _playing = value;
            PlayButtonText.text = _playing ? "Pause" : "Play";
        }
    }

    public float timeline;
    public Slider[] PreviewTimeline;

    public Camera EditorCamera;
    public Camera SceneCamera;

    public RectTransform CameraTrackEntries;
    public GameObject CTEntryPrefab;

    public static Transform _LogMessageParent;
    public static GameObject _LogMessagePrefab;

    public Transform LogMessageParent;
    public GameObject LogMessagePrefab;

    static void HandleException(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            string err = condition.Split('\n')[0];
            if (!LogMessage.shown.Contains(err))
            {
                GameObject obj = Instantiate(_LogMessagePrefab, _LogMessageParent);
                obj.transform.GetChild(0).GetComponent<Text>().text = err;
                obj.transform.GetComponent<LogMessage>().message = err;
                LogMessage.shown.Add(err);
                switch (LoggingMode)
                {
                    case 1:
                        if (!NISSaveDisabled)
                        {
                            NISSaveDisabled = true;
                            obj = Instantiate(_LogMessagePrefab, _LogMessageParent);
                            obj.transform.GetChild(0).GetComponent<Text>().text = "It seems like this NIS is not fully supported by icebreaker. Saving is disabled.";
                            obj.transform.GetChild(0).GetComponent<Text>().color = Color.cyan;
                            obj.transform.GetComponent<LogMessage>().message = err;
                            obj.transform.GetComponent<LogMessage>().pinned = true;
                        }

                        break;
                    case 2:
                        if (!CameraSaveDisabled)
                        {
                            CameraSaveDisabled = true;
                            obj = Instantiate(_LogMessagePrefab, _LogMessageParent);
                            obj.transform.GetChild(0).GetComponent<Text>().text = "It seems like this camera bank is not fully supported by icebreaker. Saving is disabled.";
                            obj.transform.GetChild(0).GetComponent<Text>().color = Color.cyan;
                            obj.transform.GetComponent<LogMessage>().message = err;
                            obj.transform.GetComponent<LogMessage>().pinned = true;
                        }

                        break;
                }
            }

            try {
                log.WriteLine(condition);
                log.WriteLine(stackTrace);
                log.WriteLine();
            } catch { }
        }
        else if (type == LogType.Log)
        {
            GameObject obj = Instantiate(_LogMessagePrefab, _LogMessageParent);
            obj.transform.GetChild(0).GetComponent<Text>().text = condition;
            obj.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            obj.transform.GetComponent<LogMessage>().message = condition;
            try {
                log.WriteLine(condition);
            } catch { }
        }

        try {
            log.Flush();
        } catch { }
    }

    public Button[] HistoryButtons;
    public Dropdown GameDropdown;

    void Start()
    {
        Application.targetFrameRate = 60;
        _LogMessageParent = LogMessageParent;
        _LogMessagePrefab = LogMessagePrefab;
        Application.logMessageReceived += HandleException;
        VersionText.text = "ver. " + Application.version;
        foreach (GameObject obj in HiddenElements)
            obj.SetActive(false);
        foreach (GameObject obj in EnableOnStart)
            obj.SetActive(true);
        foreach (GameObject obj in DisableOnStart)
            obj.SetActive(false);
        foreach (GameObject obj in HiddenWhenNoEditingAllowed)
            obj.SetActive(false);
        foreach (GameObject obj in HiddenWhenNoPlayingAllowed)
            obj.SetActive(false);
        foreach (GameObject obj in HiddenWhenNoNIS)
            obj.SetActive(false);
        foreach (GameObject obj in HiddenWhenNoCamera)
            obj.SetActive(false);
        OpenedFileName.text = "";
        GameDirInput.text = PlayerPrefs.GetString("GameDir", "");
        GameDropdown.value = PlayerPrefs.GetInt("CurrentGame", 0);
        UpdateGameSelection();
        GetComponent<CanvasScaler>().scaleFactor = PlayerPrefs.GetFloat("GUIScale", 1f);
        if (PlayerPrefs.GetString("GameDir") == "")
            AboutPage.SetActive(true);
        NameField.text = PlayerPrefs.GetString("AuthorName", "Anon") == "Anon" ? "" : PlayerPrefs.GetString("AuthorName", "Anon");
        try {
            if (File.Exists("./icebreaker.log"))
                File.Delete("./icebreaker.log");
            log = new StreamWriter(new FileStream("./icebreaker.log", FileMode.Create));
        } catch { }
    }
    
    static private StreamWriter log;

    void OnApplicationQuit()
    {
        try {
            log.Close();
        } catch { }
    }

    private float timeinsec;
    public bool forceY = true;

    public void UpdateForceY(bool enable)
    {
        forceY = enable;
    }

    public void UpdateGame(int num)
    {
        PlayerPrefs.SetInt("CurrentGame", num);
        switch (num)
        {
            case 0:
                NISLoader.CurrentGame = GameDetector.Game.MostWanted;
                WorldChunksStreamer.bankName = "L2RA";
                break;
            case 1:
                NISLoader.CurrentGame = GameDetector.Game.Carbon;
                WorldChunksStreamer.bankName = "L5RA";
                break;
        }
    }
    
    private Vector3 dragOrigin;

    public static float InverseLerpUnclamped(float a, float b, float value)
    {
        if (a != b)
            return (value - a) / (b - a);
        return 0.0f;
    }

    [HideInInspector] public int cursegment;
    private int oldsegment = -1;
    private bool allow_changes;

    private string[] copied_values;
    private string[] copied_flags;
    private int interpolation_start;

    public static float timescale = 1f;

    public abstract class HistoryEntry
    {
        public abstract void Apply();

        public abstract void Restore();
    }

    public class EditCameraSegment : HistoryEntry
    {
        public EditCameraSegment(int _track, int _index, NISLoader.CameraTrackEntry _oldentry, NISLoader.CameraTrackEntry _newentry)
        {
            track = _track;
            index = _index;
            oldentry = _oldentry;
            newentry = _newentry;
        }

        private int track;
        private int index;
        private NISLoader.CameraTrackEntry oldentry;
        private NISLoader.CameraTrackEntry newentry;

        public override void Apply()
        {
            Main main = FindObjectOfType<Main>();
            main.cameratrack[track].Item2[index] = newentry;
            main.GenCameraSplines();
            main.oldsegment = -1;
        }

        public override void Restore()
        {
            Main main = FindObjectOfType<Main>();
            main.cameratrack[track].Item2[index] = oldentry;
            main.GenCameraSplines();
            main.oldsegment = -1;
        }

        public override string ToString()
        {
            return "segment #" + (index + 1) + " edit";
        }
    }

    public class SplitSegment : HistoryEntry
    {
        public SplitSegment(int _track, int _index, float _timeline, NISLoader.CameraTrackEntry _oldentry)
        {
            track = _track;
            index = _index;
            timeline = _timeline;
            oldentry = _oldentry;
        }

        private int track;
        private int index;
        private float timeline;
        private NISLoader.CameraTrackEntry oldentry;
        internal int actionNum = 1;

        public override void Apply()
        {
            Main main = FindObjectOfType<Main>();
            main.timeline = timeline;
            main.curcam = track;
            for (int i = 0; i < main.cameratrack[track].Item2.Length; i++)
            {
                if (timeline >= main.cameratrack[track].Item2[i].Time && (i == main.cameratrack[track].Item2.Length - 1 || timeline < main.cameratrack[track].Item2[i + 1].Time))
                {
                    main.cursegment = i;
                    break;
                }
            }

            main.dontUseHistory = true;
            main.SegmentEditAction(actionNum);
            main.dontUseHistory = false;
        }

        public override void Restore()
        {
            Main main = FindObjectOfType<Main>();
            if (main.TimelineLockToggle.isOn)
                main.TimelineLockToggle.isOn = false;
            var entries = main.cameratrack[track].Item2.ToList();
            entries[index] = oldentry;
            entries.RemoveAt(index + 1);
            main.cameratrack[track] = (main.cameratrack[track].Item1, entries.ToArray());
            main.GenCameraTrackPreview();
            main.GenCameraSplines();
            main.UpdCameraTrackPreview();
        }

        public override string ToString()
        {
            return "split segment";
        }
    }

    public class CreateSegment : SplitSegment
    {
        public CreateSegment(int _track, int _index, float _timeline, NISLoader.CameraTrackEntry _oldentry) : base(_track, _index, _timeline, _oldentry)
        {
            actionNum = 4;
        }

        public override string ToString()
        {
            return "new segment";
        }
    }

    public class RemoveSegment : HistoryEntry
    {
        public RemoveSegment(int _track, int _index, float _timeline, NISLoader.CameraTrackEntry _oldentry, float _oldtime)
        {
            track = _track;
            index = _index;
            timeline = _timeline;
            oldentry = _oldentry;
            oldtime = _oldtime;
        }

        private int track;
        private int index;
        private float timeline;
        private float oldtime;
        private NISLoader.CameraTrackEntry oldentry;

        public override void Apply()
        {
            Main main = FindObjectOfType<Main>();
            main.timeline = timeline;
            main.curcam = track;
            for (int i = 0; i < main.cameratrack[track].Item2.Length; i++)
            {
                if (timeline >= main.cameratrack[track].Item2[i].Time && (i == main.cameratrack[track].Item2.Length - 1 || timeline < main.cameratrack[track].Item2[i + 1].Time))
                {
                    main.cursegment = i;
                    break;
                }
            }

            main.dontUseHistory = true;
            main.SegmentEditAction(5);
            main.dontUseHistory = false;
        }

        public override void Restore()
        {
            Main main = FindObjectOfType<Main>();
            if (main.TimelineLockToggle.isOn)
                main.TimelineLockToggle.isOn = false;
            var entries = main.cameratrack[track].Item2.ToList();
            if (oldtime > 0f)
            {
                var v = entries[0];
                v.Time = oldtime;
                entries[0] = v;
            }

            entries.Insert(index, oldentry);
            main.cameratrack[track] = (main.cameratrack[track].Item1, entries.ToArray());
            main.GenCameraTrackPreview();
            main.GenCameraSplines();
            main.UpdCameraTrackPreview();
        }

        public override string ToString()
        {
            return "remove segment";
        }
    }

    public class MergeSegmentLeft : HistoryEntry
    {
        public MergeSegmentLeft(int _track, int _index, float _timeline, NISLoader.CameraTrackEntry _oldentry1, NISLoader.CameraTrackEntry _oldentry2)
        {
            track = _track;
            index = _index;
            timeline = _timeline;
            oldentry1 = _oldentry1;
            oldentry2 = _oldentry2;
        }

        private int track;
        private int index;
        private float timeline;
        private float oldtime;
        private NISLoader.CameraTrackEntry oldentry1;
        private NISLoader.CameraTrackEntry oldentry2;
        internal int actionNum = 2;

        public override void Apply()
        {
            Main main = FindObjectOfType<Main>();
            main.timeline = timeline;
            main.curcam = track;
            for (int i = 0; i < main.cameratrack[track].Item2.Length; i++)
            {
                if (timeline >= main.cameratrack[track].Item2[i].Time && (i == main.cameratrack[track].Item2.Length - 1 || timeline < main.cameratrack[track].Item2[i + 1].Time))
                {
                    main.cursegment = i;
                    break;
                }
            }

            main.dontUseHistory = true;
            main.SegmentEditAction(actionNum);
            main.dontUseHistory = false;
        }

        public override void Restore()
        {
            Main main = FindObjectOfType<Main>();
            if (main.TimelineLockToggle.isOn)
                main.TimelineLockToggle.isOn = false;
            var entries = main.cameratrack[track].Item2.ToList();
            entries[index - 1] = oldentry1;
            entries.Insert(index, oldentry2);
            main.cameratrack[track] = (main.cameratrack[track].Item1, entries.ToArray());
            main.GenCameraTrackPreview();
            main.GenCameraSplines();
            main.UpdCameraTrackPreview();
        }

        public override string ToString()
        {
            return "merge segment with prev.";
        }
    }

    public class MergeSegmentRight : MergeSegmentLeft
    {
        public MergeSegmentRight(int _track, int _index, float _timeline, NISLoader.CameraTrackEntry _oldentry1, NISLoader.CameraTrackEntry _oldentry2) : base(_track, _index, _timeline, _oldentry1, _oldentry2)
        {
            actionNum = 3;
        }

        public override string ToString()
        {
            return "merge segment with next";
        }
    }

    public class HistoryChangeCameraTrack : HistoryEntry
    {
        public HistoryChangeCameraTrack(int _track1, int _track2)
        {
            track1 = _track1;
            track2 = _track2;
        }

        private int track1;
        private int track2;

        public override void Apply()
        {
            Main main = FindObjectOfType<Main>();
            main.dontUseHistory = true;
            main.ChangeCameraTrack(track2);
            main.dontUseHistory = false;
        }

        public override void Restore()
        {
            Main main = FindObjectOfType<Main>();
            main.dontUseHistory = true;
            main.ChangeCameraTrack(track1);
            main.dontUseHistory = false;
        }

        public override string ToString()
        {
            return "change camera track";
        }
    }

    public class ChangeCameraTrackHeader : HistoryEntry
    {
        public ChangeCameraTrackHeader(int _track, NISLoader.CameraTrackHeader _header1, NISLoader.CameraTrackHeader _header2)
        {
            track = _track;
            header1 = _header1;
            header2 = _header2;
        }

        private int track;
        private NISLoader.CameraTrackHeader header1;
        private NISLoader.CameraTrackHeader header2;

        public override void Apply()
        {
            Main main = FindObjectOfType<Main>();
            main.cameratrack[track] = (header2, main.cameratrack[track].Item2);
            main.updlimit = false;
            main.dontUseHistory = true;
            main.ChangeCameraTrack(track);
            main.UpdateTitle();
            main.dontUseHistory = false;
        }

        public override void Restore()
        {
            Main main = FindObjectOfType<Main>();
            main.cameratrack[track] = (header1, main.cameratrack[track].Item2);
            main.updlimit = false;
            main.dontUseHistory = true;
            main.ChangeCameraTrack(track);
            main.UpdateTitle();
            main.dontUseHistory = false;
        }

        public override string ToString()
        {
            return "edit camera track header";
        }
    }

    public class CreateCameraTrack : HistoryEntry
    {
        public CreateCameraTrack(int _track, int _oldtrack, (NISLoader.CameraTrackHeader, NISLoader.CameraTrackEntry[]) _trackdata)
        {
            track = _track;
            oldtrack = _oldtrack;
            trackdata = _trackdata;
        }
        
        private int track;
        private int oldtrack;
        private (NISLoader.CameraTrackHeader, NISLoader.CameraTrackEntry[]) trackdata;
        
        public override void Apply()
        {
            Main main = FindObjectOfType<Main>();
            main.cameratrack.Insert(track, trackdata);
            Dropdown.OptionData opt = new Dropdown.OptionData();
            opt.text = trackdata.Item1.TrackName;
            main.dontUseHistory = true;
            main.CameraTrackSelection.options.Add(opt);
            main.CameraTrackSelection.value = 0;
            main.CameraTrackSelection.value = track;
            main.updlimit = false;
            main.ChangeCameraTrack(track);
            main.dontUseHistory = false;
        }

        public override void Restore()
        {
            Main main = FindObjectOfType<Main>();
            main.cameratrack.RemoveAt(track);
            main.dontUseHistory = true;
            main.CameraTrackSelection.options.RemoveAt(track);
            main.CameraTrackSelection.value = 0;
            main.CameraTrackSelection.value = oldtrack;
            main.updlimit = false;
            main.ChangeCameraTrack(oldtrack);
            main.dontUseHistory = false;
        }
        
        public override string ToString()
        {
            return "create camera track";
        }
    }
    
    public class HRemoveCameraTrack : HistoryEntry
    {
        public HRemoveCameraTrack(int _track, int _newtrack, (NISLoader.CameraTrackHeader, NISLoader.CameraTrackEntry[]) _trackdata)
        {
            track = _track;
            newtrack = _newtrack;
            trackdata = _trackdata;
        }
        
        private int track;
        private int newtrack;
        private (NISLoader.CameraTrackHeader, NISLoader.CameraTrackEntry[]) trackdata;
        
        public override void Apply()
        {
            Main main = FindObjectOfType<Main>();
            main.cameratrack.RemoveAt(track);
            main.dontUseHistory = true;
            main.CameraTrackSelection.options.RemoveAt(track);
            main.CameraTrackSelection.value = 0;
            main.CameraTrackSelection.value = newtrack;
            main.updlimit = false;
            main.ChangeCameraTrack(newtrack);
            main.dontUseHistory = false;
        }

        public override void Restore()
        {
            Main main = FindObjectOfType<Main>();
            main.cameratrack.Insert(track, trackdata);
            Dropdown.OptionData opt = new Dropdown.OptionData();
            opt.text = trackdata.Item1.TrackName;
            main.dontUseHistory = true;
            main.CameraTrackSelection.options.Add(opt);
            main.CameraTrackSelection.value = 0;
            main.CameraTrackSelection.value = track;
            main.updlimit = false;
            main.ChangeCameraTrack(track);
            main.dontUseHistory = false;
        }
        
        public override string ToString()
        {
            return "remove camera track";
        }
    }
    
    public class SwapCameraTrack : HistoryEntry
    {
        public SwapCameraTrack(int _track, int _entry)
        {
            track = _track;
            entry = _entry;
        }
        
        private int track;
        private int entry;

        public override void Apply()
        {
            Swap();
        }

        public override void Restore()
        {
            Swap();
        }

        void Swap()
        {
            Main main = FindObjectOfType<Main>();
            float firstlength = (entry < main.cameratrack[track].Item2.Length - 1 ? main.cameratrack[track].Item2[entry + 1].Time : 1f) - main.cameratrack[track].Item2[entry].Time;
            var v = main.cameratrack[track].Item2[entry];
            main.cameratrack[track].Item2[entry] = main.cameratrack[track].Item2[entry - 1];
            main.cameratrack[track].Item2[entry - 1] = v;
            main.cameratrack[track].Item2[entry - 1].Time = main.cameratrack[track].Item2[entry].Time;
            main.cameratrack[track].Item2[entry].Time = main.cameratrack[track].Item2[entry - 1].Time + firstlength;
            main.CtEntryIcons[entry].transform.SetSiblingIndex(entry - 1);
            main.CtEntryIcons[entry - 1].transform.SetSiblingIndex(entry);
            var im = main.CtEntryIcons[entry];
            main.CtEntryIcons[entry] = main.CtEntryIcons[entry - 1];
            main.CtEntryIcons[entry - 1] = im;
            if (entry == main.cameratrack[track].Item2.Length - 1)
            {
                main.CtEntryIcons[entry - 1].transform.GetChild(0).gameObject.SetActive(true);
                main.CtEntryIcons[entry].transform.GetChild(0).gameObject.SetActive(false);
            }
            main.ChangeCameraTrack(main.curcam);
        }
        
        public override string ToString()
        {
            return "swap segments";
        }
    }
    
    public class ResizeCameraTrack : HistoryEntry
    {
        public ResizeCameraTrack(int _track, int _entry, float _oldTime, float _curTime)
        {
            track = _track;
            entry = _entry;
            oldTime = _oldTime;
            curTime = _curTime;
        }
        
        private int track;
        private int entry;
        private float oldTime;
        private float curTime;

        public override void Apply()
        {
            Resize(curTime);
        }

        public override void Restore()
        {
            Resize(oldTime);
        }

        void Resize(float t)
        {
            Main main = FindObjectOfType<Main>();
            main.cameratrack[track].Item2[entry + 1].Time = t;
            main.UpdCameraTrackPreview();
            main.ChangeCameraTrack(main.curcam);
        }
        
        public override string ToString()
        {
            return "resize segment";
        }
    }

    public class MoveObject : HistoryEntry
    {
        public MoveObject(string _objname, NISLoader.Animation _anim, float[][] _old_values)
        {
            objname = _objname;
            anim = _anim;
            old_values = _old_values;
            float[][] _new_values = _anim.delta;
            new_values = new float[_new_values.Length][];
            for (int i = 0; i < _new_values.Length; i++)
            {
                new_values[i] = new float[_new_values[i].Length];
                for (int j = 0; j < _new_values[i].Length; j++)
                    new_values[i][j] = _new_values[i][j];
            }
        }

        internal string objname;
        private NISLoader.Animation anim;
        private float[][] old_values;
        private float[][] new_values;
        
        public override void Apply()
        {
            anim.delta = new_values;
            FindObjectOfType<Main>().rewinded_this_frame = 5;
            FindObjectOfType<Main>().dontUseHistory = true;
        }

        public override void Restore()
        {
            anim.delta = old_values;
            FindObjectOfType<Main>().rewinded_this_frame = 5;
            FindObjectOfType<Main>().dontUseHistory = true;
        }
        
        public override string ToString()
        {
            return "move object " + objname;
        }
    }
    
    public class Interpolate : MoveObject
    {
        public Interpolate(string _objname, NISLoader.Animation _anim, float[][] _old_values) : base(_objname, _anim, _old_values) { }
        
        public override string ToString()
        {
            return "interpolate " + objname;
        }
    }
    
    public class ChangeDeltaC : HistoryEntry
    {
        public ChangeDeltaC(string _objname, NISLoader.Animation[] _anim, float[][][] _old_values)
        {
            objname = _objname;
            anim = _anim;
            old_values = _old_values;
            new_values = new float[old_values.Length][][];
            for (int x = 0; x < _anim.Length; x++)
            {
                if (_anim[x] == null) continue;
                new_values[x] = new float[_anim[x].delta.Length][];
                for (int i = 0; i < _anim[x].delta.Length; i++)
                {
                    new_values[x][i] = new float[_anim[x].delta[i].Length];
                    for (int j = 0; j < _anim[x].delta[i].Length; j++)
                        new_values[x][i][j] = _anim[x].delta[i][j];
                }
            }
        }

        internal string objname;
        private NISLoader.Animation[] anim;
        private float[][][] old_values;
        private float[][][] new_values;
        internal bool selectobj = true;
        
        public override void Apply()
        {
            for (int x = 0; x < anim.Length; x++)
            {
                if (anim[x] == null) continue;
                anim[x].delta = new_values[x];
            }
            FindObjectOfType<Main>().rewinded_this_frame = 5;
            if (selectobj)
            {
                FindObjectOfType<Main>().dontUseHistory = true;
                FindObjectOfType<Main>().AnimationsEditorObjectSelected(FindObjectOfType<Main>().currentlyEditingObject);
            }
        }

        public override void Restore()
        {
            for (int x = 0; x < anim.Length; x++)
            {
                if (anim[x] == null) continue;
                anim[x].delta = old_values[x];
            }
            FindObjectOfType<Main>().rewinded_this_frame = 5;
            if (selectobj)
            {
                FindObjectOfType<Main>().dontUseHistory = true;
                FindObjectOfType<Main>().AnimationsEditorObjectSelected(FindObjectOfType<Main>().currentlyEditingObject);
            }
        }
        
        public override string ToString()
        {
            return "change " + objname + " delta count";
        }
    }

    public class PasteValues : ChangeDeltaC
    {
        public PasteValues(string _objname, NISLoader.Animation[] _anim, float[][][] _old_values) : base(_objname, _anim, _old_values)
        {
            selectobj = false;
        }
        
        public override string ToString()
        {
            return "edit " + objname + " values";
        }
    }
    
    public class InterpolateAll : ChangeDeltaC
    {
        public InterpolateAll(string _objname, NISLoader.Animation[] _anim, float[][][] _old_values) : base(_objname, _anim, _old_values)
        {
            selectobj = false;
        }
        
        public override string ToString()
        {
            return "interpolate " + objname;
        }
    }
    
    public class ImportReplay : ChangeDeltaC
    {
        public ImportReplay(string _objname, NISLoader.Animation[] _anim, float[][][] _old_values) : base(_objname, _anim, _old_values)
        {
            selectobj = false;
        }
        
        public override string ToString()
        {
            return "import replay into " + objname;
        }
    }

    List<HistoryEntry> historyAnimations = new List<HistoryEntry>();
    List<HistoryEntry> historyCamera = new List<HistoryEntry>();
    private int currentEntryAnimations = -1;
    private int currentEntryCamera = -1;
    private bool dontUseHistory;

    public void AddToHistoryNIS(HistoryEntry entry)
    {
        if (dontUseHistory) return;
        currentEntryAnimations++;
        while (historyAnimations.Count > currentEntryAnimations)
            historyAnimations.RemoveAt(historyAnimations.Count - 1);
        historyAnimations.Add(entry);
        if (historyAnimations.Count > 64)
        {
            historyAnimations.RemoveAt(0);
            currentEntryAnimations--;
        }
    }

    void UndoNIS()
    {
        if (currentEntryAnimations < 0) return;
        historyAnimations[currentEntryAnimations].Restore();
        Debug.Log("Undo " + historyAnimations[currentEntryAnimations]);
        currentEntryAnimations--;
    }

    void RedoNIS()
    {
        if (currentEntryAnimations >= historyAnimations.Count - 1) return;
        currentEntryAnimations++;
        historyAnimations[currentEntryAnimations].Apply();
        Debug.Log("Redo " + historyAnimations[currentEntryAnimations]);
    }
    
    public void AddToHistoryCam(HistoryEntry entry)
    {
        if (dontUseHistory) return;
        currentEntryCamera++;
        while (historyCamera.Count > currentEntryCamera)
            historyCamera.RemoveAt(historyCamera.Count - 1);
        historyCamera.Add(entry);
        if (historyCamera.Count > 64)
        {
            historyCamera.RemoveAt(0);
            currentEntryCamera--;
        }
    }
    
    void UndoCam()
    {
        if (currentEntryCamera < 0 || playing || RealtimeCameraEditActive) return;
        historyCamera[currentEntryCamera].Restore();
        Debug.Log("Undo " + historyCamera[currentEntryCamera]);
        currentEntryCamera--;
    }
    
    void RedoCam()
    {
        if (currentEntryCamera >= historyCamera.Count - 1 || playing || RealtimeCameraEditActive) return;
        currentEntryCamera++;
        historyCamera[currentEntryCamera].Apply();
        Debug.Log("Redo " + historyCamera[currentEntryCamera]);
    }

    public void UndoButton()
    {
        if (tabnum == 2)
            UndoNIS();
        else if (tabnum == 3)
            UndoCam();
    }
    
    public void RedoButton()
    {
        if (tabnum == 2)
            RedoNIS();
        else if (tabnum == 3)
            RedoCam();
    }

    public static bool helpshown;

    public void HelpButton(bool enable)
    {
        helpshown = enable;
    }

    void ClearHistory()
    {
        historyAnimations.Clear();
        currentEntryAnimations = -1;
        historyCamera.Clear();
        currentEntryCamera = -1;
    }

    private bool objectmovetrigger;
    private float[][] old_delta;
    private NISLoader.Animation savedanim;
    private float old_ttt = -1;
    public int animtabindex;

    void Update()
    {
        if (!activated || helpshown) return;
        if (SavedText.color.a > 0f)
            SavedText.color = new Color(1f, 1f, 1f, SavedText.color.a - Time.deltaTime * 0.5f);
        updlimit = false;
        if (playing)
        {
            timeline += Time.deltaTime * timescale / totalLength;
            if (TimelineLock && cameratrack.Count > 0)
            {
                float max = cursegment < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[cursegment + 1].Time - 0.001f : 1f;
                if (timeline >= max) {
                    timeline = max;
                    playing = false;
                }
            } else {
                if (timeline >= 1f)
                {
                    timeline = 1f;
                    playing = false;
                }
            }
        }
        timeinsec = timeline * totalLength;
        TimeText.text = (int) Mathf.Floor(timeinsec / 60f) + ":" + (timeinsec % 60f).ToString("0.00").PadLeft(5, '0');
        group.alpha = tabnum >= 2 && Input.GetKey(KeyCode.Tab) && EventSystem.current.currentSelectedGameObject == null ? 0f : 1f;
        switch (tabnum)
        {
            case 2:
                if (playing)
                    playing = false;
                gizmo.YAllowed = !forceY;
                timeline = Mathf.Round(timeinsec * 15f) / 15f / totalLength;
                PreviewTimeline[5].value = Mathf.Round(timeinsec * 15f);
                if (old_ttt != PreviewTimeline[5].value)
                    dontUseHistory = true;
                old_ttt = PreviewTimeline[5].value;
                PreviewTimeline[6].value = interpolation_start;
                CoordText.text = editorCameraMovement.target.position.x.ToString("0.00", CultureInfo.InvariantCulture) + "," + editorCameraMovement.target.position.z.ToString("0.00", CultureInfo.InvariantCulture) + "," + editorCameraMovement.target.position.y.ToString("0.00", CultureInfo.InvariantCulture);
                foreach (InputField f in RootValues)
                    f.interactable = !gizmo.isTransforming;
                foreach (InputField f in BoneValues)
                    f.interactable = !gizmo.isTransforming;
                HistoryButtons[0].interactable = currentEntryAnimations > -1;
                HistoryButtons[1].interactable = currentEntryAnimations < historyAnimations.Count - 1;
                if (gizmo.isTransforming)
                {
                    //int pos = (int)Mathf.Floor(timeinsec * 15f);
                    if (RootSubEdit.activeSelf)
                    {
                        if (EditingAnimation_t != null)
                        {
                            //if (EditingAnimation_t.type == NISLoader.AnimType.ANIM_COMPOUND)
                            //    EditingAnimation_t = EditingAnimation_t.subAnimations[0];
                            //EditingAnimation_t.delta[pos] = new float[];
                            RootValues[0].text = currentlyEditingSubObject.transform.position.x.ToString(CultureInfo.InvariantCulture);
                            RootValues[1].text = currentlyEditingSubObject.transform.position.z.ToString(CultureInfo.InvariantCulture);
                            if (!forceY)
                                RootValues[2].text = currentlyEditingSubObject.transform.position.y.ToString(CultureInfo.InvariantCulture);
                        }
                        if (EditingAnimation_q != null)
                        {
                            //if (EditingAnimation_q.type == NISLoader.AnimType.ANIM_COMPOUND)
                            //    EditingAnimation_q = EditingAnimation_q.subAnimations[0];
                            RootValues[3].text = currentlyEditingSubObject.transform.eulerAngles.z.ToString(CultureInfo.InvariantCulture);
                            RootValues[4].text = (-currentlyEditingSubObject.transform.eulerAngles.x).ToString(CultureInfo.InvariantCulture);
                            RootValues[5].text = (-(currentlyEditingSubObject.transform.eulerAngles.y - 90f)).ToString(CultureInfo.InvariantCulture);
                        }
                        if (!objectmovetrigger)
                        {
                            objectmovetrigger = true;
                            if (Input.GetKey(KeyCode.LeftShift))
                            {
                                if (EditingAnimation_q != null)
                                {
                                    savedanim = EditingAnimation_q.subAnimations[0];
                                }
                                else
                                    savedanim = null;
                            } else {
                                if (EditingAnimation_t != null)
                                {
                                    savedanim = EditingAnimation_t.subAnimations[0];
                                }
                                else
                                    savedanim = null;
                            }
                            if (savedanim != null)
                            {
                                float[][] d = savedanim.delta;
                                old_delta = new float[d.Length][];
                                for (int i = 0; i < d.Length; i++)
                                {
                                    old_delta[i] = new float[d[i].Length];
                                    for (int j = 0; j < d[i].Length; j++)
                                        old_delta[i][j] = d[i][j];
                                }
                            }
                        }
                    } else if (BoneSubEdit.activeSelf)
                    {
                        //if (EditingAnimation_s.type == NISLoader.AnimType.ANIM_COMPOUND)
                        //    EditingAnimation_s = EditingAnimation_s.subAnimations[0];
                        BoneValues[0].text = currentlyEditingSubObject.transform.localEulerAngles.x.ToString(CultureInfo.InvariantCulture);
                        BoneValues[1].text = currentlyEditingSubObject.transform.localEulerAngles.y.ToString(CultureInfo.InvariantCulture);
                        BoneValues[2].text = currentlyEditingSubObject.transform.localEulerAngles.z.ToString(CultureInfo.InvariantCulture);
                    } else {
                        // layout, do nothing
                    }
                } else
                {
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        if (Input.GetKeyDown(KeyCode.Q))
                            interpolation_start = animtabindex;
                        else if (Input.GetKeyDown(KeyCode.Z))
                        {
                            UndoNIS();
                        }
                        else if (Input.GetKeyDown(KeyCode.Y))
                        {
                            RedoNIS();
                        }
                        else if (Input.GetKeyDown(KeyCode.I))
                        {
                            float[][][] old_delta = new float[3][][];
                            old_delta[0] = new float[EditingAnimation_t.subAnimations[0].delta.Length][];
                            if (EditingAnimation_t != null)
                            {
                                for (int i = 0; i < EditingAnimation_t.subAnimations[0].delta.Length; i++)
                                {
                                    old_delta[0][i] = new float[EditingAnimation_t.subAnimations[0].delta[i].Length];
                                    for (int j = 0; j < EditingAnimation_t.subAnimations[0].delta[i].Length; j++)
                                        old_delta[0][i][j] = EditingAnimation_t.subAnimations[0].delta[i][j];
                                }
                            }
                            if (EditingAnimation_q != null)
                            {
                                old_delta[1] = new float[EditingAnimation_q.subAnimations[0].delta.Length][];
                                for (int i = 0; i < EditingAnimation_q.subAnimations[0].delta.Length; i++)
                                {
                                    old_delta[1][i] = new float[EditingAnimation_q.subAnimations[0].delta[i].Length];
                                    for (int j = 0; j < EditingAnimation_q.subAnimations[0].delta[i].Length; j++)
                                        old_delta[1][i][j] = EditingAnimation_q.subAnimations[0].delta[i][j];
                                }
                            }
                            if (EditingAnimation_s != null)
                            {
                                old_delta[2] = new float[EditingAnimation_s.subAnimations[0].delta.Length][];
                                for (int i = 0; i < EditingAnimation_s.subAnimations[0].delta.Length; i++)
                                {
                                    old_delta[2][i] = new float[EditingAnimation_s.subAnimations[0].delta[i].Length];
                                    for (int j = 0; j < EditingAnimation_s.subAnimations[0].delta[i].Length; j++)
                                        old_delta[2][i][j] = EditingAnimation_s.subAnimations[0].delta[i][j];
                                }
                            }
                            dontUseHistory = true;
                            AnimationAction(3);
                            AnimationAction(4);
                            AnimationAction(5);
                            dontUseHistory = false;
                            AddToHistoryNIS(new InterpolateAll(currentlyEditingObject, new [] { EditingAnimation_t != null ? EditingAnimation_t.subAnimations[0] : null, EditingAnimation_q != null ? EditingAnimation_q.subAnimations[0] : null, EditingAnimation_s != null ? EditingAnimation_s.subAnimations[0] : null }, old_delta));
                            Debug.Log("Interpolated successfully");
                        }
                        else if (Input.GetKey(KeyCode.LeftShift))
                        {
                            if (Input.GetKeyDown(KeyCode.C))
                            {
                                AnimationAction(6);
                            }
                            else if (Input.GetKeyDown(KeyCode.V))
                            {
                                AnimationAction(7);
                            }
                            else if (Input.GetKeyDown(KeyCode.P))
                            {
                                AnimationAction(3);
                            }
                            else if (Input.GetKeyDown(KeyCode.R))
                            {
                                AnimationAction(4);
                            }
                            else if (Input.GetKeyDown(KeyCode.S))
                            {
                                AnimationAction(5);
                            }
                        }
                    }
                    if (objectmovetrigger)
                    {
                        objectmovetrigger = false;
                        if (savedanim != null)
                        {
                            dontUseHistory = false;
                            AddToHistoryNIS(new MoveObject(currentlyEditingObject, savedanim, old_delta));
                        }
                    }
                    (float[], float[], float, bool) eval_t, eval_q, eval_s;
                    if (EditingAnimation_t != null)
                        eval_t = NISLoader.EvaluateAnimAbs(EditingAnimation_t, animtabindex);
                    else
                        eval_t = (null, null, 0f, false);
                    if (EditingAnimation_q != null)
                        eval_q = NISLoader.EvaluateAnimAbs(EditingAnimation_q, animtabindex);
                    else
                        eval_q = (null, null, 0f, false);
                    if (EditingAnimation_s != null)
                        eval_s = NISLoader.EvaluateAnimAbs(EditingAnimation_s, animtabindex);
                    else
                        eval_s = (null, null, 0f, false);
                    if (RootSubEdit.activeSelf)
                    {
                        if (eval_t.Item1 != null)
                        {
                            RootValues[0].text = eval_t.Item1[0].ToString(CultureInfo.InvariantCulture);
                            RootValues[1].text = eval_t.Item1[1].ToString(CultureInfo.InvariantCulture);
                            RootValues[2].text = eval_t.Item1[2].ToString(CultureInfo.InvariantCulture);
                            if (forceY)
                                RootValues[2].interactable = false;
                        } else
                        {
                            RootValues[0].text = "";
                            RootValues[1].text = "";
                            RootValues[2].text = "";
                            RootValues[0].interactable = false;
                            RootValues[1].interactable = false;
                            RootValues[2].interactable = false;
                        }
                        if (eval_q.Item1 != null)
                        {
                            Vector3 quat = new Quaternion(eval_q.Item1[0], eval_q.Item1[1], eval_q.Item1[2], eval_q.Item1[3]).eulerAngles;
                            RootValues[3].text = quat.x.ToString(CultureInfo.InvariantCulture);
                            RootValues[4].text = quat.y.ToString(CultureInfo.InvariantCulture);
                            RootValues[5].text = quat.z.ToString(CultureInfo.InvariantCulture);
                        } else
                        {
                            RootValues[3].text = "";
                            RootValues[4].text = "";
                            RootValues[5].text = "";
                            RootValues[3].interactable = false;
                            RootValues[4].interactable = false;
                            RootValues[5].interactable = false;
                        }
                        if (eval_s.Item1 != null)
                        {
                            RootValues[6].text = eval_s.Item1[0].ToString(CultureInfo.InvariantCulture);
                            RootValues[7].text = eval_s.Item1[1].ToString(CultureInfo.InvariantCulture);
                            RootValues[8].text = eval_s.Item1[2].ToString(CultureInfo.InvariantCulture);
                        } else
                        {
                            RootValues[6].text = "";
                            RootValues[7].text = "";
                            RootValues[8].text = "";
                            RootValues[6].interactable = false;
                            RootValues[7].interactable = false;
                            RootValues[8].interactable = false;
                        }
                    } else if (BoneSubEdit.activeSelf) {
                        for (int sk = 0; sk < skeletons.Count; sk++)
                        {
                            if (skeletons[sk].animationName == currentlyEditingObject.ToUpper() || skeletons[sk].animationName.StartsWith(currentlyEditingObject.ToUpper()))
                            {
                                for (int b = 0; b < skeletons[sk].bones.Length; b++)
                                {
                                    if (skeletons[sk].bones[b].assignedTransform == currentlyEditingSubObject)
                                    {
                                        int offset = b * 4;
                                        Vector3 q = new Quaternion(eval_s.Item1[offset], eval_s.Item1[offset + 1], eval_s.Item1[offset + 2], eval_s.Item1[offset + 3]).eulerAngles;
                                        BoneValues[0].text = q.x.ToString(CultureInfo.InvariantCulture);
                                        BoneValues[1].text = q.y.ToString(CultureInfo.InvariantCulture);
                                        BoneValues[2].text = q.z.ToString(CultureInfo.InvariantCulture);
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    } else {
                        // todo layout here
                    }
                    dontUseHistory = false;
                    if (rewinded_this_frame > 0)
                        rewinded_this_frame--;
                }
                break;
            case 3:
                if (Input.GetKey(KeyCode.LeftControl) && !RealtimeCameraEditActive)
                {
                    if (Input.GetKeyDown(KeyCode.L) && TimelineLockToggle.interactable)
                    {
                        TimelineLockToggle.isOn = !TimelineLock;
                    }
                    else if (Input.GetKeyDown(KeyCode.S))
                    {
                        SegmentEditAction(1);
                    }
                    else if (Input.GetKeyDown(KeyCode.N))
                    {
                        SegmentEditAction(4);
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        SegmentEditAction(2);
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        SegmentEditAction(3);
                    }
                    else if (Input.GetKeyDown(KeyCode.D))
                    {
                        SegmentEditAction(5);
                    }
                    else if (Input.GetKeyDown(KeyCode.Z))
                    {
                        UndoCam();
                    }
                    else if (Input.GetKeyDown(KeyCode.Y))
                    {
                        RedoCam();
                    }
                    else if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            SegmentEditAction(6);
                        }
                        else if (Input.GetKeyDown(KeyCode.V))
                        {
                            SegmentEditAction(7);
                        }
                    }
                }
                float localt = InverseLerpUnclamped(editorTimelineMin, editorTimelineMax, timeline);
                if (PreviewTimeline[1].value != Mathf.Clamp01(localt))
                    PreviewTimeline[1].handleRect.gameObject.SetActive(localt >= 0f && localt <= 1f);
                PreviewTimeline[1].value = Mathf.Clamp01(localt);
                PreviewTimeline[2].value = timeline;
                PreviewTimeline[3].value = editorTimelineMin;
                PreviewTimeline[4].value = editorTimelineMax;
                HistoryButtons[0].interactable = currentEntryCamera > -1;
                HistoryButtons[1].interactable = currentEntryCamera < historyCamera.Count - 1;
                if (!TimelineLock)
                {
                    for (int i = 0; i < cameratrack[curcam].Item2.Length; i++)
                    {
                        if (timeline >= cameratrack[curcam].Item2[i].Time && (i == cameratrack[curcam].Item2.Length - 1 || timeline < cameratrack[curcam].Item2[i + 1].Time))
                        {
                            CtEntryIcons[i].color = new Color(0.5f, 0.5f, 0.5f, 1);
                            cursegment = i;
                        }
                        else
                            CtEntryIcons[i].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    }
                }
                segmenttitle.text = "Edit segment #" + (cursegment + 1);
                foreach (InputField f in CameraEditValues)
                    f.interactable = !playing && !RealtimeCameraEditActive;
                foreach (InputField f in CameraEditFlags)
                    f.interactable = !playing && !RealtimeCameraEditActive;
                if (oldsegment != cursegment)
                {
                    allow_changes = false;
                    oldsegment = cursegment;
                    switch (cameraeditcurtab)
                    {
                        case 0:
                            CameraEditValues[0].text = cameratrack[curcam].Item2[cursegment].unk5.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[1].text = cameratrack[curcam].Item2[cursegment].EyeX.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[2].text = cameratrack[curcam].Item2[cursegment].EyeZ.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[3].text = cameratrack[curcam].Item2[cursegment].EyeY.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[4].text = cameratrack[curcam].Item2[cursegment].LookX.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[5].text = cameratrack[curcam].Item2[cursegment].LookZ.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[6].text = cameratrack[curcam].Item2[cursegment].LookY.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[7].text = cameratrack[curcam].Item2[cursegment].Tangent.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[8].text = cameratrack[curcam].Item2[cursegment].FocalLength.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[9].text = cameratrack[curcam].Item2[cursegment].unk9.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[10].text = cameratrack[curcam].Item2[cursegment].Amp.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[11].text = cameratrack[curcam].Item2[cursegment].Freq.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[12].text = cameratrack[curcam].Item2[cursegment].unk11.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[13].text = cameratrack[curcam].Item2[cursegment].unk13[4].ToString();
                            CameraEditValues[14].text = cameratrack[curcam].Item2[cursegment].unk13[0].ToString();
                            break;
                        case 1:
                            CameraEditValues[0].text = cameratrack[curcam].Item2[cursegment].unk6.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[1].text = cameratrack[curcam].Item2[cursegment].EyeX2.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[2].text = cameratrack[curcam].Item2[cursegment].EyeZ2.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[3].text = cameratrack[curcam].Item2[cursegment].EyeY2.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[4].text = cameratrack[curcam].Item2[cursegment].LookX2.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[5].text = cameratrack[curcam].Item2[cursegment].LookZ2.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[6].text = cameratrack[curcam].Item2[cursegment].LookY2.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[7].text = cameratrack[curcam].Item2[cursegment].Tangent2.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[8].text = cameratrack[curcam].Item2[cursegment].FocalLength2.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[9].text = cameratrack[curcam].Item2[cursegment].unk10.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[10].text = cameratrack[curcam].Item2[cursegment].Amp2.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[11].text = cameratrack[curcam].Item2[cursegment].Freq2.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[12].text = cameratrack[curcam].Item2[cursegment].unk12.ToString(CultureInfo.InvariantCulture);
                            CameraEditValues[13].text = cameratrack[curcam].Item2[cursegment].unk13[5].ToString();
                            CameraEditValues[14].text = cameratrack[curcam].Item2[cursegment].unk13[1].ToString();
                            break;
                        default:
                            for (int i = 0; i < 12; i++)
                                CameraEditFlags[i].text = cameratrack[curcam].Item2[cursegment].attributes[i].ToString("X");
                            CameraEditFlags[12].text = "0x" + BitConverter.ToUInt32(cameratrack[curcam].Item2[cursegment].attributes, 12).ToString("X8");
                            break;
                    }
                }
                else
                    allow_changes = true;
                break;
            case 4:
                PreviewTimeline[0].value = timeline;
                if (cameratrack.Count > 0)
                {
                    for (int i = 0; i < cameratrack[curcam].Item2.Length; i++)
                    {
                        if (timeline >= cameratrack[curcam].Item2[i].Time && (i == cameratrack[curcam].Item2.Length - 1 || timeline < cameratrack[curcam].Item2[i + 1].Time))
                        {
                            switch (cameratrack[curcam].Item2[i].attributes[10])
                            {
                                case 0:
                                    OverlayDebug.text = "";
                                    break;
                                case 2:
                                    OverlayDebug.text = "Screen fade is active";
                                    break;
                                case 3:
                                    OverlayDebug.text = "Police overlay is shown";
                                    break;
                                case 4:
                                    OverlayDebug.text = "Screen is pitch black";
                                    break;
                                default:
                                    OverlayDebug.text = "Unknown overlay is shown (" + cameratrack[curcam].Item2[i].attributes[10].ToString("X") + ")";
                                    break;
                            }
                            break;
                        }
                    }
                }
                break;
        }
        
        if ((tabnum == 2 || tabnum == 3 || tabnum == 4) && EventSystem.current.currentSelectedGameObject == null && !RealtimeCameraEditActive && !gizmo.isTransforming)
        {
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                if (playing)
                    playing = false;
                if (tabnum != 2)
                {
                    timeline -= 1f / totalLength / 15f;
                    timeline = Mathf.Round(timeline * totalLength * 15f) / 15f / totalLength;
                    timeline = Mathf.Clamp(timeline, TimelineLock && cameratrack.Count > 0 ? cameratrack[curcam].Item2[cursegment].Time + 0.001f : 0f, TimelineLock && cameratrack.Count > 0 && cursegment < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[cursegment + 1].Time - 0.001f : 1f);
                }
                else
                {
                    if (animtabindex > 0)
                        animtabindex -= 1;
                    timeline = animtabindex / 15f / totalLength;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Period))
            {
                if (playing)
                    playing = false;
                if (tabnum != 2)
                {
                    timeline += 1f / totalLength / 15f;
                    timeline = Mathf.Round(timeline * totalLength * 15f) / 15f / totalLength;
                    if (tabnum != 2)
                        timeline = Mathf.Clamp(timeline, TimelineLock && cameratrack.Count > 0 ? cameratrack[curcam].Item2[cursegment].Time + 0.001f : 0f, TimelineLock && cameratrack.Count > 0 && cursegment < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[cursegment + 1].Time - 0.001f : 1f);
                }
                else
                {
                    if (animtabindex < PreviewTimeline[5].maxValue)
                        animtabindex += 1;
                    timeline = animtabindex / 15f / totalLength;
                }
            }
        }

        if ((tabnum == 3 || tabnum == 4) && EventSystem.current.currentSelectedGameObject == null && !RealtimeCameraEditActive && !Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Space))
                TogglePlay();
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (playing)
                    playing = false;
                if (Input.GetKey(KeyCode.DownArrow))
                    timeline -= Time.deltaTime / totalLength * 0.5f;
                else if (Input.GetKey(KeyCode.UpArrow))
                    timeline -= Time.deltaTime / totalLength * 2f;
                else
                    timeline -= Time.deltaTime / totalLength;
                if (TimelineLock && cameratrack.Count > 0)
                {
                    if (timeline < cameratrack[curcam].Item2[cursegment].Time + 0.001f)
                        timeline = cameratrack[curcam].Item2[cursegment].Time + 0.001f;
                } else {
                    if (timeline < 0f)
                        timeline = 0f;
                }
            } else if (Input.GetKey(KeyCode.RightArrow))
            {
                if (playing)
                    playing = false;
                if (Input.GetKey(KeyCode.DownArrow))
                    timeline += Time.deltaTime / totalLength * 0.5f;
                else if (Input.GetKey(KeyCode.UpArrow))
                    timeline += Time.deltaTime / totalLength * 2f;
                else
                    timeline += Time.deltaTime / totalLength;
                if (TimelineLock && cameratrack.Count > 0)
                {
                    float max = cursegment < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[cursegment + 1].Time - 0.001f : 1f;
                    if (timeline >= max)
                        timeline = max;
                } else {
                    if (timeline >= 1f)
                        timeline = 1f;
                }
            }
            if (Input.GetKeyDown(KeyCode.Home))
                timeline = TimelineLock && cameratrack.Count > 0 ? cameratrack[curcam].Item2[cursegment].Time + 0.001f : 0f;
            else if (Input.GetKeyDown(KeyCode.End))
                timeline = TimelineLock && cameratrack.Count > 0 && cursegment < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[cursegment + 1].Time - 0.001f : 1f;
        }
        
        FloorGrid.transform.position = new Vector3((tabnum < 3 ? EditorCamera : SceneCamera).transform.position.x, 0f, (tabnum < 3 ? EditorCamera : SceneCamera).transform.position.z);

        if (!gizmo.isTransforming)
        {
            timeinsec = timeline * totalLength;
            foreach (NISLoader.Animation anim in anims)
            {
                NISLoader.ApplyCarMovement(ObjectsOnScene, anim, tabnum == 2 ? animtabindex : timeinsec, forceY, tabnum == 2);
            }
            for (int sk = 0; sk < skeletons.Count; sk++)
            {
                //try {
                    NISLoader.ApplyBoneAnimation(skeletons[sk].bones, skeletonAnims[skeletons[sk].animationName], tabnum == 2 ? animtabindex : timeinsec, tabnum == 2, ObjectsOnScene[skeletonAnims[skeletons[sk].animationName].GetObjectName()]);
                //} catch {}
            }
        }

        if (focusobj >= 0 && tabnum == 4)
        {
            SceneCamera.transform.position = ObjectsOnScene[alreadyDid[focusobj]].position - ObjectsOnScene[alreadyDid[focusobj]].forward * 6f + Vector3.up * 2f;
            SceneCamera.transform.LookAt(ObjectsOnScene[alreadyDid[focusobj]].position + Vector3.up * 1.5f);
            SceneCamera.focalLength = 25f;
            return;
        }

        if (RealtimeCameraEditActive)
        {
            if (Input.GetMouseButton(1))
            {
                if (CursorType != 1)
                    CursorType = 1;
                SceneCamera.transform.Translate(Vector3.right * -Input.GetAxis("Mouse X") * 0.3f);
                SceneCamera.transform.Translate(SceneCamera.transform.up * -Input.GetAxis("Mouse Y") * 0.3f, Space.World);
                return;
            }
            if (DragMouse.pointerin_counter == 0) {
                if (CursorType != 0)
                    CursorType = 0;
            } else {
                if (CursorType != 4)
                    CursorType = 4;
            }

            bool moved = false;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                SceneCamera.transform.position += SceneCamera.transform.forward * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 100f : 10f);
                moved = true;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                SceneCamera.transform.position -= SceneCamera.transform.forward * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 100f : 10f);
                moved = true;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                SceneCamera.transform.position += SceneCamera.transform.right * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 100f : 10f);
                moved = true;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                SceneCamera.transform.position -= SceneCamera.transform.right * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 100f : 10f);
                moved = true;
            }

            if (moved)
                return;

            Quaternion oldrot;
            if (!gizmo2.isTransforming) {
                oldfocus = FocusSphere.transform.position;
                oldrot = SceneCamera.transform.rotation;
                SceneCamera.transform.LookAt(FocusSphere);
                SceneCamera.transform.rotation = Quaternion.RotateTowards(oldrot, SceneCamera.transform.rotation, Time.deltaTime * 100f);
                SceneCamera.transform.eulerAngles = new Vector3(SceneCamera.transform.eulerAngles.x, SceneCamera.transform.eulerAngles.y, 0f);
                
                Vector3 eyepos;
                Vector3 lookpos;
                switch (cameratrack[curcam].Item2[cursegment].attributes[4])
                {
                    case 0:
                        Transform player_car = ObjectsOnScene.ContainsKey("Car1") ? ObjectsOnScene["Car1"] : SceneRoot;
                        eyepos = Quaternion.Euler(0f, -(-90f + player_car.eulerAngles.y), 0f) * (SceneCamera.transform.position - player_car.position);
                        lookpos = Quaternion.Euler(0f, -(-90f + player_car.eulerAngles.y), 0f) * (FocusSphere.transform.position - player_car.position);
                        break;
                    default:
                        eyepos = SceneCamera.transform.position;
                        lookpos = FocusSphere.transform.position;
                        break;
                }
                CameraEditValues[1].text = eyepos.x.ToString(CultureInfo.InvariantCulture);
                CameraEditValues[2].text = eyepos.z.ToString(CultureInfo.InvariantCulture);
                CameraEditValues[3].text = eyepos.y.ToString(CultureInfo.InvariantCulture);
                CameraEditValues[4].text = lookpos.x.ToString(CultureInfo.InvariantCulture);
                CameraEditValues[5].text = lookpos.z.ToString(CultureInfo.InvariantCulture);
                CameraEditValues[6].text = lookpos.y.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                oldrot = SceneCamera.transform.rotation;
                SceneCamera.transform.LookAt(oldfocus);
                SceneCamera.transform.rotation = Quaternion.RotateTowards(oldrot, SceneCamera.transform.rotation, Time.deltaTime * 100f);
                SceneCamera.transform.eulerAngles = new Vector3(SceneCamera.transform.eulerAngles.x, SceneCamera.transform.eulerAngles.y, 0f);
            }
        }
        timeinsec = timeline * totalLength;
        if (camerasplines.Count > 0)
        {
            for (int i = 0; i < camerasplines.Count; i++)
            {
                if (timeline >= camerasplines[i].start && timeline <= camerasplines[i].end)
                {
                    NISLoader.ApplyCameraMovement(camerasplines[i], timeline, timeinsec, ObjectsOnScene.ContainsKey("Car1") ? ObjectsOnScene["Car1"] : SceneRoot, SceneCamera, FocusSphere, RealtimeCameraEditActive);
                    break;
                }
            }
        }
        else
        {
            SceneCamera.transform.position = EditorCamera.transform.position;
            SceneCamera.transform.rotation = EditorCamera.transform.rotation;
            SceneCamera.focalLength = 50f;
        }
    }

    private Vector3 oldfocus;
    
    public GameObject EnableWorldLoadingButton;
    private bool WorldIsDisplayed;

    public void EnableWorldLoading()
    {
        WorldIsDisplayed = true;
        EnableWorldLoadingButton.SetActive(false);
        WorldChunksStreamer.Initialize();
        WorldChunksStreamer www = FindObjectOfType<WorldChunksStreamer>();
        www.RequestInitialNeededChunks();
        www.StartCoroutine(www.ChunksAutoOnDemandLoaderCoroutine());
    }

    void UpdateGameSelection()
    {
        UpdateGame(GameDropdown.value);
        GameDirectory = PlayerPrefs.GetString("GameDir");
        foreach (GameObject obj in HiddenElementsNoGame)
            obj.SetActive(GameDirectory.Length > 0);
        EnableWorldLoadingButton.SetActive(GameDirectory.Length > 0 && !WorldIsDisplayed);
        if (GameDirectory.Length > 0) {
            HelpText.text = "Current game directory: .../" + Path.GetFileName(GameDirectory);
            UpdateNisList();
        } else
            HelpText.text = "Select your game directory to begin.";
    }
    
    static void printAllKLength(string set, int k, uint hash, ref string result, ref bool found)
    { 
        int n = set.Length;  
        printAllKLengthRec(set, "", n, k, hash, ref result, ref found);
    } 
    
    static void printAllKLengthRec(string set, string prefix, int n, int k, uint hash, ref string result, ref bool found)
    {
        if (k == 0)  
        {
            if (hash == NISLoader.BinHash(prefix))
            {
                found = true;
                result = prefix;
            }
            return;
        }
        for (int i = 0; i < n; ++i) 
        {
            string newPrefix = prefix + set[i];
            printAllKLengthRec(set, newPrefix, n, k - 1, hash, ref result, ref found);
            if (found)
                return;
        } 
    }

    private static string[] extraCamBanks =
    {
        "Cinematics"
    };

    public void UpdateNisList()
    {
        foreach (RectTransform tr in NisList)
            Destroy(tr.gameObject);
        if (Directory.Exists(GameDirectory + "/NIS/"))
        {
            string[] f2 = Directory.GetFiles(GameDirectory + "/NIS/");
            bool first_passed = false;
            Dictionary<uint, string> knownNames = new Dictionary<uint, string>();
            foreach (string bun in f2)
            {
                string filename = Path.GetFileName(bun);
                if (filename.StartsWith("Scene_") && filename.EndsWith("BundleB.bun") || filename.StartsWith("SCENE_") && filename.EndsWith("BUNDLEB.BUN"))
                {
                    string nisname = filename.Substring(6).Split('_')[0];
                    knownNames.Add(NISLoader.BinHash(nisname), nisname);
                }
            }
            Dictionary<uint, string> extraNames = new Dictionary<uint, string>();
            foreach (string bank in extraCamBanks)
                extraNames.Add(NISLoader.BinHash(bank), bank);
            byte[] f = null;
            string path = "/GLOBAL/InGameB.lzc";
            if (File.Exists(GameDirectory + path))
                f = File.ReadAllBytes(GameDirectory + path);
            else if (File.Exists(GameDirectory + path.ToUpper()))
                f = File.ReadAllBytes(GameDirectory + path.ToUpper());
            f = NISLoader.DecompressJZC(f);
            
            ToggleGroup group = NisList.GetComponent<ToggleGroup>();
            GameObject entry;
            int _group = -1;
            if (f != null)
            {
                for (int i = 0; i < f.Length; i += 4)
                {
                    if (f[i] == 0x00 && f[i + 1] == 0xB2 && f[i + 2] == 0x03 && f[i + 3] == 0x80)
                    {
                        i += 4;
                        _group = 0;
                    }
                    if (f[i] == 0x01 && f[i + 1] == 0xB2 && f[i + 2] == 0x03 && f[i + 3] == 0x80)
                    {
                        i += 4;
                        _group = 1;
                    }
                    if (f[i] == 0x02 && f[i + 1] == 0xB2 && f[i + 2] == 0x03 && f[i + 3] == 0x80)
                    {
                        i += 4;
                        _group = 2;
                    }
                    if (f[i] == 0x03 && f[i + 1] == 0xB2 && f[i + 2] == 0x03 && f[i + 3] == 0x80)
                    {
                        i += 4;
                        _group = 3;
                    }
                    if (f[i] == 0x10 && f[i + 1] == 0xB2 && f[i + 2] == 0x03 && f[i + 3] == 0x00)
                    {
                        i += 4;
                        int end = i + BitConverter.ToInt32(f, i);
                        i += 4;
                        uint NISHash = BitConverter.ToUInt32(f, i);
                        i = end - 4;
                        while (i % 4 != 0)
                            i--;
                        entry = Instantiate(NisListPrefab, NisList);
                        string nnn = knownNames.ContainsKey(NISHash) ? knownNames[NISHash] : extraNames.ContainsKey(NISHash) ? extraNames[NISHash] : "0x" + NISHash.ToString("X8");
                        entry.transform.GetChild(1).GetComponent<Text>().text = "[" + _group + "] " + nnn;
                        entry.GetComponent<Toggle>().group = group;
                        if (knownNames.ContainsKey(NISHash))
                            knownNames.Remove(NISHash);
                        else
                        {
                            entry.transform.GetChild(1).GetComponent<Text>().color = Color.yellow;
                        }
                        if (!first_passed)
                        {
                            entry.GetComponent<Toggle>().isOn = true;
                            first_passed = true;
                        }
                    }
                }
            }
            foreach (uint n in knownNames.Keys)
            {
                entry = Instantiate(NisListPrefab, NisList);
                entry.transform.GetChild(1).GetComponent<Text>().text = knownNames[n];
                entry.GetComponent<Toggle>().group = group;
                entry.transform.GetChild(1).GetComponent<Text>().color = Color.cyan;
                if (!first_passed)
                {
                    entry.GetComponent<Toggle>().isOn = true;
                    first_passed = true;
                }
            }
        }
    }
    
    List<NISLoader.Animation> anims;
    List<NISLoader.Skeleton> skeletons;
    Dictionary<string, NISLoader.Animation> skeletonAnims;
    [HideInInspector]
    public List<(NISLoader.CameraTrackHeader, NISLoader.CameraTrackEntry[])> cameratrack;
    private List<NISLoader.CameraSpline> camerasplines;
    private int cameratrack_offset;
    [HideInInspector]
    public float totalLength;
    private string nisname;
    private bool activated;
    public static bool NISSaveDisabled;
    public static bool CameraSaveDisabled;
    public static int LoggingMode;

    public InputField ImportingLZCPath;
    public RectTransform CameraTrackListImport;
    List<(NISLoader.CameraTrackHeader, NISLoader.CameraTrackEntry[])> ImportCamEntries;
    public Button ImpExpButton;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReplayFrame
    {
        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotW;
        public float posX;
        public float posY;
        public float posZ;
        public float unk1;
        public float unk2;
        public float unk3;
        public float unk4;
        public float unk5;
        public float unk6;
        public float unk7;
        public float unk8;
        public float unk9;
        public float unkX;
        public float unkY;
        public float unkZ;
    }

    public void ImportCameraTrack()
    {
        if (CameraTrackListImport.childCount == 0) return;
        Toggle t = CameraTrackListImport.GetComponent<ToggleGroup>().ActiveToggles().ToArray()[0];
        int camtrack = t.transform.GetComponent<RectTransform>().GetSiblingIndex();
        cameratrack.Add(ImportCamEntries[camtrack]);
        Dropdown.OptionData opt = new Dropdown.OptionData();
        opt.text = ImportCamEntries[camtrack].Item1.TrackName;
        int oldcur = curcam;
        AddToHistoryCam(new CreateCameraTrack(cameratrack.Count - 1, oldcur, cameratrack[cameratrack.Count - 1]));
        dontUseHistory = true;
        CameraTrackSelection.options.Add(opt);
        CameraTrackSelection.value = 0;
        CameraTrackSelection.value = cameratrack.Count - 1;
        updlimit = false;
        ChangeCameraTrack(cameratrack.Count - 1);
        dontUseHistory = false;
        Debug.Log("Imported " + ImportCamEntries[camtrack].Item1.TrackName);
    }

    public void OpenNISForImporting()
    {
        foreach (RectTransform tr in CameraTrackListImport)
            Destroy(tr.gameObject);
        if (NisList2.childCount == 0) return;
        Toggle t = NisList2.GetComponent<ToggleGroup>().ActiveToggles().ToArray()[0];
        string nisname = t.transform.GetChild(1).GetComponent<Text>().text;
        bool usehash = false;
        uint hash = 0;
        if (nisname.StartsWith("0x"))
        {
            usehash = true;
            hash = Convert.ToUInt32(nisname.Substring(2), 16);
        }
        ImportCamEntries = null;
        int off;
        string path = ImportingLZCPath.text;
        if (string.IsNullOrEmpty(path))
            path = GameDirectory;
        if (!File.Exists(path) && !Directory.Exists(path))
            return;
        string gamepath, filepath;
        if (File.Exists(path))
        {
            gamepath = GameDirectory;
            filepath = path;
        }
        else
        {
            gamepath = path;
            filepath = path + "/GLOBAL/InGameB.lzc";
            if (!File.Exists(filepath))
            {
                filepath = path + "/GLOBAL/INGAMEB.LZC";
                if (!File.Exists(filepath))
                    return;
            }
        }
        if (usehash)
        {
            (off, ImportCamEntries) = NISLoader.LookupCamera(filepath, hash, true);
        }
        else
        {
            (off, ImportCamEntries) = NISLoader.LoadCameraTrack(nisname, filepath, true);
        }
        GameObject entry;
        ToggleGroup group = CameraTrackListImport.GetComponent<ToggleGroup>();
        bool first_passed = false;
        foreach (var cam in ImportCamEntries)
        {
            entry = Instantiate(NisListPrefab, CameraTrackListImport);
            string nnn = cam.Item1.TrackName;
            entry.transform.GetChild(1).GetComponent<Text>().text = nnn;
            entry.GetComponent<Toggle>().group = group;
            if (!first_passed)
            {
                entry.GetComponent<Toggle>().isOn = true;
                first_passed = true;
            }
        }
    }

    public void SelectLZCForImporting()
    {
        foreach (RectTransform tr in NisList2)
            Destroy(tr.gameObject);
        foreach (RectTransform tr in CameraTrackListImport)
            Destroy(tr.gameObject);
        ImportCamEntries = null;
        string path = ImportingLZCPath.text;
        if (string.IsNullOrEmpty(path))
            path = GameDirectory;
        if (!File.Exists(path) && !Directory.Exists(path))
            return;
        string gamepath, filepath;
        if (File.Exists(path))
        {
            gamepath = GameDirectory;
            filepath = path;
        } else {
            gamepath = path;
            filepath = path + "/GLOBAL/InGameB.lzc";
            if (!File.Exists(filepath))
            {
                filepath = path + "/GLOBAL/INGAMEB.LZC";
                if (!File.Exists(filepath))
                    return;
            }
        }
        string[] f2 = Directory.GetFiles(gamepath + "/NIS/");
        bool first_passed = false;
        Dictionary<uint, string> knownNames = new Dictionary<uint, string>();
        foreach (string bun in f2)
        {
            string filename = Path.GetFileName(bun);
            if (filename.StartsWith("Scene_") && filename.EndsWith("BundleB.bun") || filename.StartsWith("SCENE_") && filename.EndsWith("BUNDLEB.BUN"))
            {
                string nisname = filename.Substring(6).Split('_')[0];
                knownNames.Add(NISLoader.BinHash(nisname), nisname);
            }
        }
        byte[] f = File.ReadAllBytes(filepath);
        f = NISLoader.DecompressJZC(f);
        
        ToggleGroup group = NisList2.GetComponent<ToggleGroup>();
        GameObject entry;
        if (f != null)
        {
            for (int i = 0; i < f.Length; i += 4)
            {
                if (f[i] == 0x10 && f[i + 1] == 0xB2 && f[i + 2] == 0x03 && f[i + 3] == 0x00)
                {
                    i += 4;
                    int end = i + BitConverter.ToInt32(f, i);
                    i += 4;
                    uint NISHash = BitConverter.ToUInt32(f, i);
                    i = end - 4;
                    while (i % 4 != 0)
                        i--;
                    entry = Instantiate(NisListPrefab, NisList2);
                    string nnn = knownNames.ContainsKey(NISHash) ? knownNames[NISHash] : "0x" + NISHash.ToString("X8");
                    entry.transform.GetChild(1).GetComponent<Text>().text = nnn;
                    entry.GetComponent<Toggle>().group = group;
                    if (knownNames.ContainsKey(NISHash))
                        knownNames.Remove(NISHash);
                    else
                    {
                        entry.transform.GetChild(1).GetComponent<Text>().color = Color.yellow;
                    }
                    if (!first_passed)
                    {
                        entry.GetComponent<Toggle>().isOn = true;
                        first_passed = true;
                    }
                }
            }
        }
    }

    int elftablelength;

    public void OpenNIS()
    {
        ClearHistory();
        NISSaveDisabled = false;
        CameraSaveDisabled = false;
        miscoffsets.Clear();
        Toggle t = NisList.GetComponent<ToggleGroup>().ActiveToggles().ToArray()[0];
        NISLoader.SceneInfo = new NISLoader.NisScene();
        string rawn = t.transform.GetChild(1).GetComponent<Text>().text;
        nisname = rawn.Contains("]") ? rawn.Substring(rawn.IndexOf(']') + 2) : rawn;
        bool usehash = false;
        uint hash = 0;
        if (nisname.StartsWith("0x"))
        {
            usehash = true;
            hash = Convert.ToUInt32(nisname.Substring(2), 16);
        }
        elftablelength = 0;
        if (usehash)
        {
            anims = new List<NISLoader.Animation>();
            skeletons = new List<NISLoader.Skeleton>();
            LoggingMode = 2;
            (cameratrack_offset, cameratrack) = NISLoader.LookupCamera(GameDirectory, hash);
            LoggingMode = 0;
            foreach (GameObject obj in HiddenWhenNoNIS)
                obj.SetActive(false);
        }
        else
        {
            LoggingMode = 1;
            (anims, skeletons) = NISLoader.LoadAnimations(nisname, GameDirectory);

            elftablelength += 8;
            elftablelength += 8;
            elftablelength += 8;
            for (int i = anims.Count - 1; i >= 0; i--)
            {
                elftablelength += 8;
            }
            for (int i = anims.Count - 1; i >= 0; i--)
            {
                elftablelength += 8;
            }
            for (int i = anims.Count - 1; i >= 0; i--)
            {
                elftablelength += 8;
                elftablelength += 8;
                for (int childNum = anims[i].subAnimations.Count - 1; childNum >= 0; childNum--)
                {
                    elftablelength += 8;
                }
            }
            elftablelength += 4;
            elftablelength += 4;

            LoggingMode = 0;

            NISLoader.SceneType sceneType = (NISLoader.SceneType) NISLoader.SceneInfo.SceneType;
            LoggingMode = 2;
            (cameratrack_offset, cameratrack) = NISLoader.LoadCameraTrack(nisname, GameDirectory);
            /*for (int i = 0; i < cameratrack[0].Item2.Length; i++)
            {
                Debug.Log("entry " + i + " " + cameratrack[0].Item2[i].Time);
            }*/
            LoggingMode = 0;
            foreach (GameObject obj in HiddenWhenNoNIS)
                obj.SetActive(anims.Count > 0);
            HiddenWhenNoNIS[1].SetActive(true);
            NISProps[0].text = NISLoader.SceneInfo.SceneName;
            NISProps[1].text = ((NISLoader.SceneType)NISLoader.SceneInfo.SceneType).ToString();
            NISProps[2].text = NISLoader.SceneInfo.ICEContext.ToString();
            NISProps[3].text = NISLoader.SceneInfo.HaveLayout != 0 ? "Yes" : "No";
            NISProps[4].text = NISLoader.SceneInfo.HaveCarAnimation != 0 ? "Yes" : "No";
            NISProps[5].text = NISLoader.SceneInfo.VanishFrame.ToString();
            NISProps[6].text = NISLoader.SceneInfo.SeeulatorOverlayName;
            NISProps[7].text = NISLoader.SceneDescription;
            float calcdur = NISLoader.CalculateNISDuration(anims);
            NISProps[8].text = (int) Mathf.Floor(calcdur / 60f) + ":" + (calcdur % 60f).ToString("0.00").PadLeft(5, '0');
            NISProps[9].text = NISLoader.toollibver;
        }
        UpdateObjectList();
        editorCameraMovement.target.position = Vector3.zero;
        foreach (GameObject obj in HiddenElements)
            obj.SetActive(true);
        PreviewToggle.isOn = true;
        activated = true;
        CameraTrackSelection.options = new List<Dropdown.OptionData>();
        Dropdown.OptionData opt;
        foreach (var cam in cameratrack)
        {
            opt = new Dropdown.OptionData();
            opt.text = cam.Item1.TrackName;
            CameraTrackSelection.options.Add(opt);
        }
        CameraTrackSelection.value = 0;
        editorTimelineMin = 0f;
        editorTimelineMax = 1f;
        updlimit = false;
        ChangeCameraTrack(0);
        TimelineLock = false;
        TimelineLockToggle.isOn = false;
        ImpExpButton.interactable = true;
    }

    void UpdateObjectList()
    {
        foreach (string car in ObjectsOnScene.Keys)
            Destroy(ObjectsOnScene[car].gameObject);
        ObjectsOnScene.Clear();
        alreadyDid = new List<string>();
        skeletonAnims = new Dictionary<string, NISLoader.Animation>();
        List<NISLoader.Skeleton> skeletonsUnused = skeletons.ToList();
        foreach (NISLoader.Animation anim in anims)
        {
            string animobj = anim.GetObjectName();
            if (!alreadyDid.Contains(animobj))
            {
                Transform target = new GameObject(animobj).transform;
                target.SetParent(SceneRoot);
                if (anim.type == NISLoader.AnimType.ANIM_COMPOUND && anim.subAnimations.Count > 1)
                {
                    NISLoader.Skeleton skeleton = null;
                    foreach (NISLoader.Skeleton s in skeletonsUnused)
                    {
                        if (s.animationName == animobj.ToUpper() || s.animationName.StartsWith(animobj.ToUpper()))
                        {
                            skeleton = s;
                            break;
                        }
                    }

                    // HAX
                    if (skeleton == null && skeletonsUnused.Count == 1)
                    {
                        skeleton = skeletonsUnused[0];
                        skeleton.animationName = animobj.ToUpper();
                    }

                    if (skeleton == null)
                    {
                        foreach (NISLoader.Skeleton s in skeletonsUnused)
                        {
                            if (s.bones.Length == anim.subAnimations[0].delta[0].Length / 4)
                            {
                                skeleton = s;
                                skeleton.animationName = animobj.ToUpper();
                                break;
                            }
                        }
                    }

                    if (skeleton == null)
                    {
                        Debug.Log("Can't find skeleton for object " + animobj + " (skeletons loaded: " + string.Join(", ", skeletons.Select(x => x.animationName)) + ")");
                    } else
                    {
                        skeletonAnims.Add(skeleton.animationName, anim);
                        NISLoader.GenerateSkinnedObject(target, skeleton);
                        skeletonsUnused.Remove(skeleton);
                        foreach (NISLoader.Bone bone in skeleton.bones)
                            Instantiate(BoneSphere, bone.assignedTransform).transform.localPosition = Vector3.zero;
                    }
                }
                else
                {
                    if (animobj.StartsWith("Car"))
                    {
                        if (animobj == "Car1")
                            Instantiate(CarPrefabPlayer, target);
                        else
                            Instantiate(CarPrefabOpponent, target);
                    }
                    else if (animobj.StartsWith("Cop"))
                    {
                        Instantiate(CarPrefabCop, target);
                    }
                    else
                    {
                        // todo
                    }
                }
                ObjectsOnScene.Add(animobj, target);
                alreadyDid.Add(animobj);
            }
        }
        Dropdown.OptionData opt;
        FocusDropdown.options = new List<Dropdown.OptionData>();
        opt = new Dropdown.OptionData();
        opt.text = "Nothing";
        FocusDropdown.options.Add(opt);
        foreach (string n in alreadyDid)
        {
            opt = new Dropdown.OptionData();
            opt.text = n;
            FocusDropdown.options.Add(opt);
        }
        FocusDropdown.value = 0;
        focusobj = -1;
        foreach (Transform child in Objectlist)
        {
            if (child.name == "Top" || child.name == "Bottom")
                continue;
            Destroy(child.gameObject);
        }
        foreach (Transform child in Objectlist2)
        {
            if (child.name == "Top" || child.name == "Bottom")
                continue;
            Destroy(child.gameObject);
        }
        GameObject entry;
        bool firstpass = false;
        foreach (string n in alreadyDid)
        {
            string nnn = n;
            entry = Instantiate(ObjectListPrefab, Objectlist);
            entry.transform.GetChild(1).GetComponent<Text>().text = nnn;
            entry.GetComponent<Toggle>().group = ObjectListGroup;
            if (!firstpass)
            {
                firstpass = true;
                entry.GetComponent<Toggle>().isOn = true;
                currentlyEditingObject = nnn;
            }
            entry.GetComponent<Toggle>().onValueChanged.AddListener(x => AnimationsEditorObjectSelected(nnn));
        }
        firstpass = false;
        foreach (NISLoader.Animation anim in anims)
        {
            string nnn = anim.name;
            entry = Instantiate(NisListPrefab, Objectlist2);
            entry.transform.GetChild(1).GetComponent<Text>().text = nnn;
            entry.GetComponent<Toggle>().group = ObjectListGroup2;
            if (!firstpass)
            {
                firstpass = true;
                entry.GetComponent<Toggle>().isOn = true;
                ObjectNameField.text = nnn;
            }
            entry.GetComponent<Toggle>().onValueChanged.AddListener(x => {
                ObjectNameField.text = nnn;
            });
        }
        entry = Instantiate(ObjectListPrefab, Objectlist);
        entry.transform.GetChild(1).GetComponent<Text>().text = "Edit list...";
        entry.transform.GetChild(1).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.5f);
        entry.GetComponent<Toggle>().onValueChanged.AddListener(x => {
            if (!x) return;
            EditObjListMenu.SetActive(true);
            HelpButton(true);
            entry.GetComponent<Toggle>().isOn = false;
        });
    }

    public GameObject BoneSphere;

    public class RenameAnimation : HistoryEntry
    {
        public RenameAnimation(int _animnum, string _oldname, string _newname)
        {
            animnum = _animnum;
            oldname = _oldname;
            newname = _newname;
        }

        private int animnum;
        private string oldname;
        private string newname;

        public override void Apply()
        {
            Main main = FindObjectOfType<Main>();
            main.anims[animnum].name = newname;
            main.UpdateObjectList();
        }

        public override void Restore()
        {
            Main main = FindObjectOfType<Main>();
            main.anims[animnum].name = oldname;
            main.UpdateObjectList();
        }

        public override string ToString()
        {
            return "rename animation";
        }
    }

    public class CreateAnimation : HistoryEntry
    {
        public CreateAnimation(NISLoader.Animation _anim)
        {
            anim = _anim;
        }

        private NISLoader.Animation anim;

        public override void Apply()
        {
            Main main = FindObjectOfType<Main>();
            main.anims.Add(anim);
            main.UpdateObjectList();
        }

        public override void Restore()
        {
            Main main = FindObjectOfType<Main>();
            main.anims.Remove(anim);
            main.UpdateObjectList();
        }

        public override string ToString()
        {
            return "create animation";
        }
    }

    public class RemoveAnimation : HistoryEntry
    {
        public RemoveAnimation(int _animnum, NISLoader.Animation _anim)
        {
            animnum = _animnum;
            anim = _anim;
        }

        private int animnum;
        private NISLoader.Animation anim;

        public override void Apply()
        {
            Main main = FindObjectOfType<Main>();
            main.anims.Remove(anim);
            main.UpdateObjectList();
        }

        public override void Restore()
        {
            Main main = FindObjectOfType<Main>();
            main.anims.Insert(animnum, anim);
            main.UpdateObjectList();
        }

        public override string ToString()
        {
            return "remove animation";
        }
    }

    string allowedObjLetters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890_";

    public void ApplyAnimName()
    {
        if (ObjectListGroup2.ActiveToggles().ToArray().Length == 0) return;
        Toggle t = ObjectListGroup2.ActiveToggles().ToArray()[0];
        int animnum = t.transform.GetComponent<RectTransform>().GetSiblingIndex();
        int counter = 0;
        foreach (char c in ObjectNameField.text)
        {
            if (c == '_')
                counter++;
            if (!allowedObjLetters.Contains(c))
            {
                Debug.Log("Animation name contains symbols that might break the game.");
                return;
            }
        }
        if (counter < 2 || !ObjectNameField.text.EndsWith("_t") && !ObjectNameField.text.EndsWith("_q") && !ObjectNameField.text.EndsWith("_s"))
        {
            Debug.Log("Incorrect name format. Please try to stick to this format: <i>(NIS name)_(Object name)_(type: t, q, s)</i>");
            return;
        }
        if (anims[animnum].name == ObjectNameField.text || string.IsNullOrEmpty(ObjectNameField.text)) return;
        foreach (NISLoader.Animation anim in anims)
        {
            if (anim.name == ObjectNameField.text)
            {
                Debug.Log("Animation with same name already exists.");
                return;
            }
        }
        AddToHistoryNIS(new RenameAnimation(animnum, anims[animnum].name, ObjectNameField.text));
        anims[animnum].name = ObjectNameField.text;
        UpdateObjectList();
    }

    public void CreateNewAnim()
    {
        if (ObjectListGroup2.ActiveToggles().ToArray().Length == 0) return;
        int counter = 0;
        foreach (char c in ObjectNameField.text)
        {
            if (c == '_')
                counter++;
            if (!allowedObjLetters.Contains(c))
            {
                Debug.Log("Animation name contains symbols that might break the game.");
                return;
            }
        }
        if (counter < 2 || !ObjectNameField.text.EndsWith("_t") && !ObjectNameField.text.EndsWith("_q") && !ObjectNameField.text.EndsWith("_s"))
        {
            Debug.Log("Incorrect name format. Please try to stick to this format: <i>(NIS name)_(Object name)_(type: t, q, s)</i>");
            return;
        }
        if (string.IsNullOrEmpty(ObjectNameField.text)) return;
        foreach (NISLoader.Animation anim in anims)
        {
            if (anim.name == ObjectNameField.text)
            {
                Debug.Log("Animation with same name already exists.");
                return;
            }
        }
        NISLoader.Animation newAnimParent = new NISLoader.Animation();
        newAnimParent.name = ObjectNameField.text;
        newAnimParent.type = NISLoader.AnimType.ANIM_COMPOUND;
        newAnimParent.checkSum = 0; // todo
        newAnimParent.subAnimations = new List<NISLoader.Animation>();
        newAnimParent.subAnimations.Add(new NISLoader.Animation());
        NISLoader.Animation newAnim = newAnimParent.subAnimations[0];
        if (ObjectNameField.text.EndsWith("_q"))
        {
            newAnim.type = NISLoader.AnimType.ANIM_DELTAQUAT;
            newAnim.checkSum = 0; // todo
            newAnim.numDofs = 4;
            newAnim.quantBits = 0x10;
            newAnim.unk1 = 16;
        } else
        {
            newAnim.type = NISLoader.AnimType.ANIM_DELTALERP;
            newAnim.checkSum = 0; // todo
            newAnim.numDofs = 3;
            newAnim.quantBits = 0x10;
            if (ObjectNameField.text.EndsWith("_t"))
            {
                newAnim.unk1 = 12;
                newAnim.unk2 = 13;
                newAnim.unk3 = 14;
            } else
            {
                newAnim.unk1 = 20;
                newAnim.unk2 = 21;
                newAnim.unk3 = 22;
            }
        }
        int deltacount = 2;
        foreach (NISLoader.Animation anim in anims)
            if (anim.name.StartsWith(ObjectNameField.text.Substring(0, ObjectNameField.text.Length - 1)))
            {
                deltacount = anim.subAnimations[0].delta.Length;
                break;
            }
        newAnim.delta = new float[deltacount][];
        for (int i = 0; i < deltacount; i++)
        {
            newAnim.delta[i] = new float[newAnim.numDofs];
            if (newAnim.numDofs == 4)
            {
                newAnim.delta[i][0] = Quaternion.identity.x;
                newAnim.delta[i][1] = Quaternion.identity.y;
                newAnim.delta[i][2] = Quaternion.identity.z;
                newAnim.delta[i][3] = Quaternion.identity.w;
            }
        }
        AddToHistoryNIS(new CreateAnimation(newAnimParent));
        anims.Add(newAnimParent);
        UpdateObjectList();
    }

    public void RemoveAnim()
    {
        if (ObjectListGroup2.ActiveToggles().ToArray().Length == 0) return;
        Toggle t = ObjectListGroup2.ActiveToggles().ToArray()[0];
        int animnum = t.transform.GetComponent<RectTransform>().GetSiblingIndex();
        if (anims.Count <= 1)
        {
            Debug.Log("Idk why you hate them so much but you can't remove all of them I'm sorry");
            return;
        }
        AddToHistoryNIS(new RemoveAnimation(animnum, anims[animnum]));
        anims.RemoveAt(animnum);
        UpdateObjectList();
    }

    public InputField ObjectNameField;

    public Toggle TimelineLockToggle;

    public string currentlyEditingObject;
    public Transform currentlyEditingSubObject;
    public GameObject SubObjectlistMain;
    List<Transform> subobjs = new List<Transform>();
    public GameObject EditObjListMenu;

    public void AnimationsEditorObjectSelected(string objname)
    {
        rewinded_this_frame = 5;
        currentlyEditingObject = objname;
        foreach (string obj in ObjectsOnScene.Keys)
        {
            ObjectsOnScene[obj].gameObject.SetActive(!HideOtherObjs || obj == objname);
        }
        foreach (Transform child in SubObjectlist)
        {
            if (child.name == "Top" || child.name == "Bottom")
                continue;
            Destroy(child.gameObject);
        }
        GameObject entry;
        bool firstpass = false;
        List<string> objs = new List<string>();
        subobjs.Clear();
        objs.Add("Transform");
        subobjs.Add(ObjectsOnScene[currentlyEditingObject]);
        SubObjectlistMain.SetActive(false);
        foreach (var skeleton in skeletons)
        {
            if (skeleton.animationName == objname.ToUpper() || skeleton.animationName.StartsWith(objname.ToUpper()))
            {
                SubObjectlistMain.SetActive(true);
                objs.Add("Root");
                subobjs.Add(ObjectsOnScene[currentlyEditingObject].GetChild(0));
                ProcessObjChildren(ObjectsOnScene[currentlyEditingObject].GetChild(0), objs, " ");
                break;
            }
        }
        int ind = 0;
        foreach (string n in objs)
        {
            int nnn = ind++;
            entry = Instantiate(ObjectListPrefab, SubObjectlist);
            entry.transform.GetChild(1).GetComponent<Text>().text = n;
            entry.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 20f);
            entry.GetComponent<Toggle>().group = SubObjectListGroup;
            if (!firstpass)
            {
                firstpass = true;
                entry.GetComponent<Toggle>().isOn = true;
            }
            entry.GetComponent<Toggle>().onValueChanged.AddListener(x => AnimationsEditorSubObjectSelected(nnn));
        }
        float max = 0f;
        EditingAnimation_t = null;
        EditingAnimation_q = null;
        EditingAnimation_s = null;
        foreach (var an in anims)
        {
            if (an.name.EndsWith("_" + currentlyEditingObject + "_t") || an.name == currentlyEditingObject + "_t")
            {
                max = Mathf.Max(max, (an.subAnimations.Count == 0 ? an.delta.Length : an.subAnimations[0].delta.Length) - 1);
                EditingAnimation_t = an;
            }
            if (an.name.EndsWith("_" + currentlyEditingObject + "_q") || an.name == currentlyEditingObject + "_q")
            {
                max = Mathf.Max(max, (an.subAnimations.Count == 0 ? an.delta.Length : an.subAnimations[0].delta.Length) - 1);
                EditingAnimation_q = an;
            }
            if (an.name.EndsWith("_" + currentlyEditingObject + "_s") || an.name == currentlyEditingObject + "_s")
            {
                max = Mathf.Max(max, (an.subAnimations.Count == 0 ? an.delta.Length : an.subAnimations[0].delta.Length) - 1);
                EditingAnimation_s = an;
            }
        }
        PreviewTimeline[5].maxValue = max;
        PreviewTimeline[6].maxValue = max;
        interpolation_start = (int)Mathf.Clamp(interpolation_start, 0, max);
        animtabindex = (int)Mathf.Clamp(animtabindex, 0, max);
        AnimationsEditorSubObjectSelected(0);
    }

    void ProcessObjChildren(Transform tr, List<string> l, string deepness)
    {
        foreach (Transform child in tr)
        {
            if (child.name.Contains("BoneSphere"))
                continue;
            l.Add(deepness + child.gameObject.name.Split('.').Last());
            subobjs.Add(child);
            ProcessObjChildren(child, l, deepness + " ");
        }
    }

    public Text ObjectEditingTitle;
    public GameObject RootSubEdit;
    public GameObject BoneSubEdit;
    public GameObject LayoutSubEdit;
    private NISLoader.Animation EditingAnimation_t;
    private NISLoader.Animation EditingAnimation_q;
    private NISLoader.Animation EditingAnimation_s;

    public void AnimationsEditorSubObjectSelected(int objindex)
    {
        rewinded_this_frame = 5;
        old_ttt = -1;
        string objnn = currentlyEditingObject;
        if (objindex != 0)
        {
            for (int sk = 0; sk < skeletons.Count; sk++)
            {
                if (skeletons[sk].animationName == currentlyEditingObject.ToUpper() || skeletons[sk].animationName.StartsWith(currentlyEditingObject.ToUpper()))
                {
                    objnn = skeletons[sk].name;
                    break;
                }
            }
        }
        ObjectEditingTitle.text = "Edit " + objnn + " -> " + (objindex == 0 ? "Transform" : objindex == 1 ? "Root" : subobjs[objindex].name.Split('.').Last());
        if (gizmo.targetRoots.Count > 0)
            gizmo.RemoveTarget(gizmo.targetRoots.Keys.ToArray()[0]);
        if (objindex == 0)
        {
            currentlyEditingSubObject = ObjectsOnScene[currentlyEditingObject];
            editorCameraMovement.target.position = currentlyEditingSubObject.position;
            gizmo.AddTarget(currentlyEditingSubObject);
            gizmo.transformType = TransformType.Move;
            gizmo.space = subobjs.Count > 1 ? TransformSpace.Global : TransformSpace.Local;
            RootSubEdit.SetActive(subobjs.Count == 1);
            BoneSubEdit.SetActive(false);
            LayoutSubEdit.SetActive(subobjs.Count > 1);
            gizmo.AllowTransformationSwitch = true;
            deltacountinp.interactable = EditingAnimation_t != null;
            deltacountinp.text = EditingAnimation_t != null ? EditingAnimation_t.subAnimations[0].delta.Length.ToString() : "";
            deltacountinpb.interactable = deltacountinp.interactable;
            return;
        }
        currentlyEditingSubObject = subobjs[objindex];
        editorCameraMovement.target.position = currentlyEditingSubObject.position;
        gizmo.AddTarget(currentlyEditingSubObject);
        gizmo.transformType = TransformType.Rotate;
        gizmo.space = TransformSpace.Local;
        RootSubEdit.SetActive(false);
        BoneSubEdit.SetActive(true);
        LayoutSubEdit.SetActive(false);
        gizmo.AllowTransformationSwitch = false;
        deltacountinp.interactable = false;
        deltacountinp.text = "";
        deltacountinpb.interactable = deltacountinp.interactable;
    }

    public InputField[] RootValues;
    public InputField[] BoneValues;
    private bool blockupdcall;

    public void RootValuesChanged()
    {
        if (blockupdcall) return;
        int pos = animtabindex;
        NISLoader.Animation an_t = EditingAnimation_t;
        NISLoader.Animation an_q = EditingAnimation_q;
        NISLoader.Animation an_s = EditingAnimation_s;
        bool changed;
        bool globalchanged = false;
        float[] old;
        float[][] d;
        float[][][] old_delta = new float[3][][];
        if (RootSubEdit.activeSelf)
        {
            if (an_t != null)
            {
                if (an_t.type == NISLoader.AnimType.ANIM_COMPOUND)
                    an_t = an_t.subAnimations[0];
                changed = false;
                if (!gizmo.isTransforming && !dontUseHistory)
                {
                    d = an_t.delta;
                    old_delta[0] = new float[d.Length][];
                    for (int i = 0; i < d.Length; i++)
                    {
                        old_delta[0][i] = new float[d[i].Length];
                        for (int j = 0; j < d[i].Length; j++)
                            old_delta[0][i][j] = d[i][j];
                    }
                }
                try
                {
                    old = an_t.delta[pos];
                    an_t.delta[pos] = new [] { float.Parse(RootValues[0].text, CultureInfo.InvariantCulture), float.Parse(RootValues[1].text, CultureInfo.InvariantCulture), float.Parse(RootValues[2].text, CultureInfo.InvariantCulture) };
                    if (old[0].ToString(CultureInfo.InvariantCulture) != RootValues[0].text || old[1].ToString(CultureInfo.InvariantCulture) != RootValues[1].text || old[2].ToString(CultureInfo.InvariantCulture) != RootValues[2].text)
                        changed = true;
                } catch {}
                if (changed)
                    globalchanged = true;
                else
                    an_t = null;
            }
            if (an_q != null)
            {
                if (an_q.type == NISLoader.AnimType.ANIM_COMPOUND)
                    an_q = an_q.subAnimations[0];
                changed = false;
                if (!gizmo.isTransforming && !dontUseHistory)
                {
                    d = an_q.delta;
                    old_delta[1] = new float[d.Length][];
                    for (int i = 0; i < d.Length; i++)
                    {
                        old_delta[1][i] = new float[d[i].Length];
                        for (int j = 0; j < d[i].Length; j++)
                            old_delta[1][i][j] = d[i][j];
                    }
                }
                try
                {
                    old = an_q.delta[pos];
                    Vector3 ollddd = new Quaternion(old[0], old[1], old[2], old[3]).eulerAngles;
                    Quaternion quat = Quaternion.Euler(float.Parse(RootValues[3].text, CultureInfo.InvariantCulture), float.Parse(RootValues[4].text, CultureInfo.InvariantCulture), float.Parse(RootValues[5].text, CultureInfo.InvariantCulture));
                    an_q.delta[pos] = new [] { quat.x, quat.y, quat.z, quat.w };
                    if (ollddd.x.ToString(CultureInfo.InvariantCulture) != RootValues[3].text || ollddd.y.ToString(CultureInfo.InvariantCulture) != RootValues[4].text || ollddd.z.ToString(CultureInfo.InvariantCulture) != RootValues[5].text)
                        changed = true;
                } catch {}
                if (changed)
                    globalchanged = true;
                else
                    an_q = null;
            }
            if (an_s != null)
            {
                if (an_s.type == NISLoader.AnimType.ANIM_COMPOUND)
                    an_s = an_s.subAnimations[0];
                changed = false;
                if (!gizmo.isTransforming)
                {
                    d = an_s.delta;
                    old_delta[2] = new float[d.Length][];
                    for (int i = 0; i < d.Length; i++)
                    {
                        old_delta[2][i] = new float[d[i].Length];
                        for (int j = 0; j < d[i].Length; j++)
                            old_delta[2][i][j] = d[i][j];
                    }
                }
                try
                {
                    old = an_s.delta[pos];
                    an_s.delta[pos] = new [] { float.Parse(RootValues[6].text, CultureInfo.InvariantCulture), float.Parse(RootValues[7].text, CultureInfo.InvariantCulture), float.Parse(RootValues[8].text, CultureInfo.InvariantCulture) };
                    if (old[0].ToString(CultureInfo.InvariantCulture) != RootValues[6].text || old[1].ToString(CultureInfo.InvariantCulture) != RootValues[7].text || old[2].ToString(CultureInfo.InvariantCulture) != RootValues[8].text)
                        changed = true;
                } catch {}
                if (changed)
                    globalchanged = true;
                else
                    an_s = null;
            }
            if (globalchanged && !gizmo.isTransforming && !dontUseHistory && rewinded_this_frame == 0)
            {
                AddToHistoryNIS(new PasteValues(currentlyEditingObject, new [] { an_t, an_q, an_s }, old_delta));
            }
        }
    }

    public void BoneValuesChanged()
    {
        int pos = animtabindex;
        NISLoader.Animation an_s = EditingAnimation_s;
        if (BoneSubEdit.activeSelf)
        {
            if (an_s.type == NISLoader.AnimType.ANIM_COMPOUND)
                an_s = an_s.subAnimations[0];
            for (int sk = 0; sk < skeletons.Count; sk++)
            {
                if (skeletons[sk].animationName == currentlyEditingObject.ToUpper() || skeletons[sk].animationName.StartsWith(currentlyEditingObject.ToUpper()))
                {
                    for (int b = 0; b < skeletons[sk].bones.Length; b++)
                    {
                        if (skeletons[sk].bones[b].assignedTransform == currentlyEditingSubObject)
                        {
                            try {
                                int offset = b * 4;
                                Quaternion quat = Quaternion.Euler(float.Parse(BoneValues[0].text, CultureInfo.InvariantCulture), float.Parse(BoneValues[1].text, CultureInfo.InvariantCulture), float.Parse(BoneValues[2].text, CultureInfo.InvariantCulture));
                                an_s.delta[pos][offset] = quat.x;
                                an_s.delta[pos][offset + 1] = quat.y;
                                an_s.delta[pos][offset + 2] = quat.z;
                                an_s.delta[pos][offset + 3] = quat.w;
                            } catch {}
                            break;
                        }
                    }
                    break;
                }
            }
        }
    }

    public Dropdown actionDropdown2;
    private string[] copied_values_transform;

    public GameObject ReplayFileSelect;
    public InputField ReplayFileSelectField;

    public void ReplayFileSelected()
    {
        if (!File.Exists(ReplayFileSelectField.text))
            throw new Exception("This file does not exist");
        byte[] replayData = File.ReadAllBytes(ReplayFileSelectField.text);
        int offset = 0;
        int deltaNum = BitConverter.ToInt32(replayData, offset);
        offset += 4;
        (float, ReplayFrame)[] source = new (float, ReplayFrame)[deltaNum];
        for (int i = 0; i < deltaNum; i++)
        {
            float t = BitConverter.ToSingle(replayData, offset);
            offset += 4;
            source[i] = (t, (ReplayFrame)CoordDebug.RawDeserialize(replayData, offset, typeof(ReplayFrame)));
            offset += Marshal.SizeOf(typeof(ReplayFrame));
        }
        int last = 0;
        int curdelta = 0;
        List<float[]> deltaPos = new List<float[]>();
        List<float[]> deltaRot = new List<float[]>();
        do
        {
            float targetTime = curdelta * (1f / 15f);
            ReplayFrame result = source[last].Item2;
            for (int j = last; j < deltaNum; j++)
            {
                if (source[j].Item1 <= targetTime && (j >= deltaNum - 1 || source[j + 1].Item1 > targetTime))
                {
                    result = source[j].Item2;
                    last = j;
                    break;
                }
            }

            deltaPos.Add(new[] {result.posZ, -result.posX, result.posY});
            Vector3 rot = new Quaternion(result.rotX, result.rotY, result.rotZ, result.rotW).eulerAngles;
            Quaternion res = Quaternion.Euler(rot.z, -rot.x, -rot.y);
            deltaRot.Add(new[] {res.x, res.y, res.z, res.w});
            curdelta++;
        } while (last < source.Length - 1);
        imreplayscript.originalPosition = EditingAnimation_t;
        imreplayscript.originalRotation = EditingAnimation_q;
        imreplayscript.replayPosition = new NISLoader.Animation();
        imreplayscript.replayPosition.name = imreplayscript.originalPosition.name;
        imreplayscript.replayPosition.type = NISLoader.AnimType.ANIM_COMPOUND;
        imreplayscript.replayPosition.subAnimations.Add(new NISLoader.Animation());
        imreplayscript.replayPosition.subAnimations[0].delta = deltaPos.ToArray();
        imreplayscript.replayRotation = new NISLoader.Animation();
        imreplayscript.replayRotation.name = imreplayscript.originalRotation.name;
        imreplayscript.replayRotation.type = NISLoader.AnimType.ANIM_COMPOUND;
        imreplayscript.replayRotation.subAnimations.Add(new NISLoader.Animation());
        imreplayscript.replayRotation.subAnimations[0].delta = deltaRot.ToArray();
        imreplayscript.timeline.maxValue = deltaRot.Count - 1;
        imreplayscript.min.maxValue = deltaRot.Count - 1;
        imreplayscript.max.maxValue = deltaRot.Count - 1;
        imreplayscript.timeline.value = (int)imreplayscript.max.maxValue / 2;
        imreplayscript.min.value = 0;
        imreplayscript.max.value = imreplayscript.max.maxValue;
        ReplayFileSelect.SetActive(false);
        gameObject.SetActive(false);
        imreplayscript.gameObject.SetActive(true);
    }

    public void AnimationAction(int ind)
    {
        if (ind == 0) return;
        actionDropdown2.value = 0;
        ind--;
        int pos1, pos2;
        float[][] d;
        switch (ind)
        {
            case 0:
                if (EditingAnimation_t == null)
                {
                    Debug.Log("You cannot import replay because this object does not have position animation");
                    return;
                }
                if (EditingAnimation_q == null)
                {
                    Debug.Log("You cannot import replay because this object does not have rotation animation");
                    return;
                }
                HelpButton(true);
                ReplayFileSelect.SetActive(true);
                break;
            case 1:
                interpolation_start = animtabindex;
                break;
            case 2:
                pos1 = animtabindex;
                pos2 = interpolation_start;
                if (pos1 > pos2)
                {
                    int temp = pos1;
                    pos1 = pos2;
                    pos2 = temp;
                }
                if (RootSubEdit.activeSelf)
                {
                    NISLoader.Animation an_t = EditingAnimation_t;
                    if (an_t != null)
                    {
                        if (an_t.type == NISLoader.AnimType.ANIM_COMPOUND)
                            an_t = an_t.subAnimations[0];
                        d = an_t.delta;
                        old_delta = new float[d.Length][];
                        for (int i = 0; i < d.Length; i++)
                        {
                            old_delta[i] = new float[d[i].Length];
                            for (int j = 0; j < d[i].Length; j++)
                                old_delta[i][j] = d[i][j];
                        }
                        Vector3 start = new Vector3(an_t.delta[pos1][0], an_t.delta[pos1][1], an_t.delta[pos1][2]);
                        Vector3 end = new Vector3(an_t.delta[pos2][0], an_t.delta[pos2][1], an_t.delta[pos2][2]);
                        Vector3 lerp;
                        for (int i = pos1 + 1; i < pos2; i++)
                        {
                            lerp = Vector3.Lerp(start, end, Mathf.InverseLerp(pos1, pos2, i));
                            an_t.delta[i] = new[] { lerp.x, lerp.y, lerp.z };
                        }
                        AddToHistoryNIS(new Interpolate(currentlyEditingObject, an_t, old_delta));
                    }
                    if (!dontUseHistory)
                        Debug.Log("Interpolated position successfully");
                } else if (BoneSubEdit.activeSelf)
                {
                    Debug.Log("Bone interpolation is not implemented yet");
                    // todo bone interpolate
                }
                break;
            case 3:
                pos1 = animtabindex;
                pos2 = interpolation_start;
                if (pos1 > pos2)
                {
                    int temp = pos1;
                    pos1 = pos2;
                    pos2 = temp;
                }
                if (RootSubEdit.activeSelf)
                {
                    NISLoader.Animation an_q = EditingAnimation_q;
                    if (an_q != null)
                    {
                        if (an_q.type == NISLoader.AnimType.ANIM_COMPOUND)
                            an_q = an_q.subAnimations[0];
                        d = an_q.delta;
                        old_delta = new float[d.Length][];
                        for (int i = 0; i < d.Length; i++)
                        {
                            old_delta[i] = new float[d[i].Length];
                            for (int j = 0; j < d[i].Length; j++)
                                old_delta[i][j] = d[i][j];
                        }
                        Quaternion start = new Quaternion(an_q.delta[pos1][0], an_q.delta[pos1][1], an_q.delta[pos1][2], an_q.delta[pos1][3]);
                        Quaternion end = new Quaternion(an_q.delta[pos2][0], an_q.delta[pos2][1], an_q.delta[pos2][2], an_q.delta[pos2][3]);
                        Quaternion lerp;
                        for (int i = pos1 + 1; i < pos2; i++)
                        {
                            lerp = Quaternion.Slerp(start, end, Mathf.InverseLerp(pos1, pos2, i));
                            an_q.delta[i] = new[] { lerp.x, lerp.y, lerp.z, lerp.w };
                        }
                        AddToHistoryNIS(new Interpolate(currentlyEditingObject, an_q, old_delta));
                    }
                    if (!dontUseHistory)
                        Debug.Log("Interpolated rotation successfully");
                } else if (BoneSubEdit.activeSelf)
                {
                    Debug.Log("Bone interpolation is not implemented yet");
                    // todo bone interpolate
                }
                break;
            case 4:
                pos1 = animtabindex;
                pos2 = interpolation_start;
                if (pos1 > pos2)
                {
                    int temp = pos1;
                    pos1 = pos2;
                    pos2 = temp;
                }
                if (RootSubEdit.activeSelf)
                {
                    NISLoader.Animation an_s = EditingAnimation_s;
                    if (an_s != null)
                    {
                        if (an_s.type == NISLoader.AnimType.ANIM_COMPOUND)
                            an_s = an_s.subAnimations[0];
                        d = an_s.delta;
                        old_delta = new float[d.Length][];
                        for (int i = 0; i < d.Length; i++)
                        {
                            old_delta[i] = new float[d[i].Length];
                            for (int j = 0; j < d[i].Length; j++)
                                old_delta[i][j] = d[i][j];
                        }
                        Vector3 start = new Vector3(an_s.delta[pos1][0], an_s.delta[pos1][1], an_s.delta[pos1][2]);
                        Vector3 end = new Vector3(an_s.delta[pos2][0], an_s.delta[pos2][1], an_s.delta[pos2][2]);
                        Vector3 lerp;
                        for (int i = pos1 + 1; i < pos2; i++)
                        {
                            lerp = Vector3.Lerp(start, end, Mathf.InverseLerp(pos1, pos2, i));
                            an_s.delta[i] = new[] { lerp.x, lerp.y, lerp.z };
                        }
                        AddToHistoryNIS(new Interpolate(currentlyEditingObject, an_s, old_delta));
                    }
                    if (!dontUseHistory)
                        Debug.Log("Interpolated scale successfully");
                } else if (BoneSubEdit.activeSelf)
                {
                    Debug.Log("Bone interpolation is not implemented yet");
                    // todo bone interpolate
                }
                break;
            case 5:
                if (RootSubEdit.activeSelf)
                {
                    copied_values_transform = new string[RootValues.Length];
                    for (int i = 0; i < RootValues.Length; i++)
                        copied_values_transform[i] = RootValues[i].text;
                    Debug.Log("Values copied");
                }
                else if (BoneSubEdit.activeSelf)
                {
                    // todo
                    Debug.Log("Bone value copy is not implemented yet");
                }
                break;
            case 6:
                if (RootSubEdit.activeSelf)
                {
                    blockupdcall = true;
                    if (copied_values_transform != null)
                    {
                        for (int i = 0; i < RootValues.Length; i++)
                            RootValues[i].text = copied_values_transform[i];
                    }
                    blockupdcall = false;
                    RootValuesChanged();
                    Debug.Log("Values pasted");
                }
                else if (BoneSubEdit.activeSelf)
                {
                    // todo
                    Debug.Log("Bone value paste is not implemented yet");
                }
                break;
        }
    }

    public bool HideOtherObjs;

    public void HideObjectsToggle(bool enable)
    {
        HideOtherObjs = enable;
        foreach (string obj in ObjectsOnScene.Keys)
        {
            ObjectsOnScene[obj].gameObject.SetActive(!HideOtherObjs || obj == currentlyEditingObject);
        }
    }

    [HideInInspector]
    public List<Image> CtEntryIcons;

    public void ChangeCameraTrack(int num)
    {
        if (updlimit) return;
        updlimit = true;
        if (!dontUseHistory)
        {
            if (curcam != num)
                AddToHistoryCam(new HistoryChangeCameraTrack(curcam, num));
        }
        curcam = num;
        if (CameraTrackSelection.value != curcam)
            CameraTrackSelection.value = curcam;
        if (RealtimeCameraEditActive)
            ToggleCameraControl();
        NewCameraTrackName.text = cameratrack.Count > 0 ? cameratrack[curcam].Item1.TrackName : "";
        NewCameraTrackBytes.text = cameratrack.Count > 0 ? BitConverter.ToString(cameratrack[curcam].Item1.Unknown1) : "";
        GenCameraTrackPreview();
        GenCameraSplines();
        UpdateTitle();
        //timeline = 0f;
        playing = false;
        oldsegment = -1;
        RemoveCameraButton.interactable = cameratrack.Count > 1;
    }

    void GenCameraTrackPreview()
    {
        foreach (Transform child in CameraTrackEntries)
            Destroy(child.gameObject);
        if (cameratrack.Count == 0)
            return;
        GameObject obj;
        CtEntryIcons = new List<Image>();
        for (int i = 0; i < cameratrack[curcam].Item2.Length; i++)
        {
            obj = Instantiate(CTEntryPrefab, CameraTrackEntries);
            CtEntryIcons.Add(obj.GetComponent<Image>());
            float localstart = Mathf.Lerp(editorTimelineMin, editorTimelineMax, cameratrack[curcam].Item2[i].Time);
            float localend = Mathf.Lerp(editorTimelineMin, editorTimelineMax, i < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[i + 1].Time : 1f);
            if (localstart == localend)
            {
                obj.SetActive(false);
                continue;
            }
            float length = localend - localstart;
            length = (860f - 4f) * length;
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(length, 24f);
            if (i == cameratrack[curcam].Item2.Length - 1)
                obj.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    
    public void UpdCameraTrackPreview()
    {
        for (int i = 0; i < cameratrack[curcam].Item2.Length; i++)
        {
            float localstart = Mathf.InverseLerp(editorTimelineMin, editorTimelineMax, cameratrack[curcam].Item2[i].Time);
            float localend = Mathf.InverseLerp(editorTimelineMin, editorTimelineMax, i < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[i + 1].Time : 1f);
            if (localstart == localend)
            {
                CtEntryIcons[i].gameObject.SetActive(false);
                continue;
            }
            CtEntryIcons[i].gameObject.SetActive(true);
            float length = localend - localstart;
            length = (860f - 4f) * length;
            CtEntryIcons[i].GetComponent<RectTransform>().sizeDelta = new Vector2(length, 24f);
        }
    }

    public Dropdown FocusDropdown;
    private int focusobj = -1;
    private List<string> alreadyDid;

    public void ChangeFocus(int objnum)
    {
        focusobj = objnum - 1;
    }

    void GenCameraSplines()
    {
        //DrawCameraPath();
        if (cameratrack.Count == 0)
        {
            camerasplines = new List<NISLoader.CameraSpline>();
            totalLength = NISLoader.CalculateNISDuration(anims);
            foreach (GameObject obj in HiddenWhenNoCamera)
                obj.SetActive(false);
            return;
        }
        foreach (GameObject obj in HiddenWhenNoCamera)
            obj.SetActive(true);
        CamProps[0].text = (int) Mathf.Floor(cameratrack[curcam].Item1.Duration / 60f) + ":" + (cameratrack[curcam].Item1.Duration % 60f).ToString("0.00").PadLeft(5, '0');
        List<NISLoader.camrec> recs = new List<NISLoader.camrec>();
        for (int i = 0; i < cameratrack[curcam].Item2.Length; i++)
            recs.Add(new NISLoader.camrec(cameratrack[curcam].Item2[i]));
        camerasplines = NISLoader.MakeSplines(recs);
        totalLength = cameratrack[curcam].Item1.Duration;
        DurationField.text = totalLength.ToString(CultureInfo.InvariantCulture);
    }

    void UpdateTitle()
    {
        string nn = nisname;
        if (!string.IsNullOrEmpty(NISLoader.SceneInfo.SceneName))
            nn = NISLoader.SceneInfo.SceneName;
        if (cameratrack != null && cameratrack.Count > 0)
        {
            OpenedFileName.text = nn + " (" + cameratrack[curcam].Item1.TrackName + ")";
            CameraTrackPropertiesName.text = "Camera Track Properties (" + cameratrack[curcam].Item1.TrackName + ")";
        }
        else
            OpenedFileName.text = nn;
    }

    public InputField GameDirInput;

    public void SelectGameDirectory()
    {
        //string[] folders = StandaloneFileBrowser.OpenFolderPanel("Game directory selection", "", false);
        //if (folders.Length == 0) return;
        //if (folders[0].Length == 0) return;
        if (GameDirInput.text.EndsWith("/") || GameDirInput.text.EndsWith("\\"))
            GameDirInput.text = GameDirInput.text.Substring(0, GameDirInput.text.Length - 1);
        PlayerPrefs.SetString("GameDir", /*folders[0]*/ GameDirInput.text);
        UpdateGameSelection();
    }
    
    public static byte[] f_orig;

    public void Save()
    {
        List<byte> f;
        int oldsize;
        SavedText.text = "Saved";
        string path;
        switch (tabnum)
        {
            case 2:
                try {
                    List<byte> animdata = new List<byte>();
                    animdata.AddRange(new byte[] {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0});
                    List<int> nameOffsets = new List<int>();
                    for (int i = 0; i < anims.Count; i++)
                    {
                        nameOffsets.Add(animdata.Count);
                        for (int ch = 0; ch < anims[i].name.Length; ch++)
                        {
                            animdata.Add(Convert.ToByte(anims[i].name[ch]));
                        }
                        animdata.Add(0);
                    }
                    List<int> animationCompoundOffsets = new List<int>();
                    List<int[]> animationChildOffsets = new List<int[]>();
                    List<int[]> animationChildMetaOffsets = new List<int[]>();
                    List<int> animationCompoundMetaOffsets = new List<int>();
                    for (int i = 0; i < anims.Count; i++)
                    {
                        switch (anims[i].type)
                        {
                            case NISLoader.AnimType.ANIM_COMPOUND:
                                while (animdata.Count % 16 != 0)
                                    animdata.Add(0);
                                animationCompoundOffsets.Add(animdata.Count);
                                animdata.AddRange(new byte[] {0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x1E, 0x30, 0x30, 0x30});
                                animationChildOffsets.Add(new int[anims[i].subAnimations.Count]);
                                animationChildMetaOffsets.Add(new int[anims[i].subAnimations.Count]);
                                for (int childNum = 0; childNum < anims[i].subAnimations.Count; childNum++)
                                {
                                    animationChildOffsets[animationChildOffsets.Count - 1][childNum] = animdata.Count;
                                    animdata.AddRange(BitConverter.GetBytes(anims[i].subAnimations[childNum].numDofs));
                                    animdata.AddRange(BitConverter.GetBytes(anims[i].subAnimations[childNum].quantBits));
                                    List<float[]> delta = anims[i].subAnimations[childNum].delta.ToList();
                                    delta.Add(new float[anims[i].subAnimations[childNum].numDofs]);
                                    for (int x = 0; x < anims[i].subAnimations[childNum].numDofs; x++)
                                        delta[delta.Count - 1][x] = Mathf.LerpUnclamped(delta.Count > 2 ? delta[delta.Count - 3][x] : 0f, delta.Count > 1 ? delta[delta.Count - 2][x] : 0f, 2f);
                                    if (anims[i].subAnimations[childNum].numDofs == 4)
                                    {
                                        // rotation fix
                                        Vector3[] deltaeuler = new Vector3[delta.Count];
                                        for (int deltaNum = 0; deltaNum < delta.Count; deltaNum++)
                                            deltaeuler[deltaNum] = new Quaternion(delta[deltaNum][0], delta[deltaNum][1], delta[deltaNum][2], delta[deltaNum][3]).eulerAngles;
                                        for (int deltaNum = 1; deltaNum < delta.Count; deltaNum++)
                                        {
                                            float x, y, z, x_old, y_old, z_old;
                                            x = deltaeuler[deltaNum].x;
                                            y = deltaeuler[deltaNum].y;
                                            z = deltaeuler[deltaNum].z;
                                            x_old = deltaeuler[deltaNum - 1].x;
                                            y_old = deltaeuler[deltaNum - 1].y;
                                            z_old = deltaeuler[deltaNum - 1].z;

                                            float delta_x = Mathf.DeltaAngle(x_old, x);
                                            float delta_y = Mathf.DeltaAngle(y_old, y);
                                            float delta_z = Mathf.DeltaAngle(z_old, z);

                                            deltaeuler[deltaNum] = new Vector3(x_old + delta_x, y_old + delta_y, z_old + delta_z);
                                        }
                                        for (int deltaNum = 0; deltaNum < delta.Count; deltaNum++)
                                        {
                                            Quaternion quat = Quaternion.Euler(deltaeuler[deltaNum]);
                                            delta[deltaNum][0] = quat.x;
                                            delta[deltaNum][1] = quat.y;
                                            delta[deltaNum][2] = quat.z;
                                            delta[deltaNum][3] = quat.w;
                                        }
                                    }
                                    double[][] header = new double[anims[i].subAnimations[childNum].numDofs][];
                                    for (int dof = 0; dof < anims[i].subAnimations[childNum].numDofs; dof++)
                                    {
                                        double header_0 = delta[1][dof] - delta[0][dof];
                                        for (int deltaNum = 2; deltaNum < delta.Count; deltaNum++)
                                        {
                                            double d = delta[deltaNum][dof] - delta[deltaNum - 1][dof];
                                            if (d < header_0)
                                                header_0 = d;
                                        }
                                        double header_1 = delta[1][dof] - delta[0][dof] - header_0;
                                        for (int deltaNum = 2; deltaNum < delta.Count; deltaNum++)
                                        {
                                            double d = delta[deltaNum][dof] - delta[deltaNum - 1][dof] - header_0;
                                            if (d > header_1)
                                                header_1 = d;
                                        }
                                        switch (anims[i].subAnimations[childNum].quantBits)
                                        {
                                            case 8:
                                                header_1 *= 0.003921568627450980392157; // / 0xff
                                                break;
                                            case 0x10:
                                                header_1 *= 1.525902189669642175937E-5; // / 0xffff
                                                break;
                                        }
                                        header[dof] = new double[3];
                                        header[dof][0] = header_0;
                                        header[dof][1] = header_1;
                                        header[dof][2] = delta[0][dof];
                                        animdata.AddRange(BitConverter.GetBytes(Convert.ToSingle(header_0)));
                                        animdata.AddRange(BitConverter.GetBytes(Convert.ToSingle(header_1)));
                                        animdata.AddRange(BitConverter.GetBytes(delta[0][dof]));
                                    }
                                    for (int deltaNum = 1; deltaNum < delta.Count; deltaNum++)
                                    {
                                        for (int x = 0; x < anims[i].subAnimations[childNum].numDofs; x++)
                                        {
                                            double compressed_value = ((double)delta[deltaNum][x] - (double)delta[deltaNum - 1][x] - header[x][0]) / header[x][1];
                                            if (double.IsNaN(compressed_value))
                                                compressed_value = 0;
                                            if (double.IsInfinity(compressed_value))
                                                compressed_value = anims[i].subAnimations[childNum].quantBits == 8 ? 0xff : 0xffff;
                                            if (compressed_value < 0)
                                                compressed_value = 0;
                                            switch (anims[i].subAnimations[childNum].quantBits)
                                            {
                                                case 8:
                                                    if (compressed_value > 0xff)
                                                        compressed_value = 0xff;
                                                    animdata.Add(Convert.ToByte(Math.Round(compressed_value)));
                                                    break;
                                                case 0x10:
                                                    if (compressed_value > 0xffff)
                                                        compressed_value = 0xffff;
                                                    animdata.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Math.Round(compressed_value))));
                                                    break;
                                            }
                                        }
                                    }
                                    while (animdata.Count % 4 != 0)
                                        animdata.Add(0);
                                    switch (anims[i].subAnimations[childNum].type)
                                    {
                                        case NISLoader.AnimType.ANIM_DELTAQUAT:
                                            animdata.AddRange(BitConverter.GetBytes((uint)0));
                                            animdata.AddRange(BitConverter.GetBytes((uint)0));
                                            break;
                                    }

                                    animationChildMetaOffsets[animationChildOffsets.Count - 1][childNum] = animdata.Count;
                                    animdata.AddRange(BitConverter.GetBytes((ushort)anims[i].subAnimations[childNum].type));
                                    animdata.AddRange(BitConverter.GetBytes(anims[i].subAnimations[childNum].checkSum));
                                    switch (anims[i].subAnimations[childNum].type)
                                    {
                                        case NISLoader.AnimType.ANIM_DELTALERP:
                                            animdata.AddRange(BitConverter.GetBytes((uint)animationChildOffsets[animationChildOffsets.Count - 1][childNum]));
                                            animdata.AddRange(BitConverter.GetBytes((ushort)anims[i].subAnimations[childNum].delta.Length));
                                            animdata.AddRange(BitConverter.GetBytes(anims[i].subAnimations[childNum].unk1)); // not sure
                                            animdata.AddRange(BitConverter.GetBytes(anims[i].subAnimations[childNum].unk2)); // not sure
                                            animdata.AddRange(BitConverter.GetBytes(anims[i].subAnimations[childNum].unk3)); // not sure
                                            break;
                                        case NISLoader.AnimType.ANIM_DELTAQUAT:
                                            animdata.AddRange(BitConverter.GetBytes((uint)animationChildOffsets[animationChildOffsets.Count - 1][childNum]));
                                            animdata.AddRange(BitConverter.GetBytes((ushort)anims[i].subAnimations[childNum].delta.Length));
                                            animdata.AddRange(BitConverter.GetBytes(anims[i].subAnimations[childNum].unk1)); // not sure
                                            break;
                                        default:
                                            throw new NotImplementedException();
                                    }
                                }
                                animationCompoundMetaOffsets.Add(animdata.Count);
                                animdata.AddRange(BitConverter.GetBytes((ushort)anims[i].type));
                                animdata.AddRange(BitConverter.GetBytes((ushort)anims[i].checkSum));
                                animdata.AddRange(BitConverter.GetBytes((uint)animationCompoundOffsets.Last()));
                                animdata.AddRange(BitConverter.GetBytes((ushort)anims[i].subAnimations.Count));
                                animdata.AddRange(BitConverter.GetBytes((ushort)anims[i].subAnimations[0].delta.Length));
                                for (int childNum = 0; childNum < anims[i].subAnimations.Count; childNum++)
                                {
                                    animdata.AddRange(BitConverter.GetBytes((uint)animationChildMetaOffsets[animationChildOffsets.Count - 1][childNum]));
                                }
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    int AnimationsOffset = animdata.Count;
                    for (int i = 0; i < anims.Count; i++)
                    {
                        animdata.AddRange(BitConverter.GetBytes((uint)animationCompoundMetaOffsets[i]));
                    }
                    int AnimationNames = animdata.Count;
                    for (int i = 0; i < anims.Count; i++)
                    {
                        animdata.AddRange(BitConverter.GetBytes((uint)nameOffsets[i]));
                    }
                    while (animdata.Count % 16 != 0)
                        animdata.Add(0);
                    int AnimationBankOffset = animdata.Count;
                    animdata.AddRange(BitConverter.GetBytes((uint)28)); // not sure
                    animdata.AddRange(BitConverter.GetBytes((uint)anims.Count));
                    int stuffOffset = animdata.Count;
                    animdata.AddRange(BitConverter.GetBytes((uint)0));
                    animdata.AddRange(BitConverter.GetBytes((uint)0));
                    int AnimationsPointerOffset = animdata.Count;
                    animdata.AddRange(BitConverter.GetBytes((uint)AnimationsOffset));
                    int NamesPointerOffset = animdata.Count;
                    animdata.AddRange(BitConverter.GetBytes((uint)AnimationNames));
                    animdata.AddRange(BitConverter.GetBytes((uint)0));
                    animdata.AddRange(BitConverter.GetBytes((uint)0));
                
                    f = f_orig.ToList();

                    /*if (NISLoader.DescriptionOffset != 0)
                    {
                        int desclength = NISLoader.SceneDescription.Length + 1;
                        while (desclength % 4 != 0)
                            desclength++;
                        f.RemoveRange(NISLoader.DescriptionOffset, desclength);
                        string NewDescription = "Edited by " + PlayerPrefs.GetString("AuthorName", "Anon") + " in icebreaker " + Application.version;
                        NISLoader.SceneDescription = NewDescription;
                        NISProps[7].text = NISLoader.SceneDescription;
                        List<byte> bb = NewDescription.ToArray().Select(x => (byte) x).ToList();
                        do
                        {
                            bb.Add(0);
                        } while (bb.Count % 4 != 0);

                        f.InsertRange(NISLoader.DescriptionOffset, bb);
                    }*/

                    f.RemoveRange(ELFChunkStart + AnimationBank_Offset, 4);
                    f.InsertRange(ELFChunkStart + AnimationBank_Offset, BitConverter.GetBytes((uint)AnimationBankOffset));
                    oldsize = BitConverter.ToInt32(f_orig, ELFChunkStart + ELFData_SizeOffset);

                    int TableOffset = 0;
                    TableOffset += 8;
                    TableOffset += 8;
                    TableOffset += 8;
                    for (int i = anims.Count - 1; i >= 0; i--)
                    {
                        TableOffset += 8;
                    }
                    for (int i = anims.Count - 1; i >= 0; i--)
                    {
                        TableOffset += 8;
                    }
                    for (int i = anims.Count - 1; i >= 0; i--)
                    {
                        TableOffset += 8;
                        TableOffset += 8;
                        for (int childNum = anims[i].subAnimations.Count - 1; childNum >= 0; childNum--)
                        {
                            TableOffset += 8;
                        }
                    }
                    TableOffset += 4;
                    TableOffset += 4;

                    int tabledelta = TableOffset - elftablelength;
                    int tboffstart = ELFChunkStart + AnimationBank_Offset + 5 * 4;

                    f.RemoveRange(ELFChunkStart + ELFData_SizeOffset, 4);
                    f.InsertRange(ELFChunkStart + ELFData_SizeOffset, BitConverter.GetBytes((uint)animdata.Count));
                    int old = BitConverter.ToInt32(f_orig, ELFChunkStart + shstrtab_offset);
                    f.RemoveRange(ELFChunkStart + shstrtab_offset, 4);
                    f.InsertRange(ELFChunkStart + shstrtab_offset, BitConverter.GetBytes((uint)(old + (animdata.Count - oldsize) + (old > tboffstart ? tabledelta : 0))));
                    old = BitConverter.ToInt32(f_orig, ELFChunkStart + strtab_offset);
                    f.RemoveRange(ELFChunkStart + strtab_offset, 4);
                    f.InsertRange(ELFChunkStart + strtab_offset, BitConverter.GetBytes((uint)(old + (animdata.Count - oldsize) + (old > tboffstart ? tabledelta : 0))));
                    old = BitConverter.ToInt32(f_orig, ELFChunkStart + symtab_offset);
                    f.RemoveRange(ELFChunkStart + symtab_offset, 4);
                    f.InsertRange(ELFChunkStart + symtab_offset, BitConverter.GetBytes((uint)(old + (animdata.Count - oldsize) + (old > tboffstart ? tabledelta : 0))));
                    old = BitConverter.ToInt32(f_orig, ELFChunkStart + rel_data_offset);
                    f.RemoveRange(ELFChunkStart + rel_data_offset, 4);
                    f.InsertRange(ELFChunkStart + rel_data_offset, BitConverter.GetBytes((uint)(old + (animdata.Count - oldsize) + (old > tboffstart ? tabledelta : 0))));
                    old = BitConverter.ToInt32(f_orig, ELFChunkStart + 32);
                    f.RemoveRange(ELFChunkStart + 32, 4);
                    f.InsertRange(ELFChunkStart + 32, BitConverter.GetBytes((uint)(old + (animdata.Count - oldsize) + (old > tboffstart ? tabledelta : 0))));
                    foreach (int off in miscoffsets)
                    {
                        old = BitConverter.ToInt32(f_orig, ELFChunkStart + off);
                        f.RemoveRange(ELFChunkStart + off, 4);
                        f.InsertRange(ELFChunkStart + off, BitConverter.GetBytes((uint)(old + (animdata.Count - oldsize) + (old > tboffstart ? tabledelta : 0))));
                    }

                    TableOffset = tboffstart + 8 - ELFChunkStart;
                    if (elftablelength == 0)
                        throw new Exception("Table length is 0");
                    f.RemoveRange(ELFChunkStart + TableOffset, elftablelength);

                    f.InsertRange(ELFChunkStart + TableOffset, BitConverter.GetBytes((uint)NamesPointerOffset));
                    f.InsertRange(ELFChunkStart + TableOffset + 4, BitConverter.GetBytes(BitConverter.ToInt32(f_orig, ELFChunkStart + TableOffset + 4)));
                    TableOffset += 8;
                    f.InsertRange(ELFChunkStart + TableOffset, BitConverter.GetBytes((uint)AnimationsPointerOffset));
                    f.InsertRange(ELFChunkStart + TableOffset + 4, BitConverter.GetBytes(BitConverter.ToInt32(f_orig, ELFChunkStart + TableOffset + 4)));
                    TableOffset += 8;
                    f.InsertRange(ELFChunkStart + TableOffset, BitConverter.GetBytes((uint)stuffOffset));
                    f.InsertRange(ELFChunkStart + TableOffset + 4, BitConverter.GetBytes(BitConverter.ToInt32(f_orig, ELFChunkStart + TableOffset + 4)));
                    TableOffset += 8;
                    for (int i = anims.Count - 1; i >= 0; i--)
                    {
                        f.InsertRange(ELFChunkStart + TableOffset, BitConverter.GetBytes((uint)(AnimationNames + i * 4)));
                        f.InsertRange(ELFChunkStart + TableOffset + 4, BitConverter.GetBytes(BitConverter.ToInt32(f_orig, ELFChunkStart + TableOffset + 4)));
                        TableOffset += 8;
                    }
                    for (int i = anims.Count - 1; i >= 0; i--)
                    {
                        f.InsertRange(ELFChunkStart + TableOffset, BitConverter.GetBytes((uint)(AnimationsOffset + i * 4)));
                        f.InsertRange(ELFChunkStart + TableOffset + 4, BitConverter.GetBytes(BitConverter.ToInt32(f_orig, ELFChunkStart + TableOffset + 4)));
                        TableOffset += 8;
                    }
                    for (int i = anims.Count - 1; i >= 0; i--)
                    {
                        f.InsertRange(ELFChunkStart + TableOffset, BitConverter.GetBytes((uint)(animationCompoundMetaOffsets[i] + 12)));
                        f.InsertRange(ELFChunkStart + TableOffset + 4, BitConverter.GetBytes(BitConverter.ToInt32(f_orig, ELFChunkStart + TableOffset + 4)));
                        TableOffset += 8;
                        f.InsertRange(ELFChunkStart + TableOffset, BitConverter.GetBytes((uint)(animationCompoundMetaOffsets[i] + 4)));
                        f.InsertRange(ELFChunkStart + TableOffset + 4, BitConverter.GetBytes(BitConverter.ToInt32(f_orig, ELFChunkStart + TableOffset + 4)));
                        TableOffset += 8;
                        for (int childNum = anims[i].subAnimations.Count - 1; childNum >= 0; childNum--)
                        {
                            f.InsertRange(ELFChunkStart + TableOffset, BitConverter.GetBytes((uint) (animationChildMetaOffsets[i][childNum] + 4)));
                            f.InsertRange(ELFChunkStart + TableOffset + 4, BitConverter.GetBytes(BitConverter.ToInt32(f_orig, ELFChunkStart + TableOffset + 4)));
                            TableOffset += 8;
                        }
                    }
                    f.InsertRange(ELFChunkStart + TableOffset, BitConverter.GetBytes((uint)0));
                    TableOffset += 4;
                    f.InsertRange(ELFChunkStart + TableOffset, BitConverter.GetBytes((uint)0));
                    TableOffset += 4;

                    f.RemoveRange(ELFChunkStart + ELFData_Offset, oldsize);
                    f.InsertRange(ELFChunkStart + ELFData_Offset, animdata);
                    old = BitConverter.ToInt32(f_orig, ELFChunkSize);
                    f.RemoveRange(ELFChunkSize, 4);
                    f.InsertRange(ELFChunkSize, BitConverter.GetBytes((uint)(old + (animdata.Count - oldsize) + tabledelta)));

                    path = "/NIS/Scene_" + nisname + "_BundleB.bun";
                    if (!File.Exists(GameDirectory + path))
                    {
                        path = path.ToUpper();
                        if (!File.Exists(GameDirectory + path))
                            throw new Exception("File does not exist");
                    }
                
                    File.WriteAllBytes(GameDirectory + path, f.ToArray());
                } catch (Exception e)
                {
                    Debug.LogError(e);
                    SavedText.text = "Error";
                }
                break;
            case 3:
                try
                {
                    path = "/GLOBAL/InGameB.lzc";
                    if (!File.Exists(GameDirectory + path))
                    {
                        path = path.ToUpper();
                        if (!File.Exists(GameDirectory + path))
                            throw new Exception("File does not exist");
                    }
                    byte[] f_orig = File.ReadAllBytes(GameDirectory + path);
                    f_orig = NISLoader.DecompressJZC(f_orig);
                    f = f_orig.ToList();
                    List<byte> chunk = new List<byte>();
                    uint hash = nisname.StartsWith("0x") ? Convert.ToUInt32(nisname.Substring(2), 16) : NISLoader.BinHash(nisname);
                    chunk.AddRange(BitConverter.GetBytes(hash));
                    chunk.AddRange(BitConverter.GetBytes(cameratrack.Count));
                    for (int i = 0; i < cameratrack.Count; i++)
                    {
                        var hh = cameratrack[i].Item1.Clone();
                        if (hh.Duration == hh.DurationCarbon)
                        {
                            hh.Duration = 0f;
                        }
                        hh.entryCount = (short)cameratrack[i].Item2.Length;
                        chunk.AddRange(CoordDebug.RawSerialize(hh));
                        for (int j = 0; j < cameratrack[i].Item2.Length; j++)
                        {
                            switch (cameratrack[i].Item2[j].type)
                            {
                                case 0:
                                    chunk.AddRange(CoordDebug.RawSerialize(cameratrack[i].Item2[j].obj0));
                                    break;
                            }
                        }
                    }

                    bool updlistpls = false;

                    if (NISLoader.SizeOffset == 0)
                    {
                        updlistpls = true;
                        for (int i = 0; i < f_orig.Length; i += 4)
                        {
                            if (f[i] == 0x00 && f[i + 1] == 0xB2 && f[i + 2] == 0x03 && f[i + 3] == 0x80)
                            {
                                i += 4;
                                NISLoader.SizeOffset = i;
                                break;
                            }
                        }
                        if (NISLoader.SizeOffset == 0)
                            throw new Exception("Can't find parent bank for camera, this may be very bad idea to save...");
                    }

                    if (cameratrack_offset == 0)
                    {
                        int ccc = BitConverter.ToInt32(f_orig, NISLoader.SizeOffset);
                        f.RemoveRange(NISLoader.SizeOffset, 4);
                        f.InsertRange(NISLoader.SizeOffset, BitConverter.GetBytes(ccc + chunk.Count + 8));
                        int offset = NISLoader.SizeOffset + 4 + ccc;
                        f.InsertRange(offset, new byte[] { 0x10, 0xB2, 0x03, 0x00 });
                        offset += 4;
                        f.InsertRange(offset, BitConverter.GetBytes(chunk.Count));
                        offset += 4;
                        f.InsertRange(offset, chunk);
                    }
                    else
                    {
                        oldsize = BitConverter.ToInt32(f_orig, cameratrack_offset + 4);
                        f.RemoveRange(NISLoader.SizeOffset, 4);
                        f.InsertRange(NISLoader.SizeOffset, BitConverter.GetBytes(BitConverter.ToInt32(f_orig, NISLoader.SizeOffset) + (chunk.Count - oldsize)));
                        f.RemoveRange(cameratrack_offset + 4, 4);
                        f.InsertRange(cameratrack_offset + 4, BitConverter.GetBytes(chunk.Count));
                        f.RemoveRange(cameratrack_offset + 8, oldsize);
                        f.InsertRange(cameratrack_offset + 8, chunk);
                    }
                    File.WriteAllBytes(GameDirectory + path, f.ToArray());
                    if (updlistpls)
                        UpdateNisList();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    SavedText.text = "Error";
                }
                break;
        }
        
        SavedText.color = Color.white;
    }

    public InputField deltacountinp;
    public Button deltacountinpb;

    public InputField NameField;

    public void NameChanged()
    {
        string n = NameField.text;
        if (string.IsNullOrEmpty(n))
            PlayerPrefs.SetString("AuthorName", "Anon");
        else
            PlayerPrefs.SetString("AuthorName", n);
    }

    public void ApplyDeltaCount()
    {
        short val;
        short old_v;
        try {
            val = short.Parse(deltacountinp.text);
        } catch { return; }
        if (val < 1) return;
        NISLoader.Animation[] anims = new NISLoader.Animation[3];
        float[][][] old_delta = new float[3][][];
        if (EditingAnimation_t != null)
        {
            anims[0] = EditingAnimation_t.subAnimations[0];
            var d = EditingAnimation_t.subAnimations[0].delta;
            old_delta[0] = new float[d.Length][];
            for (int i = 0; i < d.Length; i++)
            {
                old_delta[0][i] = new float[d[i].Length];
                for (int j = 0; j < d[i].Length; j++)
                    old_delta[0][i][j] = d[i][j];
            }
            old_v = (short)d.Length;
            float[][] new_d = new float[val][];
            for (int i = 0; i < val; i++)
            {
                new_d[i] = d[Mathf.Clamp(i, 0, old_v - 1)];
            }
            EditingAnimation_t.subAnimations[0].delta = new_d;
        }
        if (EditingAnimation_s != null)
        {
            anims[1] = EditingAnimation_s.subAnimations[0];
            var d = EditingAnimation_s.subAnimations[0].delta;
            old_delta[1] = new float[d.Length][];
            for (int i = 0; i < d.Length; i++)
            {
                old_delta[1][i] = new float[d[i].Length];
                for (int j = 0; j < d[i].Length; j++)
                    old_delta[1][i][j] = d[i][j];
            }
            old_v = (short)d.Length;
            float[][] new_d = new float[val][];
            for (int i = 0; i < val; i++)
            {
                new_d[i] = d[Mathf.Clamp(i, 0, old_v - 1)];
            }
            EditingAnimation_s.subAnimations[0].delta = new_d;
        }
        if (EditingAnimation_q != null)
        {
            anims[2] = EditingAnimation_q.subAnimations[0];
            var d = EditingAnimation_q.subAnimations[0].delta;
            old_delta[2] = new float[d.Length][];
            for (int i = 0; i < d.Length; i++)
            {
                old_delta[2][i] = new float[d[i].Length];
                for (int j = 0; j < d[i].Length; j++)
                    old_delta[2][i][j] = d[i][j];
            }
            old_v = (short)d.Length;
            if (old_v == 0)
            {
                d = new float[1][];
                d[0] = new float[EditingAnimation_q.subAnimations[0].numDofs];
                for (int i = 0; i < d[0].Length; i++)
                    d[0][i] = EditingAnimation_q.subAnimations[0].header[i][2];
                old_v = 1;
            }
            float[][] new_d = new float[val][];
            for (int i = 0; i < val; i++)
            {
                new_d[i] = d[Mathf.Clamp(i, 0, old_v - 1)];
            }
            EditingAnimation_q.subAnimations[0].delta = new_d;
        }
        AnimationsEditorObjectSelected(currentlyEditingObject);
        AddToHistoryNIS(new ChangeDeltaC(currentlyEditingObject, anims, old_delta));
        Debug.Log("Delta count changed");
        float calcdur = NISLoader.CalculateNISDuration(this.anims);
        NISProps[8].text = (int) Mathf.Floor(calcdur / 60f) + ":" + (calcdur % 60f).ToString("0.00").PadLeft(5, '0');
    }
 
    public static int ELFChunkSize;
    public static int ELFChunkStart;
    public static int ELFData_Offset;
    public static int ELFData_SizeOffset;
    public static int AnimationBank_Offset;

    public static int shstrtab_offset;
    public static int strtab_offset;
    public static int symtab_offset;
    public static int rel_data_offset;
    public static List<int> miscoffsets = new List<int>();

    public void TogglePlay()
    {
        if (RealtimeCameraEditActive) return;
        playing = !playing;
        if (timeline >= (TimelineLock && cursegment < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[cursegment + 1].Time - 0.001f : 1f))
            timeline = TimelineLock ? cameratrack[curcam].Item2[cursegment].Time + 0.001f : 0f;
    }

    public void TabChanged()
    {
        if (updlimit) return;
        if (RealtimeCameraEditActive)
            ToggleCameraControl();
        foreach (string obj in ObjectsOnScene.Keys)
        {
            ObjectsOnScene[obj].gameObject.SetActive(true);
        }
        tabnum = tabgroup.GetComponent<ToggleGroup>().ActiveToggles().ToArray()[0].transform.GetSiblingIndex();
        bool editingallowed = tabnum == 2 && !NISSaveDisabled || tabnum == 3 && !CameraSaveDisabled;
        foreach (GameObject obj in HiddenWhenNoEditingAllowed)
            obj.SetActive(editingallowed);
        bool playingallowed = tabnum != 0 && tabnum != 1 && tabnum != 2;
        foreach (GameObject obj in HiddenWhenNoPlayingAllowed)
            obj.SetActive(playingallowed);
        if (!editingallowed)
            SavedText.color = Color.clear;
        updlimit = true;
        if (!playingallowed)
            playing = false;
        FourByThree.SetActive(tabnum == 4);
        TabBG.color = new Color(0f, 0f, 0f, tabnum == 4 ? 0.5f : 1f);
        editorCameraMovement.enabled = tabnum == 2;
        if (tabnum == 2)
        {
            AnimationsEditorObjectSelected(currentlyEditingObject);
        } else {
            if (gizmo.targetRoots.Count > 0)
                gizmo.RemoveTarget(gizmo.targetRoots.Keys.ToArray()[0]);
        }
        FocusSphere.gameObject.SetActive(tabnum == 3);
        oldsegment = -1;
        //DrawCameraPath();
    }

    public void TimelineRewinded(float t)
    {
        if (RealtimeCameraEditActive) return;
        if (t != timeline)
        {
            playing = false;
            timeline = t;
            if (TimelineLock && cameratrack.Count > 0)
            {
                float max = cursegment < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[cursegment + 1].Time - 0.001f : 1f;
                if (timeline > max)
                    timeline = max;
                else if (timeline < cameratrack[curcam].Item2[cursegment].Time + 0.001f)
                    timeline = cameratrack[curcam].Item2[cursegment].Time + 0.001f;
            }
        }
    }

    private int rewinded_this_frame;

    public void TimelineRewindedAnim(float t)
    {
        if (tabnum != 2) return;
        animtabindex = (int)Mathf.Round(t);
        t = t / 15f / totalLength;
        TimelineRewinded(t);
        rewinded_this_frame = 3;
    }

    [HideInInspector]
    public float editorTimelineMin;
    [HideInInspector]
    public float editorTimelineMax = 1f;

    public void EditorMinChanged(float t)
    {
        float old = editorTimelineMin;
        editorTimelineMin = Mathf.Clamp(t, 0f, editorTimelineMax);
        if (old != t)
            UpdCameraTrackPreview();
    }
    
    public void EditorMaxChanged(float t)
    {
        float old = editorTimelineMax;
        editorTimelineMax = Mathf.Clamp(t, editorTimelineMin, 1f);
        if (old != t)
            UpdCameraTrackPreview();
    }

    public static bool CameraSmoothingEnabled = false;

    public void RenameCameraTrack()
    {
        var old = cameratrack[curcam].Item1.Clone();
        var v = cameratrack[curcam].Item1;
        v.TrackName = NewCameraTrackName.text;
        CameraTrackSelection.options[curcam].text = NewCameraTrackName.text;
        if (RealtimeCameraEditActive)
            ToggleCameraControl();
        float val = cameratrack[curcam].Item1.Duration;
        try
        {
            val = float.Parse(DurationField.text, CultureInfo.InvariantCulture);
        } catch {}
        v.Duration = val;
        try
        {
            string[] ss = NewCameraTrackBytes.text.Split('-');
            for (int i = 0; i < v.Unknown1.Length; i++)
                v.Unknown1[i] = (byte) Convert.ToInt32(ss[i], 16);
        } catch {}
        cameratrack[curcam] = (v, cameratrack[curcam].Item2);
        updlimit = false;
        ChangeCameraTrack(curcam);
        UpdateTitle();
        AddToHistoryCam(new ChangeCameraTrackHeader(curcam, old, v.Clone()));
    }

    public void NewCameraTrack()
    {
        if (RealtimeCameraEditActive)
            ToggleCameraControl();
        NISLoader.CameraTrackHeader header = new NISLoader.CameraTrackHeader();
        header.TrackName = NewCameraTrackName.text;
        float val = cameratrack[curcam].Item1.Duration;
        try
        {
            val = float.Parse(DurationField.text, CultureInfo.InvariantCulture);
        } catch {}

        header.Unknown1 = new byte[16];
        try
        {
            string[] ss = NewCameraTrackBytes.text.Split('-');
            for (int i = 0; i < header.Unknown1.Length; i++)
                header.Unknown1[i] = (byte) Convert.ToInt32(ss[i], 16);
        }
        catch
        {
            for (int i = 0; i < header.Unknown1.Length; i++)
                header.Unknown1[i] = cameratrack[curcam].Item1.Unknown1[i];
        }
        header.Duration = val;
        header.entryCount = cameratrack[curcam].Item1.entryCount;
        NISLoader.CameraTrackEntry[] entries = new NISLoader.CameraTrackEntry[cameratrack[curcam].Item2.Length];
        for (int i = 0; i < entries.Length; i++)
        {
            switch (cameratrack[curcam].Item2[i].type)
            {
                case 0:
                    entries[i] = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[i].obj0.Clone());
                    break;
            }
        }
        cameratrack.Add((header, entries));
        Dropdown.OptionData opt = new Dropdown.OptionData();
        opt.text = header.TrackName;
        int oldcur = curcam;
        AddToHistoryCam(new CreateCameraTrack(cameratrack.Count - 1, oldcur, cameratrack[cameratrack.Count - 1]));
        dontUseHistory = true;
        CameraTrackSelection.options.Add(opt);
        CameraTrackSelection.value = 0;
        CameraTrackSelection.value = cameratrack.Count - 1;
        updlimit = false;
        ChangeCameraTrack(cameratrack.Count - 1);
        dontUseHistory = false;
    }

    public void RemoveCameraTrack()
    {
        if (cameratrack.Count <= 1) return;
        if (RealtimeCameraEditActive)
            ToggleCameraControl();
        AddToHistoryCam(new HRemoveCameraTrack(curcam, Mathf.Min(curcam, cameratrack.Count - 2), cameratrack[curcam]));
        cameratrack.RemoveAt(curcam);
        dontUseHistory = true;
        CameraTrackSelection.options.RemoveAt(curcam);
        int oldcur = curcam;
        CameraTrackSelection.value = 0;
        CameraTrackSelection.value = Mathf.Min(oldcur, cameratrack.Count - 1);
        updlimit = false;
        ChangeCameraTrack(CameraTrackSelection.value);
        dontUseHistory = false;
    }

    public Button RemoveCameraButton;

    public Dropdown actionDropdown;

    public CropReplayUI imreplayscript;

    public void SegmentEditAction(int num)
    {
        if (num == 0) return;
        if (RealtimeCameraEditActive) return;
        playing = false;
        List<NISLoader.CameraTrackEntry> entries;
        NISLoader.CameraTrackEntry base_entry;
        NISLoader.CameraTrackEntry cur_entry;
        NISLoader.CameraTrackEntry old_entry;
        NISLoader.CameraTrackEntry old_entry2;
        actionDropdown.value = 0;
        num -= 1;
        NISLoader.camrec rec;
        (NISLoader.camrec, NISLoader.camrec) split_result;
        if (TimelineLockToggle.isOn)
            TimelineLockToggle.isOn = false;
        switch (num)
        {
            case 0:
                switch (cameratrack[curcam].Item2[cursegment].type)
                {
                    default:
                        old_entry = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment].obj0.Clone());
                        break;
                }
                rec = new NISLoader.camrec(cameratrack[curcam].Item2[cursegment]);
                split_result = rec.SplitInTwo(cursegment < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[cursegment + 1].Time : 1f);
                entries = cameratrack[curcam].Item2.ToList();
                entries[cursegment] = split_result.Item1.e;
                entries.Insert(cursegment + 1, split_result.Item2.e);
                cameratrack[curcam] = (cameratrack[curcam].Item1, entries.ToArray());
                GenCameraTrackPreview();
                GenCameraSplines();
                UpdCameraTrackPreview();
                if (!dontUseHistory)
                    AddToHistoryCam(new SplitSegment(curcam, cursegment, timeline, old_entry));
                break;
            case 1:
                if (cursegment == 0) return;
                switch (cameratrack[curcam].Item2[cursegment - 1].type)
                {
                    default:
                        old_entry = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment - 1].obj0.Clone());
                        old_entry2 = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment].obj0.Clone());
                        break;
                }
                base_entry = cameratrack[curcam].Item2[cursegment - 1];
                cur_entry = cameratrack[curcam].Item2[cursegment];
                base_entry.unk6 = cur_entry.unk6;
                base_entry.EyeX2 = cur_entry.EyeX2;
                base_entry.EyeZ2 = cur_entry.EyeZ2;
                base_entry.EyeY2 = cur_entry.EyeY2;
                base_entry.LookX2 = cur_entry.LookX2;
                base_entry.LookZ2 = cur_entry.LookZ2;
                base_entry.LookY2 = cur_entry.LookY2;
                base_entry.Tangent2 = cur_entry.Tangent2;
                base_entry.FocalLength2 = cur_entry.FocalLength2;
                base_entry.unk10 = cur_entry.unk10;
                base_entry.Amp2 = cur_entry.Amp2;
                base_entry.Freq2 = cur_entry.Freq2;
                base_entry.unk12 = cur_entry.unk12;
                entries = cameratrack[curcam].Item2.ToList();
                entries[cursegment - 1] = base_entry;
                entries.RemoveAt(cursegment);
                cameratrack[curcam] = (cameratrack[curcam].Item1, entries.ToArray());
                GenCameraTrackPreview();
                GenCameraSplines();
                UpdCameraTrackPreview();
                if (!dontUseHistory)
                    AddToHistoryCam(new MergeSegmentLeft(curcam, cursegment, timeline, old_entry, old_entry2));
                break;
            case 2:
                if (cursegment == cameratrack[curcam].Item2.Length - 1) return;
                switch (cameratrack[curcam].Item2[cursegment].type)
                {
                    default:
                        old_entry = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment].obj0.Clone());
                        old_entry2 = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment + 1].obj0.Clone());
                        break;
                }
                base_entry = cameratrack[curcam].Item2[cursegment];
                cur_entry = cameratrack[curcam].Item2[cursegment + 1];
                base_entry.unk6 = cur_entry.unk6;
                base_entry.EyeX2 = cur_entry.EyeX2;
                base_entry.EyeZ2 = cur_entry.EyeZ2;
                base_entry.EyeY2 = cur_entry.EyeY2;
                base_entry.LookX2 = cur_entry.LookX2;
                base_entry.LookZ2 = cur_entry.LookZ2;
                base_entry.LookY2 = cur_entry.LookY2;
                base_entry.Tangent2 = cur_entry.Tangent2;
                base_entry.FocalLength2 = cur_entry.FocalLength2;
                base_entry.unk10 = cur_entry.unk10;
                base_entry.Amp2 = cur_entry.Amp2;
                base_entry.Freq2 = cur_entry.Freq2;
                base_entry.unk12 = cur_entry.unk12;
                entries = cameratrack[curcam].Item2.ToList();
                entries[cursegment] = base_entry;
                entries.RemoveAt(cursegment + 1);
                cameratrack[curcam] = (cameratrack[curcam].Item1, entries.ToArray());
                GenCameraTrackPreview();
                GenCameraSplines();
                UpdCameraTrackPreview();
                if (!dontUseHistory)
                    AddToHistoryCam(new MergeSegmentRight(curcam, cursegment + 1, timeline, old_entry, old_entry2));
                break;
            case 3:
                switch (cameratrack[curcam].Item2[cursegment].type)
                {
                    default:
                        old_entry = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment].obj0.Clone());
                        break;
                }
                rec = new NISLoader.camrec(cameratrack[curcam].Item2[cursegment]);
                split_result = rec.SplitInTwo(cursegment < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[cursegment + 1].Time : 1f);
                base_entry = split_result.Item2.e;
                cur_entry = cameratrack[curcam].Item2[cursegment];
                base_entry.unk5 = cur_entry.unk6;
                base_entry.EyeX = cur_entry.EyeX2;
                base_entry.EyeZ = cur_entry.EyeZ2;
                base_entry.EyeY = cur_entry.EyeY2;
                base_entry.LookX = cur_entry.LookX2;
                base_entry.LookZ = cur_entry.LookZ2;
                base_entry.LookY = cur_entry.LookY2;
                base_entry.Tangent = cur_entry.Tangent2;
                base_entry.FocalLength = cur_entry.FocalLength2;
                base_entry.unk9 = cur_entry.unk10;
                base_entry.Amp = cur_entry.Amp2;
                base_entry.Freq = cur_entry.Freq2;
                base_entry.unk11 = cur_entry.unk12;
                base_entry.unk6 = cur_entry.unk6;
                base_entry.EyeX2 = cur_entry.EyeX2;
                base_entry.EyeZ2 = cur_entry.EyeZ2;
                base_entry.EyeY2 = cur_entry.EyeY2;
                base_entry.LookX2 = cur_entry.LookX2;
                base_entry.LookZ2 = cur_entry.LookZ2;
                base_entry.LookY2 = cur_entry.LookY2;
                base_entry.Tangent2 = cur_entry.Tangent2;
                base_entry.FocalLength2 = cur_entry.FocalLength2;
                base_entry.unk10 = cur_entry.unk10;
                base_entry.Amp2 = cur_entry.Amp2;
                base_entry.Freq2 = cur_entry.Freq2;
                base_entry.unk12 = cur_entry.unk12;
                entries = cameratrack[curcam].Item2.ToList();
                entries.Insert(cursegment + 1, base_entry);
                cameratrack[curcam] = (cameratrack[curcam].Item1, entries.ToArray());
                GenCameraTrackPreview();
                GenCameraSplines();
                UpdCameraTrackPreview();
                if (!dontUseHistory)
                    AddToHistoryCam(new CreateSegment(curcam, cursegment, timeline, old_entry));
                break;
            case 4:
                if (cameratrack[curcam].Item2.Length <= 1) return;
                switch (cameratrack[curcam].Item2[cursegment].type)
                {
                    default:
                        old_entry = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment].obj0.Clone());
                        break;
                }
                entries = cameratrack[curcam].Item2.ToList();
                entries.RemoveAt(cursegment);
                float old_time = 0f;
                if (cursegment == 0)
                {
                    var v = entries[0];
                    old_time = v.Time;
                    v.Time = 0f;
                    entries[0] = v;
                }
                cameratrack[curcam] = (cameratrack[curcam].Item1, entries.ToArray());
                GenCameraTrackPreview();
                GenCameraSplines();
                UpdCameraTrackPreview();
                if (!dontUseHistory)
                    AddToHistoryCam(new RemoveSegment(curcam, cursegment, timeline, old_entry, old_time));
                break;
            case 5:
                if (cameraeditcurtab < 2) {
                    copied_values = new string[CameraEditValues.Length];
                    for (int i = 0; i < CameraEditValues.Length; i++)
                        copied_values[i] = CameraEditValues[i].text;
                } else {
                    copied_flags = new string[CameraEditFlags.Length];
                    for (int i = 0; i < CameraEditFlags.Length; i++)
                        copied_flags[i] = CameraEditFlags[i].text;
                }
                Debug.Log("Values copied");
                break;
            case 6:
                blockupdcall = true;
                if (cameraeditcurtab < 2) {
                    if (copied_values != null)
                    {
                        for (int i = 0; i < CameraEditValues.Length; i++)
                            CameraEditValues[i].text = copied_values[i];
                        blockupdcall = false;
                        CameraValuesUpdated();
                    }
                } else {
                    if (copied_flags != null)
                    {
                        for (int i = 0; i < CameraEditFlags.Length; i++)
                            CameraEditFlags[i].text = copied_flags[i];
                        blockupdcall = false;
                        CameraFlagsUpdated();
                    }
                }
                blockupdcall = false;
                Debug.Log("Values pasted");
                break;
        }
        var h = cameratrack[curcam].Item1;
        h.entryCount = (short)cameratrack[curcam].Item2.Length;
        cameratrack[curcam] = (h, cameratrack[curcam].Item2);
    }

    public Material LineMaterial;

    public void DrawCameraPath()
    {
        foreach (Transform rend in LineRenderers)
            Destroy(rend.gameObject);
        if (tabnum != 3) return;
        GameObject obj;
        LineRenderer renderer;
        NISLoader.CameraTrackEntry e;
        Vector3 startpos;
        Vector3 endpos;
        (float[], float[], float, bool) eval;
        Vector3 player_car_position;
        Quaternion player_car_rotation;
        
        for (int i = 0; i < cameratrack[curcam].Item2.Length; i++)
        {
            obj = new GameObject(i.ToString());
            obj.transform.SetParent(LineRenderers);
            renderer = obj.AddComponent<LineRenderer>();
            renderer.material = LineMaterial;
            renderer.widthMultiplier = 0.1f;
            renderer.startColor = Color.green;
            renderer.endColor = Color.green;
            e = cameratrack[curcam].Item2[i];
            startpos = new Vector3(e.EyeX, e.EyeY, e.EyeZ);
            endpos = new Vector3(e.EyeX2, e.EyeY2, e.EyeZ2);
            if (e.attributes[4] == 0x00)
            {
                NISLoader.Animation anim_pos = null;
                NISLoader.Animation anim_rot = null;
                foreach (NISLoader.Animation an in anims)
                {
                    if (an.name.EndsWith("_Car1_t"))
                        anim_pos = an;
                    else if (an.name.EndsWith("_Car1_q"))
                        anim_rot = an;
                }

                if (anim_pos != null && anim_rot != null)
                {
                    RaycastHit hit;

                    eval = NISLoader.EvaluateAnim(anim_pos, e.Time * cameratrack[curcam].Item1.Duration);
                    player_car_position = Vector3.Lerp(new Vector3(eval.Item1[0], eval.Item1[2], eval.Item1[1]), new Vector3(eval.Item2[0], eval.Item2[2], eval.Item2[1]), eval.Item3);
                    player_car_position = new Vector3(player_car_position.x, forceY ? NISLoader.GetGroundY(player_car_position, out hit) : player_car_position.y, player_car_position.z);
                    eval = NISLoader.EvaluateAnim(anim_rot, e.Time * cameratrack[curcam].Item1.Duration);
                    player_car_rotation = Quaternion.Lerp(new Quaternion(eval.Item1[0], eval.Item1[1], eval.Item1[2], eval.Item1[3]), new Quaternion(eval.Item2[0], eval.Item2[1], eval.Item2[2], eval.Item2[3]), eval.Item3);
                    startpos = player_car_position + Quaternion.Euler(0f, -90f + (90f - player_car_rotation.z), 0f) * startpos;

                    eval = NISLoader.EvaluateAnim(anim_pos, (i < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[i + 1].Time : 1f) * cameratrack[curcam].Item1.Duration);
                    player_car_position = Vector3.Lerp(new Vector3(eval.Item1[0], eval.Item1[2], eval.Item1[1]), new Vector3(eval.Item2[0], eval.Item2[2], eval.Item2[1]), eval.Item3);
                    player_car_position = new Vector3(player_car_position.x, forceY ? NISLoader.GetGroundY(player_car_position, out hit) : player_car_position.y, player_car_position.z);
                    eval = NISLoader.EvaluateAnim(anim_rot, (i < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[i + 1].Time : 1f) * cameratrack[curcam].Item1.Duration);
                    player_car_rotation = Quaternion.Lerp(new Quaternion(eval.Item1[0], eval.Item1[1], eval.Item1[2], eval.Item1[3]), new Quaternion(eval.Item2[0], eval.Item2[1], eval.Item2[2], eval.Item2[3]), eval.Item3);
                    endpos = player_car_position + Quaternion.Euler(0f, -90f + (90f - player_car_rotation.z), 0f) * startpos;
                }
            }
            renderer.SetPositions(new [] {startpos, endpos});
            
            obj = new GameObject(i.ToString());
            obj.transform.SetParent(LineRenderers);
            renderer = obj.AddComponent<LineRenderer>();
            renderer.material = LineMaterial;
            renderer.widthMultiplier = 0.1f;
            renderer.startColor = Color.red;
            renderer.endColor = Color.red;
            e = cameratrack[curcam].Item2[i];
            startpos = new Vector3(e.LookX, e.LookY, e.LookZ);
            endpos = new Vector3(e.LookX2, e.LookY2, e.LookZ2);
            if (e.attributes[4] == 0x00)
            {
                NISLoader.Animation anim_pos = null;
                NISLoader.Animation anim_rot = null;
                foreach (NISLoader.Animation an in anims)
                {
                    if (an.name.EndsWith("_Car1_t"))
                        anim_pos = an;
                    else if (an.name.EndsWith("_Car1_q"))
                        anim_rot = an;
                }

                if (anim_pos != null && anim_rot != null)
                {
                    RaycastHit hit;

                    eval = NISLoader.EvaluateAnim(anim_pos, e.Time * cameratrack[curcam].Item1.Duration);
                    player_car_position = Vector3.Lerp(new Vector3(eval.Item1[0], eval.Item1[2], eval.Item1[1]), new Vector3(eval.Item2[0], eval.Item2[2], eval.Item2[1]), eval.Item3);
                    player_car_position = new Vector3(player_car_position.x, forceY ? NISLoader.GetGroundY(player_car_position, out hit) : player_car_position.y, player_car_position.z);
                    eval = NISLoader.EvaluateAnim(anim_rot, e.Time * cameratrack[curcam].Item1.Duration);
                    player_car_rotation = Quaternion.Lerp(new Quaternion(eval.Item1[0], eval.Item1[1], eval.Item1[2], eval.Item1[3]), new Quaternion(eval.Item2[0], eval.Item2[1], eval.Item2[2], eval.Item2[3]), eval.Item3);
                    startpos = player_car_position + Quaternion.Euler(0f, -90f + (90f - player_car_rotation.z), 0f) * startpos;

                    eval = NISLoader.EvaluateAnim(anim_pos, (i < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[i + 1].Time : 1f) * cameratrack[curcam].Item1.Duration);
                    player_car_position = Vector3.Lerp(new Vector3(eval.Item1[0], eval.Item1[2], eval.Item1[1]), new Vector3(eval.Item2[0], eval.Item2[2], eval.Item2[1]), eval.Item3);
                    player_car_position = new Vector3(player_car_position.x, forceY ? NISLoader.GetGroundY(player_car_position, out hit) : player_car_position.y, player_car_position.z);
                    eval = NISLoader.EvaluateAnim(anim_rot, (i < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[i + 1].Time : 1f) * cameratrack[curcam].Item1.Duration);
                    player_car_rotation = Quaternion.Lerp(new Quaternion(eval.Item1[0], eval.Item1[1], eval.Item1[2], eval.Item1[3]), new Quaternion(eval.Item2[0], eval.Item2[1], eval.Item2[2], eval.Item2[3]), eval.Item3);
                    endpos = player_car_position + Quaternion.Euler(0f, -90f + (90f - player_car_rotation.z), 0f) * startpos;
                }
            }
            renderer.SetPositions(new [] {startpos, endpos});
        }
    }

    public GameObject CameraEditValuesRoot;
    public GameObject CameraEditFlagsRoot;
    public ToggleGroup CameraEditTabRoot;
    private int cameraeditcurtab;
    public InputField[] CameraEditValues;
    public InputField[] CameraEditFlags;

    public void CameraEditTabChanged()
    {
        if (updlimit) return;
        if (playing)
            playing = false;
        cameraeditcurtab = CameraEditTabRoot.GetComponent<ToggleGroup>().ActiveToggles().ToArray()[0].transform.GetSiblingIndex();
        updlimit = true;
        CameraEditValuesRoot.SetActive(cameraeditcurtab < 2);
        CameraEditFlagsRoot.SetActive(cameraeditcurtab == 2);
        oldsegment = -1;
        if (RealtimeCameraEditActive)
            ToggleCameraControl();
        if (cameraeditcurtab == 0)
            timeline = cameratrack[curcam].Item2[cursegment].Time + 0.001f;
        else if (cameraeditcurtab == 1)
            timeline = cursegment < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[cursegment + 1].Time - 0.001f : 1f;
    }

    private NISLoader.CameraTrackEntry CameraEditOld;

    public void CameraValuesUpdated()
    {
        if (playing || !allow_changes) return;
        if (blockupdcall) return;
        bool changed = false;
        NISLoader.CameraTrackEntry old_;
        switch (cameratrack[curcam].Item2[cursegment].type)
        {
            default:
                old_ = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment].obj0.Clone());
                break;
        }
        float old;
        switch (cameraeditcurtab)
        {
            case 0:
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].unk5;
                    cameratrack[curcam].Item2[cursegment].unk5 = float.Parse(CameraEditValues[0].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].unk5)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].EyeX;
                    cameratrack[curcam].Item2[cursegment].EyeX = float.Parse(CameraEditValues[1].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].EyeX)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].EyeZ;
                    cameratrack[curcam].Item2[cursegment].EyeZ = float.Parse(CameraEditValues[2].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].EyeZ)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].EyeY;
                    cameratrack[curcam].Item2[cursegment].EyeY = float.Parse(CameraEditValues[3].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].EyeY)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].LookX;
                    cameratrack[curcam].Item2[cursegment].LookX = float.Parse(CameraEditValues[4].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].LookX)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].LookZ;
                    cameratrack[curcam].Item2[cursegment].LookZ = float.Parse(CameraEditValues[5].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].LookZ)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].LookY;
                    cameratrack[curcam].Item2[cursegment].LookY = float.Parse(CameraEditValues[6].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].LookY)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].Tangent;
                    cameratrack[curcam].Item2[cursegment].Tangent = float.Parse(CameraEditValues[7].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].Tangent)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].FocalLength;
                    cameratrack[curcam].Item2[cursegment].FocalLength = float.Parse(CameraEditValues[8].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].FocalLength)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].unk9;
                    cameratrack[curcam].Item2[cursegment].unk9 = float.Parse(CameraEditValues[9].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].unk9)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].Amp;
                    cameratrack[curcam].Item2[cursegment].Amp = float.Parse(CameraEditValues[10].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].Amp)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].Freq;
                    cameratrack[curcam].Item2[cursegment].Freq = float.Parse(CameraEditValues[11].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].Freq)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].unk11;
                    cameratrack[curcam].Item2[cursegment].unk11 = float.Parse(CameraEditValues[12].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].unk11)
                        changed = true;
                } catch {}
                try {
                    old = cameratrack[curcam].Item2[cursegment].unk13[4];
                    cameratrack[curcam].Item2[cursegment].unk13[4] = byte.Parse(CameraEditValues[13].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].unk13[4])
                        changed = true;
                } catch { }
                try {
                    old = cameratrack[curcam].Item2[cursegment].unk13[0];
                    cameratrack[curcam].Item2[cursegment].unk13[0] = byte.Parse(CameraEditValues[14].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].unk13[0])
                        changed = true;
                } catch { }
                break;
            default:
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].unk6;
                    cameratrack[curcam].Item2[cursegment].unk6 = float.Parse(CameraEditValues[0].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].unk6)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].EyeX2;
                    cameratrack[curcam].Item2[cursegment].EyeX2 = float.Parse(CameraEditValues[1].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].EyeX2)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].EyeZ2;
                    cameratrack[curcam].Item2[cursegment].EyeZ2 = float.Parse(CameraEditValues[2].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].EyeZ2)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].EyeY2;
                    cameratrack[curcam].Item2[cursegment].EyeY2 = float.Parse(CameraEditValues[3].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].EyeY2)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].LookX2;
                    cameratrack[curcam].Item2[cursegment].LookX2 = float.Parse(CameraEditValues[4].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].LookX2)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].LookZ2;
                    cameratrack[curcam].Item2[cursegment].LookZ2 = float.Parse(CameraEditValues[5].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].LookZ2)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].LookY2;
                    cameratrack[curcam].Item2[cursegment].LookY2 = float.Parse(CameraEditValues[6].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].LookY2)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].Tangent2;
                    cameratrack[curcam].Item2[cursegment].Tangent2 = float.Parse(CameraEditValues[7].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].Tangent2)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].FocalLength2;
                    cameratrack[curcam].Item2[cursegment].FocalLength2 = float.Parse(CameraEditValues[8].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].FocalLength2)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].unk10;
                    cameratrack[curcam].Item2[cursegment].unk10 = float.Parse(CameraEditValues[9].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].unk10)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].Amp2;
                    cameratrack[curcam].Item2[cursegment].Amp2 = float.Parse(CameraEditValues[10].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].Amp2)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].Freq2;
                    cameratrack[curcam].Item2[cursegment].Freq2 = float.Parse(CameraEditValues[11].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].Freq2)
                        changed = true;
                } catch {}
                try
                {
                    old = cameratrack[curcam].Item2[cursegment].unk12;
                    cameratrack[curcam].Item2[cursegment].unk12 = float.Parse(CameraEditValues[12].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].unk12)
                        changed = true;
                } catch {}
                try {
                    old = cameratrack[curcam].Item2[cursegment].unk13[5];
                    cameratrack[curcam].Item2[cursegment].unk13[5] = byte.Parse(CameraEditValues[13].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].unk13[5])
                        changed = true;
                } catch { }
                try {
                    old = cameratrack[curcam].Item2[cursegment].unk13[1];
                    cameratrack[curcam].Item2[cursegment].unk13[1] = byte.Parse(CameraEditValues[14].text, CultureInfo.InvariantCulture);
                    if (old != cameratrack[curcam].Item2[cursegment].unk13[1])
                        changed = true;
                } catch { }
                break;
        }
        GenCameraSplines();
        if (changed && !RealtimeCameraEditActive)
        {
            NISLoader.CameraTrackEntry new_;
            switch (cameratrack[curcam].Item2[cursegment].type)
            {
                default:
                    new_ = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment].obj0.Clone());
                    break;
            }
            AddToHistoryCam(new EditCameraSegment(curcam, cursegment, old_, new_));
        }
    }

    public void CameraFlagsUpdated()
    {
        if (playing || !allow_changes) return;
        if (blockupdcall) return;
        bool changed = false;
        NISLoader.CameraTrackEntry old_;
        switch (cameratrack[curcam].Item2[cursegment].type)
        {
            default:
                old_ = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment].obj0.Clone());
                break;
        }
        byte old;
        for (int i = 0; i < 12; i++)
        {
            try
            {
                old = cameratrack[curcam].Item2[cursegment].attributes[i];
                cameratrack[curcam].Item2[cursegment].attributes[i] = (byte) Convert.ToInt32(CameraEditFlags[i].text, 16);
                if (old != cameratrack[curcam].Item2[cursegment].attributes[i])
                    changed = true;
            } catch {}
        }
        try
        {
            uint hash = Convert.ToUInt32(CameraEditFlags[12].text.Substring(2), 16);
            byte[] bb = BitConverter.GetBytes(hash);
            for (int i = 12; i < 16; i++)
            {
                old = cameratrack[curcam].Item2[cursegment].attributes[i];
                cameratrack[curcam].Item2[cursegment].attributes[i] = bb[i - 12];
                if (old != cameratrack[curcam].Item2[cursegment].attributes[i])
                    changed = true;
            }
        } catch {}
        GenCameraSplines();
        if (changed)
        {
            NISLoader.CameraTrackEntry new_;
            switch (cameratrack[curcam].Item2[cursegment].type)
            {
                default:
                    new_ = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment].obj0.Clone());
                    break;
            }
            AddToHistoryCam(new EditCameraSegment(curcam, cursegment, old_, new_));
        }
    }

    public InputField DurationField;
    public bool TimelineLock;

    public void UpdateTimelineLock(bool enable)
    {
        TimelineLock = enable;
    }

    public Text CameraControlButtonText;
    public bool RealtimeCameraEditActive;

    public void ToggleCameraControl()
    {
        RealtimeCameraEditActive = !RealtimeCameraEditActive;
        CameraControlButtonText.text = RealtimeCameraEditActive ? "Press here again when you're done" : "Enable WYSIWYG camera edit mode";
        if (RealtimeCameraEditActive) {
            playing = false;

            switch (cameratrack[curcam].Item2[cursegment].type)
            {
                default:
                    CameraEditOld = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment].obj0.Clone());
                    break;
            }

            Vector3 eyepos;
            Vector3 lookatpos;
            if (cameraeditcurtab == 0)
            {
                eyepos = new Vector3(cameratrack[curcam].Item2[cursegment].EyeX, cameratrack[curcam].Item2[cursegment].EyeY, cameratrack[curcam].Item2[cursegment].EyeZ);
                lookatpos = new Vector3(cameratrack[curcam].Item2[cursegment].LookX, cameratrack[curcam].Item2[cursegment].LookY, cameratrack[curcam].Item2[cursegment].LookZ);
            }
            else
            {
                eyepos = new Vector3(cameratrack[curcam].Item2[cursegment].EyeX2, cameratrack[curcam].Item2[cursegment].EyeY2, cameratrack[curcam].Item2[cursegment].EyeZ2);
                lookatpos = new Vector3(cameratrack[curcam].Item2[cursegment].LookX2, cameratrack[curcam].Item2[cursegment].LookY2, cameratrack[curcam].Item2[cursegment].LookZ2);
            }

            switch (cameratrack[curcam].Item2[cursegment].attributes[4])
            {
                case 0x00:
                    Transform player_car = ObjectsOnScene.ContainsKey("Car1") ? ObjectsOnScene["Car1"] : SceneRoot;
                    eyepos = player_car.position + Quaternion.Euler(0f, -90f + player_car.eulerAngles.y, 0f) * eyepos;
                    lookatpos = player_car.position + Quaternion.Euler(0f, -90f + player_car.eulerAngles.y, 0f) * lookatpos;
                    break;
            }
            SceneCamera.transform.position = eyepos;
            SceneCamera.transform.LookAt(lookatpos);
            FocusSphere.position = lookatpos;
            
            if (cameraeditcurtab == 0)
                timeline = cameratrack[curcam].Item2[cursegment].Time + 0.001f;
            else
                timeline = cursegment < cameratrack[curcam].Item2.Length - 1 ? cameratrack[curcam].Item2[cursegment + 1].Time - 0.001f : 1f;
            gizmo2.AddTarget(FocusSphere);
        }
        else
        {
            gizmo2.RemoveTarget(FocusSphere);
            NISLoader.CameraTrackEntry new_;
            switch (cameratrack[curcam].Item2[cursegment].type)
            {
                default:
                    new_ = new NISLoader.CameraTrackEntry(cameratrack[curcam].Item2[cursegment].obj0.Clone());
                    break;
            }
            AddToHistoryCam(new EditCameraSegment(curcam, cursegment, CameraEditOld, new_));
            
        }
    }

    public void Bigger()
    {
        GetComponent<CanvasScaler>().scaleFactor = Mathf.Clamp(GetComponent<CanvasScaler>().scaleFactor + 0.1f, 1f, 2f);
        PlayerPrefs.SetFloat("GUIScale", GetComponent<CanvasScaler>().scaleFactor);
    }

    public void Smaller()
    {
        GetComponent<CanvasScaler>().scaleFactor = Mathf.Clamp(GetComponent<CanvasScaler>().scaleFactor - 0.1f, 1f, 2f);
        PlayerPrefs.SetFloat("GUIScale", GetComponent<CanvasScaler>().scaleFactor);
    }

    //public void OpenKofi()
    //{
    //    Application.OpenURL("https://ko-fi.com/r033cx");
    //}
}
