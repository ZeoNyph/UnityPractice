using System;
using System.Transactions;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{

    public enum EPlayerState
    {
        Room,
        LHall
    }

    [Header("Anchor Point Settings")]
    [SerializeField]
    private Camera _cam;
    [SerializeField]
    private float _xRestriction = 20f;
    [SerializeField]
    private float _yRestriction = 50f;
    [SerializeField]
    private float _camRotationSpeed = 5f;
    [SerializeField]
    private float _flashRotationSpeed = 5f;
    [SerializeField]
    private Light _flash;
    [SerializeField]
    private bool _isFlashlightOn = true;

    private Animator _animator;
    private EPlayerState _state = EPlayerState.Room;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cam.enabled = true;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_animator.IsInTransition(0))
        {
            HandleFlashlight();
            Vector3 mousePosition = _cam.ScreenToViewportPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 1000f;
            mousePosition.x = Math.Max(0, Math.Min(1, mousePosition.x));
            mousePosition.y = Math.Max(0, Math.Min(1, mousePosition.y));
            Vector3 mouseWorldPoint = _cam.ViewportToWorldPoint(mousePosition);
            Quaternion newCamRotation = Quaternion.Euler(_xRestriction - (2 * _xRestriction * mousePosition.y), (2 * _yRestriction * mousePosition.x) - _yRestriction, 0f);
            _cam.transform.rotation = Quaternion.Slerp(_cam.transform.rotation, newCamRotation, Time.deltaTime * _camRotationSpeed);
            Vector3 lightDir = Vector3.RotateTowards(transform.position, mouseWorldPoint, 90f, 90f);
            Quaternion newFlashRotation = Quaternion.LookRotation(lightDir);
            _flash.transform.rotation = Quaternion.Slerp(_flash.transform.rotation, newFlashRotation, _flashRotationSpeed * Time.deltaTime);
            Debug.DrawRay(transform.position, lightDir, Color.red);
            InputAction interactAction = InputSystem.actions.FindAction("Interact");
            InputAction flashlightAction = InputSystem.actions.FindAction("Jump");
            if (interactAction.WasPressedThisFrame())
            {
                HandleInteract(mousePosition);
            }
            if (flashlightAction.WasPressedThisFrame())
            {
                _isFlashlightOn = !_isFlashlightOn;
            }
        }

        
    }

    void HandleInteract(Vector3 mousePosition)
    {
        LayerMask mask = LayerMask.GetMask("Interactable");
        Ray ray = _cam.ViewportPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
        {
            if (hit.collider.gameObject.name == "LHallTrigger")
            {
                if (_state.Equals(EPlayerState.Room))
                {
                    _animator.SetTrigger("MovingToLHall");
                    _state = EPlayerState.LHall;
                }
                else
                {
                    _animator.SetTrigger("MovingToRoom");
                    _state = EPlayerState.Room;
                }
            }
        }
        else { Debug.Log("No hit!"); }
    }

    void HandleFlashlight()
    {
        if (_isFlashlightOn) _flash.intensity = 10000f;
        else _flash.intensity = 0f;
    }
}
