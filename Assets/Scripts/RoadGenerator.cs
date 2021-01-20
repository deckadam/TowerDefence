using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadGenerator : MonoBehaviour
{
    public static event Action<Transform[]> OnRoadGenerationCompleted;
    public PathGenerationData data;

    private bool _wasLastTurnLeft;
    private int _straightPathCount;

    private Transform[,] _currentCells;

    private int _width;
    private int _height;

    private bool[,] _pickedTiles;
    private Transform _pathTileParent;

    private void Awake()
    {
        _pathTileParent = SharedData.ins.mapParent;
        GridGenerator.OnGridGenerationCompleted += GeneratePath;
    }

    private void OnDestroy()
    {
        GridGenerator.OnGridGenerationCompleted -= GeneratePath;
    }

    private void GeneratePath(Transform[,] cells)
    {
        _currentCells = cells;
        _width = cells.GetLength(0);
        _height = cells.GetLength(1);

        var path = new List<Transform>();
        _pickedTiles = new bool[_width, _height];

        var pathEntranceIndex = GetBottomEntrance();

        CheckBeforeChange(pathEntranceIndex, path);

        var pickedCell = PickCell(pathEntranceIndex);
        CheckBeforeChange(pickedCell, path);

        var status = false;
        while (!status)
        {
            var previousCell = pickedCell;
            pickedCell = PickCell(pickedCell);

            if (!IsInBorders(pickedCell))
            {
                pickedCell = previousCell;
                continue;
            }

            if (IsWallReached(pickedCell))
            {
                previousCell.y += 1;
                pickedCell = previousCell;
            }

            if (IsCeilingReached(pickedCell))
                break;

            status = CheckBeforeChange(pickedCell, path);
        }

        ChangeCellWithPathTile(pickedCell, path);

        var pathArray = path.ToArray();

        if (pathArray.Length == 1)
        {
            throw new Exception("Path generation error or grid size too small");
        }


        RaiseOnRoadGenerationCompleted(pathArray);
    }

    private bool CheckBeforeChange(Vector2Int index, List<Transform> path)
    {
        if (IsCeilingReached(index))
        {
            Debug.LogError(index);
            return true;
        }

        ChangeCellWithPathTile(index, path);
        return false;
    }

    private void ChangeCellWithPathTile(Vector2Int index, List<Transform> path)
    {
        var pos = _currentCells[index.x, index.y].position;
        Destroy(_currentCells[index.x, index.y].gameObject);
        var pathTile = Instantiate(data.pathTile, pos, Quaternion.identity);
        pathTile.transform.SetParent(_pathTileParent);
        _currentCells[index.x, index.y] = pathTile.transform;
        path.Add(pathTile.transform);
    }

    private Vector2Int PickCell(Vector2Int index)
    {
        var chance = Random.Range(0f, 1f);

        if (chance > data.rotationChance.Evaluate(_straightPathCount))
        {
            if (IsLeft())
            {
                index.x += 1;
            }
            else
            {
                index.x -= 1;
            }

            _straightPathCount = 0;
        }
        else
        {
            index.y += 1;

            if (!IsCeilingReached(index))
                _pickedTiles[index.x, index.y] = true;

            _straightPathCount++;
            return index;
        }


        _pickedTiles[index.x, index.y] = true;
        return index;
    }

    private bool IsLeft()
    {
        var chance = Random.Range(0f, 1f);
        return chance > 0.5f;
    }

    private bool IsWallReached(Vector2Int index)
    {
        if (index.x <= 1) return true;
        if (index.x >= _width - 1) return true;

        return false;
    }

    private bool IsInBorders(Vector2Int index)
    {
        if (index.x <= 0) return false;
        if (index.x >= _width) return false;
        if (index.y <= 0) return false;
        if (index.y >= _height) return false;
        
        return true;
    }


    private Vector2Int GetBottomEntrance()
    {
        var width = _currentCells.GetLength(0);
        var index = Random.Range(1, width - 1);
        
        _pickedTiles[index, 0] = true;

        return new Vector2Int(index, 0);
    }

    private bool IsCeilingReached(Vector2Int index) => index.y >= _height - 1;

    private void RaiseOnRoadGenerationCompleted(Transform[] obj)
    {
        OnRoadGenerationCompleted?.Invoke(obj);
    }
}