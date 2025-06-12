using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    private PlayerControls _playerControls;
    private bool _isDragging = false;
    private Camera _cam;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _cam = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _playerControls.Enable();

        _playerControls.Player.Press.started += OnPressStarted;
        _playerControls.Player.Press.canceled += OnPressEnded;
    }

    private void OnDisable()
    {
        _playerControls.Disable();

        _playerControls.Player.Press.started -= OnPressStarted;
        _playerControls.Player.Press.canceled -= OnPressEnded;
    }

    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos;

#if UNITY_STANDALONE || UNITY_EDITOR
        screenPos = Mouse.current.position.ReadValue();
#elif UNITY_IOS || UNITY_ANDROID
        screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
#endif

        Ray ray = _cam.ScreenPointToRay(screenPos);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit)
        {
            if (hit.transform == transform)
            {
                _isDragging = true;
            }
        }
    }

    void OnPressEnded(InputAction.CallbackContext ctx)
    {
        _isDragging = false;
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (_isDragging)
            {
                Vector2 screenPos = _playerControls.Player.Position.ReadValue<Vector2>();
                Vector3 worldPoint = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _cam.WorldToScreenPoint(transform.position).z));
                CmdMove(worldPoint);
            }
        }
    }

    [Command]
    private void CmdMove(Vector3 worldPoint)
    {
        _rb.MovePosition(worldPoint);
    }
}