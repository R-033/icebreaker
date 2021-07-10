#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EnableThreading : MonoBehaviour
{
    void Start()
    {
        Debug.Log("setting values,,,");
        PlayerSettings.WebGL.threadsSupport = true;
    }
}
#endif