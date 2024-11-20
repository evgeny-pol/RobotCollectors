using TMPro;
using UnityEngine;

[RequireComponent(typeof(ResourceBase))]
[RequireComponent(typeof(ResourceStorage))]
public class ResourceBaseView : MonoBehaviour
{
    [SerializeField] private OrderLineRenderer _orderLineRenderer;
    [SerializeField] private TextMeshPro _resourcesCountText;

    private ResourceBase _resourceBase;
    private ResourceStorage _resourceStorage;

    private void Awake()
    {
        _resourceBase = GetComponent<ResourceBase>();
        _resourceStorage = GetComponent<ResourceStorage>();
    }

    private void OnEnable()
    {
        _resourceBase.OrderPositionChanged += OnOrderPositionChanged;
        _resourceStorage.ResourceCountChanged += OnResourceCountChanged;
    }

    private void OnDisable()
    {
        _resourceBase.OrderPositionChanged -= OnOrderPositionChanged;
        _resourceStorage.ResourceCountChanged -= OnResourceCountChanged;
    }

    private void OnOrderPositionChanged(Vector3? orderPosition)
    {
        if (orderPosition.HasValue)
            _orderLineRenderer.SetOrderPosition(orderPosition.Value);
        else
            _orderLineRenderer.HideOrderPosition();
    }

    private void OnResourceCountChanged(int newCount)
    {
        _resourcesCountText.text = newCount.ToString();
    }
}
