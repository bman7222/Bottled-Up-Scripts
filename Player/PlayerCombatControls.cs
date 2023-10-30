using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCombatControls : MonoBehaviour/*, iHitResponder*/
{
    private Character character;

  /////////////  Rev Meter related Variables  ////////////////////////// 

    ////////////////   Input    ///////////////////////////////////////////// 
    [Header("The controller input to use")]
    [SerializeField] private PlayerInput playerActions;
    private InputAction rev;
    private InputAction lightAttackInput;
    private InputAction mediumAttackInput;
    private InputAction heavyAttackInput;
    private InputAction dash;
    private InputAction block;

    ////////////////  Offensive Moves  ////////////////////////////////////// 
    /*
    [Header("The hitbox to summon when the player presses attack")]
    [SerializeField] private HitBox _hitboxTap;*/


   /* [Header("Bool which says if the player is attacking or not")]
    [SerializeField] private bool isTapAtking = false;*/
    
    /*[Header("The Array of all possible attacks a player can create")]
    public HitBox[] attackArray;*/

    public float dmgModifier = 1f;

    [Header("Attack Total Frames")]

    public float lightAtkFrames;

    public float mediumAtkFrames;

    public float heavyAtkFrames;

    public enum attackStage
    {
        none,
        light,
        medium,
        heavy,
    }

    private attackStage currentAttackStage;

    private attackStage lastInputAttackStage;

    public List<attackStage> currentCombo;

  ///////////////  Defensive Moves  ///////////////////////////////////////
    [Header("Dash")]
    [SerializeField] private bool isDashing = false;
    [SerializeField] private float dashForce = 90f;
    [SerializeField] private float dashDuration = 0.3f;
    private Vector3 appliedForce;

    [Header("Block and Parrying")]
    [SerializeField] private bool isBlocking = false;
    [SerializeField] private float maxBlockDuration = 3f;
    [SerializeField] public bool parrySuccess = false;

    ///////////////  Cooldowns   //////////////////////////////////////////// 
    [Header("Cooldown times")]
    [SerializeField] private float dashCooldownTime;

    [Header("Cooldowns")]
    [SerializeField] private bool DashOnCD = false;
    

  /////////////  Hitbox and Hurtbox stuff  //////////////////////////////// 
    //the list of all game objects hit by the attack

    private float attackBufferLength = 0.04f;
    private float attackBufferCount = -11f;

    ///////////////    Methods     ///////////////////////////////////////// 
    //set player actions
    private void Awake()
    {

        character = GetComponent<Character>();

        dashCooldownTime = PlayerCooldowns.dash;

        playerActions = new PlayerInput();

        lastInputAttackStage = attackStage.none;

        currentAttackStage = attackStage.none;

        currentCombo = new List<attackStage>();
    }

    //On enable, set player actions and inputs
    private void OnEnable()
    {
        rev = playerActions.Player.BuildRev;
        rev.Enable();
        rev.started += context =>
        {
            //PlayerMovementScript.Instance.SetMoveSpeed(PlayerMovementScript.Instance.GetBaseMoveSpeed() * revMoveSpdFactor); //make the player move slowly while building rev
            //if rev effects aren't emitting already, then start to emit them

        };

        rev.performed += context =>
        {
            character.playRevEffect();
            character.buildRevMeter();
            //revEffectsTimer = revEffectsDespawnTime;
        };

        lightAttackInput = playerActions.Player.lightAttack;
        lightAttackInput.Enable();
        lightAttackInput.performed += context => lightAttack();

        mediumAttackInput = playerActions.Player.mediumAttack;
        mediumAttackInput.Enable();
        mediumAttackInput.performed += context => mediumAttack();

        heavyAttackInput = playerActions.Player.heavyAttack;
        heavyAttackInput.Enable();
        heavyAttackInput.performed += context => heavyAttack();

        dash = playerActions.Player.DodgeDash;
        dash.Enable();
        dash.performed += context => DodgeDash();

        block = playerActions.Player.Block;
        block.Enable();
        block.started += context => Block();
        block.canceled += context => EndBlock();    //automatically end blocking state if the player stops holding block
    }

    //disable player actions and inputs
    private void OnDisable()
    {
        rev.Disable();
        lightAttackInput.Disable();
        mediumAttackInput.Disable();
        heavyAttackInput.Disable();
        dash.Disable();
        block.Disable();
    }


    public void handleCombatUpdate()
    {
        attackBufferCount -= character.getMyPhysicsTime();

        //Cannot perform another action while in attack 
        if (character.getState() == Character.State.Attack) { return; }

        handleAttack();

        if (isDashing) //apply dash force to the player for the dash duration
        {
            character.stopMovement();
            character.getRigidBody().AddForce(appliedForce, ForceMode.Impulse);
        }
    }

    public void clearCurrentCombo()
    {
        currentCombo.Clear();
    }

    void handleAttackVars()
    {
        //Function

        attackBufferCount = -99;

        character.refreshHitList();

        character.stopMovement();

        character.changeState(Character.State.Attack);

        character.getSpriteRendererAnim().SetTrigger("isAttacking");
    }


    void handleAttack()
    {

        if ((character.getState() == Character.State.Idle || character.getState() == Character.State.HitStop || character.getState() == Character.State.Combo) && attackBufferCount >= 0 && !currentCombo.Contains(lastInputAttackStage))
        {

            //Already did an attack before
        
            if (character.getState() == Character.State.HitStop || character.getState() == Character.State.Combo)
            {

                if (character.getTimer(timer.timerType.hitStopTimer) != null)
                {

                    character.getTimer(timer.timerType.attackTimer).subtractTime(character.getTimer(timer.timerType.hitStopTimer).getTimeElapsed());
                   
                    character.getTimer(timer.timerType.hitStopTimer).callTimerFinish();
                    
                    character.removeTimer(timer.timerType.hitStopTimer);
                   
                }
            }

            BunchHolder attackToUse;

            //Set total time to the attack that was input's total time
            float totalAttackTime = 0f;

            //Do according to attack used

            switch (lastInputAttackStage)
            {
                case attackStage.light:
                    totalAttackTime = lightAtkFrames;

                    currentAttackStage = attackStage.light;

                    character.setAttackToUse(0);

                    character.getSpriteRendererAnim().SetFloat("onAttack", 0f);  //For Artists: onAttack is a float that controls the attack animation to do. 0 = atk1, 1 = atk2, 2 = atk3

                    break;
                case attackStage.medium:
                    totalAttackTime = mediumAtkFrames;

                    currentAttackStage = attackStage.medium;

                    character.setAttackToUse(1);

                    character.getSpriteRendererAnim().SetFloat("onAttack", 1f);  //For Artists: onAttack is a float that controls the attack animation to do. 0 = atk1, 1 = atk2, 2 = atk3

                    break;

                case attackStage.heavy:
                    totalAttackTime = heavyAtkFrames;

                    currentAttackStage = attackStage.heavy;

                    character.setAttackToUse(2);

                    character.getSpriteRendererAnim().SetFloat("onAttack", 2f);  //For Artists: onAttack is a float that controls the attack animation to do. 0 = atk1, 1 = atk2, 2 = atk3

                    break;
                case attackStage.none:
                    totalAttackTime = 0f;

                    currentAttackStage = attackStage.none;

                    break;
            }
           
            //add to combo

            currentCombo.Add(currentAttackStage);

            //Check combo ordering. Must be Something lighter to something heavier or vice versa.

            if (currentCombo.Count > 1)
            {
                if (currentCombo[0] == attackStage.medium && currentCombo[1] == attackStage.heavy)
                {
                    currentCombo.Add(attackStage.light);
                }
                else if (currentCombo[0] == attackStage.medium && currentCombo[1] == attackStage.light)
                {
                    currentCombo.Add(attackStage.heavy);
                }
                else if (currentCombo[0] == attackStage.heavy && currentCombo[1] == attackStage.light)
                {
                    currentCombo.Add(attackStage.medium);
                }
                else if (currentCombo[0] == attackStage.light && currentCombo[1] == attackStage.heavy)
                {
                    currentCombo.Add(attackStage.medium);
                }
            }


            //character is attacking and they are in hitstop or combo state
            if (character.timerInList(timer.timerType.attackTimer) && (character.getState()==Character.State.HitStop || character.getState() == Character.State.Combo))
            {
                //character.setTimerInList(timer.timerType.attackTimer, 0.38f);

                Debug.Log("ALREADY ATKING");

                handleAttackVars();

                character.setTimerInList(timer.timerType.attackTimer, totalAttackTime);

            }
            else
            {
                try
                {

                    frameAction a = new frameAction("PlayerCombatControls: character.exitAttack", character.exitAttack);

                    character.createTimer(totalAttackTime, a, timer.timerType.attackTimer);

                    handleAttackVars();
                }
                catch
                {
                    Debug.Log("ATK FAIL");

                    return;
                }
            }
          
            // REV 
            if (character.checkRevAttackModifier())
            {
                dmgModifier = 2f;
            }
            else
            {
                dmgModifier = 1f;
        
            }
        }

    }

    void lightAttack()
    {
        
        attackBufferCount = attackBufferLength;

        lastInputAttackStage = attackStage.light;

        character.setRevCost();
       
    }

    void mediumAttack()
    {
        lastInputAttackStage = attackStage.medium;

        attackBufferCount = attackBufferLength;

        character.setRevCost();

    }

    void heavyAttack()
    {
        lastInputAttackStage = attackStage.heavy;

        attackBufferCount = attackBufferLength;

        character.setRevCost();

    }

    void HoldAttack()
    {

    }
    void DodgeDash()
    {
        if (character.getMyPhysicsTime() <= 0)
        {
            return;
        }
        if (!DashOnCD)
        {
          
            Vector3 dashDirection;
         
            if(PlayerMovementScript.Instance.moveDirection.magnitude > 0) 
            {
                dashDirection = new Vector3(PlayerMovementScript.Instance.moveDirection.x, 0f, PlayerMovementScript.Instance.moveDirection.y).normalized;
            }
            
            else 
            {
                dashDirection = new Vector3(PlayerMovementScript.Instance.lastMoveDirection.x, 0f, PlayerMovementScript.Instance.lastMoveDirection.y).normalized;
            }
            appliedForce = dashDirection * dashForce;
          
            isDashing = true;
           
            PlayerMovementScript.Instance.canMove = false;
        
            character.getHurtResponder().SetAllHurtboxesInactive();

            gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 143f/255f, 253f/255f);
          
            Invoke("EndDash", dashDuration);

      
            DashOnCD = true;

            StartCoroutine("DashCooldown");
        }
        
    }
    //stop adding force to the player after dash duration 
    private void EndDash()
    {
        isDashing = false;

        character.allowMovement();

        character.getHurtResponder().SetHurtboxActive("DefaultHurtbox");

        character.getSpriteRenderer().color = Color.white;
    }

    //current attack player is doing (Light=1,Medium2,Heavy=3,None=0)
    public attackStage getCurrentAttackStage()
    {
        return currentAttackStage;
    }
    // do parry stuff
    void Parry()
    {
        // increase rev meter
        character.addParryVal();
    }
    // check for parrying first, block otherwise
    void Block()
    {
        // check for parrying during the parry time window
        while (character.getSpriteRendererAnim().GetBool("isParrying"))
        {
            if (parrySuccess)
            {
                Debug.Log("Parried successfully");
                Parry();
            }
        }

        // no parry occurred, block normally
        isBlocking = true;
        //disable player movement during blocking
        PlayerMovementScript.Instance.canMove = false;

        //change the active hurtbox to blocking hurtbox that takes less damage
        character.getHurtResponder().SetHurtboxInactive("DefaultHurtbox");
        character.getHurtResponder().SetHurtboxActive("BlockHurtbox");

        //change the sprite color to indicate blocking active
        gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 200f / 255f, 120f / 255f);

        // stop blocking after a certain duration
        Invoke("EndBlock", maxBlockDuration);
        

    }
    // end blocking state, reset player defaults
    void EndBlock()
    {
        isBlocking = false;
        //enable player movement
        PlayerMovementScript.Instance.canMove = true;

        //change active hurtbox back to default
        character.getHurtResponder().SetHurtboxInactive("BlockHurtbox");
        character.getHurtResponder().SetHurtboxActive("DefaultHurtbox");

        //change the sprite color back to normal
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;

    }


    IEnumerator DashCooldown()
    {
        //wait for dash's cooldown to end
        yield return new WaitForSeconds(dashCooldownTime);
        DashOnCD = false;
    }
    
    public void PlayerDeath()
    {
        //perimsh
        SceneManagerScript.Instance.SetPreviousCombatScene(SceneManager.GetActiveScene().name);
        SceneManagerScript.Instance.LoadScene("GameOver");
        Debug.Log("You died");
    }

/*
    //function to spawn hitboxes
    public void SpawnHitbox()
    {
        HitBox attack = Instantiate(attackArray[0], Vector3.zero, Quaternion.identity);
        attack.transform.SetParent(transform);
        attack.IncreaseDamage(dmgModifier);
        *//*            foreach(HitBox h in hitResponder.myHitBoxes)
                    {
                        h.gameObject.SetActive(true);
                    }*//*

        //if last moving right, 1,0 or diagonally right 0.5,0.5
        if (PlayerMovementScript.Instance.lastMoveDirection.x > 0)
        {
            //summon attack array at 0 at current position + x offset
            attack.transform.position = transform.TransformPoint(new Vector3(attack.summonPosition.x, attack.summonPosition.y, 0f));
        }
        //if last moving left, -1,0 or diagonally right -0.5,0.5
        else if (PlayerMovementScript.Instance.lastMoveDirection.x < 0)
        {
            //summon attack array at 0 at current position - x offset
            attack.transform.position = transform.TransformPoint(new Vector3(-attack.summonPosition.x, attack.summonPosition.y, 0f));
        }
        //if last moving up, 0,1
        else if (PlayerMovementScript.Instance.lastMoveDirection.y > 0)
        {
            //summon attack array at 0 at current position - z offset
            attack.transform.position = transform.TransformPoint(new Vector3(0f, attack.summonPosition.y, attack.summonPosition.z));
        }
        //if last moving down 0,-1
        else if (PlayerMovementScript.Instance.lastMoveDirection.y < 0)
        {
            //summon attack array at 0 at current position - z offset
            attack.transform.position = transform.TransformPoint(new Vector3(0f, attack.summonPosition.y, -attack.summonPosition.z));
        }
        //if never moved 
        else
        {
            //summon attack array at 0 at current position - z offset
            attack.transform.position = transform.TransformPoint(new Vector3(0f, 1f, -attack.summonPosition.z));
            PlayerMovementScript.Instance.lastMoveDirection = new Vector2(0f, -1f);
        }
        //reset the damage modifier after a single attack
        dmgModifier = 1f;
    }
   

    */

    public void testFunction()
    {
        Debug.Log("TEST COMPLETE");
    }
}

