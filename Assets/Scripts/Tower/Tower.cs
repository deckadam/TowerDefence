using System;
using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private Collider[] _lockedTraverser;
    private float _range;
    private int _damage;

    //Cache the data and start the coroutine
    public void Initialize(float range, int damage)
    {
        _range = range;
        _damage = damage;
        _lockedTraverser = new Collider[1];

        StartCoroutine(SearchForTraverser());
    }

    //Search for traversers 
    //Shoot with cooldown
    //If dead or left the range pick a new one and lock onto it
    private IEnumerator SearchForTraverser()
    {
        var waiter = new WaitForSeconds(1f);
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
                }
                else
                {
                    _lockedTraverser[0] = null;
                    continue;
                }

                yield return waiter;
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


    private bool ShootProjectile(Transform target)
    {
        if (target.gameObject.activeSelf)
            return target.GetComponent<Traverser>().TakeDamage(_damage);
        return true;
    }

    private void OnDrawGizmos()
    {
        if (_lockedTraverser[0] != null)
            Gizmos.DrawLine(transform.position, _lockedTraverser[0].transform.position);
    }
}