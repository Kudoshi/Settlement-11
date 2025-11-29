using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    List<string> attack_anim = new List<string> {"attack1", "attack2"};
    public Animator animator;
    public int comboCount;
    public float reset;
    public float resetTime;

    // Update is called once per frame
    void Update()
    {
        // Player Normal Attack Combo
        PlayerNormalAttack();

        // Player Skill Attacks
        PlayerSkillAttack();
    }

    void PlayerNormalAttack()
    {
        if (Input.GetButtonDown("Fire1") && comboCount < 2)
        {
            Debug.Log("update");
            animator.SetTrigger(attack_anim[comboCount]);
            comboCount++;
            reset = 0f;
        }
        if (comboCount > 0)
        {
            reset += Time.deltaTime;
            if (reset > resetTime)
            {
                animator.SetTrigger("reset");
                comboCount = 0;
            }
        }
        if (comboCount == 2)
        {
            resetTime = 2f;
            comboCount = 0;
        }
        else
        {
            resetTime = 1f;
        }
    }

    void PlayerSkillAttack()
    {
        if (Input.GetKeyDown("f"))
        {
            animator.SetTrigger("skill_attack");
        }
    }
}
