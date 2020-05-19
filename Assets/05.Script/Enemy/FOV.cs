using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour
{
    public bool IsViewTarget(Vector3 targetPos,float angle)
    {
        return angle > Vector3.Angle(transform.forward, targetPos-transform.position) ? true : false;
    }
}
