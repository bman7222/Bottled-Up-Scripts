using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunchHolderParser 
{
    private Character character;

    public BunchHolderParser()
    {

    }

    public BunchHolderParser(Character c)
    {
        this.character = c;
    }

    public void parseBunchHolder(int currentFrame, BunchHolder bunches)
    {

        foreach(Bunch b in bunches.bunches)
        {
            if(currentFrame>=b.beginFrame && currentFrame <= b.endFrame && (character.getState()==Character.State.Attack || character.getState() == Character.State.HitStop ||
                character.getState() == Character.State.Combo ))
            {
                //cast

                character.castForHurtBoxes(b);

                character.displayHitboxes();
            }

            if (currentFrame == b.endFrame && b.shouldRefresh)
            {
                //refresh 

                character.refreshHitList();
            }

            if(currentFrame == b.endFrame)
            {
                character.clearHitboxDisplay();
            }
        }
    }

    public void hazardParseBunchHolder(int currentFrame, BunchHolder bunches, Vector3 hitPoint)
    {
        Debug.Log("Hazard Bunch parsing");
        foreach (Bunch b in bunches.bunches)
        {
            if (currentFrame >= b.beginFrame && currentFrame <= b.endFrame)
            {
                //cast
                character.castForHurtBoxes(b, hitPoint);

                character.displayHitboxes();
            }

            if (currentFrame == b.endFrame && b.shouldRefresh)
            {
                //refresh 

                character.refreshHitList();
            }

            if (currentFrame == b.endFrame)
            {
                character.clearHitboxDisplay();
            }
        }
    }

   

}
