using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The data that is sent to the hit and hurt responders

[System.Serializable]
public class HitData 
{
    [Header("The damage of the hit")]
    public int damage;
    [Header("The hurtBox affected by the hit")]
    public HurtBox hurtBox;
    [Header("The hit box doing the hit")]
    public HitBox hitBox;
    [Header("The location the hit connected")]
    public Vector3 hitPoint;


    //check to see if a confirmed hit has occurred. This means a hit box and a hurt box have collided with one another
    public bool validateHitData()
    {
   
        //if the hurtBox is not null
        if(hurtBox != null)
        {

            //if the hurtBox checks for hitBoxes and returns true, this means it has detected a hitBox it collided with
            if (hurtBox.checkForHitBoxes(this))
            {

                //If the responder of the hurtBox is null or the hurtBox responder validates that it has been hurt, this means the hit data has confirmed a hit has occurred.
                if (hurtBox.hurtResponder == null || hurtBox.hurtResponder.validateHasBeenHurt(this))
                {
 
                    //if the hit responder is null or the hit detector's hit responder validates that it has hit a hurtBox, this means the hit data has confirmed a hit has occurred.
                    if (hitBox.hitResponder == null || hitBox.hitResponder.validateHasConnectedHit(this))
                    {
                        //get damage after modifiers
                        damage = calculateDamage();
                        //the hit data is valid
                        return true;
                    }
                    
                }
            }
        }

        //the hit data is not valid
        return false;
    }

    public int calculateDamage()
    {
        int finalDamage = damage;
        //apply any damage modifiers
        finalDamage = Mathf.RoundToInt(finalDamage*(100-hurtBox.damageReductionPercent)/ 100f);
        //1hitBox.resetDamageIncrease(); //remove the modifer for the next hit
        
        return finalDamage;
    }
}

//INTERFACES LIKE A C++ .h file 

//The sees if a hitBox has landed a hit. If it does then it responds accordingly


/*public interface iHitResponder
{
    //the damage the hit will deal
    int damage { get; }

    //checks if the hit data is a valid hit
    public bool validateHasConnectedHit(HitData hitData);

    //The response once a hit is connected
    public void Response(HitData hitData);
}

//The hit box, checks to see if it collided with a hurtBox
public interface iHitBox
{
    //the responder of the hitBox that they report back to
    public iHitResponder HitResponder { get; set; }
    //look for hurt boxes 
    public void checkForHurtBoxes();
}

//The responder hurtBoxes send data to. Validates hit data and gives a response accordingly
public interface iHurtResponder
{
    // check hit data to see if the hurtBox has been hurt 
    public bool validateHasBeenHurt(HitData hitData);
    //The response after being hit
    public void Response(HitData hitData);
}

//Gives hit data to the hurt responder if it makes contact with a hitBox
public interface iHurtBox
{
    //bool which determines if the hurtBox is active or not
    public bool Active { get; }
    //The game object who this hurtBox belongs to 
    public GameObject Owner { get;}
    //The transform of the game object the hurtBox is on
    public Transform Transform { get; }
    //The hurt responder the hurtBox responds to 
    public iHurtResponder HurtResponder { get; set; }
    //validates if the hurtBox has been hurt
    public bool checkForHitBoxes(HitData hitData);

}*/
