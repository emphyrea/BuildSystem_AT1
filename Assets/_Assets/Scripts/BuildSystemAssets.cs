using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystemAssets : MonoBehaviour
{
    public static BuildSystemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public PlacedObjectTypeSO[] placedObjectTypeSOArray;

    public PlacedObjectTypeSO floor;
    public PlacedObjectTypeSO workbench;
    public PlacedObjectTypeSO tent;

    public FloorEdgeObjTypeSO[] floorEdgeObjTypeSOArray;

    public FloorEdgeObjTypeSO wall;


    public PlacedObjectTypeSO GetPlacedObjectTypeSOFromName(string placedObjectTypeSOName)
    {
        foreach (PlacedObjectTypeSO placedObjectTypeSO in placedObjectTypeSOArray)
        {
            if (placedObjectTypeSO.name == placedObjectTypeSOName)
            {
                return placedObjectTypeSO;
            }
        }
        return null;
    }

    public FloorEdgeObjTypeSO GetFloorEdgeObjectTypeSOFromName(string floorEdgeObjectTypeSOName)
    {
        foreach (FloorEdgeObjTypeSO floorEdgeObjectTypeSO in floorEdgeObjTypeSOArray)
        {
            if (floorEdgeObjectTypeSO.name == floorEdgeObjectTypeSOName)
            {
                return floorEdgeObjectTypeSO;
            }
        }
        return null;
    }

}
