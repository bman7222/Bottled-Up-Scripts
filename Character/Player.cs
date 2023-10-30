using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    protected override void Awake()
    {
        base.Awake();


        //character

        playerCombat = GetComponent<PlayerCombatControls>();

        playerMovement = GetComponent<PlayerMovementScript>();

        revHandler = GetComponent<revMeterHandler>();

        groundPos = 0f;

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void handleMovement()
    {
        playerMovement.handleMovement();
    }


    public override void handleCombatUpdate()
    {
        base.handleCombatUpdate();

        playerCombat.handleCombatUpdate();
    }

    public override void exitAttack()
    {
        base.exitAttack();

        playerCombat.clearCurrentCombo();

    }

    public override void handleBeingHurt()
    {
        base.handleBeingHurt();

        playerCombat.clearCurrentCombo();
    }

    public override void allowMovement()
    {
        base.allowMovement();

        playerCombat.clearCurrentCombo();
    }

}

//Wave manager - mono behavior 
//holds list of waves 
//Holds a list of all alive enemies, when list becomes empty, move onto next wave
    //Wave - Scriptable OBJ
    //Holds a list of mini waves
       //Mini Wave - Class
       //List of enemies
       //what enemies?
