using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField, Min(0)] private float _moveSensitivity = 20;
    [SerializeField, Min(1)] private float _raycastDistance = 100;
    [SerializeField] private LayerMask _selectionLayers;
    [SerializeField] private LayerMask _orderLayers;
    [SerializeField] private CameraMover _cameraMover;
    [SerializeField] private ErrorLog _errorLog;

    private ISelectable _selectedObject;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.Game.Select.performed += OnSelectPerformed;
        _playerInput.Game.Order.performed += OnOrderPerformed;
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        _playerInput.Game.Select.performed -= OnSelectPerformed;
        _playerInput.Game.Order.performed -= OnOrderPerformed;
    }

    private void Update()
    {
        Vector2 movementInput = _playerInput.Game.CameraMovement.ReadValue<Vector2>();

        if (movementInput != Vector2.zero)
        {
            Vector2 movement = _moveSensitivity * Time.deltaTime * movementInput;
            _cameraMover.Move(new Vector3(movement.x, 0, movement.y));
        }
    }

    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        if (RaycastUnderCursor(_selectionLayers, out RaycastHit raycastHit)
            && raycastHit.collider.TryGetComponent(out ISelectable selectableObject))
        {
            if (selectableObject != _selectedObject)
            {
                Deselect();
                _selectedObject = selectableObject;
                _selectedObject.Highlight();
            }
        }
        else
        {
            Deselect();
        }
    }

    private void OnOrderPerformed(InputAction.CallbackContext context)
    {
        if (_selectedObject != null
            && RaycastUnderCursor(_orderLayers, out RaycastHit raycastHit))
        {
            if (_selectedObject.TryOrder(raycastHit.point, out string errorMessage) == false)
                _errorLog.ShowError(errorMessage);
        }
    }

    private bool RaycastUnderCursor(LayerMask raycastLayers, out RaycastHit raycastHit)
    {
        Vector2 mousePosition = _playerInput.Game.MousePosition.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        return Physics.Raycast(ray, out raycastHit, _raycastDistance, raycastLayers);
    }

    private void Deselect()
    {
        if (_selectedObject != null)
        {
            _selectedObject.Dehighlight();
            _selectedObject = null;
        }
    }
}
