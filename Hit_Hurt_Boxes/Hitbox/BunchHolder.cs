using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "testAttack", menuName = "ScriptableObjects/BunchHolder")]
public class BunchHolder : ScriptableObject
{
    [Tooltip("The name of the bunch holder. Holds bunches of hitboxes. All hit boxes for any given attack")]
    public string bunchHolderName;

    [Space(2f)]
    [Header("List of Bunches")]

    public List<Bunch> bunches;

}
