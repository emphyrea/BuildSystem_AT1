using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObjUI : MonoBehaviour
{
    private Dictionary<PlacedObjectTypeSO, Transform> placedObjectTypeTransformDic;
    private Dictionary<FloorEdgeObjTypeSO, Transform> floorEdgeObjectTypeTransformDic;

    private void Awake()
    {
        placedObjectTypeTransformDic = new Dictionary<PlacedObjectTypeSO, Transform>();
        floorEdgeObjectTypeTransformDic = new Dictionary<FloorEdgeObjTypeSO, Transform>();

        placedObjectTypeTransformDic[BuildSystemAssets.Instance.floor] = transform.Find("FloorBtn");
        placedObjectTypeTransformDic[BuildSystemAssets.Instance.workbench] = transform.Find("WorkbenchBtn");
        placedObjectTypeTransformDic[BuildSystemAssets.Instance.tent] = transform.Find("TentBtn");

        floorEdgeObjectTypeTransformDic[BuildSystemAssets.Instance.wall] = transform.Find("WallBtn");

        foreach (PlacedObjectTypeSO placedObjectTypeSO in placedObjectTypeTransformDic.Keys)
        {
            placedObjectTypeTransformDic[placedObjectTypeSO].GetComponent<Button_UI>().ClickFunc = () => {
                GridBuildingSystem.Instance.SelectPlacedObjectTypeSO(placedObjectTypeSO);
            };
        }

        foreach (FloorEdgeObjTypeSO floorEdgeObjectTypeSO in floorEdgeObjectTypeTransformDic.Keys)
        {
            floorEdgeObjectTypeTransformDic[floorEdgeObjectTypeSO].GetComponent<Button_UI>().ClickFunc = () => {
                GridBuildingSystem.Instance.SelectFloorEdgeObjectTypeSO(floorEdgeObjectTypeSO);
            };
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        GridBuildingSystem.Instance.OnSelectedChanged += GridBuildingSystem_OnSelectedChanged;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            GridBuildingSystem.Instance.SelectPlacedObjectTypeSO(BuildSystemAssets.Instance.floor);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GridBuildingSystem.Instance.SelectPlacedObjectTypeSO(BuildSystemAssets.Instance.workbench);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GridBuildingSystem.Instance.SelectPlacedObjectTypeSO(BuildSystemAssets.Instance.tent);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GridBuildingSystem.Instance.SelectFloorEdgeObjectTypeSO(BuildSystemAssets.Instance.wall);
        }
    }

    private void GridBuildingSystem_OnSelectedChanged(object sender, System.EventArgs e)
    {
        RefreshSelectedVisual();
    }

    private void RefreshSelectedVisual()
    {
        foreach (PlacedObjectTypeSO placedObjectTypeSO in placedObjectTypeTransformDic.Keys)
        {
            placedObjectTypeTransformDic[placedObjectTypeSO].Find("Selected").gameObject.SetActive(false);

            if (GridBuildingSystem.Instance.GetPlaceObjectType() == GridBuildingSystem.PlaceObjType.GridObj)
            {
                placedObjectTypeTransformDic[placedObjectTypeSO].Find("Selected").gameObject.SetActive(
                    GridBuildingSystem.Instance.GetPlacedObjectTypeSO() == placedObjectTypeSO
                );
            }
        }

        foreach (FloorEdgeObjTypeSO floorEdgeObjectTypeSO in floorEdgeObjectTypeTransformDic.Keys)
        {
            floorEdgeObjectTypeTransformDic[floorEdgeObjectTypeSO].Find("Selected").gameObject.SetActive(false);

            if (GridBuildingSystem.Instance.GetPlaceObjectType() == GridBuildingSystem.PlaceObjType.EdgeObj)
            {
                floorEdgeObjectTypeTransformDic[floorEdgeObjectTypeSO].Find("Selected").gameObject.SetActive(
                    GridBuildingSystem.Instance.GetFloorEdgeObjectTypeSO() == floorEdgeObjectTypeSO
                );
            }
        }

    }

}
