using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthControl : MonoBehaviour
{



    protected int curHp;
    protected float damageCoef;
    protected bool isDie=false;
    private Animator animator;
    public delegate void DieHandler();
    public event DieHandler OnDie;
    public int CurHp
    {
        get
        {
            return curHp;
        }
        set
        {
            curHp = value;
        }
    }
    public bool IsDie
    {
        get {
            return isDie;
        }
        set
        {
            isDie = value;
        }
    }
    public float DamageCoef
    {
        get
        {
            return damageCoef;
        }
        set
        {
            damageCoef = value;
        }
    }

    public int maxHp = 100;
    protected virtual void Awake()
    {
        damageCoef = 1f;
        curHp = maxHp;
        animator = GetComponent<Animator>();
    }
    public virtual void Damaged(int damage)
    {
        
        curHp -= (int)((float)damage* damageCoef);
        if(curHp<=0)
        {
            Die();
            return;
        }
    }
    protected virtual void Die()
    {
        IsDie = true;
        animator.SetTrigger("Die");
        OnDie();
    }
    public virtual  void Recoverd(int amount)
    {
        if (CurHp + amount >= maxHp)
        {
            CurHp = maxHp;
        }
        else
        {
            CurHp += amount;
        }
    }
    public virtual void Revive()
    {
        curHp = maxHp;
    }
    
    public float CurHpRatio()
    {
        return (float)curHp/(float)maxHp;

    }
    public IEnumerator Disappear(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }
}
