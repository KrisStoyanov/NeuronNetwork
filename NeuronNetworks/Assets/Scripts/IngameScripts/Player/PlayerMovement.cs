using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Plane dragPlane;

    // The difference between where the mouse is on the drag plane and 
    // where the origin of the object is on the drag plane
    private Vector3 offset;

    private Camera Camera;
    void Awake()
    {
        Camera = Camera.main;
    }

    void Start()
    {
        //MenuManager.Instance.Push(nameof(HUD));
    }

    private void OnMouseDown()
    {
        dragPlane = new Plane(Camera.transform.forward, transform.position);
        Ray camRay = Camera.ScreenPointToRay(Input.mousePosition);

        float planeDist;
        dragPlane.Raycast(camRay, out planeDist);
        offset = transform.position - camRay.GetPoint(planeDist);
    }

    private void OnMouseDrag()
    {
        Ray camRay = Camera.ScreenPointToRay(Input.mousePosition);

        float planeDist;
        dragPlane.Raycast(camRay, out planeDist);
        transform.position = camRay.GetPoint(planeDist) + offset;
    }

    void Update()
    {

    }


}
