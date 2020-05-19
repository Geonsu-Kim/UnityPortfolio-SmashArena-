using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator avatar;

    public Image[] skillImage;
    public Text[] skillCoolTimeText;
    public int[] skillCoolTime;
    public float speed = 2f;

    private bool isUsingNormalAttack = false;
    private bool isBlocking = false;
    private bool isUsingSkill = false;
    public bool IsUsingSkill
    {
        set
        {
            isUsingSkill = value;
        }
        get
        {
            return isUsingSkill;
        }
    }
    public bool Block
    {
        set
        {
            isBlocking = value;
        }
        get
        {
            return isBlocking;
        }
    }

    private bool[] skillCoolDown;
    private float normalAttackTime;
    private float[] curSkillCoolTime;
    private Vector3 direction;
    WaitForSeconds ws = new WaitForSeconds(0.5f);
    WaitForSeconds ws_coolTimeInterval = new WaitForSeconds(0.1f);

    WaitForSeconds Skill3 = new WaitForSeconds(4f);
    private void Awake()
    {
        for (int i = 0; i < 3; i++) { skillCoolTimeText[i].enabled = false ; }
    }
    void Start()
    {
        avatar = GetComponent<Animator>();
        skillCoolDown = new bool[3];
        curSkillCoolTime = new float[3];
        for (int i = 0; i < 3; i++) { skillCoolDown[i] = false; }
    }

    float h, v;

    public void OnStickChanged(Vector2 stickPos)
    {
        h = stickPos.x;
        v = stickPos.y;
    }
    // Update is called once per frame
    void Update()
    {
        if (avatar)
        {
            if (!isUsingNormalAttack && !isBlocking&& !isUsingSkill)
            {
                avatar.SetFloat("Speed", (h * h + v * v));
                direction = h * Vector3.right + v * Vector3.forward;
                if (h * h + v * v >= 0.1f)
                {
                    transform.Translate(direction * Time.deltaTime * speed, Space.World);
                }
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }
    }

    public void NormalAttackBtnDown()
    {

        isUsingNormalAttack = true;
        StartCoroutine(NormalAttackCombo());
       
    }
    public void NormalAttackBtnUp()
    {
        if (isUsingNormalAttack)
        {
            isUsingNormalAttack = false;
        }
    }
        IEnumerator NormalAttackCombo()
        {

            if (Time.time - normalAttackTime > 0.25f)
            {
                normalAttackTime = Time.time;
                while (isUsingNormalAttack)
                {
                    avatar.SetBool("NormalAttack", true);
                    yield return ws;
                }
                avatar.SetBool("NormalAttack", false);
            }
        }
     public void Skill(int idx)
     {
        if (!skillCoolDown[idx])
        {
            skillCoolDown[idx] = true;
            isUsingSkill = true;
            avatar.SetTrigger("Skill");
            switch (idx)
            {
                case 0:
                    avatar.SetInteger("SkillIdx",0);
                    break;

                case 1:
                    avatar.SetInteger("SkillIdx", 1);
                    break;

                case 2:
                    isUsingSkill = false;
                    avatar.SetBool("Skill3_Spin", true);
                    avatar.SetInteger("SkillIdx", 2);
                    StartCoroutine(Skill3SpinAttack());
                    break;
            }
            StartCoroutine(SkillCoolDown(idx));
            StartCoroutine(CoolTimeCounter(idx));
        }
    }
    IEnumerator Skill3SpinAttack()
    {
        yield return Skill3;
        avatar.SetBool("Skill3_Spin", false);
    }
    IEnumerator SkillCoolDown(int idx)
    {
        curSkillCoolTime[idx] = 0;
        skillImage[idx].fillAmount = 0;
        while (skillCoolTime[idx] - curSkillCoolTime[idx] >= 0)
        {
            curSkillCoolTime[idx] += Time.smoothDeltaTime;
            skillImage[idx].fillAmount = curSkillCoolTime[idx] / skillCoolTime[idx];
            yield return null;
        }
        skillCoolDown[idx] = false;
        yield  break;
    }
    IEnumerator CoolTimeCounter(int idx)
    {
        skillCoolTimeText[idx].enabled = true ;
        while (skillCoolTime[idx] - curSkillCoolTime[idx] >= 0)
        {
            yield return ws_coolTimeInterval;
            skillCoolTimeText[idx].text = (skillCoolTime[idx] - curSkillCoolTime[idx]).ToString("N1");
        }
        skillCoolTimeText[idx].enabled = false;
        yield break;
    }


    public void BlockDown()
    {
        isBlocking = true;
        avatar.SetBool("Block",true);
    }
    public void BlockUp()
    {
        isBlocking = false;
        avatar.SetBool("Block", false);
    }

}
