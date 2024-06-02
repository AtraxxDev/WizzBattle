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
            Boom();
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

        Collider[] colliders=Physics.OverlapSphere(transform.position,radius);

        foreach(Collider collider in colliders)
        {
            Rigidbody rig=collider.GetComponent<Rigidbody>();
            if(rig!=null)
            {
                //m_AudioSource.PlayOneShot(_Hurt);                
                rig.AddExplosionForce(explosionForce, transform.position, radius, 2f, ForceMode.Impulse);
                //ApplyDAmage(rig.gameObject.GetComponent<Health>(); Esta función ya la tiene en PlayerHealth
            }
        }

        //m_AudioSource.PlayOneShot(_explosion);
       _explosionEffect.Play();

        yield return new WaitForSeconds(1);        

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        ObjectPool pool = FindObjectOfType<ObjectPool>();
        if (pool != null)
        {
           // curBounces = 0;
            pool.ReturnToPool(gameObject);

        }
    }

    /*
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip _Hurt;
    [SerializeField] private AudioClip _explosion;
    [SerializeField] private ParticleSystem _explosiónPartycle;
    private GameObject _mine;

    private void Start()
    {
        _explosiónPartycle.Stop();
        _mine= GetComponent<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")
        {
            _explosiónPartycle.Play();
            m_AudioSource.PlayOneShot(_Hurt);
            m_AudioSource.PlayOneShot(_explosion);
            _mine.GetComponent<MeshRenderer>().enabled = false;
        }
    }*/
}
