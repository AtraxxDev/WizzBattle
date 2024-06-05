using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileScript : NetworkBehaviour
{
    public float lifeTime = 5f;
    private float time;

    private int floorLayer;
    private int playerLayer;
    private int obstacLayer;

    [SerializeField] private Rigidbody RgbMagic;
    [SerializeField] private int NumOfBounces = 3;

    private Vector3 lastVelocity;
    private float curSpeed;
    private Vector3 direction;
    private int curBounces = 0;

    private void Awake()
    {
        // Cache the layer indices for performance
        floorLayer = LayerMask.NameToLayer("Floor");
        playerLayer = LayerMask.NameToLayer("Player");
        obstacLayer = LayerMask.NameToLayer("Obstacle");
    }

    private void OnEnable()
    {
        time = 0;
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (time >= lifeTime)
        {
            ReturnToPool();
        }
    }

    private void LateUpdate()
    {
        lastVelocity = RgbMagic.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == floorLayer || collision.gameObject.layer == playerLayer)
        {
            ReturnToPool();
        }
        if (collision.gameObject.layer == obstacLayer)
        {
            if (curBounces >= NumOfBounces) return;
            curSpeed = lastVelocity.magnitude;
            direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);

            RgbMagic.velocity = direction * Mathf.Max(curSpeed, 0);
            curBounces++;
        }
    }

    private void ReturnToPool()
    {
        ObjectPool pool = FindObjectOfType<ObjectPool>();
        if (pool != null)
        {
            curBounces = 0;
            pool.ReturnToPool(gameObject);
        }
    }
}
