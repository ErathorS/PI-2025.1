using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class Player_Move : MonoBehaviourPun
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

    [Header("Referência do Joystick")]
    public FixedJoystick joystickMovement;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;

    private Animator animator;
    public bool IsWalking { get; private set; }

    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // Se este player não for o dono, desativa controles e câmera
        if (!photonView.IsMine)
        {
            GetComponent<Player_Move>().enabled = false;

            if (cameraTransform != null)
                cameraTransform.gameObject.SetActive(false);

            return;
        }

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // Só executa se for o dono do personagem
        if (!photonView.IsMine) return;

        // Verifica se está no chão
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Garante que o jogador fique grudado no chão
        }

        // Entrada de movimento via joystick
        float horizontal = joystickMovement.Horizontal;
        float vertical = joystickMovement.Vertical;

        // Direção com base na câmera
        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        direction = Vector3.ClampMagnitude(direction, 1f);

        Vector3 move = cameraTransform.right * direction.x + cameraTransform.forward * direction.z;
        move.y = 0f;

        // Determina velocidade atual
        currentSpeed = canRun && Input.GetKey(runningKey) ? runSpeed : walkSpeed;

        // Move o jogador
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Aplica gravidade
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Atualiza animação
        IsWalking = direction.magnitude > 0f;
        animator.SetBool("IsWalking", IsWalking);
    }

    public void OnJump()
    {
        // Apenas se for dono e estiver no chão
        if (!photonView.IsMine || !isGrounded) return;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}
