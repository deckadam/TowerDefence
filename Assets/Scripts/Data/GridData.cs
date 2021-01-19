using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "GridData", order = 0)]
public class GridData : ScriptableObject
{
    [Range(3, 999)]
    public int columnCount;

    [Range(3, 999)]
    public int rowCount;

    public float cellSize;
}