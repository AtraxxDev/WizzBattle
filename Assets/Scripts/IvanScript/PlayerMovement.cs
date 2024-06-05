using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float defaultLength = 50;
    [SerializeField] private int _numOfReflections = 2;
    [SerializeField] private NetworkVariable<Vector3> _colorPlayer=new NetworkVariable<Vector3>();

    [Space]
    [Header("Lineas")]
    public LineRenderer _lineRenderer;
    [SerializeField] private Transform _lineTransform;
    private RaycastHit hit;
    private Ray ray;
    private Vector3 _moveItdirection;


    [Space]
    public bool playerDed=false;
    [SerializeField] private float speed = 400f;

    private Rigidbody rb;
    private Camera mainCamera;
    [SerializeField]private PlayerHealth playerHealth;
    public Animator playerAnim;

    private void Start()
    {
       
        if (IsOwner)
        {
            rb = GetComponent<Rigidbody>();
            mainCamera = Camera.main;
            playerHealth = GetComponent<PlayerHealth>();
            playerAnim = GetComponent<Animator>();
        }
    }
     

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (IsOwner && !playerDed)
        {
            PlayerMov();
            FacingCamera();
            DrawLaser();
            rb.detectCollisions = true;
            // DrawLineOld();
        }
    }

    private void PlayerMov()
    {
        // Movimiento
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        if(movement==Vector3.zero)
        {
            playerAnim.SetFloat("Speed", 0);
        }
        else
        {
            playerAnim.SetFloat("Speed", 0.5f);
        }

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

    private void DrawLaser()
    {
        ray =new Ray(_lineTransform.position,_lineTransform.forward);

        _lineRenderer.positionCount = 1;
        //De aquí va a salir la linea desde el FirePoint del Jugador
        _lineRenderer.SetPosition(0, _lineTransform.position);

        float remainLength = defaultLength;

        for(int i=0;i<_numOfReflections;i++)
        {
            if (Physics.Raycast(ray.origin,ray.direction, out hit, remainLength, layerMask))
            {
                _lineRenderer.positionCount += 1;
                _lineRenderer.SetPosition(_lineRenderer.positionCount-1,hit.point);

                remainLength-=Vector3.Distance(ray.origin,hit.point);

                ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
            }
            else
            {
                _lineRenderer.positionCount += 1;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, ray.origin + (ray.direction * remainLength));
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(IsOwner)
        {
            if (collision.gameObject.CompareTag("Projectile"))
            {
                playerHealth.TakeDamage(1);
                // Debug.Log(playerHealth.ToString());
            }
        }

    }

}
