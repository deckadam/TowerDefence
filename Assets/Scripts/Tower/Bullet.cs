using DG.Tweening;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    public void Initialize(Transform target)
    {
        transform.DOMove(target.position, speed)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(gameObject));
    }
}