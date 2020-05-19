using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform targetTr;
    public float originHeight = 6f;
    public float originForward = -1f;
    public float originRight = 0f;
    public float originDamp = 1f;


    private float height;
    private float _forward;
    private float _right;
    private float damp;

    public float Damp
    {
        get
        {
            return damp;
        }
        set
        {
            damp = value;
        }
    }
    void Start()
    {
        height = originHeight - targetTr.localScale.y;
        _forward = originForward;
        _right = originRight;
        damp = originDamp;
    }
    public void ChangeTarget(Transform tr,float _height,float _forward_,float _damp)
    {
        targetTr = tr;
        height = _height - targetTr.localScale.y;
       _forward = _forward_;
        damp = _damp;
    }
    private void LateUpdate()
    {

        transform.position = Vector3.Lerp(transform.position, targetTr.position + Vector3.up * height + Vector3.forward * _forward + Vector3.right * _right, damp) ;
        transform.LookAt(targetTr);
    }
}