using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CropReplayUI : MonoBehaviour
{
    public Main main;
    public ImportReplayScript impReplayScript;
    public Slider timeline;
    public Slider min;
    public Slider max;
    [HideInInspector]
    public NISLoader.Animation originalPosition;
    [HideInInspector]
    public NISLoader.Animation replayPosition;
    [HideInInspector]
    public NISLoader.Animation originalRotation;
    [HideInInspector]
    public NISLoader.Animation replayRotation;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            if (timeline.value > 0)
                timeline.value -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.Period))
        {
            if (timeline.value < timeline.maxValue)
                timeline.value += 1;
        }

        if (min.value > max.value)
            min.value = max.value;
        
        NISLoader.ApplyCarMovement(main.ObjectsOnScene, replayPosition, Mathf.Clamp(timeline.value, min.value, max.value), main.forceY, true, (NISLoader.SceneType)NISLoader.SceneInfo.SceneType != NISLoader.SceneType.NIS_SCENE_LOCATION_SPECIFIC);
        NISLoader.ApplyCarMovement(main.ObjectsOnScene, replayRotation, Mathf.Clamp(timeline.value, min.value, max.value), main.forceY, true, (NISLoader.SceneType)NISLoader.SceneInfo.SceneType != NISLoader.SceneType.NIS_SCENE_LOCATION_SPECIFIC);
        main.editorCameraMovement.target.position = main.currentlyEditingSubObject.position;
        main.editorCameraMovement.transform.position = main.editorCameraMovement.target.position - (main.editorCameraMovement.rotation * Vector3.forward * main.editorCameraMovement.currentDistance + main.editorCameraMovement.targetOffset);
    }
    
    void OnEnable()
    {
        main.gizmo.RemoveTarget(main.gizmo.targetRoots.Keys.ToArray()[0]);
        max.value = max.maxValue;
    }
    
    public void Cancel()
    {
        gameObject.SetActive(false);
        main.gameObject.SetActive(true);
        main.HelpButton(false);
        main.gizmo.AddTarget(main.currentlyEditingSubObject);
    }
    
    public void Done()
    {
        gameObject.SetActive(false);
        impReplayScript.originalPosition = originalPosition;
        impReplayScript.originalRotation = originalRotation;
        float[][] old_delta = replayPosition.subAnimations[0].delta;
        float[][] old_delta2 = replayRotation.subAnimations[0].delta;
        replayPosition.subAnimations[0].delta = new float[(int)max.value - (int)min.value][];
        replayRotation.subAnimations[0].delta = new float[(int)max.value - (int)min.value][];
        for (int i = 0; i < replayPosition.subAnimations[0].delta.Length; i++)
        {
            replayPosition.subAnimations[0].delta[i] = old_delta[(int) min.value + i];
            replayRotation.subAnimations[0].delta[i] = old_delta2[(int) min.value + i];
        }
        impReplayScript.replayPosition = replayPosition;
        impReplayScript.replayRotation = replayRotation;
        impReplayScript.timeline.maxValue = main.PreviewTimeline[5].maxValue;
        impReplayScript.min.maxValue = main.PreviewTimeline[5].maxValue;
        impReplayScript.max.maxValue = main.PreviewTimeline[5].maxValue;
        impReplayScript.timeline.value = (int) main.PreviewTimeline[5].maxValue / 2;
        impReplayScript.max.value = impReplayScript.max.maxValue;
        impReplayScript.gameObject.SetActive(true);
    }
}
