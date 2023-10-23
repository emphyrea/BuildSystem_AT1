using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    private PlacedObjectTypeSO placedObjectTypeSO;
    private Vector2Int origin;
    private PlacedObjectTypeSO.Dir dir;

    public static PlacedObject Create(Vector3 worldPos, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO)
    {
        Transform placedObjTrans = Instantiate(placedObjectTypeSO.prefab, worldPos, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));

        PlacedObject placedObject = placedObjTrans.GetComponent<PlacedObject>();

        placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.origin = origin;
        placedObject.dir = dir;

        return placedObject;
    }

    public virtual void DestroySelf()
    {
        Destroy(gameObject);
    }

    public List<Vector2Int> GetGridPosList()
    {
        return placedObjectTypeSO.GetGridPosList(origin, dir);
    }

}
