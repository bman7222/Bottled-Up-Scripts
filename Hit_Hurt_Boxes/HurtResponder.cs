using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

[System.Serializable]
public class HurtResponder 
{
    private Character character;

    [Tooltip("the list of hurt boxes which belong to the hurt responder. This is done automatically by each hurtbox")]
    //the list of hurt boxes which belong to the hurt responder
    private List<HurtBox> myHurtboxes;

    public bool checkingParry;
    /// at the top 
    private GameObject damagePopUpPrefab;

    /* [SerializeField] private bool takingDamage = true;
     public float damageInterval = 1f;*/

    // Start is called before the first frame update
    public HurtResponder()
    {
        this.myHurtboxes = new List<HurtBox>();

        this.checkingParry = false;

        this.myHurtboxes = new List<HurtBox>();
    }

    public HurtResponder(Character c)
    {
        this.character = c;

        this.myHurtboxes = new List<HurtBox>();

        this.checkingParry = false;

        this.myHurtboxes = new List<HurtBox>();

        this.damagePopUpPrefab = c.damagePopUpPrefab;

        initializeHurtBoxes();
    }



    //this function always returns true. Check if the hit data 
    public bool validateHasBeenHurt(HitData hitData)
    {
        return true;
    }

    //Respond after hurtbox touched by hitbox
    public void Response(HitData hitData)
    {

       /* if(gameObject.tag == "Player")
        {
            // check if the hit is occurring during the parrying time window
            if (checkingParry)
            {
                // parry was a success, don't take damage
                PlayerCombatControls.Instance.parrySuccess = true;

                // reset parry checking
                checkingParry = false;
                return;
            }
        }*/

        // no parry or parry failed, take damage normally
        //1Debug.Log(this.name + " has taken "+hitData.damage+ " damage by "+hitData.hitBox.gameObject.name);

        ///right after debug damage print
        DamagePopups.Create(damagePopUpPrefab, character.gameObject.transform.position, hitData.damage);

        if (character.gameObject.tag == "Player")
        {
            HealthBar.Instance.TakeDamage(hitData.damage);
            HealthBar.Instance.CheckDeath();
            CameraShaker.Instance.ShakeOnce(5f, 1f, 0f, 1f);
       
        }
        else if(character.gameObject.tag == "Enemy")
        {

            if(character.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHP))
            {
                enemyHP.TakeDamage(hitData.damage);


            }

            CameraShaker.Instance.ShakeOnce(2f, 1f, 0f, 1f);
            //set hit stun state

        }

        if (character.timerInList(timer.timerType.vibrateTimer))
        {

            character.setTimerInList(timer.timerType.vibrateTimer, hitData.hitBox.vibrateStun);
        }
        else
        {

            //in the future thia will be charater exit and character enter state
            //change to character enter vibrate???

            frameAction a = new frameAction("HurtResponder: character.enterVibrate", character.enterVibrate);

            frameAction b = new frameAction("HurtResponder: character.exitVibrate", character.exitVibrate);

            character.createTimer(a, hitData.hitBox.vibrateStun, b, hitData, timer.timerType.vibrateTimer);

        }


    }

    private void AddAllHurtBoxChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.GetComponent<HurtBox>() && !(myHurtboxes.Contains(child.gameObject.GetComponent<HurtBox>())))
            {
                myHurtboxes.Add(child.gameObject.GetComponent<HurtBox>());
            }
            AddAllHurtBoxChildren(child);
        }
    }

    public void initializeHurtBoxes()
    {
        AddAllHurtBoxChildren(character.gameObject.transform);

        //for each hurt box in myHurtboxes, set their hurt responder to this hurt responder
        foreach (HurtBox hurtbox in myHurtboxes)
            hurtbox.hurtResponder = this;
    }

    /*  IEnumerator DamageRate()
      {
          yield return new WaitForSeconds(damageInterval);
          takingDamage = true;
      }*/
    // activate a specific hurtbox
    public void SetHurtboxActive(string hurtboxName)
    {
        foreach (HurtBox hurtbox in myHurtboxes)
        {
            if (hurtbox.name == hurtboxName)
            {
                hurtbox.active = true;
            }
        }
    }
    // deactivate a specific hurtbox
    public void SetHurtboxInactive(string hurtboxName)
    {
        foreach (HurtBox hurtbox in myHurtboxes)
        {
            if (hurtbox.name == hurtboxName)
            {
                hurtbox.active = false;
            }
        }
    }
    // essentially iframes by deactivating all hurtboxes
    public void SetAllHurtboxesInactive()
    {
        foreach (HurtBox hurtbox in myHurtboxes)
        {
            hurtbox.active = false;
        }
    }
}
