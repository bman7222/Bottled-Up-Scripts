using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DELETE ME
public class SetHitboxActive : MonoBehaviour
{
    [SerializeField] Animator anim;

    [SerializeField] private GameObject player;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        player = gameObject.transform.parent.gameObject;
    }

    private void Update()
    {
        
    }
    public void SetActive()
    {
        //player.GetComponent<PlayerCombatControls>().SpawnHitbox();
        //Debug.Log("spawning hitbox");
    }
    public void SetInactive()
    {
        /*player.GetComponent<PlayerCombatControls>().DespawnHitbox();*/
        //Debug.Log("despawning hitbox");
    }

    public void attackOffCooldown()
    {
        //TEMPORARY CODE
      /*  PlayerCombatControls.Instance.state = State.Idle;
        PlayerCombatControls.Instance.AttackOnCD = false;
        PlayerCombatControls.Instance.theSRAnim.SetBool("isFighting", false);*/
    }
    public void attackOnCooldown()
    {/*
        PlayerCombatControls.Instance.AttackOnCD = true;
        PlayerCombatControls.Instance.theSRAnim.SetBool("isFighting", true);*/
    }

}
