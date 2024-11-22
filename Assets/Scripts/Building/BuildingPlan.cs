using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuildingPlan : MonoBehaviour, IColliderOwner
{
    public Collider Collider { get; private set; }

    private void Awake()
    {
        Collider = GetComponent<Collider>();
    }
}
