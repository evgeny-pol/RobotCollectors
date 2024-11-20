using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrderLineRenderer : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _lineRenderer.enabled = false;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.SetPosition(0, transform.position);
    }

    public void SetOrderPosition(Vector3 targetPosition)
    {
        _lineRenderer.SetPosition(1, targetPosition);
        _lineRenderer.enabled = true;
    }

    public void HideOrderPosition()
    {
        _lineRenderer.enabled = false;
    }
}
