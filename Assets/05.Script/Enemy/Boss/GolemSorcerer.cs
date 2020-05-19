using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
[RequireComponent(typeof(HealthControl))]

[RequireComponent(typeof(FOV))]

[RequireComponent(typeof(CheckUsingCasting))]

[RequireComponent(typeof(NavAgentCtrl))]
[RequireComponent(typeof(Rigidbody))]

public class GolemSorcerer : MonoBehaviour
{

    public Transform[] summonPos;

    private FOV fov;
    private NavAgentCtrl nav;
    private HealthControl health;
    private Animator animator;
    private Transform playerTr;
    private CapsuleCollider capsule;
    private CheckUsingCasting check;
    private WaitForSeconds ws;
    private Rigidbody rb;

    private GameObject temp;
    private RaycastHit[] hits;
    private Vector3 temp_targetPos;

    private string summonmMonsterName;
    private float delay;
    private float curDist = 0;
    private void Awake()
    {

        ws = new WaitForSeconds(1f);
        fov = GetComponent<FOV>();
        nav = GetComponent<NavAgentCtrl>();
        health = GetComponent<HealthControl>();
        animator = GetComponent<Animator>();
        check = GetComponent<CheckUsingCasting>();
        capsule = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        {
            if (player != null)
            {
                playerTr = player.GetComponent<Transform>();
            }

        }

        nav._Awake();
    }
    private void OnEnable()
    {
        health.OnDie += this.Die;
        nav.Turn(true);
        StartCoroutine(Action());
    }
    // Update is called once per frame
    IEnumerator Action()
    {

        while (!health.IsDie)
        {
            if (gameObject.activeSelf)
            {
                int patternIdx = Random.Range(0, 4);
                switch (patternIdx)
                {
                    case 0:
                        if (!fov.IsViewTarget(playerTr.position, 30f))
                        {
                            StartCoroutine(LerpRotate(playerTr, 2f));
                            yield return ws;
                        }
                        delay = 3f;
                        break;
                    case 1:
                        delay = 5f;
                        break;
                    case 2:
                        curDist = Vector3.Distance(playerTr.position, transform.position);
                        if (curDist > 3f)
                        {
                            StartCoroutine(CheckDist());
                            animator.SetBool("Move", true);
                            nav.TraceTarget(playerTr.position);
                            delay = 4f;
                            yield return YieldInstructionCache.WaitForSeconds(delay);
                        }
                        delay = 5f;
                        break;
                    case 3:

                        delay = 3f;
                        break;
                }

                animator.SetBool("Move", false);
                animator.SetInteger("PatternIdx", patternIdx);
                animator.SetTrigger("Attack");

                nav.Stop();
                yield return YieldInstructionCache.WaitForSeconds(delay);
            }
        }
        yield break;
    }
    void Die()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        nav.Stop();
        nav.Turn(false);
        rb.isKinematic = true;
        capsule.enabled = false;

        health.OnDie -= this.Die;
        StopAllCoroutines();
        StartCoroutine(health.Disappear(3f));
    }
    void AttackPattern(AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter.Equals("StoneEdge"))
        {
            if (animationEvent.intParameter == 0)
            {
                temp = ObjectPoolManager.instance.GetObject("BulletIndicator", true);
                temp_targetPos = playerTr.position + Vector3.up * 0.1f;
                if (temp != null)
                {
                    temp.transform.position = temp_targetPos;
                    temp.SetActive(true);
                    temp.GetComponent<ParticleSystem>().Play();
                }
            }
            else if (animationEvent.intParameter == 1)
            {
                temp = ObjectPoolManager.instance.GetObject("StoneEdge", true);
                if (temp != null)
                {
                    temp.transform.position = temp_targetPos;
                    temp.transform.rotation = Quaternion.Euler(-90f, 0, 0);
                    temp.SetActive(true);
                    temp.GetComponent<ParticleSystem>().Play();
                    check.CheckSphere(temp_targetPos, 2f, 1 << 10, 10);
                }
            }
        }
        else if (animationEvent.stringParameter.Equals("Healing"))
        {
            hits = check.GetSphereHits(transform.position, 10f, 1 << 9);
            for(int i = 0; i < hits.Length; i++)
            {
                temp = ObjectPoolManager.instance.GetObject("BuffIndicator", true);
                if (temp != null)
                {
                    temp.transform.position = hits[i].collider.gameObject.transform.position + Vector3.up * 0.2f;
                    temp.SetActive(true);
                    temp.GetComponent<ParticleSystem>().Play();
                    hits[i].collider.gameObject.GetComponent<HealthControl>().Recoverd(30);
                }
            }
        }
        else if (animationEvent.stringParameter.Equals("Rock Slide"))
        {
            StartCoroutine(RockSlide());
        }
        else if (animationEvent.stringParameter.Equals("Summon"))
        {
            for (int i = 0; i < 4; i++)
            {
                int summonIdx = Random.Range(0, 2);
                summonmMonsterName = summonIdx == 0 ? "[NORMAL]Slime" : "[NORMAL]ShellSlime";
                summonIdx = Random.Range(0, summonPos.Length);
                temp = ObjectPoolManager.instance.GetObject("BuffIndicator", true);
                if (temp != null)
                {
                    temp.transform.position = summonPos[summonIdx].position+Vector3.up*0.1f;
                    temp.SetActive(true);
                    temp.GetComponent<ParticleSystem>().Play();
                }
                temp = ObjectPoolManager.instance.GetObject(summonmMonsterName,false);
                if (temp != null)
                {
                    temp.transform.position = summonPos[summonIdx].position;
                    temp.GetComponent<EnemyAI>().Revive(true);
                }
            }
        }
    }
    IEnumerator RockSlide()
    {
        for(int i = 0; i < 20; i++)
        {
            Vector3 pos = transform.position + new Vector3(Random.Range(-7f, 7f), 0, Random.Range(-7f, 7f));
            StartCoroutine(RockFall(pos + Vector3.up * 10f, pos,3f));
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
    }
    IEnumerator RockFall(Vector3 from, Vector3 to, float time)
    {
        GameObject rockIndicator = ObjectPoolManager.instance.GetObject("BulletIndicator", true);
        if (rockIndicator != null)
        {
            rockIndicator.transform.position = to + Vector3.up * 0.2f;
            rockIndicator.SetActive(true);
            rockIndicator.GetComponent<ParticleSystem>().Play();
            yield return YieldInstructionCache.WaitForSeconds(time);
        }
        else yield break;
        GameObject rock = ObjectPoolManager.instance.GetObject("Rock", false);
        if (rock != null)
        {
            rock.transform.position = from;
            rock.transform.rotation = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
            rock.GetComponent<EnemyBulletControl>().ColliderEnable();
            rock.GetComponent<EnemyBulletControl>().Revive();
            rock.GetComponent<Rigidbody>().AddForce((to - from).normalized * 2000f);
        }

        else yield break;
    }
    IEnumerator LerpRotate(Transform target, float time)
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
    IEnumerator CheckDist()
    {
        while (true)
        {
            curDist = Vector3.Distance(playerTr.position, transform.position);
            if (curDist <= 5f) { break; }
            yield return null;
        }
        nav.Stop();

        animator.SetBool("Move", false);
    }
}
