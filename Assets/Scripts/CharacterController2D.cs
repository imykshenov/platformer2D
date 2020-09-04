using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// ���������� ����, ������� ����������� ��� ������ ������.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// ������������ �������� ���������� ���������. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// ����������� �������� ���������.
	[SerializeField] private bool m_AirControl = false;							// ����� �� ����� ��������� ���������� �� ����� ������;
	[SerializeField] private LayerMask m_WhatIsGround;                          // �����(����) ��� ����� ����� ��� ���������A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// ������� ���������, ��� ����������� �� ����� �� ��������.
	[SerializeField] private Transform m_CeilingCheck;							// �������� ������� ��������� ��� ������������, ��������� �������.
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// ��������� ������� ��������� (boxcollider) ����� �������� ������.

	const float k_GroundedRadius = .1f; // ������ ����� ����������, ��� ���� ����� ���������� �� ����� �� ��������.
	private bool m_Grounded;            // ��������� �� �� ����� ��������.
	const float k_CeilingRadius = .2f; // ������ ����� ���������, ������ �� ����� ������
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // ��� ����������� � ����� ������� �������� �����
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// ������ ���������� ��� �� �� �����, ���� cirkulcollider ���������� ��� ������, ��� ���������� ��� �����
		// ��� ����� ����� ������������ ����, �� Sample Assets �� ����� �������������� � �������� �������.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool crouch, bool jump)
	{
		// ������ �� �������� ������ ���� �� ������
		if (crouch)
		{
			// ���� ��� ���������� ���� �������, ������� ������ ��� ������, ������� ��� ���������.
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//��� ���������� ���������� ���� �� �������� ��� ��������� � �������
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// ��������� �������� � ������� ���������
				move *= m_CrouchSpeed;

				// ��������� ���� �� ����������� �� ����� ����������
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// �������� ��������� ����� �� ������
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// �������� ��������� � ������� ��� ������� �������� 
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// ����������� � ���������� � ���������
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// ������������ ����������� �������� ���������, ����� �� �������������
			if (move > 0 && !m_FacingRight)
			{
				// ���������
				Flip();
			}
			// ������� � ������ �������
			else if (move < 0 && m_FacingRight)
			{

				Flip();
			}
		}
		// �������� �� ������, ������ �������� 
		if (m_Grounded && jump)
		{
			// ���������� ��� ������������ ����
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
	}

	// ������� ��� ��������� ���������
	private void Flip()
	{

		m_FacingRight = !m_FacingRight;


		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}


/*
public class PlayerController : MonoBehaviour
{
    //����������, ������������������ � start()
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;



    //����������, ������������ � Inspector
    [SerializeField] private LayerMask ground;
    [SerializeField] private float Speed = 5.0f;
    [SerializeField] private float JumpForce = 10.0f;
    [SerializeField] private int cherries = 0;
    [SerializeField] private Text cherryText;
    [SerializeField] private float inputDamage = 5.0f;

    // ������������ ���������, �� ��������� idle
    private enum State { idle, run, jump, fall, hurt }
    private State state = State.idle;


    private void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }


    private void Update()
    {
        // ������ ��������
        if (state != State.hurt)
        {
            Movement();
        }

        AnimationState(); // ���������� �������, ������� ������ �������� �������.
        animator.SetInteger("state", (int)state); // ���������� ����� �����, �.�. � �������� ���� ��������� � ������(������� ��������)

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            cherries += 1;
            cherryText.text = cherries.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (state == State.fall)
            {
                enemy.DeathTrigger();
                Jump();
            }
            else
            {
                state = State.hurt;
                if (collision.gameObject.transform.position.x > transform.position.x)
                {
                    //�������� ����� �� �����
                    rb.velocity = new Vector2(-inputDamage, rb.velocity.y);
                }
                else
                {
                    // �������� ������ �� �����
                    rb.velocity = new Vector2(inputDamage, rb.velocity.y);
                }
            }
        }
    }


    private void Movement()
    {
        float Hdirection = Input.GetAxis("Horizontal");
        // �������� ������
        if (Hdirection < 0)
        {
            rb.velocity = new Vector2(-Speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }
        // �������� �����
        else if (Hdirection > 0)
        {
            rb.velocity = new Vector2(Speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }
        // ������
        if (Input.GetButtonDown("Jump") && col.IsTouchingLayers(ground))
        {
            Jump();
        }

    }

    private void Jump()
    {
        rb.velocity = new Vector2(0, JumpForce);
        state = State.jump;
    }
    // ������ �������� ������� ��� ��������
    private void AnimationState()
    {
        if (state == State.jump)
        {
            if (rb.velocity.y < 0.1f)
            {
                state = State.fall;
            }
        }
        else if (state == State.fall)
        {
            if (col.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 2.0f)
        {
            state = State.run;
        }
        else
        {
            state = State.idle;
        }
    }
}

*/