using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static int pointerin_counter;
    private bool pointerin;
    private bool locked;
    public int RequiredMouse = 4;
    
    void Update()
    {
        if (pointerin)
        {
            if (Input.GetMouseButtonDown(0))
                locked = true;
        } else {
            if (locked && !Input.GetMouseButton(0))
            {
                locked = false;
                pointerin_counter = Mathf.Max(0, pointerin_counter - 1);
                if (pointerin_counter == 0)
                    Main.CursorType = 0;
            }
        }
    }

    void OnDisable()
    {
        if (pointerin || locked)
        {
            pointerin_counter = Mathf.Max(0, pointerin_counter - 1);
            pointerin = false;
            locked = false;
            if (pointerin_counter == 0)
                Main.CursorType = 0;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (pointerin || locked) return;
        pointerin = true;
        pointerin_counter++;
        Main.CursorType = RequiredMouse;
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        pointerin = false;
        if (!locked)
            pointerin_counter = Mathf.Max(0, pointerin_counter - 1);
        if (!locked && pointerin_counter == 0)
            Main.CursorType = 0;
    }
}
