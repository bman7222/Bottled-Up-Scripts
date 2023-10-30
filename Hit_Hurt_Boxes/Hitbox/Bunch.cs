using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bunch 
{

    [Tooltip("The name of the bunch of hit boxes")]
    public string bunchName;

    [Space(2f)]
    [Header("Hit Boxes")]

    [Tooltip("The list of hit boxes in the bunch")]
    public List <HitBox> hitBoxList;

    [Space(2f)]
    [Header("Frame Data ")]

    [Tooltip("The frame the bunch will begin casting")]
    public int beginFrame;

    [Tooltip("The frame the bunch will stop casting")]
    public int endFrame;

    [Space(2f)]
    [Header("Multi-Hit")]

    [Tooltip("The bool which determines if the bunch is apart of a multi-hit attack. If this is true the hit responders list of objects hit will refresh once the bunch is on its end frame")]
    public bool shouldRefresh;
}
