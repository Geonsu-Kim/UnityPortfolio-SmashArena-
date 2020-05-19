using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class ForestGolemCtrl : EnemyAI
{
    // Start is called before the first frame update

    public Transform ThrowingPos;

    private BoxCollider box;
    private int patternIdx;
    private bool attacking = false;
    private GameObject temp_Rock;

    private Transform originalParent;
    private Vector3 targetPos;
    protected override void Awake()
    {
        box = GetComponent<BoxCollider>();

        box.enabled = false;
        base.Awake();
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
                    animator.SetBool("Move", false);
                    Attack();
                    break;
                case State.TRACE:
                    if (!attacking)
                    {
                        nav.TraceTarget(playerTr.position);
                        animator.SetBool("Move", true);
                    }

                    break;
                case State.STOP:
                    nav.Stop();
                    animator.SetBool("Move", false);
                    break;
                case State.DIE:
                    Die();
                    break;
            }
        }
        yield return base.Action();
    }
    protected override void Attack()
    {
        attacking = true;
        base.Attack();
        if (CanAtk)
        {
            if (curDist <= attackDist && curDist > 5f)
            {
                patternIdx = 1;

            }
            else if (curDist <= 5f)
            {
                patternIdx = 0;
            }
            animator.SetTrigger("Attack");

            animator.SetInteger("AtkIdx", patternIdx);
            StartCoroutine(AtkDelay());
        }
    }

    void AttackPattern(AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter.Equals("Punch"))
        {
            check.CheckBox(transform.position + transform.up + transform.right * -0.7f + transform.forward * 1f, new Vector3(1.4f, 2f) * 0.5f, transform.forward, 2.5f, 1 << 10, 20);
        }
        else if (animationEvent.stringParameter.Equals("Throwing"))
        {
            if (animationEvent.intParameter == 0)
            {

                targetPos = playerTr.position;
                GameObject indicator = ObjectPoolManager.instance.GetObject("BulletIndicator", true);
                if (indicator != null)
                {
                    indicator.transform.position = targetPos + Vector3.up * 0.2f;
                    indicator.SetActive(true);
                    indicator.GetComponent<ParticleSystem>().Play();
                }
            }
            else if (animationEvent.intParameter == 1)
            {
                temp_Rock = ObjectPoolManager.instance.GetObject("Rock", false);
                if (temp_Rock != null)
                {
                    originalParent = temp_Rock.transform.parent;
                    temp_Rock.transform.SetParent(ThrowingPos);

                    temp_Rock.transform.localPosition = Vector3.zero;
                    temp_Rock.GetComponent<EnemyBulletControl>().ColliderDisable();
                    temp_Rock.GetComponent<EnemyBulletControl>().Revive();
                }

            }
            else
            {
                temp_Rock.transform.parent = originalParent;

                temp_Rock.GetComponent<EnemyBulletControl>().ColliderEnable();
                temp_Rock.GetComponent<Rigidbody>().AddForce((targetPos - temp_Rock.transform.position).normalized * 2000f);
            }
        }
    }

    
    public override void Revive(bool summon)
    {
        health.OnDie += Die;
        attacking = false;
        CanAtk = true;
        box.enabled = true;
        base.Revive(summon);
    }
    protected override void Die()
    {
        health.OnDie -= Die;
        box.enabled = false;
        base.Die();
    }
    protected override IEnumerator AtkDelay()
    {
        yield return base.AtkDelay();
        CanAtk = false;
        yield return YieldInstructionCache.WaitForSeconds(3f);

        StartCoroutine(LerpRotate(playerTr, 0.5f));
        CanAtk = true;
        attacking = false;
    }
}
