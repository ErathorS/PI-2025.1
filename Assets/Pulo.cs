using UnityEngine;

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

    void LateUpdate()
    {
        if (Input.GetButtonDown("Jump") && (!groundCheck || groundCheck.isGrounded))
        {
            rb.AddForce(Vector3.up * 100 * jumpStrength);
            animator.SetTrigger("Jump");
        }
    }
}
