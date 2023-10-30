using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HitBox //, iHitBox
{
    [Tooltip("The name of the hitbox")]
     public string hitboxName;

    [Space(2f)]
    [Header("Size Properties")]

    [Tooltip("The size of the hitBox")]
    public Vector3 hitBoxSize;

    [Space(2f)]
    [Header("Attack Properties")]

    [Tooltip("The Damage this hitBox will deal")]
    public int damage;

    [Tooltip("The amount the character who is hit will be stunned")]
    public float hitStun;

    [Tooltip("The amount the character who does the hit will be momentarily paused")]
    public float hitStop;

    [Tooltip("The amount the character will vibrate before receiving knock back")]
    public float vibrateStun;

    [Tooltip("The target will be knocked back with a force of O")]
    public float knockbackForce;

    [Tooltip("The target will be knocked back at an angle of O")]
    public float knockbackAngle;

    [Space(2f)]
    [Header("Summon Properties")]

    [Tooltip("Spawn this hitbox at the summon position. It is also the center of the hitbox")]
     public Vector3 summonOffset;

    [Tooltip("The layer to check for hits")]
    public LayerMask m_layermask;

    [Tooltip("The hit responder of this hitbox, is set automatically so that the parent's responder is the responder to all children")]
     public HitResponder hitResponder;


}