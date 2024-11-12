using TMPro;
using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
public class ResourceBaseView : MonoBehaviour
{
    [SerializeField] private TextMeshPro _resourcesCountText;

    private ResourceStorage _resourceStorage;

    private void Awake()
    {
        _resourceStorage = GetComponent<ResourceStorage>();
    }

    private void OnEnable()
    {
        _resourceStorage.ResourceCountChanged += OnResourceCountChanged;
    }

    private void OnDisable()
    {
        _resourceStorage.ResourceCountChanged -= OnResourceCountChanged;
    }

    private void OnResourceCountChanged(int newCount)
    {
        _resourcesCountText.text = newCount.ToString();
    }
}
