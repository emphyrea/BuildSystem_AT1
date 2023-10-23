using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance;

    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList=null;
    private PlacedObjectTypeSO placedObjectTypeSO;

    [SerializeField] private LayerMask layerMask;

    const float GridHeight = 1f;

    [SerializeField] private List<FloorEdgeObjTypeSO> floorEdgeObjTypeSOList=null;
    private FloorEdgeObjTypeSO floorEdgeObjTypeSO;

    private List<GridXZ<GridObject>> gridList;
    private GridXZ<GridObject> grid;

    private PlaceObjType placeObjType;
    private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;
    public event EventHandler OnActiveGridLevelChanged;

    private bool isDemolishActive;
    public enum PlaceObjType
    {
        GridObj,
        EdgeObj
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        { 
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        int gridWidth = 50;
        int gridHeight = 50;
        float cellSize = 1f;

        gridList = new List<GridXZ<GridObject>>();
        int gridLevels = 3;
        float gridLevelSize = GridHeight;
        for(int i = 0; i < gridLevels; i++)
        {
           GridXZ<GridObject> grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, gridLevelSize * i, 0), (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
            gridList.Add(grid);
        }
        grid = gridList[0];
        placedObjectTypeSO = null;
    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private PlacedObject placedObj;
        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetPlacedObj(PlacedObject placedObj)
        {
            this.placedObj = placedObj;
        }

        public void ClearPlacedObj()
        {
            placedObj = null;
        }

        public PlacedObject GetPlacedObject()
        {
            return placedObj;
        }

        public bool CanBuild()
        {
            return placedObj == null;
        }

        public override string ToString()
        {
            return x + "," + z + "\n" + placedObj;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
    }

    public PlaceObjType GetPlaceObjectType()
    {
        return placeObjType;
    }

    public Quaternion GetPlacedObjRot()
    {
        if (placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    internal Vector3 GetMouseWorldSnappedPos()
    {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPos(layerMask);
        grid.GetXZ(mousePosition, out int x, out int z);

        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPos(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        }
        else
        {
            return mousePosition;
        }
    }


    public FloorEdgeObjTypeSO GetFloorEdgeObjectTypeSO()
    {
        return floorEdgeObjTypeSO;
    }

    public FloorEdgePosition GetMouseFloorEdgePosition()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, layerMask))
        {
            // Raycast Hit Edge Object
            if (raycastHit.collider.TryGetComponent(out FloorEdgePosition floorEdgePosition))
            {
                return floorEdgePosition;
            }
        }

        return null;
    }

    public int GetActiveGridLevel()
    {
        return gridList.IndexOf(grid);
    }

    private void Update()
    {
        HandleGridSelect();
        HandleGridSelectAutomated();
        HandleTypeSelect();
        HandleNormalObjPlacement();
        HandleEdgeObjPlacement();
        HandleDirRotation();
        HandleDemolish();

        if (Input.GetMouseButtonDown(1)) { DeselectObjectType(); }
    }
    #region Demolish
    private void HandleDemolish()
    {
        if(isDemolishActive && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPos(layerMask);
            PlacedObject placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
            if (placedObject != null)
            {
                // Demolish
                placedObject.DestroySelf();

                List<Vector2Int> gridPositionList = placedObject.GetGridPosList();
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObj();
                }
            }
        }
    }
    private void SetDemolishActive()
    {
        placedObjectTypeSO = null;
        isDemolishActive = true;
        RefreshSelectedObjectType();
    }
    public bool IsDemolishActive()
    {
        return isDemolishActive;
    }

    #endregion Demolish
    private void HandleDirRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }
    }

    public bool IsValidGridPos(Vector2Int gridPosition)
    {
        return grid.IsValidGridPos(gridPosition);
    }

    #region TypeSelect
    private void HandleTypeSelect()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) { SetDemolishActive(); }
    }
    public void SelectPlacedObjectTypeSO(PlacedObjectTypeSO placedObjectTypeSO)
    {
        placeObjType = PlaceObjType.GridObj;
        this.placedObjectTypeSO = placedObjectTypeSO;
        RefreshSelectedObjectType();
    }

    public void SelectFloorEdgeObjectTypeSO(FloorEdgeObjTypeSO floorEdgeObjectTypeSO)
    {
        placeObjType = PlaceObjType.EdgeObj;
        this.floorEdgeObjTypeSO = floorEdgeObjectTypeSO;
        RefreshSelectedObjectType();
    }
    #endregion TypeSelect

    private void HandleGridSelectAutomated()
    {
        Vector3 mousePos = Mouse3D.GetMouseWorldPos(layerMask);
        float gridHeight = GridHeight;
        int newGridIndex = Mathf.RoundToInt (mousePos.y / GridHeight);
        grid = gridList[newGridIndex];
        OnActiveGridLevelChanged?.Invoke(this, EventArgs.Empty);

    }

    private void HandleGridSelect()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            int nextSelectedGridIndex = (gridList.IndexOf(grid) + 1) % gridList.Count;
            grid = gridList[nextSelectedGridIndex];
            OnActiveGridLevelChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void DeselectObjectType()
    {
        placedObjectTypeSO = null;
        floorEdgeObjTypeSO = null;

        isDemolishActive = false;

        RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }
    #region TryPlaceObj
   
    //overrides
    public bool TryPlaceObj(Vector2Int placedObjectOrigin, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir, out PlacedObject placedObject)
    {
        return TryPlaceObj(grid, placedObjectOrigin, placedObjectTypeSO, dir, out placedObject);
    }
    public bool TryPlaceObj(GridXZ<GridObject> grid, Vector2Int placedObjOrigin, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir, out PlacedObject placedObj)
    {
        List<Vector2Int> gridPosList = placedObjectTypeSO.GetGridPosList(placedObjOrigin, dir);
        bool canBuild = true;

        foreach (Vector2Int gridPos in gridPosList)
        {
            bool isValidPos = grid.IsValidGridPos(gridPos);
            if (!isValidPos)
            {
                canBuild = false;
                break;
            }
            if (!grid.GetGridObject(gridPos.x, gridPos.y).CanBuild())
            {
                canBuild = false;
                break;
            }
        }

        if (canBuild)
        {
            Vector2Int rotOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjWorldPos = grid.GetWorldPos(placedObjOrigin.x, placedObjOrigin.y) + new Vector3(rotOffset.x, 0, rotOffset.y) * grid.GetCellSize();

            placedObj = PlacedObject.Create(placedObjWorldPos, placedObjOrigin, dir, placedObjectTypeSO);

            foreach (Vector2Int gridPos in gridPosList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObj(placedObj);
            }

            OnObjectPlaced?.Invoke(placedObj, EventArgs.Empty);

            return true;
        }
        else
        {
            placedObj = null;
            return false;
        }
    }

    #endregion TryPlaceObj

    private void HandleNormalObjPlacement()
    {
        if(placeObjType == PlaceObjType.GridObj)
        {
            if(Input.GetMouseButtonDown(0) && placedObjectTypeSO != null)
            {
                Vector3 mousePos = Mouse3D.GetMouseWorldPos(layerMask);

                grid.GetXZ(mousePos, out int x, out int z);

                Vector2Int placedObjOrigin = new Vector2Int(x, z);

                if (TryPlaceObj(placedObjOrigin, placedObjectTypeSO, dir, out PlacedObject placedObj))
                {
                    //place object
                    Debug.Log("Placed");
                }
                else
                {
                    return;
                }


            }
        }
    }

    private void HandleEdgeObjPlacement() //Nesting
    {
        if(placeObjType == PlaceObjType.EdgeObj)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, layerMask))
            {
                // Raycast Hit Edge Object
                if (raycastHit.collider.TryGetComponent(out FloorEdgePosition floorEdgePosition) && 
                    raycastHit.collider.transform.parent.TryGetComponent(out FloorPlacedObj floorPlacedObject))
                {
                    // Found parent FloorPlacedObject
                    if (Input.GetMouseButtonDown(0) && floorEdgeObjTypeSO != null)
                    {
                        // Place Object on Edge
                        floorPlacedObject.PlaceEdge(floorEdgePosition.edge, floorEdgeObjTypeSO);
                    }

                }
            }
        }
    }
}
