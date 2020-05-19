using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(NavAgentCtrl))]
[RequireComponent(typeof(BossHealthCtrl))]
[RequireComponent(typeof(FOV))]
[RequireComponent(typeof(CheckUsingCasting))]
public class ForestDragonCtrl : MonoBehaviour
{
    public ParticleSystem flame;
    public Transform[] patrolPos;
    public Transform firePos;
    public ParticleSystem[] Skills;
    public EnemyDOTCtrl flameThrower;
    public Transform landingPos;


    private Projector indicatorProjector;
    private WaitForSeconds ws;
    private WaitForSeconds fireWallCreateWs;
    private WaitForSeconds fireWallDisapearWs;

    private FOV fov;
    private NavAgentCtrl nav;
    private HealthControl health;
    private Animator animator;
    private Transform playerTr;
    private CapsuleCollider capsule;
    private CheckUsingCasting check;
    private Rigidbody rb;
    private RaycastHit[] hits;

    private int damage = 0;
    private int maxEnragedAtk = 0;
    private int enragedCount = 0;
    private float curDist = 0;
    private float delay = 0.5f;
    private float enrageRatio = 0.75f;
    private bool isFlying = false;

    // Start is called before the first frame update
    void Awake()
    {
        ws = new WaitForSeconds(2f);
        fireWallCreateWs = new WaitForSeconds(1.0f);
        fireWallDisapearWs = new WaitForSeconds(3.0f);
        fov = GetComponent<FOV>();
        nav = GetComponent<NavAgentCtrl>();
        health = GetComponent<HealthControl>();
        animator = GetComponent<Animator>();
        check = GetComponent<CheckUsingCasting>();
        capsule = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        {
            if (player!=null)
            {
                playerTr = player.GetComponent<Transform>();
            }

        }
        nav._Awake();
    }
    private void OnEnable()
    {
        nav.Turn(true);
        for (int i = 0; i < Skills.Length; i++)
        {
            Skills[i].Stop();
        }
        
        health.OnDie += this.Die;
        flame.Stop();
        flameThrower.ColliderDisable();

        StartCoroutine(Action());
        StartCoroutine(CheckHp());
    }
    //지상 공격 패턴
    void GroundAttack(AnimationEvent animationEvent)
    {

        //불꽃엄니
        if (animationEvent.stringParameter.Equals("BasicAttack"))
        {
            check.CheckBox(transform.position + transform.up + transform.forward * 4f, new Vector2(2, 1) * 0.5f, transform.forward, 2f,1<<10, damage);
            Skills[2].Play();
        }

        //돌진
        else if (animationEvent.stringParameter.Equals("TakeDown"))
        {
            if (animationEvent.intParameter == 0)
            {

                check.CheckBox(transform.position + transform.up, new Vector2(5, 1) * 0.5f, transform.forward, 4.75f, 1 << 10, damage);
            }

            else if (animationEvent.intParameter == 1)
            {

                check.CheckBox(transform.position + transform.up + transform.forward * 4.75f, new Vector2(5, 1) * 0.5f, transform.forward, 4.75f, 1 << 10, damage);
            }
        }
        //화염볼
        else if (animationEvent.stringParameter.Equals("FireballShoot"))
        {
            StartCoroutine(FireballShoot(firePos.position, playerTr.position, 1.5f));
        }
        //화염방사
        else if (animationEvent.stringParameter.Equals("FlameThrower"))
        {
            if (animationEvent.intParameter == 0)
            {
                flameThrower.ColliderEnable();
                Skills[0].Play();
            }

            else if (animationEvent.intParameter == 1)
            {
                flameThrower.ColliderDisable();
            }
        }
        //오버히트
        else if (animationEvent.stringParameter.Equals("Scream"))
        {
            check.CheckSphere(transform.position, 7f, 1 << 10, damage);
            Skills[1].Play();
        }
    }
    //이착륙 조정
    void TakingOffLanding()
    {
        StopAllCoroutines();
        if (isFlying)
        {
            rb.isKinematic = false;
            capsule.enabled = true;
            isFlying = false;
        }
        else
        {
            rb.isKinematic = true;
            capsule.enabled = false;
            isFlying = true;

            nav.Speed = 25f;
            nav.Stop();
        }
        StartCoroutine(ActionControl());
    }
    //행동 컨트롤 On/Off
    IEnumerator ActionControl()
    {
        yield return ws;
        if (isFlying)
        {
            nav.Turn(false);
        }
        else
        {

            nav.Turn(true);
            StartCoroutine(CheckHp());
        }
       
        StartCoroutine(Action());


    }

    //행동 전체 컨트롤
    IEnumerator Action()
    {
        while (!health.IsDie)
        {
            if (!isFlying)
            {

                int patternIdx = Random.Range(0, 6);
                switch (patternIdx)
                {
                    case 0: // 불꽃엄니

                        Debug.Log(!fov.IsViewTarget(playerTr.position, 15f));
                        curDist = Vector3.Distance(playerTr.position, transform.position); //플레이어랑 일정 거리 두기
                        if (curDist > 5f)
                        {
                            StartCoroutine(CheckDist());
                            animator.SetBool("Move", true);
                            nav.TraceTarget(playerTr.position);
                            delay = 4f;
                            yield return YieldInstructionCache.WaitForSeconds(delay);
                        }
                        else if (!fov.IsViewTarget(playerTr.position, 15f))
                        {
                            StartCoroutine(LerpRotate(playerTr, 2f));
                            yield return ws;
                        }
                        damage = 100;
                        animator.SetTrigger("BasicAttack");
                        delay = 3f;
                        break;
                    case 1: // 돌진

                        curDist = Vector3.Distance(playerTr.position, transform.position); //플레이어랑 일정 거리 두기
                        if (curDist > 5f)
                        {
                            StartCoroutine(CheckDist());
                            animator.SetBool("Move", true);
                            nav.TraceTarget(playerTr.position);
                            delay = 4f;
                            yield return YieldInstructionCache.WaitForSeconds(delay);
                        }
                        else if (!fov.IsViewTarget(playerTr.position, 20f))
                        {
                            StartCoroutine(LerpRotate(playerTr, 2f));
                            yield return ws;
                        }
                        damage = 200;
                        animator.SetTrigger("TakeDown");
                        delay = 3f;                
                        break;
                    case 3: // 화염볼

                        if (!fov.IsViewTarget(playerTr.position, 30f))
                        {
                            
                            StartCoroutine(LerpRotate(playerTr, 2f));
                            yield return ws;
                        }
                        animator.SetTrigger("FireballShoot");
                        delay = 3f;
                        break;
                    case 4: // 화염방사

                        if (!fov.IsViewTarget(playerTr.position, 45f))
                        {
                            StartCoroutine(LerpRotate(playerTr, 2f));
                            yield return ws;
                        }
                        animator.SetTrigger("FlameThrower");
                        delay = 5.5f;
                        break;
                    case 5: // 오버히트

                        damage = 500;
                        animator.SetTrigger("Scream");
                        delay = 6f;
                        break;

                }
                animator.SetBool("Move", false);

                nav.Stop();
                yield return YieldInstructionCache.WaitForSeconds(delay);
            }
            else
            {

                int enragedAtkCount = 0;


                while (enragedAtkCount < maxEnragedAtk)
                {
                    Debug.Log(enragedAtkCount);
                    int patternIdx =Random.Range(0, 2);
                    switch (patternIdx)
                    {
                        case 0: // 화염벽
                            delay = 1.5f;
                            int patrolIdxFrom = Random.Range(0, patrolPos.Length);
                            int patrolIdxTo = (patrolIdxFrom + patrolPos.Length / 2) % patrolPos.Length;
                            Debug.Log(patrolIdxFrom + "  " + patrolIdxTo);
                            transform.position = patrolPos[patrolIdxFrom].position;
                            FireRoadIndicator(patrolIdxTo, patrolIdxFrom);

                            transform.LookAt(patrolPos[patrolIdxTo]);
                            yield return YieldInstructionCache.WaitForSeconds(delay);
                            StartCoroutine(LerpMove(patrolPos[patrolIdxTo].position,0.25f,3f));
                            StartCoroutine(FIreWallCreate(patrolPos[patrolIdxTo].position, patrolPos[patrolIdxFrom].position));
                            delay = 3f;
                            break;
                        case 1: // 폭격

                            StartCoroutine(AerialBombing());
                            delay = 7f;
                            break;
                    }

                    animator.SetBool("Gliding", false);

                    yield return YieldInstructionCache.WaitForSeconds(delay);

                    enragedAtkCount++;
                    yield return YieldInstructionCache.WaitForSeconds(0.5f);
                }
                transform.LookAt(playerTr);
                StartCoroutine(LerpMove(landingPos.position, 2f,0.125f));
                yield return YieldInstructionCache.WaitForSeconds(0.5f);
                animator.SetBool("Flying",false );
                yield break;
            }
        }
        yield break;
    }
    public void Die()
    {
        StopAllCoroutines();
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        nav.Stop();
        nav.Turn(false);
        capsule.enabled = false;

        health.OnDie -= this.Die;
        StartCoroutine(health.Disappear(5f));
    }
    //화염벽 생성
    IEnumerator FIreWallCreate(Vector3 from, Vector3 to)
    {
        GameObject firewall = ObjectPoolManager.instance.GetObject("FireWall",true);
        if (firewall != null)
        {
            yield return fireWallCreateWs;
            firewall.transform.position = (to - from) * 0.5f + from+Vector3.up * 0.3f;
            firewall.transform.LookAt(to);
            firewall.transform.rotation = firewall.transform.rotation * Quaternion.Euler(0, 90f, 0);
            firewall.SetActive(true);
            firewall.GetComponent<ParticleSystem>().Play();
            firewall.GetComponent<EnemyDOTCtrl>().ColliderEnable();
            yield return fireWallDisapearWs;
            firewall.GetComponent<EnemyDOTCtrl>().ColliderDisable();
        }
        else
        {
            yield break;
        }
    }

    //화염벽 위치 표시
    void FireRoadIndicator(int to,int from)
    {
        GameObject fireWallIndicator = ObjectPoolManager.instance.GetObject("Indicator", true);
        if (fireWallIndicator != null)
        {
            indicatorProjector = fireWallIndicator.GetComponent<Projector>();
            if (indicatorProjector != null)
            {
                fireWallIndicator.SetActive(true);
                StartCoroutine(ColorChange(indicatorProjector,1.0f));
                fireWallIndicator.transform.position = patrolPos[from].position;
                fireWallIndicator.transform.LookAt(patrolPos[to].position);
            }
        }
    }

    //플레이어와 보스 사이의 거리 체크
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



    //코루틴으로 회전보간 변화
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
    //코루틴으로 이동보간 변화
    IEnumerator LerpMove(Vector3 targetPos, float time,float speed)
    {
        float elapsedTime = 0.0f;
        animator.SetBool("Gliding", true);
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, elapsedTime * speed);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("Gliding", false);
        yield return 0;
    }
    //코루틴으로 색깔 보간 변화
    IEnumerator ColorChange(Projector projector,float time)
    {
        float elapsedTime = 0.0f;
        projector.material.color = Color.red;

        while (elapsedTime < time)
        {
            projector.material.color = Color.Lerp(projector.material.color, Color.black, elapsedTime * 0.0625f);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return 0;
    }

    //화염볼 소환 코루틴
    IEnumerator FireballShoot(Vector3 from, Vector3 to, float time)
    {
        GameObject fireBallIndicator = ObjectPoolManager.instance.GetObject("BulletIndicator", true);
        if (fireBallIndicator != null)
        {
            fireBallIndicator.transform.position = to + Vector3.up * 0.3f;
            fireBallIndicator.SetActive(true);
            fireBallIndicator.GetComponent<ParticleSystem>().Play();
            yield return YieldInstructionCache.WaitForSeconds(time);
        }
        else yield break;
        GameObject fireBall = ObjectPoolManager.instance.GetObject("FireBall", false);
        if (fireBall != null)
        {
            fireBall.transform.position = from;
            fireBall.GetComponent<EnemyBulletControl>().ColliderEnable();
            fireBall.GetComponent<EnemyBulletControl>().Revive();
            fireBall.GetComponent<Rigidbody>().AddForce((to - from).normalized * 1000f);
        }

        else yield break;
    }
    //화염볼 폭격 코루틴
    IEnumerator AerialBombing()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return YieldInstructionCache.WaitForSeconds(0.5f);
            StartCoroutine(FireballShoot(playerTr.position + Vector3.up * 10f, playerTr.position, 1f));
        }
    }
    //남은 체력에 따른 필살기 발동
    IEnumerator CheckHp()
    {
        if (enragedCount > 1)
            yield break;
        while (true)
        {
            yield return ws;
            if (health.CurHpRatio() <= enrageRatio)
            {
                switch (enragedCount)
                {
                    case 0:

                        enrageRatio = 0.5f;
                        maxEnragedAtk = 3;
                        break;

                    case 1:

                        enrageRatio = 0.25f;

                        maxEnragedAtk = 6;
                        break;
                }
                enragedCount++;
                animator.SetBool("Flying", true);
                break;
            }
        }
    }
}
