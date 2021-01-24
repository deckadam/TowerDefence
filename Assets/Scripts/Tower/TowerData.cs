using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "TowerData", order = 3)]
public class TowerData : ScriptableObject
{
    public TowerLevel[] towerLevels;

    [Space]
    public Bullet bulletPrefab;

    [System.Serializable]
    public struct TowerLevel
    {
        public Tower prefab;
        public int cost;
        public int damagePerSecond;
        public float range;
        
        [Tooltip("Lower is faster")]
        public float attackDuration;
    }
}