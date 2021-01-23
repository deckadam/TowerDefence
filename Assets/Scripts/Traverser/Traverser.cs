using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Traverser : MonoBehaviour
{
    [SerializeField] private Image healthBar;

    private Transform _nextNode;
    private Transform[] _path;
    private int _pathLength;
    private int _maxHealth;
    private int _currentHealth;

    //Submit to on game failed event to stop movement when raised
    private void Awake()
    {
        GameEvents.OnGameFailed += Stop;
    }

    //Revoke submission to prevent null referenec error
    private void OnDestroy()
    {
        GameEvents.OnGameFailed -= Stop;
    }

    //Stop all coroutines is used because movement operation is done with coroutines
    private void Stop()
    {
        StopAllCoroutines();
    }

    //Cache the necessary data and start the movement coroutine
    public void Initialize(Transform[] path, float speed, int health)
    {
        _path = path;
        _pathLength = path.Length;
        _maxHealth = health;
        _currentHealth = health;

        AdjustHealthBar();

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
                    // GameEvents.OnFinalDestinationReached?.Invoke();
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

    //Apply damage to traverser and check if it's still alive
    public bool TakeDamage(int damage)
    {
        _currentHealth -= damage;
        AdjustHealthBar();
        Debug.LogError(_currentHealth);
        if (_currentHealth > 0) return false;
        Destroy(gameObject);
        return true;
    }

    //Set the fill ratio of the health bar
    private void AdjustHealthBar()
    {
        var ratio = (float)_currentHealth / (float)_maxHealth;
        healthBar.fillAmount = ratio;
    }
}