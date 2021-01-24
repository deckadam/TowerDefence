using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private Collider[] _lockedTraverser;
    private Bullet _bulletPreab;
    private float _range;
    private int _damage;
    private float _attackDuration;

    //Submit to game failing event to stop the towers in that situation
    private void Awake()
    {
        GameEvents.OnGameFailed += Stop;
    }

    //Revoking submission from the game failing event to prevent destroyed towers null reference error
    private void OnDestroy()
    {
        GameEvents.OnGameFailed -= Stop;
    }

    //Using a separate method to submit to events to be able to revoke submission later
    private void Stop()
    {
        StopAllCoroutines();
    }

    //Cache the data and start the coroutine
    public void Initialize(TowerData data, int level)
    {
        _attackDuration = data.towerLevels[level].attackDuration;
        _damage = data.towerLevels[level].damagePerSecond;
        _range = data.towerLevels[level].range;
        
        _lockedTraverser = new Collider[1];
        _bulletPreab = data.bulletPrefab;
        
        StartCoroutine(SearchForTraverser());
    }

    //Search for traversers 
    //Shoot with cooldown
    //If dead or left the range pick a new one and lock onto it
    private IEnumerator SearchForTraverser()
    {
        var waiter = new WaitForSeconds(_attackDuration);
        while (true)
        {
            if (_lockedTraverser[0] != null)
            {
                if (IsStillInRange())
                {
                    var result = ShootProjectile(_lockedTraverser[0].transform);

                    if (result)
                    {
                        _lockedTraverser[0] = null;
                    }

                    yield return waiter;
                }
                else
                {
                    _lockedTraverser[0] = null;
                }
            }
            else
            {
                Physics.OverlapSphereNonAlloc(transform.position, _range, _lockedTraverser, 1 << 9);

                yield return null;
            }
        }
    }

    //If traverser left the range of the tower pick return false
    private bool IsStillInRange()
    {
        var currentPosition = transform.position;
        var targetPosition = _lockedTraverser[0].transform.position;

        var distance = Vector3.Distance(currentPosition, targetPosition);

        return !(distance > _range);
    }


    //Shooting the projectile operation boolean return value is coming from the death situation of the traverser
    private bool ShootProjectile(Transform target)
    {
        if (target.gameObject.activeSelf)
        {
            CreateBullet(target);
            return target.GetComponent<Traverser>().TakeDamage(_damage);
        }

        return true;
    }

    private void CreateBullet(Transform target)
    {
        var newBullet = Instantiate(_bulletPreab, transform.position, Quaternion.identity);
        newBullet.Initialize(target);
    }

    //For visualizing who the target is 
    private void OnDrawGizmos()
    {
        if (_lockedTraverser[0] != null)
            Gizmos.DrawLine(transform.position, _lockedTraverser[0].transform.position);
    }
}