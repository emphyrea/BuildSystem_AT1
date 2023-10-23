using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlacedObj : PlacedObject
{
    public enum Edge
    {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] private FloorEdgePosition upFloorEdgePosition;
    [SerializeField] private FloorEdgePosition downFloorEdgePosition;
    [SerializeField] private FloorEdgePosition leftFloorEdgePosition;
    [SerializeField] private FloorEdgePosition rightFloorEdgePosition;


    private FloorEdgePlacedObj upEdgeObj;
    private FloorEdgePlacedObj downEdgeObj;
    private FloorEdgePlacedObj leftEdgeObj;
    private FloorEdgePlacedObj rightEdgeObj;

    public void PlaceEdge(Edge edge, FloorEdgeObjTypeSO floorEdgeObjTypeSO)
    {
        FloorEdgePosition floorEdgePosition = GetFloorEdgePos(edge);

        Transform floorEdgeObjTrans = Instantiate(floorEdgeObjTypeSO.prefab, floorEdgePosition.transform.position, floorEdgePosition.transform.rotation);

        FloorEdgePlacedObj currentFloorEdgePlacedObj = GetFloorEdgePlacedObj(edge);
        if(currentFloorEdgePlacedObj != null)
        {
            Destroy(currentFloorEdgePlacedObj.gameObject);
        }

        FloorEdgePlacedObj floorEdgePlacedObj = floorEdgeObjTrans.GetComponent<FloorEdgePlacedObj>();
        SetFloorEdgePlacedObj(edge, floorEdgePlacedObj);
    }

    private void SetFloorEdgePlacedObj(Edge edge, FloorEdgePlacedObj floorEdgePlacedObj)
    {
        switch (edge)
        {
            default:
            case Edge.Up:
                upEdgeObj = floorEdgePlacedObj;
                break;
            case Edge.Down:
                downEdgeObj = floorEdgePlacedObj;
                break;
            case Edge.Left:
                leftEdgeObj = floorEdgePlacedObj;
                break;
            case Edge.Right:
                rightEdgeObj = floorEdgePlacedObj;
                break;
        }
    }

    private FloorEdgePlacedObj GetFloorEdgePlacedObj(Edge edge)
    {
        switch (edge)
        {
            default:
            case Edge.Up:
                return upEdgeObj;
            case Edge.Down:
                return downEdgeObj;
            case Edge.Left:
                return leftEdgeObj;
            case Edge.Right:
                return rightEdgeObj;
        }
    }

    private FloorEdgePosition GetFloorEdgePos(Edge edge)
    {
        switch (edge)
        {
            default:
            case Edge.Up:
                return upFloorEdgePosition;
            case Edge.Down:
                return downFloorEdgePosition;
            case Edge.Left:
                return leftFloorEdgePosition;
            case Edge.Right:
                return rightFloorEdgePosition;
        }
    }

    public override void DestroySelf()
    {
        if (upEdgeObj != null) Destroy(upEdgeObj.gameObject);
        if (downEdgeObj != null) Destroy(downEdgeObj.gameObject);
        if (leftEdgeObj != null) Destroy(leftEdgeObj.gameObject);
        if (rightEdgeObj != null) Destroy(rightEdgeObj.gameObject);

        base.DestroySelf();
    }
}
