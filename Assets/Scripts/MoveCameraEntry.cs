using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCameraEntry : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private float x;
    private Main main;
    private int mode;
    private int entry;
    private bool somethingwasdone;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        x = eventData.position.x;
        main = FindObjectOfType<Main>();
        mode = 0;
        entry = transform.parent.GetSiblingIndex();
        somethingwasdone = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (entry > 0)
        {
            if (mode == 0 && eventData.position.x < x - 50f || mode == -1 && eventData.position.x > x - 50f)
            {
                mode = mode == 0 ? -1 : 0;
                float firstlength = (entry < main.cameratrack[main.curcam].Item2.Length - 1 ? main.cameratrack[main.curcam].Item2[entry + 1].Time : 1f) - main.cameratrack[main.curcam].Item2[entry].Time;
                var v = main.cameratrack[main.curcam].Item2[entry];
                main.cameratrack[main.curcam].Item2[entry] = main.cameratrack[main.curcam].Item2[entry - 1];
                main.cameratrack[main.curcam].Item2[entry - 1] = v;
                main.cameratrack[main.curcam].Item2[entry - 1].Time = main.cameratrack[main.curcam].Item2[entry].Time;
                main.cameratrack[main.curcam].Item2[entry].Time = main.cameratrack[main.curcam].Item2[entry - 1].Time + firstlength;
                main.CtEntryIcons[entry].transform.SetSiblingIndex(entry - 1);
                main.CtEntryIcons[entry - 1].transform.SetSiblingIndex(entry);
                var im = main.CtEntryIcons[entry];
                main.CtEntryIcons[entry] = main.CtEntryIcons[entry - 1];
                main.CtEntryIcons[entry - 1] = im;
                if (entry == main.cameratrack[main.curcam].Item2.Length - 1)
                {
                    main.CtEntryIcons[entry - 1].transform.GetChild(0).gameObject.SetActive(true);
                    main.CtEntryIcons[entry].transform.GetChild(0).gameObject.SetActive(false);
                }
                somethingwasdone = true;
            }
        }
        if (entry < main.cameratrack[main.curcam].Item2.Length - 1)
        {
            if (mode == 0 && eventData.position.x > x + 50f || mode == 1 && eventData.position.x < x + 50f)
            {
                mode = mode == 0 ? 1 : 0;
                float firstlength = (entry + 1 < main.cameratrack[main.curcam].Item2.Length - 1 ? main.cameratrack[main.curcam].Item2[entry + 2].Time : 1f) - main.cameratrack[main.curcam].Item2[entry + 1].Time;
                var v = main.cameratrack[main.curcam].Item2[entry];
                main.cameratrack[main.curcam].Item2[entry] = main.cameratrack[main.curcam].Item2[entry + 1];
                main.cameratrack[main.curcam].Item2[entry + 1] = v;
                main.cameratrack[main.curcam].Item2[entry].Time = main.cameratrack[main.curcam].Item2[entry + 1].Time;
                main.cameratrack[main.curcam].Item2[entry + 1].Time = main.cameratrack[main.curcam].Item2[entry].Time + firstlength;
                main.CtEntryIcons[entry].transform.SetSiblingIndex(entry + 1);
                main.CtEntryIcons[entry + 1].transform.SetSiblingIndex(entry);
                var im = main.CtEntryIcons[entry];
                main.CtEntryIcons[entry] = main.CtEntryIcons[entry + 1];
                main.CtEntryIcons[entry + 1] = im;
                if (entry + 1 == main.cameratrack[main.curcam].Item2.Length - 1)
                {
                    main.CtEntryIcons[entry].transform.GetChild(0).gameObject.SetActive(true);
                    main.CtEntryIcons[entry + 1].transform.GetChild(0).gameObject.SetActive(false);
                }
                somethingwasdone = true;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (somethingwasdone)
            main.ChangeCameraTrack(main.curcam);
    }
}
