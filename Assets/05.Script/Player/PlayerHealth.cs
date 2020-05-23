using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : HealthControl
{
    private PlayerMovement movement;
    private WaitForSeconds ws;

    private int curGuardGauge;
    public int maxGuardGauge=500;
    public Image HpBar;
    public Image GuardBar;
    public Image BloodScreen;
    public Text hpText;
    public Text guardText;
    protected override void Awake()
    {
        base.Awake();
        curGuardGauge = maxGuardGauge;
        movement = GetComponent<PlayerMovement>();
        ws = new WaitForSeconds(0.1f);
        StartCoroutine(GuardGauge());
        hpText.text = string.Format("{0}/{1}", curHp.ToString(), maxHp.ToString());
        guardText.text = string.Format("{0}/{1}", curGuardGauge.ToString(), maxGuardGauge.ToString());
    }
    public override void Damaged(int damage)
    {
        if (!movement.Block)
        {
            curHp -= damage;
            if (CurHpRatio() < 0.2f)
            {
                BloodScreen.color = new Color(1, 0, 0, 0.25f);
            }
            else
            {
                BloodScreen.color = new Color(1, 0, 0, 0f);
            }
            HpBar.fillAmount = (float)curHp / (float)maxHp;
            if (curHp <= 0)
            {
                IsDie = true;
                return;
            }
        }
        else
        {
            if (curGuardGauge <= 0)
            {
                curHp -= damage;

                HpBar.fillAmount = (float)curHp / (float)maxHp;

                if (curHp <= 0)
                {
                    IsDie = true;
                    return;
                }
            }
            else
            {
                if (curGuardGauge - damage < 0)
                {
                    curGuardGauge = 0;
                    curHp-= (damage- curGuardGauge);
                    HpBar.fillAmount = (float)curHp / (float)maxHp;
                }
                else
                {
                    curGuardGauge -= damage;
                }

                GuardBar.fillAmount = (float)curGuardGauge / (float)maxGuardGauge;
            }
        }

        hpText.text = string.Format("{0}/{1}", curHp.ToString(), maxHp.ToString());
        guardText.text = string.Format("{0}/{1}", curGuardGauge.ToString(), maxGuardGauge.ToString());
    }
    IEnumerator GuardGauge()
    {
        while (!IsDie)
        {
            if(curGuardGauge< maxGuardGauge)
            {
                if (!movement.Block)
                {
                    curGuardGauge += 1;
                    GuardBar.fillAmount = (float)curGuardGauge / (float)maxGuardGauge;
                    guardText.text = string.Format("{0}/{1}", curGuardGauge.ToString(), maxGuardGauge.ToString());
                }
            }
            yield return ws;
        }

    }
}
