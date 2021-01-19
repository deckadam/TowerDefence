using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadGenerator : MonoBehaviour
{
    public PathGenerationData data;

    private bool _wasLastTurnLeft;
    private int _straightPathCount;

    private Transform[,] _currentCells;

    private int _width;
    private int _height;

    private List<Transform> _path;
    private bool[,] _pickedTiles;

    private void Awake()
    {
        GridGenerator.GridGenerationCompleted += GeneratePath;
    }

    private void OnDestroy()
    {
        GridGenerator.GridGenerationCompleted -= GeneratePath;
    }

    private void GeneratePath(Transform[,] cells)
    {
        _currentCells = cells;
        _width = cells.GetLength(0);
        _height = cells.GetLength(1);
        _path = new List<Transform>();
        _pickedTiles = new bool[_width, _height];

        var pathEntranceIndex = GetBottomEntrance();

        ChangeCellWithPath(pathEntranceIndex);

        var pickedCell = PickCell(pathEntranceIndex);

        var status = false;
        while (!status)
        {
            ChangeCellWithPath(pickedCell);

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


            status = IsCeilingReached(pickedCell);
        }
    }

    private void ChangeCellWithPath(Vector2Int index)
    {
        var pos = _currentCells[index.x, index.y].position;
        Destroy(_currentCells[index.x, index.y].gameObject);
        var pathTile = Instantiate(data.pathTile, pos, Quaternion.identity);
        _currentCells[index.x, index.y] = pathTile.transform;
        _path.Add(pathTile.transform);
    }

    private Vector2Int PickCell(Vector2Int index)
    {
        var backUp = index;
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
        }
        else
        {
            index.y += 1;
            _pickedTiles[index.x, index.y] = true;
            _straightPathCount++;
            return index;
        }

        /*
            if (!IsInBorders(index))
            {
                backUp.y += 1;
    
                return backUp;
            }
    */
        if (_pickedTiles[index.x, index.y])
        {
            backUp.y += 1;
            _pickedTiles[backUp.x, backUp.y] = true;

            return backUp;
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

    private bool IsCeilingReached(Vector2Int index) => index.y >= _height;
}