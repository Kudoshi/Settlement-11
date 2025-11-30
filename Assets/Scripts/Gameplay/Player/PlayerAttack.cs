using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerAttack : MonoBehaviour
{
    // Testing attack behavior
    [Header("Attacking")]
    public Animator animator;
    //public Camera playerCamera;
    //public float attackDistance = 3f;
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public BoxCollider attackCollider;
    //public LayerMask attackLayer;

    public GameObject hitEffect;
    //... Audio here

    bool isAttacking = false;
    bool readyToAttack = true;
    int attackCount;

    // Animations
    public const string ATTACK1 = "Attack1";
    public const string ATTACK2 = "Attack2";
    public const string SKILL_ATTACK = "SkillAttack";
    public const string IDLE = "Idle";

    string currentAnimationState;

    private void Start()
    {
        // Disable attack collider at start
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    public void Attack()
    {
        if (!readyToAttack || isAttacking) return;

        readyToAttack = false;
        isAttacking = true;

        attackCollider.enabled = true;

        Invoke(nameof(ResetAttack), attackSpeed);

        SetAnimations();

        if (attackCount == 0)
        {
            ChangeAnimationState(ATTACK1);
            attackCount++;
        }
        else
        {
            ChangeAnimationState(ATTACK2);
            attackCount = 0;
        }
    }

    private void SetAnimations()
    {
        if (!isAttacking)
        {
            attackCollider.enabled = false;
            ChangeAnimationState(IDLE);
        }
    }

    public void ChangeAnimationState(string newState)
    {
        // stop same animation from interrupting itself
        if (currentAnimationState == newState) return;

        // play the animation
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }


    void Update()
    {
        // Attack input
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
        // Skill attack input
        if (Input.GetKeyDown("q"))
        {
            SkillAttack();
        }
    }

    private void SkillAttack()
    {
        if (!readyToAttack || isAttacking) return;

        readyToAttack = false;
        isAttacking = true;

        attackCollider.enabled = true;

        // Fire fly should be add as condition here later
        // skill will be fix when fire fly meter is done
        Invoke(nameof(ResetAttack), attackSpeed);

        ChangeAnimationState(SKILL_ATTACK);
        attackCount = 0;
    }

    void ResetAttack()
    {
        Debug.Log("Attack Reset");
        isAttacking = false;
        readyToAttack = true;
        attackCollider.enabled = false;
    }

    void AttackRaycast()
    {
        //if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
        //{
        //    HitTarget(hit.point);

        //    if (hit.transform.TryGetComponent<Testing_EnemyHealth>(out Testing_EnemyHealth enemy))
        //    {
        //        enemy.TakeDamage();
        //    }
        //}
    }

    void HitTarget(Vector3 pos)
    {
        Debug.Log("Hit at " + pos);
        // Audio here...

        // VFX...
    }

}
