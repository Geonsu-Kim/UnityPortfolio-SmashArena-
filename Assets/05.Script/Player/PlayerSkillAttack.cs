using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CheckUsingCasting))]
public class PlayerSkillAttack : MonoBehaviour
{
    public Transform firePos;
    public GameObject[] skillEffect;
    public TrailRenderer bladeTrail;
    private PlayerMovement movement;
    public CheckUsingCasting check;

    private GameObject temp;
    public float width;
    public float length;
    public float height;
    // Start is called before the first frame update
    void Awake()
    {
        check = GetComponent<CheckUsingCasting>();
        movement = GetComponent<PlayerMovement>();
        bladeTrail.enabled = false;
        for(int i = 0; i < skillEffect.Length; i++)
        {
            skillEffect[i].GetComponent<ParticleSystem>().Stop();
        }

    }
    void Attack(AnimationEvent animationEvent)
    {

        switch (animationEvent.intParameter)
        {
            case 0:
                check.CheckBox(transform.position, new Vector2(1.5f, 3f), transform.forward, 1.5f, 1 << 9, 10);
                check.HitEffect(check.GetBoxHits(transform.position, new Vector2(1.5f, 3f), transform.forward, 1.5f, 1 << 9));
                break;
            case 1:
                check.CheckBox(transform.position, new Vector2(2f, 3f), transform.forward, 1.6f, 1 << 9, 15);
                check.HitEffect(check.GetBoxHits(transform.position, new Vector2(2f, 3f), transform.forward, 1.6f, 1 << 9));
                break;
            case 2:
                check.CheckBox(transform.position, new Vector2(2.5f, 3f), transform.forward, 1.8f, 1 << 9, 20);
                check.HitEffect(check.GetBoxHits(transform.position, new Vector2(2.5f, 3f), transform.forward, 1.8f, 1 << 9));
                break;
        }

    }
    public void TrailEffect(AnimationEvent animationEvent)
    {
        if (animationEvent.intParameter == 0)
        {
            bladeTrail.enabled = true;
        }
        else
        {
            bladeTrail.enabled = false;
        }
    }
    public void SkillEffect(AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter.Equals("On"))
        {
            switch (animationEvent.intParameter)
            {
                case 0:
                    check.CheckBoxAddForce(transform.position, new Vector2(6f, 3f), transform.forward, 3.5f, 1 << 9, 50,5);
                    check.HitEffect(check.GetBoxHits(transform.position, new Vector2(6f, 3f), transform.forward, 3.5f, 1 << 9));
                    skillEffect[0].GetComponent<ParticleSystem>().Play();
                    break;
                case 1:
                    if (animationEvent.floatParameter == 0)
                    {
                        skillEffect[1].GetComponent<ParticleSystem>().Play();
                        check.CheckBoxAddForce(transform.position, new Vector2(3f, 3f), transform.forward, 3.5f, 1 << 9, 10, 5f);
                        check.HitEffect(check.GetBoxHits(transform.position, new Vector2(3f, 3f), transform.forward, 3.5f, 1 << 9));
                    }
                    else if (animationEvent.floatParameter == 1)
                    {
                        check.CheckBoxAddForce(transform.position+transform.forward*3.5f, new Vector2(3f, 3f), transform.forward, 3.5f, 1 << 9, 10, 5f);

                        check.HitEffect(check.GetBoxHits(transform.position + transform.forward * 3.5f, new Vector2(3f, 3f), transform.forward, 3.5f,1<<9));
                    }
                    else if (animationEvent.floatParameter == 2)
                    {
                        skillEffect[1].GetComponent<ParticleSystem>().Play();

                        skillEffect[2].GetComponent<ParticleSystem>().Play();

                        skillEffect[3].GetComponent<ParticleSystem>().Play();

                        check.CheckBoxAddForce(transform.position, new Vector2(4.5f, 3f), transform.forward, 3.5f, 1 << 9, 30, 5f);
                        check.HitEffect(check.GetBoxHits(transform.position, new Vector2(4.5f, 3f), transform.forward, 3.5f, 1 << 9));

                    }
                    else if (animationEvent.floatParameter == 3)
                    {
                        check.CheckBoxAddForce(transform.position + transform.forward * 3.5f, new Vector2(9f, 3f), transform.forward, 3.5f, 1 << 9, 30, 5);
                        check.HitEffect(check.GetBoxHits(transform.position + transform.forward * 3.5f, new Vector2(9f, 3f), transform.forward, 3.5f, 1 << 9));
                    }
                    break;
                case 2:
                    if (animationEvent.floatParameter == 0)
                    {
                        var ms = skillEffect[4].GetComponent<ParticleSystem>().main;
                        ms.loop = true;
                        skillEffect[4].GetComponent<ParticleSystem>().Play();
                    }
                    else
                    {
                        check.CheckSphereAddForce(transform.position, 3f, 1 << 9, 10,0.25f);
                        check.HitEffect(check.GetSphereHits(transform.position, 3f, 1 << 9));
                    }
                    break;
            }
        }
        else
        {
            movement.IsUsingSkill = false;
            var ms = skillEffect[4].GetComponent<ParticleSystem>().main;
            ms.loop = false;
        }
        
    }


}
