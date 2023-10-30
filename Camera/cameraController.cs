using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    //The target the camera will follow
    private Transform target;

    Character character;

    //Singleton ref of camera
    public static cameraController Instance;

    [Header("Change the offset of the camera in the y direction by O and the z direction by O")]
    //the offset of the camera in the y and z diections
    [SerializeField] private float offsetY, offsetZ;

    public bool InCameraBound = false;

    [SerializeField] private bool ZAxisOn = false;


    // Start is called before the first frame update
    void Awake()
    {
        //Locate the player and set their transform to the target
        target = FindObjectOfType<Player>().gameObject.transform;
        character = target.GetComponent<Character>();

        //set instance
        Instance = this;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //If there is no target, locate a new target 
        if (!target)
        {
            //set target
            target = FindObjectOfType<Player>().gameObject.transform;
            character = target.GetComponent<Character>();
        }



        //transform.position = new Vector3(target.position.x, target.position.y, target.position.z);
        //make the camera follow the player

        if (character.getState() == Character.State.Vibrate)
        {
            transform.position = transform.position;
        }
        else
        {
            if (!InCameraBound)
            {
                if (ZAxisOn)
                {
                    transform.position = new Vector3(target.position.x, target.position.y + offsetY, target.position.z-offsetZ);
                }
                else
                {
                    transform.position = new Vector3(target.position.x, target.position.y + offsetY, -offsetZ);
                }
            }
            else
            {
                if (ZAxisOn)
                {
                    transform.position = new Vector3(0f, offsetY, target.position.z - offsetZ);
                }
            }
        }

        

    }
    
}


