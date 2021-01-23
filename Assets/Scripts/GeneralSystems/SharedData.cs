using UnityEngine;

public class SharedData : MonoSingleton<SharedData>
{
    public GameObject cellPrefab;
    public Transform mapParent;
    public Transform traverserParent;
}
