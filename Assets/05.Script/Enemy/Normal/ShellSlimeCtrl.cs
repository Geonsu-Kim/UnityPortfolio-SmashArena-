using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellSlimeCtrl : EnemyAI
{
    private bool defend = false;
    private bool defendOnce = false;
    private SphereCollider sphere;
    ParticleSystem p;
    WaitForSeconds atkDelay;
    public float atkDelayTime = 5f;
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
                    if (!defend)
                    {
                        animator.SetBool("Move", false);
                        nav.Stop();
                        Attack();
                    }
                        break;
                    case State.TRACE:
                    if (!defend)
                    {
                        nav.TraceTarget(playerTr.position);
                        animator.SetBool("Move", true);
                    }
                        break;
                    case State.STOP:
                        nav.Stop();
                        animator.SetBool("Move", false);
                        break;
                }
            
        }
        yield return base.Action();
    }
    protected override void Attack()
    {
        transform.LookAt(playerTr);
        if (CanAtk)
        {
            base.Attack();
            if (!defendOnce && health.CurHpRatio() <= 0.5f)
            {
                Defend();
                return;
            }

            animator.SetTrigger("Attack");
            StartCoroutine(AtkDelay());
        }
    }
    private void AttackPattern(AnimationEvent animationEvent)
    {
        check.CheckBox(transform.position, new Vector2(1f, 1f) * 0.5f, transform.forward, 4f, 1 << 10,30);
        check.HitEffect(check.GetBoxHits(transform.position, new Vector2(1f, 1f) * 0.5f, transform.forward, 3, 1 << 10));
    }
    void Defend()
    {
        StartCoroutine(DefenseMode());
        defendOnce = true;
            nav.Stop();
            animator.SetBool("Move", false);
    }
    
    public override  void Revive(bool summon)
    {
        health.OnDie += Die;
        sphere.enabled = true;
        base.Revive(summon);
    }
    protected override void Die()
    {
        base.Die();
        health.OnDie -= Die;
        sphere.enabled = false;
        defend = false;
        defendOnce = false;
    }
    protected override IEnumerator AtkDelay()
    {
        yield return base.AtkDelay();
        CanAtk = false;
        yield return atkDelay;
        CanAtk = true;

    }
    IEnumerator DefenseMode()
    {
        defend = true;
        animator.SetBool("Defend",true);

        animator.SetTrigger("StartDefend");
        health.DamageCoef = 0.3f;
        yield return new WaitForSeconds(10f);

        health.DamageCoef = 1f;

        animator.SetBool("Defend", false);
        defend = false;
    }
    // Start is called before the first frame update

}
