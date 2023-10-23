using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEdgePlacedObj : MonoBehaviour
{
    [SerializeField] private FloorEdgeObjTypeSO floorEdgeObjTypeSO;

    public string GetFloorEdgeObjTypeSOName()
    {
        return floorEdgeObjTypeSO.name;
    }
}
