using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpPower = 7f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform groundCheck;

    private SpriteRenderer spriteRenderer;
    private Animator anim;

    Rigidbody2D rb;
    float input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        input = Input.GetAxisRaw("Horizontal");
        if (DialogueSystem.Instance.isDialogueActive) input = 0;
        Move();
    }

    void Move()
    {
        if(input < 0) spriteRenderer.flipX = false;
        else if(input > 0) spriteRenderer.flipX = true;

        if(input == 0) anim.SetBool("IsMove", false);
        else anim.SetBool("IsMove", true);

        bool isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundMask);

        if(isGround) anim.SetBool("IsOnAir", false);
        else anim.SetBool("IsOnAir", true);

        rb.linearVelocity = new Vector2(input * moveSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && isGround && !DialogueSystem.Instance.isDialogueActive)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
    }
}
