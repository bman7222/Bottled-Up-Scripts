using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class HitResponder /*iHitResponder.*/
{
    private Character character;

    //the list of all game objects hit by the attack
    [Tooltip("the list of all game objects hit by the attack")]
    public List<GameObject> hitList;
 
   public HitResponder()
    {
        hitList = new List<GameObject>();
    }

    public HitResponder(Character c)
    {
        this.character = c;

        this.hitList = new List<GameObject>();

    }

    //Confirm that the hitbox has landed a valid hit
    public bool /*iHitResponder.*/validateHasConnectedHit(HitData hitData)
    {
        //If the hit data hurt box tag is the same as the game object tag, or if the object being hurt has already been hurt before, return false, else, return true
        try
        {
            if (hitData.hurtBox.owner.tag != null && (hitData.hurtBox.owner.tag == character.gameObject.tag || hitList.Contains(hitData.hurtBox.owner))) { return false; }
            return true;
        }
        catch
        {
            return false;
        }
    }

    //Do a response after landing a hit
   public  void /*iHitResponder.*/Response(HitData hitData)
    {
        //Add object hit so that it is not repeatedly hit
        hitList.Add(hitData.hurtBox.owner);

        //myTime.setTimer(hitData.hitStop, myTime.resetTimer);
        //myTime.Stop(hitData.hitStop);
        try
        {
            if (!character.timerInList(timer.timerType.hitStopTimer))
            {

                frameAction a = new frameAction("Hit Responder: character.enterHitStop", character.enterHitStop);

                frameAction b = new frameAction("Hit Responder: character.exitHitStop", character.exitHitStop);

                character.createTimer(timer.timerType.attackTimer, a, hitData.hitBox.hitStop, b, timer.timerType.hitStopTimer);
            }
            else
            {
                character.setTimerInList(timer.timerType.hitStopTimer, hitData.hitBox.hitStop);
            }
        }
        catch { }

        
    }

    public Character getCharacter()
    {
        return character;
    }

    public void refreshHitList()
    {
        try
        {
            for (int i = 0; i < hitList.Count; i++)
            {
                hitList[i] = null;
            }
        }
        catch
        {
            //Debug.Log("Error doing hit list for loop");
        }

        try
        {
            //Debug.Log("Cleared");

            hitList.Clear();
        }
        catch
        {
            //Debug.Log("Cannot clear hitList");
        }
    }

}

/*#region EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(HitResponder))]
public class HitResponderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        HitResponder hitResponder = new HitResponder();
        
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Details");

        EditorGUILayout.BeginHorizontal();

        //Serialize 

        EditorGUILayout.EndHorizontal();
    }
}
#endif
#endregion*/