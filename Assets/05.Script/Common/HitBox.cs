using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private GameObject temp;
    public RaycastHit[] hits;
    public void CheckBox(Vector3 Origin, Vector2 halfExtension, Vector3 direction, float maxDistance,int layerMask,int damage)
    {
        hits = Physics.BoxCastAll(Origin, halfExtension, direction, Quaternion.identity, maxDistance, layerMask);
        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].collider.gameObject.GetComponent<HealthControl>().Damaged(damage);
        }
    }
    
    public void CheckSphere(Vector3 Origin, float radius, int layerMask, int damage)
    {
        hits = Physics.SphereCastAll(Origin, radius, transform.forward, 0.01f, layerMask);
       
        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].collider.gameObject.GetComponent<HealthControl>().Damaged(damage);
        }
    }
    public RaycastHit[] GetBoxHits(Vector3 Origin, Vector2 halfExtension, Vector3 direction, float maxDistance, int layerMask)
    {
        return Physics.BoxCastAll(Origin, halfExtension, direction, Quaternion.identity, maxDistance, layerMask); ;
    }
    public RaycastHit[] GetSphereHits(Vector3 Origin, float radius, int layerMask)
    {
        return Physics.SphereCastAll(Origin, radius, transform.forward, 0.01f, layerMask);


    }
    public void  CheckBoxAddForce(Vector3 Origin, Vector2 halfExtension, Vector3 direction, float maxDistance, int layerMask, int damage,float force)
    {
        hits = Physics.BoxCastAll(Origin, halfExtension, direction, Quaternion.identity, maxDistance, layerMask);
        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].collider.gameObject.GetComponent<HealthControl>().Damaged(damage);
            hits[i].collider.gameObject.GetComponent<Rigidbody>().AddForce((hits[i].collider.gameObject.transform.position-transform.position).normalized * force, ForceMode.Impulse);
        }
    }
    public void CheckSphereAddForce(Vector3 Origin, float radius, int layerMask, int damage,float force)
    {
        hits = Physics.SphereCastAll(Origin, radius, transform.forward, 0.01f, layerMask);

        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].collider.gameObject.GetComponent<HealthControl>().Damaged(damage);
            hits[i].collider.gameObject.GetComponent<Rigidbody>().AddForce(-hits[i].collider.gameObject.transform.forward * 5f, ForceMode.Impulse);
        }
    }
    public void HitEffect(RaycastHit[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            temp = ObjectPoolManager.instance.GetObject("HitEffect", true);
            if (temp != null)
            {
                temp.transform.position = hits[i].point;
                temp.SetActive(true);
                temp.GetComponent<ParticleSystem>().Play();
            }

        }
    }
}
