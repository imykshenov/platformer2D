using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogScript : Enemy
{
    // Start is called before the first frame update
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;

    [SerializeField] private float jumpLength = 10f;
    [SerializeField] private float jumpHight = 10f;
    [SerializeField] private LayerMask ground;


    private Collider2D coll;


    private bool facingLeft = true;


    protected override void Start()
    {
        base.Start();
        coll = GetComponent<Collider2D>();
    }
    private void Update()
    {
       //Смена из прыжка в падение лягушки
        if(anim.GetBool("Jump"))
        {
            if(rb.velocity.y < .1f)
            {
                anim.SetBool("Fall", true);
                anim.SetBool("Jump", false);
            }
        }

        if(coll.IsTouchingLayers(ground) && anim.GetBool("Fall"))
        {
            anim.SetBool("Fall", false);
        }
    }



    private void Move()
    {
        if (facingLeft)
        {

            if (transform.position.x > leftCap)
            {
                //проверяем если спрайт находится справа, если ээто не так, разворачиваемся
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1);
                }
                // gsh;jr 
                if (coll.IsTouchingLayers(ground))
                {
                    //прыжок
                    rb.velocity = new Vector2(-jumpLength, jumpHight);
                    anim.SetBool("Jump", true);
                }
            }
            else
            {
                facingLeft = false;
            }

        }
        else
        {
            if (transform.position.x < rightCap)
            {
                //проверяем если спрайт находится справа, если ээто не так, разворачиваемся
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1);
                }
                // 
                if (coll.IsTouchingLayers(ground))
                {
                    //прыжок
                    rb.velocity = new Vector2(jumpLength, jumpHight);
                    anim.SetBool("Jump", true);
                }
            }
            else
            {
                facingLeft = true;
            }
        }
    }


}
