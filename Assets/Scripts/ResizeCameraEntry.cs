using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeCameraEntry : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private float x;
    private float orig_value;
    private Main main;
    private int entry;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        x = eventData.position.x;
        main = FindObjectOfType<Main>();
        entry = transform.parent.parent.GetSiblingIndex();
        orig_value = main.cameratrack[main.curcam].Item2[entry + 1].Time;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        main.cameratrack[main.curcam].Item2[entry + 1].Time = Mathf.Clamp(
            orig_value + (eventData.position.x - x) * 0.00115f * (main.editorTimelineMax - main.editorTimelineMin) / PlayerPrefs.GetFloat("GUIScale", 1f), 
            main.cameratrack[main.curcam].Item2[entry].Time + 1f / 30f / main.totalLength, 
            entry + 1 < main.cameratrack[main.curcam].Item2.Length - 1 ? main.cameratrack[main.curcam].Item2[entry + 2].Time - 1f / 30f / main.totalLength : 1f);
        main.UpdCameraTrackPreview();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        main.ChangeCameraTrack(main.curcam);
        main.AddToHistoryCam(new Main.ResizeCameraTrack(main.curcam, entry, orig_value, main.cameratrack[main.curcam].Item2[entry + 1].Time));
    }
}
