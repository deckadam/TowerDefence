using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathGenerator : MonoBehaviour
{
    public PathGenerationData data;

    private bool _wasLastTurnLeft;
    private int _straightPathCount;

    private Transform[,] _currentCells;

    private int _width;
    private int _height;

    private bool[,] _pickedTiles;
    private Transform _pathTileParent;
    private List<Transform> _neighbours;

    private void Awake()
    {
        _pathTileParent = SharedData.ins.mapParent;
        GameEvents.OnGridGenerationCompleted += GeneratePath;
    }

    private void OnDestroy()
    {
        GameEvents.OnGridGenerationCompleted -= GeneratePath;
    }

    private void GeneratePath(Transform[,] cells)
    {
        _currentCells = cells;
        _width = cells.GetLength(0);
        _height = cells.GetLength(1);

        _neighbours = new List<Transform>();

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

            if (!IsInBorders(pickedCell.x, pickedCell.y))
            {
                pickedCell = previousCell;
                continue;
            }

            if (IsWallReached(pickedCell.x))
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

        GameEvents.OnNeighboursGenerated?.Invoke(_neighbours);
        GameEvents.OnRoadGenerationCompleted?.Invoke(pathArray);
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
        var temp = _currentCells[index.x, index.y];
        var pos = temp.position;
        Destroy(temp.gameObject);

        CollectNeighbours(index);

        if (_neighbours.Contains(temp))
        {
            _neighbours.Remove(temp);
        }

        var pathTile = Instantiate(data.pathTile, pos, Quaternion.identity);
        pathTile.transform.SetParent(_pathTileParent);
        _currentCells[index.x, index.y] = pathTile.transform;
        path.Add(pathTile.transform);
    }

    private void CollectNeighbours(Vector2Int index)
    {
        AddNeighbourIfPossible(index.x + 1, index.y);
        AddNeighbourIfPossible(index.x - 1, index.y);
        AddNeighbourIfPossible(index.x, index.y + 1);
        AddNeighbourIfPossible(index.x, index.y - 1);
    }

    private void AddNeighbourIfPossible(int x, int y)
    {
        if (!IsNeighbourInBorders(x, y)) return;
        if (_pickedTiles[x, y]) return;
        AddNeighbourIfNotAdded(_currentCells[x, y]);
    }

    private void AddNeighbourIfNotAdded(Transform neighbour)
    {
        if (!_neighbours.Contains(neighbour))
            _neighbours.Add(neighbour);
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

    private bool IsWallReached(int x)
    {
        if (x <= 1) return true;
        if (x >= _width - 1) return true;

        return false;
    }

    private bool IsInBorders(int x, int y)
    {
        if (x <= 0) return false;
        if (x >= _width) return false;
        if (y <= 0) return false;
        if (y >= _height) return false;

        return true;
    }

    private bool IsNeighbourInBorders(int x, int y)
    {
        if (x < 0) return false;
        if (x > _width) return false;
        if (y < 0) return false;
        if (y >= _height) return false;

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
}