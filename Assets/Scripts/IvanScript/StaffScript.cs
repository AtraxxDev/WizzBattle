using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;
using Unity.Netcode;

public class StaffScript : NetworkBehaviour
{
    [Header("Ref")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _bombPoint;
    [SerializeField] private PlayerMovement _playerRef;
    [SerializeField] private Animator playerAnim;
    public ObjectPool bulletPool;
    public BombPool bombPool;
    //public GameObject Bullet;

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
        if(IsOwner)
        {
            nextTimeFire = 1 / FireRate;
            nextTimeBomb = 1 / BombRate;
            _playerRef = GetComponent<PlayerMovement>();
            playerAnim = GetComponent<Animator>();
        }
       
        //GameObject bulletPools = GameObject.Find("ObjectPool2");
        //  if(bulletPools != null )
        //bulletPool = bulletPools.GetComponent<ObjectPool>();
    }

    private void FixedUpdate()
    {
        if(IsOwner)
        {
            time += Time.deltaTime;
            timeBomb += Time.deltaTime;
            if (!_playerRef.playerDed)
            {
                if (Input.GetMouseButton(0) && time >= nextTimeFire)
                {
                    //Debug.Log("Pum");
                    playerAnim.SetTrigger("Attack");
                    FireBall();
                    time = 0;
                }
                //Click Derecho para crear bombas e izq para crear bolas de fuego
                if (Input.GetMouseButton(1) && timeBomb >= nextTimeBomb && activeBombs <= maxBombs)
                {
                    // Debug.Log("Boom");
                    playerAnim.SetTrigger("Attack");
                    Bombs();
                    timeBomb = 0;
                }

            }
        }
       
    }

    private void FireBall()
    {
        GameObject Fire=bulletPool.GetFromPool();
        Fire.transform.position = _firePoint.position;    
        Fire.transform.rotation= Quaternion.identity;

        Rigidbody bulletRB=Fire.GetComponent<Rigidbody>();
        if(bulletRB != null)
        {
            bulletRB.velocity = Vector3.zero;
            bulletRB.AddForce(_firePoint.forward * BulletForce);
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
