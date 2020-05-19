using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCtrl : EnemyAI
{
    private SphereCollider sphere;
    WaitForSeconds atkDelay;
    public float atkDelayTime = 3f;
    protected override void Awake()
    {
        sphere = GetComponent<SphereCollider>();

        sphere.enabled = false;
        base.Awake();
        atkDelay = new WaitForSeconds(atkDelayTime);
    }
    protected override IEnumerator Action()
    {
        while (!health.IsDie)
        {
            yield return ws;
            switch (state)
            {
                case State.ATTACK:
                    nav.Stop();
                    animator.SetBool("Move",false);
                    Attack();
                    break;
                case State.TRACE:
                    nav.TraceTarget(playerTr.position);
                    animator.SetBool("Move", true);
                    break;
                case State.STOP:
                    nav.Stop();
                    animator.SetBool("Move", false);
                    break;
            }
        }
        Die();
        yield return base.Action();
    }
    
    protected override void Attack()
    {
        transform.LookAt(playerTr);
        if (CanAtk)
        {
            StartCoroutine(AtkDelay());
            animator.SetTrigger("Attack"); 
            animator.SetInteger("PatternIdx", Random.Range(0, 5));
        }
    }
    private void AttackPattern(AnimationEvent animationEvent)
    {
        
        if (animationEvent.intParameter ==0)
        {
            check.CheckBox(transform.position, new Vector2(1, 1) * 0.5f, transform.forward, 2f, 1 << 10, 5);
            check.HitEffect(check.GetBoxHits(transform.position, new Vector2(1, 1) * 0.5f, transform.forward, 2f, 1 << 10));
        }
        else{

            check.CheckBoxAddForce(transform.position, new Vector2(1.5f, 1.5f) * 0.5f, transform.forward, 1.5f, 1 << 10, 20,1f);
            check.HitEffect(check.GetBoxHits(transform.position, new Vector2(1.5f, 1.5f) * 0.5f, transform.forward, 1.5f, 1 << 10));
        }
    }
    public override void Revive(bool summon)
    {
        health.OnDie += Die;
        sphere.enabled = true;
        base.Revive(summon);
    }
    protected override void Die()
    {
        health.OnDie -= Die;
        base.Die();
        sphere.enabled = false;
    }
    protected override IEnumerator AtkDelay()
    {
        yield return base.AtkDelay();
        CanAtk = false;
        yield return atkDelay;
        CanAtk = true;
    }

}
