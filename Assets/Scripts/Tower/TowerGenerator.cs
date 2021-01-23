using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TowerGenerator : MonoBehaviour
{
    [SerializeField] private TowerData data;

    //Keeps track of available slots 
    //When finished if possible replenished with previous level towers
    private List<Transform> _availableTowerSpots;

    //Activelyplaced towers
    private List<Transform> _currentTowerBatch;

    private int _currentSpawnLevel;
    private int _currentTowerCost;

    //Submit to events 
    private void Awake()
    {
        GameEvents.OnGenerateTowerButtonPressed += SpawnNewTower;
        GameEvents.OnNeighboursGenerated += OnNeighboursGenerated;
        _currentTowerCost = data.towerLevels[0].cost;
    }

    //Revoke submission from events
    private void OnDestroy()
    {
        GameEvents.OnGenerateTowerButtonPressed -= SpawnNewTower;
        GameEvents.OnNeighboursGenerated -= OnNeighboursGenerated;
    }

    //When neighbours are generated generate necessary data
    private void OnNeighboursGenerated(List<Transform> neighbours)
    {
        _availableTowerSpots = neighbours;
        _currentTowerBatch = new List<Transform>();
    }

    //Spawn new tower if possible
    //Possibility is check by available slots and money
    private void SpawnNewTower()
    {
        if (!MoneyManager.ins.IsAffordable(_currentTowerCost)) return;
        if (_availableTowerSpots.Count == 0) return;

        var temp = RetrieveRandomNeighbour();
        var pos = temp.position;

        IfHasAlreadyOccupiedDestroy(temp);

        CreateTower(pos, temp);

        CheckCurrentLevelStatus();
        MoneyManager.ins.ReduceMoney(_currentTowerCost);
    }

    //Actual tower creation part
    private void CreateTower(Vector3 pos, Transform target)
    {
        var currentData = data.towerLevels[_currentSpawnLevel];

        var newTower = Instantiate(data.towerLevels[_currentSpawnLevel].prefab, pos, Quaternion.identity);
        newTower.transform.SetParent(target, true);
        newTower.Initialize(currentData.range, currentData.damagePerSecond);

        _currentTowerBatch.Add(target);
    }

    //Pick a random tower slot from the available tower slot list
    private Transform RetrieveRandomNeighbour()
    {
        var randomTowerIndex = Random.Range(0, _availableTowerSpots.Count);
        var temp = _availableTowerSpots[randomTowerIndex];
        _availableTowerSpots.RemoveAt(randomTowerIndex);
        return temp;
    }

    //If already a tower is habitant in the slot destroy it to be replaced with upgraded version
    private void IfHasAlreadyOccupiedDestroy(Transform target)
    {
        var temp = target.childCount;
        if (temp == 0) return;
        for (var i = 0; i < temp; i++)
        {
            Destroy(target.GetChild(i).gameObject);
        }
    }

    //Checks if all slots are filled with current tower level
    //If filled increases level at max to limit
    //Resets the slot data and currently active tower data
    private void CheckCurrentLevelStatus()
    {
        if (_availableTowerSpots.Count != 0) return;
        if (data.towerLevels.Length <= _currentSpawnLevel + 1) return;

        _currentSpawnLevel++;
        _currentTowerCost = data.towerLevels[_currentSpawnLevel].cost;
        _availableTowerSpots = new List<Transform>(_currentTowerBatch);
        _currentTowerBatch.Clear();
    }
}