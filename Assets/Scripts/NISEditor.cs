using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
#endif

public class NISEditor : MonoBehaviour
{
    public Scrollbar timeline;
    public GameObject PlayIcon;
    public GameObject StopIcon;
    public Text OutputText;

    public string NISName;
    public string CameraTrack;
    public int NISID;
    public bool Load;
    public bool LoadByID;
    public bool DrawBoneNames;
    
    public Vector3 StartPosition;
    public float StartRotation;
    public Transform car;

    private bool isPlaying;
    private NISLoader loader;
    private List<NISLoader.CameraSpline> cameratrack;
    private float totalLength;
    private float timeinSec;
    private float ct;

    private float camTimeline;
    private float fixedTimeline;

    void Start()
    {
        loader = FindObjectOfType<NISLoader>();
        if (loader == null)
            loader = gameObject.AddComponent<NISLoader>();
    }

    void OnEnable()
    {
        isPlaying = false;
        cameratrack = null;
        totalLength = 1f;
        PlayIcon.SetActive(true);
        StopIcon.SetActive(false);
        OutputText.text = "No data to display";
    }

    void OnDisable()
    {
        if (Camera.main == null)
            return;
        Camera.main.usePhysicalProperties = false;
        Camera.main.fieldOfView = 79f;
        if (loader.LetterBox != null)
            Destroy(loader.LetterBox);
        //TurnOnCamera();
    }
    
    Dictionary<string, Transform> objs = new Dictionary<string, Transform>();
    public GameObject ArrowTest;
    private List<NISLoader.Animation> anims;
    private List<NISLoader.Skeleton> skeletons;
    Transform target;
    //Dictionary<Transform, CarController> ye = new Dictionary<Transform, CarController>();
    private bool animslocal;

    /*(Vector3, Quaternion) GetBonePos(NISLoader.Bone[] bones, int ind)
    {
        Vector3 startpos = Vector3.zero;
        Quaternion startrot = Quaternion.identity;
        if (bones[ind].parent != -1)
            (startpos, startrot) = GetBonePos(bones, bones[ind].parent);
        startrot *= bones[ind].rotation;
        startpos += startrot * new Vector3(bones[ind].position.x, bones[ind].position.y, bones[ind].position.z);
        return (startpos, startrot);
    }
    
    Color[] c = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta
    };

    void OnDrawGizmos()
    {
        if (skeletons == null)
            return;
        Dictionary<string, NISLoader.Animation> skeletonAnims = new Dictionary<string, NISLoader.Animation>();
        foreach (NISLoader.Animation anim in anims)
        {
            if (anim.type == NISLoader.AnimType.ANIM_COMPOUND && anim.subAnimations.Count > 1)
                skeletonAnims.Add(anim.name.Split('_')[1].ToUpper(), anim);
        }
        for (int sk = 0; sk < skeletons.Count; sk++)
        {
            string objname = skeletons[sk].attachedMesh.name.Replace("01", "");
            Vector3 offset = Vector3.zero;
            foreach (string objName in objs.Keys)
            {
                if (objName.ToUpper() == objname)
                {
                    offset = objs[objName].position;
                    break;
                }
            }
            Vector3 globalpos;
            Vector3 parentpos;
            Gizmos.color = c[sk];
            loader.ApplyBoneAnimation(skeletons[sk].bones, skeletonAnims[objname], timeinSec);
            for (int boneNum = 0; boneNum < skeletons[sk].bones.Length; boneNum++)
            {
                globalpos = GetBonePos(skeletons[sk].bones, boneNum).Item1;
                globalpos = new Vector3(globalpos.x, globalpos.z, globalpos.y);
                if (DrawBoneNames)
                    Handles.Label(offset + globalpos, skeletons[sk].bones[boneNum].name);
                if (skeletons[sk].bones[boneNum].parent != -1)
                {
                    parentpos = GetBonePos(skeletons[sk].bones, skeletons[sk].bones[boneNum].parent).Item1;
                    parentpos = new Vector3(parentpos.x, parentpos.z, parentpos.y);
                    Gizmos.DrawLine(offset + globalpos, offset + parentpos);
                }
            }
        }
    }*/

    string GetBonePath(NISLoader.Bone[] bones, int ind)
    {
        string path = bones[ind].name;
        if (bones[ind].parent != -1)
            path = GetBonePath(bones, bones[ind].parent) + "/" + path;
        return path;
    }

    private Dictionary<string, NISLoader.Animation> skeletonAnims;

    /*void Update()
    {
        if (isPlaying && camTimeline < 1f)
        {
            camTimeline += Time.deltaTime / totalLength;
            timeline.value = camTimeline;
            if (camTimeline >= 1f)
                TogglePlay();
        }
        if (Load)
        {
            Load = false;
            timeline.value = 0f;
            (anims, skeletons) = loader.LoadAnimations(NISName);
            skeletonAnims = new Dictionary<string, NISLoader.Animation>();
            foreach (NISLoader.Animation anim in anims)
            {
                if (anim.type == NISLoader.AnimType.ANIM_COMPOUND && anim.subAnimations.Count > 1)
                    skeletonAnims.Add(anim.name.Split('_')[1].ToUpper(), anim);
            }
            NISLoader.SceneType sceneType = (NISLoader.SceneType) loader.SceneInfo.SceneType;
            animslocal = sceneType != NISLoader.SceneType.NIS_SCENE_LOCATION_SPECIFIC;
            try
            {
                cameratrack = loader.MakeSplines(loader.LoadCameraTrack(NISName, 0, false, CameraTrack, out totalLength));
                TurnOffCamera();
            }
            catch
            {
                cameratrack = null;
                totalLength = loader.CalculateNISDuration(anims);
                TurnOnCamera();
            }
            
            foreach (string _car in objs.Keys)
            {
                if (ye.ContainsKey(objs[_car]) || car == objs[_car])
                    continue;
                Destroy(objs[_car].gameObject);
            }

            List<string> alreadyDid = new List<string>();
            objs.Clear();
            ye.Clear();
            foreach (NISLoader.Animation anim in anims)
            {
                string[] args = anim.name.Split('_');
                if (!alreadyDid.Contains(args[1]))
                {
                    if (car != null && objs.Count == 0)
                    {
                        objs.Add(args[1], car);
                        if (FindObjectOfType<GlobalWorldController>() != null)
                            ye.Add(car, car.GetComponent<CarController>());
                    }
                    else
                    {
                        target = new GameObject(args[1]).transform;
                        if (anim.type == NISLoader.AnimType.ANIM_COMPOUND && anim.subAnimations.Count > 1)
                        {
                            NISLoader.Skeleton skeleton = null;
                            foreach (NISLoader.Skeleton s in skeletons)
                                if (s.attachedMesh.name.Replace("01", "") == args[1].ToUpper())
                                {
                                    skeleton = s;
                                    break;
                                }
                            if (skeleton == null)
                                Debug.LogError("Can't find skeleton for object " + args[1]);
                            else
                                loader.GenerateSkinnedObject(target, skeleton);
                        }
                        else
                        {
                            Instantiate(ArrowTest, target);
                        }
                        objs.Add(args[1], target);
                    }
                    alreadyDid.Add(args[1]);
                }
            }
            if (isPlaying)
                TogglePlay();
        }

        if (LoadByID)
        {
            LoadByID = false;
            timeline.value = 0f;
            cameratrack = loader.MakeSplines(loader.LoadCameraTrack("", NISID, true, "", out totalLength));
            if (isPlaying)
                TogglePlay();
        }

        if (anims == null) return;
        
        timeinSec = camTimeline * totalLength;
        OutputText.text = $"{loader.SceneInfo.SceneName}\n{loader.SceneInfo.SeeulatorOverlayName}\n{loader.SceneDescription}\n" + (cameratrack != null ? $"Camera: {loader.CameraTrackHash} ({string.Join(", ", loader.CameraTrackNames)})\n{BitConverter.ToString(loader.cam_attrs)}\n" : "") + $"{RaceScript.TimeString(timeinSec)}\n";

        foreach (NISLoader.Animation anim in anims)
        {
            loader.ApplyCarMovement(objs, ye, anim, timeinSec, animslocal, true);
        }

        for (int sk = 0; sk < skeletons.Count; sk++)
        {
            loader.ApplyBoneAnimation(skeletons[sk].bones, skeletonAnims[skeletons[sk].animationName], timeinSec);
        }

        if (cameratrack == null) return;
        
        for (int i = 0; i < cameratrack.Count; i++)
        {
            if (camTimeline >= cameratrack[i].start && camTimeline <= cameratrack[i].end)
            {
                /*NISLoader.camrec rec = cameratrack[i].cam[0];
                for (int j = 0; j < cameratrack[i].cam.Count; j++)
                {
                    if (camTimeline >= cameratrack[i].cam[j].e.Time && camTimeline <= cameratrack[i].cam[Mathf.Min(j, cameratrack[i].cam.Count - 1)].e.Time)
                    {
                        rec = cameratrack[i].cam[j];
                        break;
                    }
                }*/
                //Vector3 camstart = StartPosition;
                //float camrot = StartRotation;
                //loader.ApplyCameraMovement(cameratrack[i], camstart, Quaternion.Euler(0f, camrot, 0f), camTimeline, timeinSec, car == null ? objs["Car1"] : car);
                /*OutputText.text += "\n" + Mathf.Lerp(rec.e.FocalLength, rec.e.FocalLength, Mathf.InverseLerp(rec.e.Time, f, camTimeline));
                OutputText.text += "\n" + Mathf.Lerp(rec.e.unk9, rec.e.unk10, Mathf.InverseLerp(rec.e.Time, f, camTimeline));
                OutputText.text += "\n" + Mathf.Lerp(rec.e.unk11, rec.e.unk12, Mathf.InverseLerp(rec.e.Time, f, camTimeline));
                OutputText.text += "\n" + Vector3.Distance( Vector3.Lerp(new Vector3(rec.e.LookX, rec.e.LookY, rec.e.LookZ), new Vector3(rec.e.LookX2, rec.e.LookY2, rec.e.LookZ2), camTimeline),
                                                            Vector3.Lerp(new Vector3(rec.e.EyeX, rec.e.EyeY, rec.e.EyeZ), new Vector3(rec.e.EyeX2, rec.e.EyeY2, rec.e.EyeZ2), camTimeline));*/
                /*break;
            }
        }
    }*/
        
    public void OnRewind(float value)
    {
        if (isPlaying)
            return;
        camTimeline = timeline.value;
        fixedTimeline = timeline.value;
        timeinSec = fixedTimeline * totalLength;
    }

    public void TogglePlay()
    {
        isPlaying = !isPlaying;
        PlayIcon.SetActive(!isPlaying);
        StopIcon.SetActive(isPlaying);
        if (isPlaying && camTimeline >= 1f)
        {
            camTimeline = 0f;
            fixedTimeline = 0f;
            timeline.value = 0f;
        }
    }
}