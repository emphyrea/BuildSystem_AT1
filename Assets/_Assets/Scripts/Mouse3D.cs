using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse3D
{
    private Camera mainCam;
    private LayerMask layerMask;
    public static Mouse3D Instance { get { return instance; }    }

    private static Mouse3D instance = new Mouse3D();

    private Mouse3D() { }

    public static Vector3 GetMouseWorldPos(LayerMask layerMask) => Instance.GetMouseWorldPos_Instance(layerMask);
    private Vector3 GetMouseWorldPos_Instance(LayerMask layerMask)
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
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
