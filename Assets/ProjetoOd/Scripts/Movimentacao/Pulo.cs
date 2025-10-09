using UnityEngine;
using UnityEngine.InputSystem;

public class Pulo : MonoBehaviour
{
    Rigidbody rb;
    public float jumpStrength = 2;
    [SerializeField] Animator animator;
    GroundCheck groundCheck;

    void Reset()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
        animator = GetComponentInChildren<Animator>();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Só pula se apertar o botão e estiver no chão
        if (context.performed && (!groundCheck || groundCheck.isGrounded))
        {
            rb.AddForce(Vector3.up * 100 * jumpStrength);
            animator.SetTrigger("Jump");
        }
    }
}
