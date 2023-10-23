using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingEdgeGhost : MonoBehaviour
{
    private Transform visual;
    private PlacedObjectTypeSO placedObjectTypeSO;

    // Start is called before the first frame update
    void Start()
    {
        RefreshVisual();

        GridBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }



    private void Instance_OnSelectedChanged(object sender, System.EventArgs e)
    {
        RefreshVisual();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        FloorEdgePosition floorEdgePosition = GridBuildingSystem.Instance.GetMouseFloorEdgePosition();
        if (floorEdgePosition != null)
        {
            transform.position = Vector3.Lerp(transform.position, floorEdgePosition.transform.position, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, floorEdgePosition.transform.rotation, Time.deltaTime * 25f);
        }
        else
        {
            Vector3 targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPos();
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 25f);
        }
    }
    private void RefreshVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }
        if (GridBuildingSystem.Instance.GetPlaceObjectType() == GridBuildingSystem.PlaceObjType.EdgeObj)
        {
            FloorEdgeObjTypeSO floorEdgeObjTypeSO = GridBuildingSystem.Instance.GetFloorEdgeObjectTypeSO();
            if (floorEdgeObjTypeSO != null)
            {
                visual = Instantiate(floorEdgeObjTypeSO.visual, Vector3.zero, Quaternion.identity);
                visual.parent = transform;
                visual.localPosition = Vector3.zero;
                visual.localEulerAngles = Vector3.zero;
                SetLayerRecursive(visual.gameObject, 14);
            }
        }

    }

    private void SetLayerRecursive(GameObject targetGameObject, int layer)
    {
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
}

