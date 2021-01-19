﻿using Cinemachine;
using UnityEngine;

public class TargetGroupAdjuster : MonoSingleton<TargetGroupAdjuster>
{
    public CinemachineTargetGroup targetGroup;
    public float targetSize;

    private void Awake()
    {
        GridGenerator.GridGenerationCompleted += Adjust;
    }

    private void OnDestroy()
    {
        GridGenerator.GridGenerationCompleted -= Adjust;
    }

    //Adjust target transform
    private void Adjust(Transform[,] targets)
    {
        var corners = GetCornerTransforms(targets);
        foreach (var target in corners)
        {
            targetGroup.AddMember(target, 1, targetSize);
        }
    }

    private Transform[] GetCornerTransforms(Transform[,] targets)
    {
        var result = new Transform[4];

        var cellsWidth = targets.GetLength(0) - 1;
        var cellsHeight = targets.GetLength(1) - 1;

        result[0] = targets[0, 0];
        result[1] = targets[cellsWidth, 0];
        result[2] = targets[0, cellsHeight];
        result[3] = targets[cellsWidth, cellsHeight];

        return result;
    }
}