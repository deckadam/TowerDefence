using System;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private GridData data;

    public Vector3 cellCenterOffset;
    public Transform basePlane;


    private Transform[,] _cells;


    //Debug
    public float gizmoCubeSize;
    public bool drawCellGizmos;
    public List<Vector3> poss;
    public bool adjustPlaneSize;

    private void OnDrawGizmos()
    {
        if (adjustPlaneSize)
        {
            basePlane.localScale = CalculateBasePlaneSize();
        }

        if (!drawCellGizmos) return;
        var positions = GetGridPositions();
        poss = new List<Vector3>();
        foreach (var position in positions)
        {
            poss.Add(position);
            Gizmos.DrawCube(position, gizmoCubeSize * Vector3.one);
        }
    }
    //Debug

    //Calculate grid positions according to grid data
    private Vector3[,] GetGridPositions()
    {
        var columnCount = data.columnCount;
        var rowCount = data.rowCount;
        var cellSize = data.cellSize;

        var positions = new Vector3[columnCount * 2, rowCount * 2];
        var basePosition = basePlane.position;
        for (var i = -columnCount; i < columnCount; i++)
        {
            for (var j = -rowCount; j < rowCount; j++)
            {
                var offset = new Vector3(i * cellSize, j * cellSize, -0.1f);
                var firstIndex = i + columnCount;
                var secondIndex = j + rowCount;
                positions[firstIndex, secondIndex] = basePosition + offset + cellCenterOffset;
            }
        }

        return positions;
    }

    //Generate cells on given positions
    private Transform[,] GenerateCells(Vector3[,] cellPositions)
    {
        var cellPrefab = SharedData.ins.cellPrefab;
        var firstLength = cellPositions.GetLength(0);
        var secondLength = cellPositions.GetLength(1);
        var allTransforms = new Transform[firstLength, secondLength];

        var cellParent = SharedData.ins.mapParent;

        for (var i = 0; i < cellPositions.GetLength(0); i++)
        {
            for (var j = 0; j < cellPositions.GetLength(1); j++)
            {
                var position = cellPositions[i, j];
                var temp = Instantiate(cellPrefab, position, Quaternion.identity);
                temp.transform.SetParent(cellParent);
                allTransforms[i, j] = temp.transform;
            }
        }

        return allTransforms;
    }

    //Set up the grid system
    private void Start()
    {
        var cellPositions = GetGridPositions();
        _cells = GenerateCells(cellPositions);
        GameEvents.OnGridGenerationCompleted(_cells);
    }


//Sets base plane size bigger than necessary to fill camera for mobile phone resolutions
    private Vector3 CalculateBasePlaneSize()
    {
        var columnCount = data.columnCount;
        var rowCount = data.rowCount;
        var cellSize = data.cellSize;

        var x = (columnCount * 2) * cellSize / 5;
        var z = (rowCount * 2) * cellSize / 5;

        return new Vector3(x, 1, z);
    }
}