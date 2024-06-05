using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class StaffScript : NetworkBehaviour
{
    [Header("Ref")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _bombPoint;
    [SerializeField] private PlayerMovement _playerRef;
    [SerializeField] private Animator playerAnim;
    [SerializeField] private NetworkAnimator networkAnimator; // Añadir referencia al NetworkAnimator
    public ObjectPool bulletPool;
    public BombPool bombPool;

    [Space]
    [SerializeField] private float FireRate = 5;
    [SerializeField] private float BombRate = 2;
    [SerializeField] private float BulletForce = 40;

    private float time;
    private float nextTimeFire;
    private float timeBomb;
    private float nextTimeBomb;
    private int activeBombs;
    private const int maxBombs = 2;

    private void Start()
    {
        if (IsOwner)
        {
            nextTimeFire = 1 / FireRate;
            nextTimeBomb = 1 / BombRate;
            _playerRef = GetComponent<PlayerMovement>();
            playerAnim = GetComponent<Animator>();
            networkAnimator = GetComponent<NetworkAnimator>(); // Obtener referencia al NetworkAnimator
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            time += Time.deltaTime;
            timeBomb += Time.deltaTime;
            if (!_playerRef.playerDed)
            {
                if (Input.GetMouseButton(0) && time >= nextTimeFire)
                {
                    networkAnimator.SetTrigger("Attack"); // Usar NetworkAnimator para sincronizar el Trigger
                    FireBallServerRpc(_firePoint.position, _firePoint.forward);
                    time = 0;
                }
                if (Input.GetMouseButton(1) && timeBomb >= nextTimeBomb && activeBombs <= maxBombs)
                {
                    networkAnimator.SetTrigger("Attack"); // Usar NetworkAnimator para sincronizar el Trigger
                    Bombs();
                    timeBomb = 0;
                }
            }
        }
    }

    [ServerRpc]
    private void FireBallServerRpc(Vector3 firePointPosition, Vector3 fireDirection)
    {
        FireBallClientRpc(firePointPosition, fireDirection);
    }

    [ClientRpc]
    private void FireBallClientRpc(Vector3 firePointPosition, Vector3 fireDirection)
    {
        GameObject fire = bulletPool.GetFromPool();
        fire.transform.position = firePointPosition;
        fire.transform.rotation = Quaternion.identity;

        Rigidbody bulletRB = fire.GetComponent<Rigidbody>();
        if (bulletRB != null)
        {
            bulletRB.velocity = Vector3.zero;
            bulletRB.AddForce(fireDirection * BulletForce);
        }
    }

    private void Bombs()
    {
        GameObject bomb = bombPool.GetFromPool();
        bomb.transform.position = _firePoint.position;
        bomb.transform.rotation = Quaternion.identity;
        activeBombs++; // Increment active bomb count
    }

    public void HandleBombReturned()
    {
        activeBombs--; // Decrement active bomb count when bomb returns to pool
    }
}
