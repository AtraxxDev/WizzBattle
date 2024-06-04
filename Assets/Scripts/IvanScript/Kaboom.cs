using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Kaboom : MonoBehaviour
{

    public float lifeTime = 5f;
    private float time;

    private int destructableLayer;

    //[SerializeField] private AudioSource m_AudioSource;
   // [SerializeField] private AudioClip _Hurt;
  //  [SerializeField] private AudioClip _explosion;
    [SerializeField] ParticleSystem _explosionEffect;
    private Rigidbody rb;
    //int damage = 100;


    public float radius = 10f;
    public float explosionForce = 10f;

    private void Awake()
    {
        // Cache the layer indices for performance
        destructableLayer = LayerMask.NameToLayer("Destructable");
    }

    private void OnEnable()
    {
        time = 0;
        _explosionEffect.Stop();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();        
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (time >= lifeTime)
        {
            // Debug.Log("kABOOM");
            StartCoroutine(Boom());

            rb.detectCollisions = true;
            //eturnToPool();
        }
        else
        {
            rb.detectCollisions = false;
        }
    }

    private IEnumerator Boom()
    {
        //m_AudioSource.PlayOneShot(_explosion);
        _explosionEffect.Play();

        Collider[] colliders=Physics.OverlapSphere(transform.position,radius);

        foreach(Collider collider in colliders)
        {
            Rigidbody rig=collider.GetComponent<Rigidbody>();
            if(rig!=null)
            {
                //m_AudioSource.PlayOneShot(_Hurt);                
                rig.AddExplosionForce(explosionForce, transform.position, radius, 2f, ForceMode.Impulse);
                PlayerHealth health = rig.gameObject.GetComponent<PlayerHealth>();
                if (health != null) health.TakeDamage(2);
                //ApplyDAmage(rig.gameObject.GetComponent<Health>(); Esta función ya la tiene en PlayerHealth
            }

            // Check if the GameObject has the tag "Destructable" and disable its BoxCollider
            if (collider.CompareTag("Destructable"))
            {
                BoxCollider boxCollider = collider.GetComponent<BoxCollider>();
                Animator boxAnimator=collider.GetComponent<Animator>();
                if (boxCollider != null)
                {
                    boxCollider.enabled = false;
                    boxAnimator.SetTrigger("Booom");
                }
            }
        }


        yield return new WaitForSeconds(1);

        ReturnToPool();
    }

    public void ReturnToPool()
    {
        BombPool pool = FindObjectOfType<BombPool>();
        if (pool != null)
        {
           // curBounces = 0;
            pool.ReturnToPool(gameObject);
        }

        StaffScript staffScript=FindObjectOfType<StaffScript>();
        if(staffScript != null) {
            staffScript.HandleBombReturned();
        }
    }


}
