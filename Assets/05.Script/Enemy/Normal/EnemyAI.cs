using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavAgentCtrl))]
[RequireComponent(typeof(NormalHealthCtrl))]
[RequireComponent(typeof(FOV))]
[RequireComponent(typeof(HitBox))]
public class EnemyAI : MonoBehaviour
{
    public enum State { STOP, TRACE, ATTACK, DIE }
    public enum MonsterType { NORMAL,SUMMON}
    public State state = State.TRACE;
    public MonsterType type = MonsterType.NORMAL;
    public float attackDist;

   
    protected float curDist;
    protected WaitForSeconds ws = new WaitForSeconds(0.5f);
    protected NavAgentCtrl nav;
    protected NormalHealthCtrl health;
    protected Animator animator;
    protected Transform playerTr;
    protected Rigidbody rb;
    protected FOV fov;
    protected HitBox check;
    protected bool CanAtk = true;


    protected virtual void Awake()
    {
        nav = GetComponent<NavAgentCtrl>();
        health = GetComponent<NormalHealthCtrl>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        fov = GetComponent<FOV>();
        check = GetComponent<HitBox>();
        nav._Awake();

        rb.isKinematic = true;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTr=player.GetComponent<Transform>();
            transform.LookAt(playerTr);
        }
        
    }
    IEnumerator CheckState()    
    {
        yield return ws;
        while (!health.IsDie)
        {
            curDist = Vector3.Distance(transform.position, playerTr.position);
            if (state == State.DIE) { yield break; }
            else if (curDist <= attackDist)
            {
                state = State.ATTACK;
            }
            else
            {
                state = State.TRACE;
            }
            yield return ws;
        }
        state = State.DIE;
        
       
    }
    protected virtual IEnumerator Action()
    {
        yield break ;
    }
    protected virtual void Attack() { }
    public virtual void Revive(bool summon)
    {
        if (summon)
        {
            type = MonsterType.SUMMON;
        }
        else
        {
            type = MonsterType.NORMAL;
        }
        this.gameObject.layer = LayerMask.NameToLayer("Enemy");
        rb.isKinematic = false;
        health.Revive();
        nav.Turn(true);
        health.IsDie = false;
        state = State.TRACE;
        this.gameObject.SetActive(true);
        nav.TraceTarget(playerTr.position);

        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }
    protected virtual void Die()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        nav.Stop();
        nav.Turn(false);
        rb.isKinematic = true;
        StopAllCoroutines();
        StartCoroutine(health.Disappear(2f));
    }

    protected virtual IEnumerator AtkDelay()
    {
        yield break;
    }
    protected IEnumerator LerpRotate(Transform target, float time)
    {
        float elapsedTime = 0.0f;
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        animator.SetBool("Move", true);
        while (elapsedTime < time)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, elapsedTime * 0.125f);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("Move", false);
        yield return 0;
    }
}
