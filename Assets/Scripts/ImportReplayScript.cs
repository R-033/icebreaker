using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ImportReplayScript : MonoBehaviour
{
    public Main main;
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
        max.value = min.value + replayPosition.subAnimations[0].delta.Length - 1;

        if (timeline.value < min.value || timeline.value > max.value)
        {
            NISLoader.ApplyCarMovement(main.ObjectsOnScene, originalPosition, timeline.value, main.forceY, true, (NISLoader.SceneType)NISLoader.SceneInfo.SceneType != NISLoader.SceneType.NIS_SCENE_LOCATION_SPECIFIC);
            NISLoader.ApplyCarMovement(main.ObjectsOnScene, originalRotation, timeline.value, main.forceY, true, (NISLoader.SceneType)NISLoader.SceneInfo.SceneType != NISLoader.SceneType.NIS_SCENE_LOCATION_SPECIFIC);
        } else {
            NISLoader.ApplyCarMovement(main.ObjectsOnScene, replayPosition, timeline.value - min.value, main.forceY, true, (NISLoader.SceneType)NISLoader.SceneInfo.SceneType != NISLoader.SceneType.NIS_SCENE_LOCATION_SPECIFIC);
            NISLoader.ApplyCarMovement(main.ObjectsOnScene, replayRotation, timeline.value - min.value, main.forceY, true, (NISLoader.SceneType)NISLoader.SceneInfo.SceneType != NISLoader.SceneType.NIS_SCENE_LOCATION_SPECIFIC);
        }

        main.editorCameraMovement.target.position = main.currentlyEditingSubObject.position;
        main.editorCameraMovement.transform.position = main.editorCameraMovement.target.position - (main.editorCameraMovement.rotation * Vector3.forward * main.editorCameraMovement.currentDistance + main.editorCameraMovement.targetOffset);
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
        float[][][] old_delta = new float[2][][];
        old_delta[0] = new float[originalPosition.subAnimations[0].delta.Length][];
        for (int i = 0; i < originalPosition.subAnimations[0].delta.Length; i++)
        {
            old_delta[0][i] = new float[originalPosition.subAnimations[0].delta[i].Length];
            for (int j = 0; j < originalPosition.subAnimations[0].delta[i].Length; j++)
                old_delta[0][i][j] = originalPosition.subAnimations[0].delta[i][j];
        }
        old_delta[1] = new float[originalRotation.subAnimations[0].delta.Length][];
        for (int i = 0; i < originalRotation.subAnimations[0].delta.Length; i++)
        {
            old_delta[1][i] = new float[originalRotation.subAnimations[0].delta[i].Length];
            for (int j = 0; j < originalRotation.subAnimations[0].delta[i].Length; j++)
                old_delta[1][i][j] = originalRotation.subAnimations[0].delta[i][j];
        }
        
        for (int i = (int)min.value; i <= (int)max.value; i++)
        {
            if (i < originalPosition.subAnimations[0].delta.Length)
                originalPosition.subAnimations[0].delta[i] = replayPosition.subAnimations[0].delta[i - (int)min.value];
            if (i < originalRotation.subAnimations[0].delta.Length)
                originalRotation.subAnimations[0].delta[i] = replayRotation.subAnimations[0].delta[i - (int)min.value];
        }
        
        main.AddToHistoryNIS(new Main.ImportReplay(main.currentlyEditingObject, new [] { originalPosition.subAnimations[0], originalRotation.subAnimations[0] }, old_delta));
        
        Debug.Log("Replay imported successfully");
        
        Cancel();
    }
}
