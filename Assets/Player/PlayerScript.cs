using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{

    public Camera cam;
    public float xRestriction = 20f;
    public float yRestriction = 50f;
    public Light flash;
    bool isFlashlightOn = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        HandleFlashlight();
        Vector3 mousePosition = cam.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 1000f;
        mousePosition.x = Math.Max(0, Math.Min(1, mousePosition.x));
        mousePosition.y = Math.Max(0, Math.Min(1, mousePosition.y));
        Vector3 mouseWorldPoint = cam.ViewportToWorldPoint(mousePosition);
        cam.transform.rotation = Quaternion.Euler(xRestriction - (2 * xRestriction * mousePosition.y), (2 * yRestriction * mousePosition.x) - yRestriction, 0f);
        Vector3 lightDir = Vector3.RotateTowards(transform.position, mouseWorldPoint, 90f, 90f);
        // lightDir.z = -lightDir.z;
        flash.transform.rotation = Quaternion.LookRotation(lightDir);
        Debug.DrawRay(transform.position, lightDir, Color.red);
        InputAction interactAction = InputSystem.actions.FindAction("Interact");
        InputAction flashlightAction = InputSystem.actions.FindAction("Jump");
        if (interactAction.WasPressedThisFrame())
        {
            HandleInteract(mousePosition);
            isFlashlightOn = false;
        }
        if (flashlightAction.WasPressedThisFrame())
        {
            isFlashlightOn = !isFlashlightOn;
        }
    }

    void HandleInteract(Vector3 mousePosition)
    {
        LayerMask mask = LayerMask.GetMask("Interactable");
        Ray ray = cam.ViewportPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
        {
            Debug.Log("You selected something!");
        }
        else { Debug.Log("No hit!"); }
    }

    void HandleFlashlight()
    {
        if (isFlashlightOn) flash.intensity = 1000f;
        else flash.intensity = 0f;
    }
}
