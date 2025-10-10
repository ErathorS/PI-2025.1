using UnityEngine;

public class MovimentacaoIsometrica : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float moveSpeed = 5f;
    public FixedJoystick joystick;

    private Rigidbody rb;
    private Animator anim;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovementInput();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void HandleMovementInput()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;
    }

    private void Move()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }
}
