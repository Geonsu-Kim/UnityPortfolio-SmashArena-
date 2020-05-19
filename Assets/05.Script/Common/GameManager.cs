using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private int gold=0;

    public Vector3 goldOffset = new Vector3(0, 2.7f, 0);

    private GameObject GoldImg;
    private Transform PlayerTr;
    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            gold = value;
        }
    }
    public Text goldText;
    public static GameManager instance;
    void Awake()
    {
        PlayerTr = GameObject.Find("Player").transform;
        instance = this;
        goldText.text = string.Format("{0}", Gold.ToString());
    }
    public void GetGold(int addGold)
    {
        GoldImg=ObjectPoolManager.instance.GetObject("Gold", true);
        GoldImg.SetActive(true);
        var _gold = GoldImg.GetComponent<InScreenUI>();
        _gold.offset = goldOffset;
        _gold.targetTr = PlayerTr;
        Gold += addGold;
        goldText.text = string.Format("{0}", Gold.ToString());
    }
}
