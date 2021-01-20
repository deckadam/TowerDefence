using System;
using System.Collections;
using UnityEngine;

public class Traverser : MonoBehaviour
{
    public static event Action OnFinalDestinationReached;

    private Transform _nextNode;
    private Transform[] _path;
    private int _pathLength;

    //Submit to on game failed event to stop movement when raised
    private void Awake()
    {
        GameManager.OnGameFailed += Stop;
    }

    //Revoke submission to prevent null referenec error
    private void OnDestroy()
    {
        GameManager.OnGameFailed -= Stop;
    }

    //Stop all coroutines is used because movement operation is done with coroutines
    private void Stop()
    {
        StopAllCoroutines();
    }


    //Cache the necessary data and start the movement coroutine
    public void Initialize(Transform[] path, float speed)
    {
        _path = path;
        _pathLength = path.Length;
        transform.position = _path[0].position + Vector3.down;
        StartCoroutine(Traverse(speed));
    }

    //Traverse through the path till reach the end
    private IEnumerator Traverse(float speed)
    {
        var currentNodeIndex = 1;
        _nextNode = _path[1];

        while (true)
        {
            var currentPosition = transform.position;
            var nextPosition = Vector3.MoveTowards(currentPosition, _nextNode.position, speed * Time.deltaTime);
            var distance = Vector3.Distance(nextPosition, currentPosition);
            transform.position = nextPosition;
            if (distance < Mathf.Epsilon)
            {
                var isRoadFinished = PickNextTileIfPossible(currentNodeIndex++);
                if (isRoadFinished)
                {
                    RaiseOnFinalDestinationReached();
                    break;
                }
            }

            yield return null;
        }
    }

    //Check if the last tile of the path has been reached or not
    private bool PickNextTileIfPossible(int index)
    {
        if (index + 1 >= _pathLength) return true;
        _nextNode = _path[index + 1];
        return false;
    }

    private void RaiseOnFinalDestinationReached()
    {
        OnFinalDestinationReached?.Invoke();
    }
}