using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour/*, iHurtBox*/
{

    [Header("Determines if the hurtbox is active or not")]
    [SerializeField] public bool active = true;
    [Header("The game object the hurtbox belongs to, this is automatically set to the root parent")]
    [SerializeField] public GameObject owner = null;

    [Header("How much damage reduction factor the hurtbox has")]
    [SerializeField] public float damageReductionPercent = 0f;

    //the hurt responder of this hurtbox 
    [Header("The hurt responder of this hurtbox, is set automatically so that the parent's responder is the responder to all children")]
    [SerializeField] public HurtResponder hurtResponder;

    //bool which determines if the hurtBox is active or not


    //The game object who this hurtBox belongs to 


    //The transform of the game object the hurtBox is on

    //The hurt responder the hurtBox responds to. 
  /*  public iHurtResponder HurtResponder { get => HurtResponder; set => myHurtResponder = value; 
       *//* get
        {

            if (!done)
            {

                done = true;
                Debug.Log("saaaa");
                Debug.Break();
                return HurtResponder;
            }
            return null;
        }
        set
        {
            if (!done)
            {
                myHurtResponder = value;
            }
        }*//*

    }*/


    void Awake()
    {
        //set owner to the root parent
        GameObject myHurtResponder = getRootParent();
        owner = myHurtResponder;
        //myHurtResponder.GetComponent<HurtResponder>().myHurtboxes.Add(this);
        gameObject.layer = LayerMask.NameToLayer("Hurtbox");

    }

    //validates if the hurtBox has been hurt
    public bool checkForHitBoxes(HitData hitData)
    {
        //If the hurt responder is null, debug message
        if (hurtResponder == null) Debug.Log(gameObject+" No Responder");

            //this function will always be true
            return true;
    }

    //Get the game object that is the first parent/root parent. 
    public GameObject getRootParent()
    {
        //Try 
        try
        {
            //Get the parent
            GameObject rootParent = transform.parent.gameObject;

            //make sure it is not null
            if (!rootParent)
            {
                Debug.Log("The root parent is null");
                return this.gameObject;
            }

            //Keep getting the parent of the game object until there are no more parents
            while (rootParent.transform.parent != null)
            {
                rootParent = rootParent.transform.parent.gameObject;

                //If a parent has a rigid body, break the loop
                //if (rootParent.GetComponent<Rigidbody>()) break;
            }

            //return the rootParent which will 
            return rootParent;

        }
        //If the try fails, return the inserted owner
        catch
        {
            Debug.Log("The root parent is null");
            return this.gameObject;
        }
    }

}
