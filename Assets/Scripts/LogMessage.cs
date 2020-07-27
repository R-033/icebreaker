using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMessage : MonoBehaviour
{
    public CanvasGroup group;
    private float timeout = 5f;
    public static List<string> shown = new List<string>();
    [HideInInspector]
    public string message;
    [HideInInspector]
    public bool pinned;

    void Update()
    {
        if (pinned)
            transform.SetSiblingIndex(transform.parent.childCount - 1);
        if (timeout > 0f)
        {
            timeout -= Time.deltaTime;
            return;
        }
        group.alpha -= Time.deltaTime;
        if (group.alpha <= 0f)
        {
            Destroy(gameObject);
            shown.Remove(message);
        }
    }
}
