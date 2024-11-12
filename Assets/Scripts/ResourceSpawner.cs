using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField, Min(0)] private int _countMax = 10;
    [SerializeField, Min(1)] private float _spawnAreaSize = 1;
    [SerializeField, Min(0)] private float _spawnInterval = 1;
    [Tooltip("Радиус сферы которая используется для проверки отсутствия объектов при спавне ресурса.")]
    [SerializeField, Min(0)] private float _checkEmptyRadius = 1;
    [Tooltip("Слои которые используются при проверке отсутствия объектов при спавне ресурса.")]
    [SerializeField] private LayerMask _checkEmptyLayers;
    [Tooltip("Максимальное количество попыток найти свободное место при спавне ресурса.")]
    [SerializeField, Min(1)] private int _maxSpawnAttempts = 10;
    [SerializeField] private Resource _resourceToSpawn;

    private ObjectPool<Resource> _resourcePool;
    private Coroutine _spawnCoroutine;

    private void Awake()
    {
        _resourcePool = new ObjectPool<Resource>(CreateResource, GetResource, ReleaseResource, DestroyResource);
    }

    private void OnEnable()
    {
        _spawnCoroutine = StartCoroutine(SpawnResources());
    }

    private void OnDisable()
    {
        this.TryStopCoroutine(_spawnCoroutine);
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
        if (_resourcePool.CountActive >= _countMax)
            return;

        float halfSize = _spawnAreaSize / 2;

        for (int spawnAttempt = 1; spawnAttempt <= _maxSpawnAttempts; ++spawnAttempt)
        {
            Vector3 position = transform.position + new Vector3(Random.Range(-halfSize, halfSize), 0, Random.Range(-halfSize, halfSize));
            
            if (Physics.CheckSphere(position, _checkEmptyRadius, _checkEmptyLayers) == false)
            {
                Resource resource = _resourcePool.Get();
                Quaternion rotation = Quaternion.Euler(0, MathUtils.CircleDegrees * Random.value, 0);
                resource.transform.SetPositionAndRotation(position, rotation);
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
