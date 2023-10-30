using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


#if UNITY_EDITOR
public class characterEditor : Editor
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    static void OnDrawGizmos(Character character, GizmoType gizmoType)
    {
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = new Color(1f, 0f, 0f, 0.1f);

        if (character.getShouldDisplayHitboxes() && Application.isPlaying)
        {
            foreach (HitBox hb in character.getHitboxToDisplay())
            {
                Vector3 _scaledSize = hb.hitBoxSize;

                float summonX = hb.summonOffset.x;

                try
                {
                    if (character.getSpriteRenderer().flipX)
                    {
                        summonX *= -1;
                    }
                }
                catch
                {
                    summonX = 1;
                }

                Gizmos.DrawCube(character.transform.TransformPoint(summonX,hb.summonOffset.y,hb.summonOffset.z),
                    new Vector3(_scaledSize.x, _scaledSize.y, _scaledSize.z));

                /*Gizmos.DrawCube(character.transform.TransformPoint(hb.summonOffset) - character.transform.up * ((_scaledSize.y - character.getThicknessOfBoxcastSweep()/2)), 
                    new Vector3(_scaledSize.x, _scaledSize.y, _scaledSize.z));*/
            }
        }



    }

}
#endif