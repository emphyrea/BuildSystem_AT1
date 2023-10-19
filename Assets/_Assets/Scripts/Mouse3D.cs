using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse3D : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private LayerMask layerMask;
    public static Mouse3D Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        mainCam = Camera.main;
    }


    public static Vector3 GetMouseWorldPos() => Instance.GetMouseWorldPos_Instance();
    private Vector3 GetMouseWorldPos_Instance()
    {
       Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, 999f, layerMask))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
