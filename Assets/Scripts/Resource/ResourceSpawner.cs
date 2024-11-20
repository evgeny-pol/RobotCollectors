using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField, Min(0)] private int _countMax = 10;
    [SerializeField, Min(1)] private float _spawnAreaRadius = 100;
    [SerializeField, Min(0)] private float _spawnInterval = 1;
    [Tooltip("Радиус сферы которая используется для проверки отсутствия объектов при спавне ресурса.")]
    [SerializeField, Min(0)] private float _checkEmptyRadius = 1;
    [Tooltip("Слои которые используются при проверке отсутствия объектов при спавне ресурса.")]
    [SerializeField] private LayerMask _checkEmptyLayers;
    [Tooltip("Максимальное количество попыток найти свободное место при спавне ресурса.")]
    [SerializeField, Min(1)] private int _maxSpawnAttempts = 10;
    [SerializeField] private Resource _resourceToSpawn;
    [SerializeField] private List<ResourceBase> _resourceBases;

    private ObjectPool<Resource> _resourcePool;
    private Coroutine _spawnCoroutine;

    private void Awake()
    {
        _resourcePool = new ObjectPool<Resource>(CreateResource, GetResource, ReleaseResource, DestroyResource);
    }

    private void OnEnable()
    {
        foreach (ResourceBase resourceBase in _resourceBases)
            resourceBase.ResourceBaseBuilt += OnResourceBaseBuilt;

        _spawnCoroutine = StartCoroutine(SpawnResources());
    }

    private void OnDisable()
    {
        this.TryStopCoroutine(_spawnCoroutine);

        foreach (ResourceBase resourceBase in _resourceBases)
            resourceBase.ResourceBaseBuilt -= OnResourceBaseBuilt;
    }

    private void OnResourceBaseBuilt(ResourceBase resourceBase)
    {
        _resourceBases.Add(resourceBase);
        resourceBase.ResourceBaseBuilt += OnResourceBaseBuilt;
    }

    private Resource CreateResource()
    {
        Resource resource = Instantiate(_resourceToSpawn);
        resource.Deactivated += OnResourceDeactivated;
        return resource;
    }

    private void GetResource(Resource resource)
    {
        resource.gameObject.SetActive(true);
        resource.PlaySpawnEffects();
    }

    private void ReleaseResource(Resource resource)
    {
        resource.gameObject.SetActive(false);
    }

    private void DestroyResource(Resource resource)
    {
        resource.Deactivated -= OnResourceDeactivated;
        Destroy(resource.gameObject);
    }

    private void OnResourceDeactivated(Resource resource)
    {
        _resourcePool.Release(resource);
    }

    private void TrySpawn()
    {
        if (_resourcePool.CountActive >= _countMax || _resourceBases.Count == 0)
            return;

        for (int spawnAttempt = 1; spawnAttempt <= _maxSpawnAttempts; ++spawnAttempt)
        {
            ResourceBase resourceBase = _resourceBases[Random.Range(0, _resourceBases.Count)];
            Vector2 randomOffset = _spawnAreaRadius * Random.insideUnitCircle;
            Vector3 randomPosition = resourceBase.transform.position + new Vector3(randomOffset.x, 0, randomOffset.y);

            if (Physics.CheckSphere(randomPosition, _checkEmptyRadius, _checkEmptyLayers) == false)
            {
                Resource resource = _resourcePool.Get();
                Quaternion rotation = Quaternion.Euler(0, MathUtils.CircleDegrees * Random.value, 0);
                resource.transform.SetPositionAndRotation(randomPosition, rotation);
                break;
            }
        }
    }

    private IEnumerator SpawnResources()
    {
        var spawnInterval = new WaitForSeconds(_spawnInterval);

        while (enabled)
        {
            TrySpawn();
            yield return spawnInterval;
        }
    }
}
