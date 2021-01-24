using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathGenerator : MonoBehaviour
{
    [SerializeField] private PathGenerationData data;

    private int _straightPathCount;

    private Transform[,] _currentTiles;

    private int _width;
    private int _height;

    private bool[,] _pickedTiles;
    private Transform _pathTileParent;
    private List<Transform> _neighbours;
    private List<Transform> _path = new List<Transform>();

    //Submit to necessary events and cache the data
    private void Awake()
    {
        GameEvents.OnGridGenerationCompleted += GeneratePath;
        _pathTileParent = SharedData.ins.mapParent;
        _neighbours = new List<Transform>();
    }

    //Revoke submisison from events 
    private void OnDestroy()
    {
        GameEvents.OnGridGenerationCompleted -= GeneratePath;
    }

    //Generating path operations
    private void GeneratePath(Transform[,] cells)
    {
        //Cache the data
        _currentTiles = cells;
        _width = cells.GetLength(0);
        _height = cells.GetLength(1);

        //This bool array acts as a checker board to mark picked tiles
        _pickedTiles = new bool[_width, _height];

        //Pick a random entrance point at the bottom to start the map
        var pathEntranceIndex = GetBottomEntrance();
        ChangeCellWithPathTile(pathEntranceIndex);

        //This variables are created out of the loop to keep it clear
        var pickedCell = PickCell(pathEntranceIndex);
        var previousCell = pickedCell;
        ChangeCellWithPathTile(pickedCell);

        //Looping till we reach the ceiling of the grid
        while (true)
        {
            //Picking a cell position to be checked
            pickedCell = PickCell(pickedCell);

            //Checking if it is inside the borders if not go back to previous cell and pick a new one
            if (!IsInBorders(pickedCell.x, pickedCell.y))
            {
                pickedCell = previousCell;
                continue;
            }

            //Checking if we reached to side walls if so pick againg
            if (IsWallReached(pickedCell.x))
            {
                pickedCell = previousCell;
                continue;
            }

            //If all conditions are met pick the cell
            ChangeCellWithPathTile(pickedCell);


            //Checking for ceiling reached situation if so break the loop
            if (IsCeilingReached(pickedCell))
                break;

            //Cache the current cell as previous to use in the next iteration
            previousCell = pickedCell;
        }

        //Collect the neighbours and distribute with the event
        _neighbours = CollectNeighbours();
        GameEvents.OnNeighboursGenerated?.Invoke(_neighbours);
        
        //Distribute path data
        GameEvents.OnPathGenerationCompleted?.Invoke(_path);
    }

    //Swaps the empty tile with a path tile
    private void ChangeCellWithPathTile(Vector2Int index)
    {
        //Caches the tile to prevent repetition
        var temp = _currentTiles[index.x, index.y];
        //Caches the position data to use it when spawning the path tile
        var pos = temp.position;
        //Destroys the object to be swapped
        Destroy(temp.gameObject);
        //Creates the new path tile and positions it in the desired position
        var pathTile = Instantiate(data.pathTile, pos, Quaternion.identity);
        //Settin the parent so hierarchy stays clean
        pathTile.transform.SetParent(_pathTileParent);
        //Swaps the current tile data to be used later if needed
        _currentTiles[index.x, index.y] = pathTile.transform;
        //Adds the new path tile to path data
        _path.Add(pathTile.transform);
        //Marks the boolean check system with true as a cache data to use on other calculations to gain performance
        //With this method we dont search for whole path tile system 
        _pickedTiles[index.x, index.y] = true;
    }

    //Collects the whole neighbours of the path tiles
    private List<Transform> CollectNeighbours()
    {
        //Results are kept in this list to be returned
        var result = new List<Transform>();
        //Cached to not repeat getting length operation
        var firstIndex = _pickedTiles.GetLength(0);
        //Cached to not repeat getting length operation
        var secondIndex = _pickedTiles.GetLength(1);

        //This array keeps the taken neighbours so we don't take the same tile again and again
        var neighbourCheckList = new bool[firstIndex, secondIndex];
        for (var i = 0; i < firstIndex; i++)
        {
            for (var j = 0; j < secondIndex; j++)
            {
                //Is the tile neighbour to a tower check
                //And is the tile has been taken by a road check
                //First check must be true to be neighbour as first condition
                //Second check is if the tile is a road it can't be a spot for towers so it must be false
                if (IsNeighbourToATower(i, j) && !_pickedTiles[i, j])
                {
                    //The tiles that pass the test are markes as true
                    neighbourCheckList[i, j] = true;
                }
            }
        }

        //Iterating over the whole check list
        for (var i = 0; i < neighbourCheckList.GetLength(0); i++)
        {
            for (var j = 0; j < neighbourCheckList.GetLength(1); j++)
            {
                //If a tile has been check we are adding it to result list to be used as a neighbour tile
                if (neighbourCheckList[i, j])
                {
                    result.Add(_currentTiles[i, j]);
                }
            }
        }

        return result;
    }

    //Checks if the desired index is inside the limits if so checks if one of the neighbours is a path tile
    //If a path tile is neighbour to our current index it we take it as a neighbour tile
    private bool IsNeighbourToATower(int x, int y)
    {
        if (IsNeighbourInBorders(x + 1, y) && _pickedTiles[x + 1, y]) return true;
        if (IsNeighbourInBorders(x - 1, y) && _pickedTiles[x - 1, y]) return true;
        if (IsNeighbourInBorders(x, y + 1) && _pickedTiles[x, y + 1]) return true;
        if (IsNeighbourInBorders(x, y - 1) && _pickedTiles[x, y - 1]) return true;
        return false;
    }


    //Pick a random cell according to rotation chance desired in the data
    private Vector2Int PickCell(Vector2Int index)
    {
        //Pick a random float value between zero and one to check if the rotation should be done or not
        var chance = Random.Range(0f, 1f);

        //Evaluate the current desired chance from the animation curve and compare with the value evaluated by Random.Range method
        if (chance > data.rotationChance.Evaluate(_straightPathCount))
        {
            //Another random check for shall we pick a left or right turn
            if (IsLeft())
            {
                index.x += 1;
            }
            else
            {
                index.x -= 1;
            }

            //Straight path count is used for increasing the rotation chance as straight roads repeats itself
            //After a turn this value returns to zero so rolling chance system resets itself to starting position
            _straightPathCount = 0;
        }
        else
        {
            //If a turn has not been taken index is inremented on the y axis so upper tile is being taken
            index.y += 1;


            //Straingt path count is getting incremented so the chance system can recalibrate
            _straightPathCount++;
            return index;
        }

        return index;
    }

    //Pick a random rotation direction
    private bool IsLeft()
    {
        var chance = Random.Range(0f, 1f);
        return chance > 0.5f;
    }

    //Check if the current x position that wanted to pick is inside the side walls
    private bool IsWallReached(int x)
    {
        if (x <= 1) return true;
        if (x >= _width - 1) return true;

        return false;
    }

    //Check if the tile coordinates is inside the limits
    //Which restricts the side walls by one tile to let there be a tower spot
    private bool IsInBorders(int x, int y)
    {
        if (x <= 0) return false;
        if (x >= _width) return false;
        if (y <= 0) return false;
        if (y >= _height) return false;

        return true;
    }

    //Mostyle same with is in borders method but this method allows side wall tiles to be picked as tower spot
    private bool IsNeighbourInBorders(int x, int y)
    {
        if (x < 0) return false;
        if (x >= _width) return false;
        if (y < 0) return false;
        if (y >= _height) return false;

        return true;
    }


    //Get a random bottom entrance point from the grid system
    private Vector2Int GetBottomEntrance()
    {
        var width = _currentTiles.GetLength(0);
        var index = Random.Range(1, width - 1);

        _pickedTiles[index, 0] = true;

        return new Vector2Int(index, 0);
    }

    //Check if the ceiling of the grid system has been reached
    private bool IsCeilingReached(Vector2Int index) => index.y >= _height - 1;
}