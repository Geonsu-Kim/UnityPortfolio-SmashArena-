using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class EnemyBulletControl : MonoBehaviour
{
    public GameObject explosion;
    public GameObject projectile;

    public int damage;
    public float range;

    public Vector3 impactNormal; //Used to rotate impactparticle.
    private WaitForSeconds ws;
    private RaycastHit[] hits;

    private SphereCollider sphere;
    private BoxCollider box;
    private CapsuleCollider capsule;
    private Rigidbody rb;

    private void Awake()
    {        
        sphere = GetComponent<SphereCollider>();
        box = GetComponent<BoxCollider>();
        capsule = GetComponent<CapsuleCollider>();
        ws = new WaitForSeconds(1f);
        rb = GetComponent<Rigidbody>();
        explosion.SetActive(false);
        projectile.SetActive(true);
        ColliderEnable();
    }
    public void ColliderEnable()
    {
        if (sphere != null)
        {
            sphere.enabled = true;
        }
        if (box != null)
        {
            box.enabled = true;
        }
        if (capsule != null)
        {
            capsule.enabled = true;
        }

        rb.isKinematic = false;
    }
    public void ColliderDisable()
    {
        if (sphere != null)
        {
            sphere.enabled = false;
        }
        if (box != null)
        {
            box.enabled = false;
        }
        if (capsule != null)
        {
            capsule.enabled = false;
        }

        rb.isKinematic = true;
    }
    void OnTriggerEnter(Collider other)
    {

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        explosion.transform.rotation = Quaternion.FromToRotation(Vector3.up, impactNormal);
        explosion.SetActive(true);
        projectile.SetActive(false);
        ColliderDisable();
        CheckPlayerSphere(range);
        StartCoroutine(DestroyThis());

    }
    public void Revive()
    {
        gameObject.SetActive(true);
        explosion.SetActive(false);
        projectile.SetActive(true);

    }
    IEnumerator DestroyThis()
    {
        yield return ws;
        gameObject.SetActive(false);
    }
    void CheckPlayerSphere( float radius)
    {
        hits = Physics.SphereCastAll(transform.position, radius, transform.position, 0f, 1 << 10);
        for (int i = 0; i < hits.Length; i++)
        {
            

            hits[i].collider.gameObject.GetComponent<HealthControl>().Damaged(damage);
        }
    }

}