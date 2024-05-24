using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 400f;

    private Rigidbody rb;
    private Camera mainCamera;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (IsOwner)
        {
            mainCamera = Camera.main;
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            PlayerMov();
            FacingCamera();
        }


    }

    private void PlayerMov()
    {
        // Movimiento
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(Vector3.ClampMagnitude(movement, 1) * speed);
    }

    private void FacingCamera()
    {
        // Mouse
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);

            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
    }
}
