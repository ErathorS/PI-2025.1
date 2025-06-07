using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Move : MonoBehaviour
{
    [Header("Movimentação")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public bool canRun = true;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Pulo e Gravidade")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Referência da Câmera")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private Animator animator;
    public bool IsWalking { get; private set; }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // Verifica se o jogador está no chão
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Garante que o jogador permaneça no chão
        }

        // Entrada de movimento
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Direção relativa à câmera
        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        direction = Vector3.ClampMagnitude(direction, 1f);

        Vector3 move = cameraTransform.right * direction.x + cameraTransform.forward * direction.z;
        move.y = 0f; // Impede movimento vertical indesejado

        // Determina a velocidade atual
        float currentSpeed = canRun && Input.GetKey(runningKey) ? runSpeed : walkSpeed;

        // Move o jogador
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Pulo
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Aplica gravidade
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Atualiza o estado de animação
        IsWalking = direction.magnitude > 0f;
        animator.SetBool("isWalking", IsWalking);
    }
}
