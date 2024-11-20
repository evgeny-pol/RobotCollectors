using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [Tooltip("Максимальная дистанция отдаления от центра сцены по горизонтали.")]
    [SerializeField, Min(0)] private float _maxDistance = 100;

    public void Move(Vector3 movement)
    {
        Vector3 newPosition = transform.position + movement;
        newPosition.x = ClampDistance(newPosition.x);
        newPosition.z = ClampDistance(newPosition.z);
        transform.position = newPosition;
    }

    private float ClampDistance(float distance)
    {
        return Mathf.Clamp(distance, -_maxDistance, _maxDistance);
    }
}
