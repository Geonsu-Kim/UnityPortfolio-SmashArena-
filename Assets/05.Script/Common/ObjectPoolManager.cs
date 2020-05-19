using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;
    private WaitForSeconds DeactTime; // 시간 지연 비활성화

    public GameObject[] gameObjects; // 오브젝트 풀링 대상 객체
    private Dictionary<string, List<GameObject>> Pools_gameObjects; // 오브젝트 풀 딕셔너리 < 풀링 대상 객체 이름 , 오브젝트 풀 >
    public Transform[] parents; // 하이어라키상 객체 그룹화
    public int[] gameObjectsCount; // 객체별 생성 갯수
    private GameObject temp; // 임시 객체

    private void Awake()
    {
        instance = this;
        temp = new GameObject();
        DeactTime = new WaitForSeconds(4.0f);
        Pools_gameObjects = new Dictionary<string, List<GameObject>>();
        for(int i =0; i< gameObjects.Length; i++)
        {
            Pools_gameObjects.Add(gameObjects[i].name, new List<GameObject>());
        }
        for(int i=0;i< Pools_gameObjects.Count; i++)
        {
            Create(gameObjects[i], temp, Pools_gameObjects[gameObjects[i].name], parents[i], gameObjectsCount[i]);
        }

    }

    void Create(GameObject target,GameObject  temp,List<GameObject> Pool,Transform parent,int count)
    {
        for (int i = 0; i < count; i++)
        {
            temp = Instantiate(target, parent.position, parent.rotation);
            temp.SetActive(false);
            temp.transform.SetParent(parent);
            Pool.Add(temp);
        }
    }

    public GameObject GetObject(string objname,bool deact)
    {
        for (int i = 0; i <Pools_gameObjects[objname].Count; i++)
        {
            if (!Pools_gameObjects[objname][i].activeSelf)
            {
                if (deact)
                {
                    StartCoroutine(Deactivate(Pools_gameObjects[objname][i]));
                }
                return Pools_gameObjects[objname][i];
            }
        }
        return null;
    }
    IEnumerator Deactivate(GameObject target)
    {
        yield return DeactTime;
        target.SetActive(false);
        target.transform.position = Vector3.zero;
    }


}
