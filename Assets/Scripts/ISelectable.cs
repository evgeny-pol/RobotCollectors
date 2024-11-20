using UnityEngine;

public interface ISelectable
{
    void Highlight();
    void Dehighlight();
    bool TryOrder(Vector3 targetPosition, out string errorMessage);
}
