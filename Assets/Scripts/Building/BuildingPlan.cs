using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuildingPlan : MonoBehaviour, IColliderOwner
{
    private Collider _collider;

    public Collider Collider => _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
}
