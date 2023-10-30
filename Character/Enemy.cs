using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [Space(2f)]
    [Header("Enemy Move")]
    [Tooltip("The movement type of the enemy")]
    public enemyMovement.enemyMoveType moveType;

    [Tooltip("If the enemy movement is one that requires future prediction, this is the amount of time in the future that will be predicted. EX: If the target is moving with a velocity of 1 the predict will be 1*movementOffset")]
    public float movementOffset;

    [Tooltip("Time between enemy attacks")]
    public float attackCooldownTime;

    //public Animator spriteAnimator;
    public Animator attack3DAnimator;
    //public SpriteRenderer spriteRenderer;
    //public GameObject attack3DObject;

    protected override void Awake()
    {
        base.Awake();

        //enemy 
        enemyMove = GetComponent<enemyMovement>();

        target = FindObjectOfType<Player>().gameObject;

        enemyMove.setCharacter(this);

        enemyMove.moveType = moveType;

        enemyMove.movementOffset = movementOffset;

        switch (enemyMove.moveType)
        {
            case enemyMovement.enemyMoveType.Seek:
                shouldMove = true;
                break;
            case enemyMovement.enemyMoveType.Wander:
                shouldMove = true;
                break;
            case enemyMovement.enemyMoveType.Flee:
                shouldMove = true;
                break;
            case enemyMovement.enemyMoveType.Pursuit:
                shouldMove = true;
                break;
            case enemyMovement.enemyMoveType.Evade:
                shouldMove = true;
                break;
            case enemyMovement.enemyMoveType.Still:
                shouldMove = true;
                break;
        }

        groundPos = 0.55f;

    }

    // Start is called before the first frame update
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void handleMovement()
    {
        enemyMove.handleMovement();
    }


    public override void handleCombatUpdate()
    {
        base.handleCombatUpdate();

        enemyMove.handleCombatUpdate();
    }

    public override void handleBeingHurt()
    {

        setAttackOnCD(false);

        if (myTime.timerInList(timer.timerType.attackCooldownTimer))
        {
            myTime.setTimerInList(timer.timerType.attackCooldownTimer, -1f);
            myTime.removeTimer(timer.timerType.attackCooldownTimer);
        }

        //set timers to 0
        if (myTime.timerInList(timer.timerType.prepareAttackTimer))
        {
            myTime.setTimerInList(timer.timerType.prepareAttackTimer, -1f);
            myTime.removeTimer(timer.timerType.prepareAttackTimer);
        }

        if (myTime.timerInList(timer.timerType.attackTimer))
        {
            myTime.setTimerInList(timer.timerType.attackTimer, -1f);
            myTime.removeTimer(timer.timerType.attackTimer);
        }
    }

    public override void handleExitHurt()
    {
        Debug.Log("IM DONE BEING HURT");
    }

    public override void handleAttackStartup()
    {
        //spriteAnimator.SetTrigger(0);
        theSRAnim.SetTrigger("AtkStart");
        //print("SR Anim " + theSRAnim.name + " just triggered 0!");
        attack3DAnimator.SetTrigger("AtkStart");

       // gameObject.GetComponent<enemyMovement>().isAttacking = true;
        //if (spriteRenderer.flipX) attack3DObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        //else attack3DObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f); //it's 180 by default

        Vector3 targetPos = new Vector3(target.gameObject.transform.position.x, 0f, target.gameObject.transform.position.z);
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);

        enemyMove.posBeforeDash = (targetPos - pos).normalized;
    }

    public override void enterHitStop()
    {
        rb.velocity = Vector3.zero;
        //characterObjectAnim.speed = 0f;
        changeState(Character.State.HitStop);
    }

    public override void enterCombo()
    {
        //theSRAnim.speed = 1f;
        //characterObjectAnim.speed = 1f;
        changeState(Character.State.Combo);
    }


    public override void exitAttack()
    {
        base.exitAttack();

        clearHitboxDisplay();

        refreshHitList();

        attackOnCooldown();

        stopMovement();

        Action theAction = () => attackOffCooldown();

        frameAction a = new frameAction("Enemy: attackOffCooldown", theAction);

        myTime.createTimer(1f, a, timer.timerType.attackCooldownTimer);

        gameObject.GetComponent<enemyMovement>().isAttacking = false;
    }


}
