using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NormalHealthCtrl : HealthControl
{


    private GameObject hpBar;
    private Image hpBarImage;
    public int gold;
    private bool damaged = false;
    private Canvas enemyCanvas;
    public Vector3 hpBarOffset = new Vector3(0, 2.2f, 0);


    protected override void Awake()
    {
        enemyCanvas = GameObject.Find("UI - MonsterHpCanvas").GetComponent<Canvas>();


        base.Awake();

    }
    public override void Revive()
    {
        base.Revive();
        hpBar = ObjectPoolManager.instance.GetObject("HpBarBg", false);
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];
        var _hpBar = hpBar.GetComponent<InScreenUI>();
        _hpBar.offset = hpBarOffset;
        _hpBar.targetTr = this.gameObject.transform;
        hpBarImage.fillAmount = 1;
        hpBar.SetActive(true);
    }
    public override void Damaged(int damage)
    {
        base.Damaged(damage);

        damaged = true;
        hpBarImage.fillAmount = (float)curHp / (float)maxHp;
    }
    public override void Recoverd(int amount)
    {
        base.Recoverd(amount);

        hpBarImage.fillAmount = (float)curHp / (float)maxHp;
    }
    protected override void Die()
    {
        hpBar.SetActive(false);

        base.Die();
    }

}
