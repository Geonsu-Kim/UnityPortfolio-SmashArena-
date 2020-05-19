using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossHealthCtrl : HealthControl
{
    public GameObject BossStatus;
    public Text hpText;
    public Image hpBarImage;
    private bool damaged=false;

    public int gold;
    protected override void Awake()
    {
        base.Awake();

    }
    public override void Damaged(int damage)
    {
        base.Damaged(damage);

        damaged = true;
        hpBarImage.fillAmount = (float)curHp / (float)maxHp;
        hpText.text = string.Format("{0}/{1}", curHp, maxHp);
    }
    public override void Recoverd(int amount)
    {
        base.Recoverd(amount);

        hpBarImage.fillAmount = (float)curHp / (float)maxHp;
        hpText.text = string.Format("{0}/{1}", curHp, maxHp);
    }
    protected override void Die()
    {
        BossStatus.SetActive(false);
        StopAllCoroutines();
        base.Die();
    }
    private void OnEnable()
    {
        hpBarImage.fillAmount = (float)curHp / (float)maxHp;
        hpText.text = string.Format("{0}/{1}", curHp, maxHp);
    }


}
