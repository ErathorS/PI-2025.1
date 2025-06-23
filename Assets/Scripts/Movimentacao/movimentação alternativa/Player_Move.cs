using Photon.Pun;
using UnityEngine;

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

    [Header("Outros")]
    public Transform cameraTransform;
    [SerializeField] private GameObject _ui;
    public FixedJoystick joystickMovement;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    private Animator animator;
    private PhotonView _view;

    private Dialogo npcDialogoAtual; // <- Referência ao NPC atual

    private void Start()
    {
        // _view = GetComponent<PhotonView>();

        // if (!_view.IsMine)
        // {
        //     Destroy(cameraTransform.gameObject);
        //     Destroy(_ui);
        //     return;
        // }

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        // if (!_view.IsMine) return;

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = joystickMovement.Horizontal;
        float vertical = joystickMovement.Vertical;

        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        direction = Vector3.ClampMagnitude(direction, 1f);

        Vector3 move = cameraTransform.right * direction.x + cameraTransform.forward * direction.z;
        move.y = 0f;

        currentSpeed = canRun && Input.GetKey(runningKey) ? runSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        animator.SetBool("IsWalking", direction.magnitude > 0f);
    }

    public void OnJump()
    {
        // if (!_view.IsMine || !isGrounded) return;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (!_view.IsMine) return;

        // Detecta se colidiu com um NPC que tenha o script Dialogo
        Dialogo dialogo = other.GetComponent<Dialogo>();
        if (dialogo != null)
        {
            npcDialogoAtual = dialogo;
            npcDialogoAtual.MostrarBotao(); // <- Mostra botão no Canvas
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if (!_view.IsMine) return;

        Dialogo dialogo = other.GetComponent<Dialogo>();
        if (dialogo != null && dialogo == npcDialogoAtual)
        {
            npcDialogoAtual.EsconderTudo(); // <- Esconde botão/painel se sair
            npcDialogoAtual = null;
        }
    }
}
