using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MaxCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;
 
    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
 
    void Start() { Init(); }
    void OnEnable() { Init(); }

    void OnDisable()
    {
        if (DragMouse.pointerin_counter == 0) {
            Main.CursorType = 0;
        } else
            Main.CursorType = 4;
    }
 
    public void Init()
    {
        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;
 
        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;
 
        xDeg = Vector3.Angle(Vector3.right, transform.right );
        yDeg = Vector3.Angle(Vector3.up, transform.up );
    }
 
    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate()
    {
        if (EventSystem.current.currentSelectedGameObject != null || Main.helpshown)
        {
            if (DragMouse.pointerin_counter == 0) {
                if (Main.CursorType != 0)
                    Main.CursorType = 0;
            } else {
                if (Main.CursorType != 4)
                    Main.CursorType = 4;
            }
            return;
        }
        // If Control and Alt and Middle button? ZOOM!
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Main.CursorType != 3)
                Main.CursorType = 3;
            if (Input.GetMouseButton(1))
                desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate*0.125f * Mathf.Abs(desiredDistance);
        }
        // If middle mouse and left alt are selected? ORBIT
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Main.CursorType != 1)
                Main.CursorType = 1;
            
            if (Input.GetMouseButton(1))
            {
                //grab the rotation of the camera so we can move in a psuedo local XY space
                target.rotation = transform.rotation;
                target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed * (Input.GetKey(KeyCode.LeftShift) ? 2f : 1f));
                target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed * (Input.GetKey(KeyCode.LeftShift) ? 2f : 1f), Space.World);
            }
        }
        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        else if (Input.GetMouseButton(1))
        {
            if (Main.CursorType != 2)
                Main.CursorType = 2;
            
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            ////////OrbitAngle

            //Clamp the vertical axis for the orbit
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // set camera rotation 
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;

            rotation = desiredRotation;
            transform.rotation = rotation;
        } else {
            if (DragMouse.pointerin_counter == 0) {
                if (Main.CursorType != 0)
                    Main.CursorType = 0;
            } else {
                if (Main.CursorType != 4)
                    Main.CursorType = 4;
            }
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            if (Input.GetKey(KeyCode.Space))
            {
                forward = Vector3.Scale(forward, Vector3.forward + Vector3.right).normalized;
                right = Vector3.Scale(right, Vector3.forward + Vector3.right).normalized;
            }
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                target.transform.position += forward * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 100f : 10f);
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                target.transform.position -= forward * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 100f : 10f);
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                target.transform.position += right * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 100f : 10f);
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                target.transform.position -= right * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 100f : 10f);
        }
 
        ////////Orbit Position
 
        // affect the desired Zoom distance if we roll the scrollwheel
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = desiredDistance;
 
        // calculate position based on the new currentDistance 
        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;
    }
 
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}