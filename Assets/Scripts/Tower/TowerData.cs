using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "TowerData", order = 3)]
public class TowerData : ScriptableObject
{
    public TowerLevel[] towerLevels;

    [System.Serializable]
    public struct TowerLevel
    {
        public Tower prefab;
        public int cost;
        public int damagePerSecond;
        public float range;
    }
}