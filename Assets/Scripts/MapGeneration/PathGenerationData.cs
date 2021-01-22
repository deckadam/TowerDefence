using UnityEngine;

[CreateAssetMenu(fileName = "PathGenerationData", menuName = "PathGenerationData", order = 1)]
public class PathGenerationData : ScriptableObject
{
    public AnimationCurve rotationChance;
    public GameObject pathTile;
    
}