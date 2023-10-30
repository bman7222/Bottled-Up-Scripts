using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Space(2f)]
    [Header("Attacking")]

    [Tooltip("The index of the bunch holder to use in bunch holder List attacks")]
    [SerializeField] protected int attackToUse;

    [Tooltip("The List of bunch holders this character can attack with")]
    [SerializeField] protected List<BunchHolder> attacks;

    protected Rigidbody rb;

    [Space(2f)]
    [Header("Sprite and Animation")]

    [Tooltip("The animator for the player sprite")]
    [SerializeField] protected Animator theSRAnim;

    [Tooltip("The sprite renderer player sprite")]
    [SerializeField] protected SpriteRenderer theSR;

    [Tooltip("Determines if character should move during attacks")]
    [SerializeField] protected bool shouldMove;

    public GameObject damagePopUpPrefab;

    //The animator for the player sprite


    public enum State
    {
        Idle,
        HitStun,
        EnterHitStun,
        Vibrate,
        EnterVibrate,
        HitStop,
        Attack,
        Seek,
        Combo,
        Knockback,
        PrepareEnemyAttackStartup,
        EnemyAttackStartup,
        Dashing,
        RecoverFromHit,
    }

    [Space(2f)]
    [Header("State")]
    public State state;

    //Hit and hurt


    [Space(2f)]
    [Header("Hit Responder and Hit List")]
    protected HitResponder hitResponder;

    protected HurtResponder hurtResponder;

    protected bool canMove,attackOnCD;

    //player  

    protected PlayerCombatControls playerCombat;

    protected PlayerMovementScript playerMovement;

    protected revMeterHandler revHandler;

    //vibration

    protected Vector3 initialPosition;

    protected Vector3 initialRotation;

    //enemy

    protected GameObject target;

    protected enemyMovement enemyMove;

    //other

    [Space(2f)]
    [Header("Timescale")]

    [SerializeField] protected myTimescale myTime;

    protected Animator characterObjectAnim;
    public Animator backupAnimator;

    protected float groundPos;

    //Bunches and Hitboxes

    protected BunchHolderParser bunchParser;

    protected float thicknessOfBoxcastSweep = 0.025f;

    //Gizmos hitbox display

    protected List<HitBox> hitBoxToDisplay;

    protected bool shouldDisplayHitboxes; 

    // Start is called before the first frame update
    protected virtual void Awake()
    {

        rb = GetComponent<Rigidbody>();

        state = Character.State.Idle;

        hitResponder = new HitResponder(this);

        hurtResponder = new HurtResponder(this);

        myTime = new myTimescale(this);

        bunchParser = new BunchHolderParser(this);

        if (GetComponent<Animator>())
        {
            characterObjectAnim = GetComponent<Animator>();
            backupAnimator = characterObjectAnim;

        }
        else
            characterObjectAnim = backupAnimator;

        setAttackHitResponders();

        hitBoxToDisplay = new List<HitBox>();
       
    }

    protected virtual void Update()
    {
        myTime.tickUpdate();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {

        myTime.tickFixedUpdate();

        switch (state)
        {
            case Character.State.Idle:
                refreshHitList();

                groundCharacter();

                allowMovement();

                handleMovement();

                handleCombatUpdate();

                break;

            case Character.State.HitStop:

                stopMovement();

                handleCombatUpdate();
                
                break;

            case Character.State.EnterHitStun:

                handleExitHurt();

                changeState(Character.State.HitStun);
                
                break;

            case Character.State.HitStun:

                break;

            case Character.State.Attack:

                if (!shouldMove) stopMovement();
                
                handleCombatUpdate();
                
                break;
            case Character.State.Combo:

                if (!shouldMove) stopMovement();

                handleCombatUpdate();

                break;

            case Character.State.PrepareEnemyAttackStartup:

                stopMovement();

                initialPosition = transform.localPosition;

                initialRotation = transform.eulerAngles;

                changeState(Character.State.EnemyAttackStartup);

                break;

            case Character.State.EnemyAttackStartup:

                vibrate(transform.forward, 0.2f);

                handleAttackStartup();

                break;

            case Character.State.EnterVibrate:

                stopMovement();

                initialPosition = transform.localPosition;

                initialRotation = transform.eulerAngles;

                handleBeingHurt();

                changeState(Character.State.Vibrate);

                break;

            case Character.State.Vibrate:

                vibrate(transform.forward, 0.5f);

                break;

        }
    }

    public virtual void handleMovement()
    {
        return;
    }

    public virtual void handleCombatUpdate()
    {
        //Do casting 
        if(myTime.timerInList(timer.timerType.attackTimer))
        {
            parseBunchHolder(myTime.getTimer(timer.timerType.attackTimer).getCurrentFrame(),attacks[attackToUse]);
        }

        return;
    }

    public virtual void handleAttackStartup() 
    {
        return;       
    }

    public void stopMovement()
    {
        rb.velocity = Vector3.zero;
        canMove = false;
    }

    public virtual void allowMovement()
    {
        canMove = true;
    }

    public bool getCanMove()
    {
        return canMove;
    }

    public void buildRevMeter()
    {
          revHandler.BuildRevMeter();
    }

    public void playRevEffect()
    {
        revHandler.playRevEffect();
    }

    public void addToTimerInList(timer.timerType name, float time)
    {

        myTime.addToTimerInList(name, time);
    }

    public void addParryVal()
    {
        revHandler.addParryVal();
    }

    public void setRevCost()
    {
        revHandler.setRevCost();
    }

    public bool checkRevAttackModifier()
    {
        return revHandler.checkRevAttackModifier();
    } 

    public void groundCharacter()
    {
        //transform.position = new Vector3(transform.position.x, groundPos, transform.position.z);
        return;
    }

    public State getState()
    {
        return state;
    }

    public Animator getSpriteRendererAnim()
    {
        return theSRAnim;
    }

    public SpriteRenderer getSpriteRenderer()
    {
        return theSR;
    }

    public Animator getCharacterAnim()
    {
        return characterObjectAnim;
    }

    public HitResponder getHitResponder()
    {
        return hitResponder;
    }

    public bool getAttackOnCD()
    {
        return attackOnCD;
    }

    public void setAttackOnCD(bool boolToSetTo)
    {
        attackOnCD = boolToSetTo;
    }

    public void attackOnCooldown()
    {
        attackOnCD = true;
    }

    public void attackOnCooldownTimer()
    {
        attackOnCD = true;

        frameAction a = new frameAction("Character: attackOffCooldown", attackOffCooldown);

        myTime.createTimer(1f, a ,timer.timerType.attackCooldownTimer);
    }

    public void attackOffCooldown()
    {
        attackOnCD = false;
    }

    public HurtResponder getHurtResponder()
    {
        return hurtResponder;
    }


    public void vibrate(Vector3 direction, float amplitude)
    {
        transform.localPosition = initialPosition + UnityEngine.Random.insideUnitSphere * amplitude;
    }


    public virtual void handleBeingHurt()
    {
        clearHitboxDisplay();

        refreshHitList();
        
        theSRAnim.SetBool("isMoving", false);
        
        theSRAnim.SetTrigger("isHurt");

        Debug.Log("IM HURT " + gameObject.name);

        theSRAnim.speed = 1f;

        characterObjectAnim.speed = 1f;

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

    public virtual void handleExitHurt()
    {
        theSRAnim.speed = 1f;
        characterObjectAnim.speed = 1f;
        Debug.Log("IM DONE BEING HURT");
    }

    //TIMERS //
    public float getMyPhysicsTime()
    {
        return myTime.myPhysicsTime;
    }

    public GameObject getTarget()
    {
        return target;
    }

    public timer getTimer(timer.timerType name)
    {
        return myTime.getTimer(name);
    }

    public void removeTimer(timer.timerType name)
    {
        myTime.removeTimer(name);
    }

    public bool timerInList(timer.timerType name)
    {
        return myTime.timerInList(name);
    }

    public void setTimerInList(timer.timerType name, float time)
    {
         myTime.setTimerInList(name, time);
    }

    public void addCurrentActionsToExecute(timer.timerType name, Dictionary<int, List<frameAction>> actions)
    {
        myTime.addCurrentActionsToExecute(name,actions);
    }

    public void createTimer(float time, frameAction timerFinish, timer.timerType name, Dictionary<int, List<frameAction>> executionActions)
    {
        myTime.createTimer(time, timerFinish, name, executionActions);
    }

    public void createTimer(float time, frameAction timerFinish, timer.timerType name)
    {
        myTime.createTimer(time, timerFinish, name);
    }

    public void createTimer(timer.timerType target, frameAction timerStart, float time, frameAction timerFinish, timer.timerType name)
    {
        myTime.createTimer(target, timerStart, time, timerFinish, name);
    }

    public void createTimer(frameAction timerCall, float time, frameAction timerFinishHitData, HitData hitData, timer.timerType name)
    {
        myTime.createTimer(timerCall, time, timerFinishHitData, hitData, name);
    }

    public virtual void  exitAttack()
    {
            Debug.Log("attackDone");

            changeState(Character.State.Idle);

            //attackOnCooldownTimer();

    }
    //State change

    public virtual void enterHitStop()
    {
        Debug.Log("Enter Hit stop");

        rb.velocity = Vector3.zero;

        theSRAnim.speed = 0f;

        characterObjectAnim.speed = 0f;

        changeState(Character.State.HitStop);
    }

    public void exitHitStop()
    {
        if (getTimer(timer.timerType.attackTimer) != null && getTimer(timer.timerType.hitStopTimer) != null)
        {
            //Debug.Log("TIME: " + getTimer("attackTimer").getTime() + " HITSTOP REMAIN: " + getTimer("hitStopTimer").getTime());
            
            getTimer(timer.timerType.attackTimer).subtractTime(getTimer(timer.timerType.hitStopTimer).getTimeRemaining());

            //Debug.Log("TIME REMAINING: " + getTimer("attackTimer").getTime());

            theSRAnim.speed = 1f;

            characterObjectAnim.speed = 1f;

            enterCombo();

            Debug.Log("Hit Stop finished.");
        }

    }

    public virtual void enterCombo()
    {
        theSRAnim.speed = 1f;
        characterObjectAnim.speed = 1f;
        changeState(Character.State.Combo);
    }

    public void enterVibrate()
    {
        changeState(Character.State.EnterVibrate);
    }

    public void exitVibrate(HitData hitData)
    {
        Debug.Log("Exit Vibrate");

        stopMovement();

        changeState(Character.State.EnterHitStun, hitData);

        frameAction a = new frameAction("Character: exitHitStun", exitHitStun);

        if (myTime.timerInList(timer.timerType.hitStunTimer))
        {
            myTime.removeTimer(timer.timerType.hitStunTimer);
        }

        createTimer(hitData.hitBox.hitStun, a, timer.timerType.hitStunTimer);
        


    }

    public void exitHitStun()
    {
        Debug.Log("Exit Hitstun");

        changeState(Character.State.Idle);

    }

    //function to despawn hit boxes
    public void refreshHitList()
    {
        //clear history of previously hit objects 
        hitResponder.refreshHitList();
    }


    //RB

    public Rigidbody getRigidBody()
    {
        return rb;
    }

    //STATES

    public void changeState(Character.State newState)
    {
        exitState(state);

        state = newState;

        enterState(state);
    }

    void enterState(Character.State newState)
    {
        if (newState == Character.State.Idle)
        {
            Debug.Log("ENTERING IDLE " + transform.position);
        }
    }

    public void changeState(Character.State newState, HitData hitData)
    {

        exitState(state);

        state = newState;

        enterState(state, hitData);


    }

    public virtual void exitState(Character.State oldState)
    {
        if (oldState == Character.State.Vibrate || oldState == Character.State.EnemyAttackStartup)
        {
            transform.localPosition = initialPosition;
            transform.eulerAngles = new Vector3(0f, initialRotation.y, 0f);
        }

        if (oldState == State.Attack || oldState == State.Combo || oldState == State.HitStop)
        {
            clearHitboxDisplay();

        }

    }

    void enterState(Character.State newState, HitData hitData)
    {
        if (newState == Character.State.EnterHitStun)
        {

            //Vector3 knockbackDirection = hitData.hurtBox.owner.transform.position - hitData.hitBox.hitResponder.gameObject.transform.position;

            //Debug.Log("HIT POINT: B" + hitData.hitPoint);

            Vector3 knockbackDirection = (transform.position - hitData.hitBox.hitResponder.getCharacter().transform.position).normalized;

            //Vector3 knockbackDirection = (transform.position - hitData.hitPoint).normalized;

            //Debug.Log("HIT POINT: A"+knockbackDirection);

            knockbackDirection.y = hitData.hitBox.knockbackAngle;

            //Debug.Log("KNOCKBACK: " + knockbackDirection * hitData.hitBox.knockbackForce);
            rb.AddForce(knockbackDirection * hitData.hitBox.knockbackForce, ForceMode.Impulse);

        }
    }

    void parseBunchHolder(int currentFrame, BunchHolder bunches)
    {
        bunchParser.parseBunchHolder(currentFrame, bunches);
    }

    void setAttackHitResponders()
    {
        foreach(BunchHolder bh in attacks)
        {
            foreach(Bunch b in bh.bunches)
            {
                foreach(HitBox hb in b.hitBoxList)
                {
                    hb.hitResponder = hitResponder;
                }
            }
        }
    }

    public void setAttackToUse(int attackIndex)
    {
        attackToUse = attackIndex;
    }
    

    public void castForHurtBoxes(Bunch b)
    {
        

        foreach (HitBox hb in b.hitBoxList)
        {
            //for gizmos
            hitBoxToDisplay.Add(hb);

            

            Vector3 _scaledSize = hb.hitBoxSize;

            HitData hitData = null;

            HurtBox hurtBoxCollidedWith = null;

            //BoxcastAll (center, half extents, direction, orientation, distance, layer mask)
            //halfExtents Half the size of the box in each dimension.
            //direction The direction in which to cast the box.
            //orientation Rotation of the box.
            //maxDistance The max length of the cast.
            //layermask A Layer mask that is used to selectively ignore colliders when casting a capsule.

            //do a box cast, start is where the box will begin, half extents is the scaling of the box (EX: 1,1,1 is a 1 by 1 by 1), orientation is the rotation of the box,
            //distance is the distance it will travel, layer mask is which layers will register a valid hit

            float summonX = hb.summonOffset.x;
            try
            {
                if (theSR.flipX)
                {
                    summonX *= -1;
                }
            }
            catch {  }

            //Debug.Log("character position: " + transform.position);
            //Debug.Log("summon offset: " + hb.summonOffset);

            RaycastHit[] objectsHitByBoxcast = Physics.BoxCastAll(

                transform.TransformPoint(summonX, hb.summonOffset.y, hb.summonOffset.z), //center

                //transform.TransformPoint(hb.summonOffset) - transform.up * ((_scaledSize.y - thicknessOfBoxcastSweep) / 2), //center

                new Vector3(_scaledSize.x, thicknessOfBoxcastSweep, _scaledSize.z) / 2, // half extents 

                transform.up, // direction

                transform.rotation, //rotation 

                 _scaledSize.y - thicknessOfBoxcastSweep, //distance of the cast 

                hb.m_layermask // layer mask 
                );

            /*
                        Gizmos.color = Color.red;

                        Gizmos.DrawCube(hb.summonOffset, hb.hitBoxSize);*/

            //For each hit detected
            foreach (RaycastHit objectHit in objectsHitByBoxcast)
            {

                hurtBoxCollidedWith = objectHit.collider.GetComponent<HurtBox>();

                if (hurtBoxCollidedWith != null && hurtBoxCollidedWith.active)
                {
                    //Make new hit data, which stores the damage, hitbox, hurtbox, etc of the boxcast 
                    int bonusDmg = hb.damage;
                    if (this is Player)
                    {
                        bonusDmg = (int)(hb.damage * this.playerCombat.dmgModifier);
                    }
                    hitData = new HitData
                    {
                        damage = bonusDmg,

                        hurtBox = hurtBoxCollidedWith,

                        hitBox = hb,

                        hitPoint = objectHit.point
                    };
                    Debug.Log(hitData.hurtBox);
                    

                    //If the hit data is valid call the response of the hit responder and hurt responder if they have one (will call it if they are not null)
                    if (hitData.validateHitData())
                    {

                        hitData.hitBox.hitResponder.Response(hitData);

                        hitData.hurtBox.hurtResponder.Response(hitData);
                    }
                }
            }
        }
    }

    public void castForHurtBoxes(Bunch b, Vector3 hitPoint)
    {
        Debug.Log(hitPoint);

        foreach (HitBox hb in b.hitBoxList)
        {
            Debug.Log(hitPoint);
            //for gizmos
            hitBoxToDisplay.Add(hb);

            Vector3 _scaledSize = hb.hitBoxSize;

            HitData hitData = null;

            HurtBox hurtBoxCollidedWith = null;

            //BoxcastAll (center, half extents, direction, orientation, distance, layer mask)
            //halfExtents Half the size of the box in each dimension.
            //direction The direction in which to cast the box.
            //orientation Rotation of the box.
            //maxDistance The max length of the cast.
            //layermask A Layer mask that is used to selectively ignore colliders when casting a capsule.

            //do a box cast, start is where the box will begin, half extents is the scaling of the box (EX: 1,1,1 is a 1 by 1 by 1), orientation is the rotation of the box,
            //distance is the distance it will travel, layer mask is which layers will register a valid hit

            

            float summonX = hb.summonOffset.x;
            try
            {
                if (theSR.flipX)
                {
                    summonX *= -1;
                }
            }
            catch { }


            RaycastHit[] objectsHitByBoxcast = Physics.BoxCastAll(

                hitPoint, //center

                //transform.TransformPoint(hb.summonOffset) - transform.up * ((_scaledSize.y - thicknessOfBoxcastSweep) / 2), //center

                new Vector3(_scaledSize.x, thicknessOfBoxcastSweep, _scaledSize.z) / 2, // half extents 

                transform.up, // direction

                transform.rotation, //rotation 

                 _scaledSize.y - thicknessOfBoxcastSweep, //distance of the cast 

                hb.m_layermask // layer mask 
                );

            /*
                        Gizmos.color = Color.red;

                        Gizmos.DrawCube(hb.summonOffset, hb.hitBoxSize);*/

            //For each hit detected
            foreach (RaycastHit objectHit in objectsHitByBoxcast)
            {

                hurtBoxCollidedWith = objectHit.collider.GetComponent<HurtBox>();

                if (hurtBoxCollidedWith != null && hurtBoxCollidedWith.active)
                {
                    //Make new hit data, which stores the damage, hitbox, hurtbox, etc of the boxcast 
                    hitData = new HitData
                    {

                        damage = hb.damage,

                        hurtBox = hurtBoxCollidedWith,

                        hitBox = hb,

                        hitPoint = objectHit.point
                    };
                    Debug.Log(hitData.hurtBox);


                    //If the hit data is valid call the response of the hit responder and hurt responder if they have one (will call it if they are not null)
                    if (hitData.validateHitData())
                    {

                        hitData.hitBox.hitResponder.Response(hitData);

                        hitData.hurtBox.hurtResponder.Response(hitData);
                    }
                }
            }
        }
    }

    public void displayHitboxes()
    {

        shouldDisplayHitboxes = true;
    }

    public void clearHitboxDisplay()
    {

        for(int i=0; i<hitBoxToDisplay.Count; i++)
        {
            hitBoxToDisplay[i] = null; 
        }

        try
        {
            hitBoxToDisplay.Clear();
        }
        catch
        {
            Debug.Log("Cannot clear Hitbox to display");
        }

        shouldDisplayHitboxes = false;
    }

    public bool getShouldDisplayHitboxes()
    {
        return shouldDisplayHitboxes;
    }

    public float getThicknessOfBoxcastSweep()
    {
        return thicknessOfBoxcastSweep;
    }

    public List<HitBox> getHitboxToDisplay()
    {
        return hitBoxToDisplay;
    }
    

}
