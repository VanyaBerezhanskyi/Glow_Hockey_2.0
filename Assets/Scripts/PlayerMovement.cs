using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.EventSystems;

public class PlayerMovement : NetworkBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isDragging = false;
    private Camera _cam;
    private Vector2 _screenPos = Vector2.zero;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _cam = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (_isDragging)
            {
                Vector3 worldPoint = _cam.ScreenToWorldPoint(new Vector3(_screenPos.x, _screenPos.y, _cam.WorldToScreenPoint(transform.position).z));
                CmdMove(worldPoint);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _screenPos = ctx.ReadValue<Vector2>();
    }

    [Command]
    private void CmdMove(Vector3 worldPoint)
    {
        _rb.MovePosition(worldPoint);
    }
}