using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
public class StageCtrl : MonoBehaviour
{
    public GameObject[] Monster;

    protected WaitForSeconds[] SpawnTime;
    public float[] spawnIntervaltime;

    public GameObject[] Gate;
    public GameObject[] LockedGate;
    public GameObject StageProduction;
    public Transform[] SpawnPts;
    public Text proText;
    
    public int bonusExp;
    public float stageTime;
    protected bool clear = false;
    protected bool start = false;
    protected int prize;
    protected RaycastHit[] monsterHits;
    protected SphereCollider buttonColl;
    protected FollowCam cam;
    protected Transform playerTr;
    protected GameObject temp;
    protected ParticleSystem particle;

    protected WaitForSeconds StageTime;
    protected WaitForSeconds WaitTime;

    protected WaitForFixedUpdate Check;

    protected Vector3 mapSize;
    public float lenght;
    public float width;

    public bool Clear
    {
        get
        {
            return clear;
        }
        set
        {
            clear = value;
        }
    }
    protected virtual void Awake()
    {
        StageProduction.SetActive(false);
        buttonColl = GetComponent<SphereCollider>();
        particle = GetComponent<ParticleSystem>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        cam = GameObject.Find("UI - MainCam").GetComponent<FollowCam>();
        StageTime = new WaitForSeconds(stageTime);
        WaitTime = new WaitForSeconds(2f);
        SpawnTime = new WaitForSeconds[Monster.Length];
        Check = new WaitForFixedUpdate();
        for (int i = 0; i < SpawnTime.Length; i++)
        {
            SpawnTime[i] = new WaitForSeconds(spawnIntervaltime[i]);
;
        }
        for (int i = 0; i < Gate.Length; i++)
        {
            Gate[i].GetComponent<ParticleSystem>().Stop();
        }
        particle.Play();
        mapSize = new Vector3(lenght, 3f, width);
    }
    private  void OnTriggerEnter(Collider other)
    {
        start = true;
        StartCoroutine(StageStart());
    }
    private  void OnTriggerExit(Collider other)
    {
        start = false;
        StopAllCoroutines();
    }
    protected IEnumerator StageStart()
    {
        yield return WaitTime;
        if (start)
        {
            buttonColl.enabled = false;
            particle.Stop();
            cam.ChangeTarget(playerTr, cam.originHeight * 1.3f, cam.originForward * 1.3f, 0.25f);
            proText.text = "Battle Start";
            StageProduction.SetActive(true);
            Debug.Log(StageProduction.activeSelf);
            StartCoroutine(Battle());
               yield return YieldInstructionCache.WaitForSeconds(0.5f);
            cam.Damp = cam.originDamp;
        }
    }
    protected IEnumerator Spawn(GameObject monster, WaitForSeconds spawnTime)
    {
        while (!Clear)
        {
            temp=ObjectPoolManager.instance.GetObject(monster.name, false);
            temp.transform.position = SpawnPts[Random.Range(0, SpawnPts.Length)].position;
            temp.GetComponent<EnemyAI>().Revive(false);

            yield return spawnTime;
        }
    }
    protected virtual IEnumerator Battle()
    {
       for(int i=0;i< Monster.Length; i++)
        {
            StartCoroutine(Spawn(Monster[i], SpawnTime[i]));
        }
        for (int i = 0; i < Gate.Length; i++)
        {
            Gate[i].GetComponent<BoxCollider>().enabled = true;
            Gate[i].GetComponent<ParticleSystem>().Play();
        }
        yield return StageTime;
        Clear = true;
        StopAllCoroutines();

        StartCoroutine(CheckClear());
    }
    protected virtual IEnumerator CheckClear()
    {
        while (true)
        {
            monsterHits = Physics.BoxCastAll(transform.position, mapSize*0.5f, Vector3.up, Quaternion.identity, 0, 1 << 9);

            if (monsterHits.Length == 0)
            {

                for (int i = 0; i < Gate.Length; i++)
                {
                    Gate[i].GetComponent<BoxCollider>().enabled = false;

                    Gate[i].GetComponent<ParticleSystem>().Stop();
                }
                for (int i = 0; i < LockedGate.Length; i++)
                {
                    LockedGate[i].GetComponent<BoxCollider>().enabled = false;

                    LockedGate[i].GetComponent<ParticleSystem>().Stop();
                }
                proText.text = "Stage Clear!";
                StageProduction.SetActive(true);
                cam.ChangeTarget(playerTr, cam.originHeight, cam.originForward, 0.25f);
                yield return YieldInstructionCache.WaitForSeconds(0.5f);
                cam.Damp = cam.originDamp;
                break;
            }
            yield return Check;
        }
    }

}
