using UnityEngine;
using Mirror;

public class CameraRotation : NetworkBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        if (isClientOnly)
        {
            _camera.transform.Rotate(0, 0, 180);
        }
    }
}
