using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDOTCtrl : MonoBehaviour
{
    private SphereCollider sphere;
    private BoxCollider box;
    private CapsuleCollider capsule;
    public float time;
    public int damage;

    private PlayerHealth playerHealth;
    private WaitForSeconds interval;


    public List<GameObject> targetList;

    private void Awake()
    {
        sphere = GetComponent<SphereCollider>();
        box = GetComponent<BoxCollider>();
        capsule = GetComponent<CapsuleCollider>();

        targetList = new List<GameObject>();
        interval = new WaitForSeconds(time);
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
        StartCoroutine(DamageOverTime());
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
        targetList.Clear();
        StopCoroutine(DamageOverTime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            targetList.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            targetList.Remove(other.gameObject);
        }
    }
    IEnumerator DamageOverTime()
    {
        while (true)
        {
            for(int i = 0; i < targetList.Count; i++)
            {
                playerHealth = targetList[i].GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.Damaged(damage);
                }
            }
            yield return interval;
        }
    }
}
