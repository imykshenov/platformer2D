//using System.Numerics;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{

    public CharacterController2D controller;

    [SerializeField] private int cherries = 0;
    [SerializeField] private Text cherryText;

    private enum State { idle, run, jump, fall, hurt }

    private State state = State.idle;

    public Animator animator;

    public float runSpeed = 40f;

    float horisontalMove = 0f;


    bool jump = false;
    bool crouch = false;


    private void Update()
    {
        Movement();

    }

    

    public void onLanding()
    {
        animator.SetBool("isJumping", false);
    }

    public void onCrouching(bool isCrouch)
    {
        animator.SetBool("isCrouch", isCrouch);
    }

    private void FixedUpdate()
    {
        controller.Move(horisontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

    private void Movement()
    {
        horisontalMove = Input.GetAxis("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horisontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("isJumping", true);

        }
        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            cherries += 1;
            //cherryText.text = cherries.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            state = State.jump;
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (animator.GetBool("isJumping"))
            {
                 enemy.DeathTrigger();
            }  

        }
    }
}
