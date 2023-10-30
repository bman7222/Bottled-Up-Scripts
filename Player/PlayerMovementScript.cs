using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    private Character character;

    //the rigid body of the player
    private Rigidbody rb;


    //The animator for the player Object
    [Tooltip("The animator for the player Object")]
    private Animator playerObjectAnim;
    //The animator for the player sprite
    [Tooltip("The animator for the player sprite")]
    private Animator theSRAnim;
    [Tooltip("The sprite renderer player sprite")]
    private SpriteRenderer theSR;

    //The bool used to store if the players back is facing the camera or not
    private bool isMovingBackwards;

    [Tooltip("The current move speed of the player is O")]
    //the move speed of the player
    [SerializeField] private float moveSpeed = 5f;
    
    [Tooltip("The default move speed of the player")]
    [SerializeField] private float baseMoveSpeed = 5f;

    [Tooltip("The input to take from")]
    //the control input of the player
    [SerializeField] private PlayerInput playerMovement;

    //the input action of the control to use
    private InputAction move;

    [Header("Move Directions")]
    public Vector2 moveDirection;

    //holds the last moved in direction
    [Tooltip("holds the last moved n direction")]
    [SerializeField] public Vector2 lastMoveDirection;
    public Vector2 previousLastMoveDirection;

    //bool that indicates whether the player can move or not

    [Space(2f)]
    [Header("Can Move")]
    public bool canMove;

    public static PlayerMovementScript Instance;


    //set move and enable it
    private void OnEnable()
    {
        move = playerMovement.Player.Move;
        move.Enable();
    }

    //set rigid body
    private void Awake()
    {

        playerMovement = new PlayerInput();

        Instance = this;
        canMove = true;

        character = GetComponent<Character>();

        rb = GetComponent<Rigidbody>();//character.getRigidBody();

        theSR = character.getSpriteRenderer();

        theSRAnim = character.getSpriteRendererAnim();

        playerObjectAnim = character.getCharacterAnim();
    }


    //disable move 
    private void OnDisable()
    {
        move.Disable();
    }

    public float GetBaseMoveSpeed()
    {
        return baseMoveSpeed;
    }

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    //read in the player input

    public void handleMovement()
    {
       
        character.getSpriteRendererAnim().SetFloat("lastMoveY", lastMoveDirection.y);

        character.getSpriteRendererAnim().SetBool("isMoving", false);

        character.getSpriteRendererAnim().SetFloat("moveY", Input.GetAxisRaw("Vertical"));

        //read in move input
        moveDirection = move.ReadValue<Vector2>();

        //if the player is moving 
        if (moveDirection == Vector2.zero)
        {
            character.stopMovement();
            return; 
        }

            //If the player is moving diagonally 
            if ((moveDirection.x != 0 && moveDirection.y != 0))
            {
                //If the last direction the player was moving in  was in the x direction, and they were not moving in the y direction
                //This means they are moving diagonally, by pressing left or right and then pressing up or down 
                if (lastMoveDirection.x != 0 && previousLastMoveDirection.y == 0)
                {
                    //Set previous last move to last move
                    previousLastMoveDirection = lastMoveDirection;
                    //set last move to y direction
                    lastMoveDirection = new Vector2(0, moveDirection.y);
                   
                }
                //If the last direction the player was moving in was in the y direction, and they were not moving in the x direction
                //This means they are moving diagonally, by pressing up or down and then pressing left or right
                else if (lastMoveDirection.y != 0 && previousLastMoveDirection.x == 0)
                {
                    previousLastMoveDirection = lastMoveDirection;
                    lastMoveDirection = new Vector2(moveDirection.x, 0);
                }

            }
            //f not moving diagonally, reset previous last move and set last move to the current move direction
            else
            {

                previousLastMoveDirection = Vector2.zero;

                lastMoveDirection = moveDirection;

            }


        //move player 
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, 0f, moveDirection.y * moveSpeed);

        //Set sprite renderer move speed
        //theSRAnim.SetFloat("moveSpeed", rb.velocity.magnitude);

        
        //Debug.Log(rb.velocity.magnitude);
        //IF player is moving
        if (rb.velocity == Vector3.zero)
        {
            return;
        }

        theSRAnim.SetFloat("moveY", Input.GetAxisRaw("Vertical"));

        theSRAnim.SetBool("isMoving", true);

        if (!theSR.flipX && Input.GetAxisRaw("Horizontal") < 0)
        {
            theSR.flipX = true;

            playerObjectAnim.SetTrigger("Flip");
        }

        else if (theSR.flipX && Input.GetAxisRaw("Horizontal") > 0)
        {
            theSR.flipX = false;
            playerObjectAnim.SetTrigger("Flip");
        }

        if (!isMovingBackwards && Input.GetAxisRaw("Vertical") > 0)
        {
            isMovingBackwards = true;


        }

        else if (isMovingBackwards && Input.GetAxisRaw("Vertical") < 0)
        {
            isMovingBackwards = false;

        }

        theSRAnim.SetBool("movingBackwards", isMovingBackwards);

    }

}

       
    


        

        
        
        
