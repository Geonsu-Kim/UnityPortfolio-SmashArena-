using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentCtrl : MonoBehaviour
{
    public  NavMeshAgent agent;
    public float Speed
    {
        set
        {
            agent.speed = value;
        }
        get
        {
            return agent.speed;
        }
    }
    public float AngularSpeed
    {
        set
        {
            agent.angularSpeed = value;
        }
        get
        {
            return agent.angularSpeed;
        }
    }
    public void _Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }
    public void TraceTarget(Vector3 tr)
    {
        agent.isStopped = false;
        agent.SetDestination(tr);
    } 
    public void Stop()
    {
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
    }
    public void Turn(bool _set)
    {
        agent.enabled = _set;
    }

}

