using System;
using UnityEngine;

public class GridXZ<TGridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int z;
    }

    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridArray;
    private Vector3 originPos;

    public GridXZ(int gridWidth, int gridHeight, float cellSize, Vector3 originPos, Func<GridXZ<TGridObject>, int, int, TGridObject> CreateGridObject)
    {
        this.width = gridWidth;
        this.height = gridHeight;
        this.cellSize = cellSize;

        gridArray = new TGridObject[width, height];

        for(int x = 0; x < gridArray.GetLength(0); x++)
        {
            for(int z = 0; z < gridArray.GetLength(1); z++)
            {
                Debug.DrawLine(GetWorldPos(x, z), GetWorldPos(x, z + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPos(x, z), GetWorldPos(x+1, z), Color.white, 100f);
                gridArray[x, z] = CreateGridObject(this, x, z);
            }
        }
    }

    public Vector3 GetWorldPos(int x, int z) //convert grid pos to world pos
    {
        return new Vector3(x, 0, z) * cellSize + originPos;
    }

    public void GetXZ(Vector3 worldPos, out int x, out int z) //convert world pos to grid pos
    {
        x = Mathf.CeilToInt((worldPos - originPos).x / cellSize);
        z = Mathf.CeilToInt((worldPos - originPos).z / cellSize);
    }

    public void SetGridObject(Vector3 worldPos, TGridObject val)
    {
        int x, z;
        GetXZ(worldPos, out x, out z);
        SetGridObject(x, z, val);
    }

    public void SetGridObject(int x, int z, TGridObject val) //set a single value
    {
        if(x >= 0 && z >= 0 && x < width && z < height) //if x and y within grid parameters
        {
            gridArray[x, z] = val;
            if(OnGridValueChanged != null)
            {
                OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, z = z });
            }
        }
    }

    public TGridObject GetGridObject(int x, int z) //get grid pos
    {
        if (x >= 0 && z >= 0 && x < width && z < height) //if x and y within grid parameters
        {
            return gridArray[x, z];
        }
        else
        {
            return default;
        }
    }
    public TGridObject GetGridObject(Vector3 worldPos) //get world pos
    {
        int x, z;
        GetXZ(worldPos, out x, out z);
        return GetGridObject(x, z);
    }

    public int GetHeight()
    {
        return height;
    }
    public int GetWidth()
    {
        return width;
    }
    public float GetCellSize()
    {
        return cellSize;
    }

    internal bool IsValidGridPos(Vector2Int gridPos)
    {
        int x = gridPos.x;
        int z = gridPos.y;

        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}