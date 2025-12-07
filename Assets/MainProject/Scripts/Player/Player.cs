using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpPower = 7f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform groundCheck;

    Rigidbody2D rb;
    float input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        input = Input.GetAxisRaw("Horizontal");
        if(DialogueSystem.Instance.isDialogueActive) input = 0;
        Move();
    }

    void Move()
    {
        bool isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundMask);

        rb.linearVelocity = new Vector2(input * moveSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && isGround && !DialogueSystem.Instance.isDialogueActive)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
    }
}
