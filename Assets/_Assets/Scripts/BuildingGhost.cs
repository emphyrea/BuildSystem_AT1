using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour
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
        Vector3 targetPos = GridBuildingSystem.Instance.GetMouseWorldSnappedPos();
        targetPos.y = 1f;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 15f);
        transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetPlacedObjRot(), Time.deltaTime * 15f);

    }
    private void RefreshVisual()
    {
        if(visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }

        PlacedObjectTypeSO placedObjectTypeSO = GridBuildingSystem.Instance.GetPlacedObjectTypeSO();
        if(placedObjectTypeSO != null)
        {
            visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            SetLayerRecursive(visual.gameObject, 11);
        }
    }

    private void SetLayerRecursive(GameObject targetGameObject, int layer)
    {
        targetGameObject.layer = layer;
        foreach(Transform child in targetGameObject.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
}
