using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollider : MonoBehaviour
{
    public string targetTag;
    public List<GameObject> targetList;
    // Start is called before the first frame update
    private void Awake()
    {
        targetList = new List<GameObject>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(targetTag))
        {
            targetList.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(targetTag))
        {
            targetList.Remove(other.gameObject);
        }
    }
}
