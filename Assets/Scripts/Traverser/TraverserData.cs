using UnityEngine;

[CreateAssetMenu(fileName = "Traverser", menuName = "Traverser", order = 2)]
public class TraverserData : ScriptableObject
{
    public AnimationCurve spawnDurationOverTime;
    public Traverser traverser;
    public float speed;
    public int health;
}