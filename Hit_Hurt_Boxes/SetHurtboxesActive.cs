using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHurtboxesActive : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private HurtResponder hurtResponder;
    // Start is called before the first frame update
    void Awake()
    {
        // get the player game object
        player = gameObject.transform.parent.gameObject;

        // get the hurt responder of the player
        hurtResponder = player.GetComponent<HurtResponder>();

        
    }
    // start checking for hurt responses during the parry time window
    public void StartCheckingParry()
    {
        hurtResponder.checkingParry = true;
        //player.GetComponentInChildren<SpriteRenderer>().color = new Color(136f / 255f, 218f / 255f, 245 / 255f);
    }
    // stop checking for hurt responses, parry time window ended
    public void StopCheckingParry()
    {
        hurtResponder.checkingParry = false;
        //player.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

}
