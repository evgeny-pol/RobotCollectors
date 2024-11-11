using System;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(ParentConstraint))]
public class Resource : MonoBehaviour
{
    [SerializeField, Min(0)] private int _value = 1;
    [SerializeField] private ParticleSystem _particleSystem;

    private ParentConstraint _parentConstraint;

    public event Action<Resource> Deactivated;

    public int Value => _value;

    private void Awake()
    {
        _parentConstraint = GetComponent<ParentConstraint>();
    }

    public void PlaySpawnEffects()
    {
        _particleSystem.Play();
    }

    public void Attach(Transform target)
    {
        var constraintSource = new ConstraintSource
        {
            sourceTransform = target,
            weight = 1
        };
        _parentConstraint.AddSource(constraintSource);
        _parentConstraint.constraintActive = true;
    }

    public void Detach()
    {
        _parentConstraint.constraintActive = false;

        while (_parentConstraint.sourceCount > 0)
            _parentConstraint.RemoveSource(0);
    }

    public void Deactivate()
    {
        Deactivated?.Invoke(this);
    }
}
