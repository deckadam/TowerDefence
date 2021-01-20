using System.Collections;
using System.Threading;
using UnityEngine;

public class TraverserGenerator : MonoBehaviour
{
    [SerializeField] private TraverserData data;

    private Transform[] _generatedPath;
    private Transform _traverserParent;

    //Submit to on road generation events and cache the necessary data
    private void Awake()
    {
        _traverserParent = SharedData.ins.traverserParent;
        RoadGenerator.OnRoadGenerationCompleted += OnRoadGenerationCompleted;
        InGameMenu.OnCountDownCompleted += StartTraverserGeneration;
        GameManager.OnGameFailed += Stop;
    }

    //Revoke submission from the events
    private void OnDestroy()
    {
        RoadGenerator.OnRoadGenerationCompleted -= OnRoadGenerationCompleted;
        InGameMenu.OnCountDownCompleted -= StartTraverserGeneration;
        GameManager.OnGameFailed -= Stop;
    }

    private void Stop()
    {
        StopAllCoroutines();
    }

    //Cache the path data to pass to generated traversers
    private void OnRoadGenerationCompleted(Transform[] path)
    {
        _generatedPath = path;
    }

    private void StartTraverserGeneration()
    {
        StartCoroutine(GenerateTraversers());
    }

    private IEnumerator GenerateTraversers()
    {
        var beginTime = Time.time;
        while (true)
        {
            CreateTraverser();
            var totalTimeTillLevelStarted = Time.time - beginTime;
            var evaluatedCooldown = data.spawnDurationOverTime.Evaluate(totalTimeTillLevelStarted);
            yield return new WaitForSeconds(evaluatedCooldown);
        }
    }

    //For spawning test traversers to see if there is any problem
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.T)) return;
        CreateTraverser();
    }

    //Creates a new traverser sets the necessary data and initializes it to start the movements
    private void CreateTraverser()
    {
        var traverser = data;
        var newTraverser = Instantiate(traverser.traverser, Vector3.zero, Quaternion.identity);
        newTraverser.transform.SetParent(_traverserParent);
        newTraverser.Initialize(_generatedPath, traverser.speed);
    }
}