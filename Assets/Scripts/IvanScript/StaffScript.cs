using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

public class StaffScript : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Transform _firePoint;
    public ObjectPool bulletPool;
    public BombPool bombPool;
    //public GameObject Bullet;

    [Space]
    [SerializeField] private float FireRate = 1;
    [SerializeField] private float BombRate = 2;
    [SerializeField] private float BulletForce = 100;

    private float time;
    private float nextTimeFire;
    private float timeBomb;
    private float nextTimeBomb;

    private void Start()
    {
        nextTimeFire = 1 / FireRate;
        nextTimeBomb = 1 / BombRate;
        //GameObject bulletPools = GameObject.Find("ObjectPool2");
        //  if(bulletPools != null )
        //bulletPool = bulletPools.GetComponent<ObjectPool>();
    }

    private void FixedUpdate()
    {
        time += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && time >= nextTimeFire)
        {
            FireBall();
            time = 0;
        }
        if (Input.GetMouseButtonDown(1) && timeBomb >= nextTimeBomb)
        {
            Bombs();
            timeBomb = 0;
        }

        /*if (time >= nextTimeFire)
        {
            // GameObject bullet= Instantiate(Bullet,_firePoint.position, Quaternion.identity);
            Debug.Log("Firing Fireball");
            GameObject bullet = bulletPool.GetFromPool();
            bullet.transform.position = _firePoint.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = Vector3.zero;
                bulletRb.AddForce(_firePoint.forward * BulletForce);
            }
            //bullet.GetComponent<Rigidbody>().AddForce(_firePoint.forward * BulletForce);

            time = 0;
        }*/
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
        GameObject rico = bombPool.GetFromPool();
        bombPool.transform.position = _firePoint.position;
        bombPool.transform.rotation = Quaternion.identity;
    }
    
}
