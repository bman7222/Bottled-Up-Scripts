using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class enemyMovement : MonoBehaviour
{
    [System.Serializable]
    public enum enemyMoveType
    {
        Seek,
        Wander,
        Flee,
        Evade,
        Pursuit,
        Still,
    }

    public enemyMoveType moveType;

    private Character character;

    public Character.State state;

    [Tooltip("If the enemy movement is one that requires future prediction, this is the amount of time in the future that will be predicted. EX: If the target is moving with a velocity of 1 the predict will be 1*movementOffset")]
    public float movementOffset;

    [Tooltip("The speed of the enemy is O")]
    [SerializeField] private float speed = 10f;

    [Tooltip("The speed of the enemy Y rotation is O")]
    private float rotationSpeed = 10f;

    [Tooltip("The offset for the circle for wandering is O")]
    [SerializeField] private float wanderOffset = 5f;

    [Tooltip("The radius of the circle the point to wander to is O")]
    [SerializeField] private float wanderRadius = 2f;

    [Tooltip("The amount to multiply the randomness in the X and Z directions when enemy is selecting where to seek to when wandering is O")]
    private float wanderJit = 1f;

    [Tooltip("The amount of randomness in the X and Z directions when enemy is selecting where to seek to when wandering is O")]
    private float wanderRange = 0.5f;

    /*[Header("If the enemy is within range O of target it will begin to slow down and arrive")]
    [SerializeField] private float slowRadius = 0.5f;*/
    [Tooltip("The radius around the target that the enemy will seek")]
    [SerializeField] private float arrivalRadius = 0.5f;

    [Tooltip("If the enemy is within range O of target it will immediately stop movement")]
    [SerializeField] private float absoluteStopRadius = 0.56f;

    //public GameObject test;

    private GameObject target;

    public Vector3 posBeforeDash;

    private myTimescale myTime;

    private Vector3 initialPosition;

    private Vector3 initialRotation;

    public float attackRadius, dashSpeed;

    public bool attackOnCD;

    private HitResponder hitResponder;
    [SerializeField] private HurtResponder hurtResponder;

    private Rigidbody rb;

    //Animation stuff
    public bool defaultLooksLeft;
    public bool isAttacking;
    public GameObject attack3DObject;

    private Animator attack3DObjectAnimator;

    // Update is called once per frame

    public void setCharacter (Character c)
    {
        this.character = c;

        character = GetComponent<Character>();

        rb = character.getRigidBody();

        target = character.getTarget();

        try
        {
            attack3DObjectAnimator = attack3DObject.GetComponent<Animator>();
        }
        catch
        {

        }
    }

    public void handleMovement()
    {

        if (checkInAttackRange() && (moveType==enemyMoveType.Seek || moveType == enemyMoveType.Pursuit))
        {
            //put player into attack after time is up 

            frameAction a = new frameAction("enemyMovement: exitattackStartup", exitattackStartup);

            if (!character.timerInList(timer.timerType.prepareAttackTimer)) {

                character.createTimer(1f, a, timer.timerType.prepareAttackTimer);

            }

            //prepare for attack startup
            character.changeState(Character.State.PrepareEnemyAttackStartup);
        }
        else
        {
            if (!(moveType == enemyMoveType.Flee))
            {
                try
                {
                    character.getSpriteRendererAnim().SetTrigger("ReturnToIdle");

                    attack3DObjectAnimator.SetTrigger("ReturnToIdle");
                }
                catch
                {

                }
            }

            Vector3 targetPos;
            //Vector3 targetVelocity;
            targetPos = new Vector3(character.getTarget().gameObject.transform.position.x, 0f, character.getTarget().gameObject.transform.position.z);

            switch (moveType)
            {
                case enemyMoveType.Seek:
                    seek(targetPos);
                    break;
                case enemyMoveType.Wander:
                    wander();
                    break;
                case enemyMoveType.Flee:
                    flee(targetPos);
                    break;
                case enemyMoveType.Pursuit:
                    pursue(target.GetComponent<Character>().getRigidBody().velocity, targetPos, movementOffset);
                    break;
                case enemyMoveType.Evade:
                    evade(target.GetComponent<Character>().getRigidBody().velocity, targetPos, movementOffset);
                    break;
                case enemyMoveType.Still:
                    character.getRigidBody().velocity = Vector3.zero;
                    break;
            }
        }
    }

    public void handleCombatUpdate()
    {
        handleAttack();
    }

    bool checkInAttackRange()
    {
        Vector3 targetPos;
        //Vector3 targetVelocity;

        targetPos = new Vector3(character.getTarget().gameObject.transform.position.x, 0f, character.getTarget().gameObject.transform.position.z);

        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);

        float distance = (targetPos - pos).magnitude;

        //target is in range, prepare the attack
        if (distance <= attackRadius && !character.getAttackOnCD())
        {
            return true;
        }

        return false;
    }


    public void exitattackStartup()
    {

        Debug.Log("Time to dash");

        //1spawnHitbox();

        //Dash towards target 
        character.changeState(Character.State.Attack);

        frameAction a = new frameAction("enemy movement: character.exitAttack", character.exitAttack);

        character.createTimer(0.15f, a, timer.timerType.attackTimer);

        isAttacking = true;

        character.getSpriteRendererAnim().SetTrigger("AtkMain");

        //if (spriteRenderer.flipX) attack3DObject.transform.rotation =Quaternion.Euler(0f,0f,0f);
        //else attack3DObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f); //it's 180 by default

        attack3DObjectAnimator.SetTrigger("AtkMain");
    }

    void handleAttack()
    {

        if (character.getState() == Character.State.Attack || character.getState() == Character.State.Combo)
        {
            character.getRigidBody().velocity = posBeforeDash * dashSpeed;
        }

    }

    Vector3 wander()
    {
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //Offset target position by current position
        Vector3 targetPos = velocity;
        targetPos -= (pos * wanderOffset);

        Vector3 wanderZone = new Vector3(UnityEngine.Random.Range(-wanderRange, wanderRange) * wanderJit, 0, (UnityEngine.Random.Range(-wanderRange, wanderRange) * wanderJit));

        wanderZone.Normalize();

        wanderZone *= wanderRadius;

        Vector3 dir = wanderZone + pos + (transform.forward * wanderOffset);

        Vector3 worldDir = transform.InverseTransformVector(dir);

        /*Vector3 localTarget = wanderZone +  pos + new Vector3(0, 0, wanderOffset);

        Vector3 worldTarget = transform.InverseTransformVector(localTarget);*/

        targetPos = dir;

        //wanderTarget.transform.position = targetPos;

        return targetPos;
    }

    void flee(Vector3 targetPos)
    {
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //find desired direction
        Vector3 desiredVelocity = pos - targetPos;
        //normalize desired direction then multiply acceleration
        desiredVelocity.Normalize();
        desiredVelocity *= speed;

        //Pull down desired direction by current velocity
        Vector3 steering = (desiredVelocity - velocity);

        rb.AddForce(steering);

        //rotate
        //Quaternion rotateTo = Quaternion.LookRotation(rb.velocity, Vector3.up);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTo, rotationSpeed);
        if (defaultLooksLeft && !isAttacking)
        {
            if (target.transform.position.x > gameObject.transform.position.x)
            {
                character.getSpriteRenderer().flipX = true;
                //attack3DObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                attack3DObject.transform.localPosition = new Vector3(1.64999962f, 0.829999924f, 0f);

            }
            else if (target.transform.position.x < gameObject.transform.position.x)
            {
                character.getSpriteRenderer().flipX = false;
                attack3DObject.transform.localPosition = new Vector3(-1.64999962f, 0.829999924f, 0f);
                //attack3DObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f); //it's 180 by default on the stress enemy
            }
        }
        else if (!isAttacking)
        {
            if (target.transform.position.x > gameObject.transform.position.x)
            {
                character.getSpriteRenderer().flipX = false;
                attack3DObject.transform.localPosition = new Vector3(-1.64999962f, 0.829999924f, 0f);
                //attack3DObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f); //it's 180 by default on the stress enemy
            }
            else if (target.transform.position.x < gameObject.transform.position.x)
            {
                character.getSpriteRenderer().flipX = true;
                attack3DObject.transform.localPosition = new Vector3(1.64999962f, 0.829999924f, 0f);
                //attack3DObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

    }



    void evade(Vector3 targetVelocity, Vector3 targetPos, float offset)
    {

        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 targetV = targetVelocity * offset;

        targetV += targetPos;

        //evadeTarget.transform.position = targetV;

        Vector3 desiredVelocity = pos - targetV;
        desiredVelocity.Normalize();
        desiredVelocity *= speed;

        //Pull down desired direction by current velocity
        Vector3 steering = (desiredVelocity - velocity);

        rb.AddForce(steering);

        //rotate
        //Quaternion rotateTo = Quaternion.LookRotation(rb.velocity, Vector3.up);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTo, rotationSpeed);
    }

    void pursue(Vector3 targetVelocity, Vector3 targetPos, float offset)
    {

        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 targetV = targetVelocity * offset;

        targetV += targetPos;

        //evadeTarget.transform.position = targetV;

        Vector3 desiredVelocity = targetV - pos;
        desiredVelocity.Normalize();
        desiredVelocity *= speed;

        //Pull down desired direction by current velocity
        Vector3 steering = (desiredVelocity - velocity);

        rb.AddForce(steering);

        //rotate
        //Quaternion rotateTo = Quaternion.LookRotation(rb.velocity, Vector3.up);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTo, rotationSpeed);

        if (defaultLooksLeft && !isAttacking)
        {
            if (target.transform.position.x > gameObject.transform.position.x)
            {
                character.getSpriteRenderer().flipX = true;
                attack3DObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            }
            else if (target.transform.position.x < gameObject.transform.position.x)
            {
                character.getSpriteRenderer().flipX = false;
                attack3DObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f); //it's 180 by default on the stress enemy
            }
        }
        else if (!isAttacking)
        {
            if (target.transform.position.x > gameObject.transform.position.x)
            {
                character.getSpriteRenderer().flipX = false;
                attack3DObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f); //it's 180 by default on the stress enemy
            }
            else if (target.transform.position.x < gameObject.transform.position.x)
            {
                character.getSpriteRenderer().flipX = true;
                attack3DObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

    }

    void seek(Vector3 targetPos)
    {

        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //find desired direction
        Vector3 desiredVelocity = targetPos - pos;
        //normalize desired direction then multiply acceleration
        desiredVelocity.Normalize();
        desiredVelocity *= speed;

        //Pull down desired direction by current velocity
        Vector3 steering = (desiredVelocity - velocity);

        //get offset, get ramped speed choose min between max speed and clipped speed
        Vector3 targetOffset = pos - targetPos;

        float mag = Mathf.Sqrt(Mathf.Pow(targetOffset.x, 2) + Mathf.Pow(targetOffset.z, 2));

        Vector3 closestRadiusPoint = targetPos + targetOffset / mag * arrivalRadius;
        //Debug.Log(targetOffset + " " + mag + " " + targetOffset / mag);

        //test.transform.position = closestRadiusPoint;

        targetOffset = closestRadiusPoint - pos;

        float dis = targetOffset.magnitude;


        if (dis <= absoluteStopRadius)
        {
            //Debug.Log("ARRIVED");
            rb.velocity = Vector3.zero;
            steering = Vector3.zero;
        }



        //PHYSICS VERSION
        //Debug.Log("STEER: " + steering);
        rb.AddForce(steering);

        //NON-PHYSICS VERSION
        //transform.position = Vector3.MoveTowards(pos,closestRadiusPoint,speed * Time.fixedDeltaTime);

        /*
        if (rb.velocity != Vector3.zero)
        {
            Quaternion rotateTo = Quaternion.LookRotation(rb.velocity, Vector3.up);
            //Quaternion rotateTo = Quaternion.LookRotation(rb.velocity, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTo, rotationSpeed);

        }*/


        //sprite flip
        //print("checking for flip!");
        //print("Target X: " + target.transform.position.x + " My X: " + gameObject.transform.position.x);
        if (defaultLooksLeft && !isAttacking) {
            if (target.transform.position.x > gameObject.transform.position.x)
            {
                character.getSpriteRenderer().flipX = true;
                attack3DObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                
            }
            else if (target.transform.position.x < gameObject.transform.position.x)
            {
                character.getSpriteRenderer().flipX = false;
                attack3DObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f); //it's 180 by default on the stress enemy
            }
        }
        else if (!isAttacking)
        {
            if (target.transform.position.x > gameObject.transform.position.x)
            {
                character.getSpriteRenderer().flipX = false;
                attack3DObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f); //it's 180 by default on the stress enemy
            }
            else if (target.transform.position.x < gameObject.transform.position.x)
            {
                character.getSpriteRenderer().flipX = true;
                attack3DObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }


        //Debug.Log("MY V: " + character.getRigidBody().velocity);

    }

    /* public void spawnHitbox()
     {
         HitBox attack = Instantiate(attackArray[0], Vector3.zero, Quaternion.identity);
         attack.transform.SetParent(transform);
         attack.transform.position = transform.TransformPoint(Vector3.zero);

     }*/


}
