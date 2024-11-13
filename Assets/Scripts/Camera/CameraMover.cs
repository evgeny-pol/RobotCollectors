using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField, Min(0)] private float _moveSensitivity = 1;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    private void Update()
    {
        Vector2 movement = _moveSensitivity * Time.deltaTime * _playerInput.Camera.CameraMovement.ReadValue<Vector2>();
        transform.Translate(movement.x, 0, movement.y, Space.World);
    }
}
