using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialStageCtrl : StageCtrl
{
    public GameObject[] SpecialMonster;

    public GameObject BossStatus;
    public float[] specialSpawnIntervaltime;
    private WaitForSeconds[] SpecialSpawnTime;
    protected override void Awake()
    {
        base.Awake();
        BossStatus.SetActive(false);
        SpecialSpawnTime = new WaitForSeconds[SpecialMonster.Length];
        for (int i = 0; i < SpecialMonster.Length; i++)
        {
            SpecialSpawnTime[i] = new WaitForSeconds(specialSpawnIntervaltime[i]);
        }

    }
    private  void OnTriggerEnter(Collider other)
    {

        start = true;
        StartCoroutine(StageStart());
    }
    private void OnTriggerExit(Collider other)
    {
        start = false;
        StopAllCoroutines();
    }
    protected override IEnumerator Battle()
    {
        for (int i = 0; i < Monster.Length; i++)
        {
            StartCoroutine(Spawn(Monster[i], SpawnTime[i]));
        }
        for (int i = 0; i < SpecialMonster.Length; i++)
        {
            StartCoroutine(BossSpawn(SpecialMonster[i], SpecialSpawnTime[i]));
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
    protected override IEnumerator CheckClear()
    {
        while (true)
        {
            int spMonsterCNT=0;
            for (int i = 0; i < SpecialMonster.Length; i++)
            {
                if (SpecialMonster[i].activeSelf)
                {
                    spMonsterCNT++;
               }
            }
            monsterHits = Physics.BoxCastAll(transform.position, mapSize * 0.5f, transform.up, Quaternion.identity, 5f, 1 << 9);

            Debug.Log(monsterHits.Length + " " + Clear);
            if (monsterHits.Length == 0&& spMonsterCNT==0)
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

                proText.text = "That's the Way to Go!";
                StageProduction.SetActive(true);
                cam.ChangeTarget(playerTr, cam.originHeight, cam.originForward, 1f);
                break;
            }
            yield return Check;
        }
    }
    private IEnumerator BossSpawn(GameObject monster, WaitForSeconds spawnTime)
    {
        yield return spawnTime;
        if (!monster.activeSelf)
        {
            monster.SetActive(true);

            BossStatus.SetActive(true);
        }
    }
}
