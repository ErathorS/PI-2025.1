using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player_Move : MonoBehaviour
{
    [Header("Velocidade")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Referências")]
    [SerializeField] Animator animator;

    Rigidbody rb;
    float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // evita tombar
        rb.interpolation = RigidbodyInterpolation.Interpolate; // movimento suave

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        // Entrada
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        // Correr ou andar
        bool isRunning = Input.GetKey(runningKey);
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Movimento relativo à rotação do jogador
        Vector3 move = transform.TransformDirection(inputDir) * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        // Animações
        bool isWalking = inputDir.magnitude > 0;
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isWalking && isRunning);
    }
}
