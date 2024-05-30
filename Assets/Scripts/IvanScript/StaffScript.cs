using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

public class StaffScript : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Transform _firePoint;
    public ObjectPool bulletPool;
    //public GameObject Bullet;

    [Space]
    [SerializeField] private float FireRate = 1;
    [SerializeField] private float BulletForce = 100;

    private float time;
    private float nextTimeFire;

    private void Start()
    {
        nextTimeFire = 1 / FireRate;
        //GameObject bulletPools = GameObject.Find("ObjectPool2");
      //  if(bulletPools != null )
        //bulletPool = bulletPools.GetComponent<ObjectPool>();
    }

    private void FixedUpdate()
    {
        time += Time.deltaTime;

        if (time >=nextTimeFire)
        {
            // GameObject bullet= Instantiate(Bullet,_firePoint.position, Quaternion.identity);
            Debug.Log("Firing Fireball");
            GameObject bullet = bulletPool.GetFromPool();
            bullet.transform.position = _firePoint.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody bulletRb=bullet.GetComponent<Rigidbody>();
            if(bulletRb != null )
            {
                bulletRb.velocity = Vector3.zero;
                bulletRb.AddForce(_firePoint.forward * BulletForce);
            }
            //bullet.GetComponent<Rigidbody>().AddForce(_firePoint.forward * BulletForce);
            
            time = 0;
        }
    }
}
