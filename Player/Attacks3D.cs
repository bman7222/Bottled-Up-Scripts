using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks3D : MonoBehaviour
{
    public GameObject yoyo3D;
    private float lastMoveX;
    private float lastMoveY;
    private Animator spriteAnimator;

    // Start is called before the first frame update
    void Awake()
    {
        spriteAnimator = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activateYoYo()
    {
        yoyo3D.SetActive(true);
    }

    public void changeDirection()
    {

        lastMoveX = spriteAnimator.GetFloat("lastMoveX");
        lastMoveY = spriteAnimator.GetFloat("lastMoveY");
        /*
        if (lastMoveY <= -0.5)
        {
            yoyo3D.transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (lastMoveY >= 0.5)
        {
            yoyo3D.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else //if(lastMoveX > -0.5 && lastMoveX < 0.5)
        {*/
            //lastMoveX = spriteAnimator.GetFloat("lastMoveX");
            if (gameObject.GetComponent<SpriteRenderer>().flipX)
            {
                yoyo3D.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                yoyo3D.transform.rotation = Quaternion.Euler(0, 180, 0);

            }
        //}
        
    }

    public void attack1()
    {
        deactivateYoYo();
        activateYoYo();
        yoyo3D.GetComponent<Animator>().Play("YoYo_Attack1");
        if(gameObject.GetComponent<SpriteRenderer>().flipX || lastMoveY != spriteAnimator.GetFloat("lastMoveY"))
            changeDirection();
        else if(!gameObject.GetComponent<SpriteRenderer>().flipX)
            yoyo3D.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    public void attack2()
    {
        deactivateYoYo();
        activateYoYo();
        yoyo3D.GetComponent<Animator>().Play("YoYo_Attack2");
        if (gameObject.GetComponent<SpriteRenderer>().flipX || lastMoveY != spriteAnimator.GetFloat("lastMoveY"))
            changeDirection();
        else if (!gameObject.GetComponent<SpriteRenderer>().flipX)
            yoyo3D.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    public void attack3()
    {
        deactivateYoYo();
        activateYoYo();
        yoyo3D.GetComponent<Animator>().Play("YoYo_Attack3");
        if (gameObject.GetComponent<SpriteRenderer>().flipX || lastMoveY != spriteAnimator.GetFloat("lastMoveY"))
            changeDirection();
        else if (!gameObject.GetComponent<SpriteRenderer>().flipX)
            yoyo3D.transform.rotation = Quaternion.Euler(0, 180, 0);
    }


    public void deactivateYoYo()
    {
        yoyo3D.SetActive(false);
    }
}
