using Photon.Pun;
using UnityEngine;

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
    private Dialogo npcDialogoAtual;

    private void Start()
    {
        _view = GetComponent<PhotonView>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (!_view.IsMine)
        {
            if (cameraTransform != null) Destroy(cameraTransform.gameObject);
            if (_ui != null) Destroy(_ui);
            return;
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (joystickMovement == null)
        {
            Debug.LogWarning("Joystick não atribuído no Player_Move.");
        }
    }

    private void Update()
    {
        if (!_view.IsMine) return;

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = joystickMovement != null ? joystickMovement.Horizontal : 0f;
        float vertical = joystickMovement != null ? joystickMovement.Vertical : 0f;

        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        direction = Vector3.ClampMagnitude(direction, 1f);

        Vector3 move = cameraTransform.right * direction.x + cameraTransform.forward * direction.z;
        move.y = 0f;

        currentSpeed = (canRun && Input.GetKey(runningKey)) ? runSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (animator != null)
        {
            bool isWalking = direction.magnitude > 0.1f;
            animator.SetBool("IsWalking", isWalking);
        }
    }

    public void OnJump()
    {
        if (!_view.IsMine || !isGrounded) return;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
    private void OnTriggerEnter(Collider other)
    {
        Dialogo dialogo = other.GetComponent<Dialogo>();
        if (dialogo != null)
        {
            npcDialogoAtual = dialogo;
            npcDialogoAtual.MostrarBotao(gameObject); // envia o jogador que entrou
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_view.IsMine) return;

        if (other.CompareTag("Empurravel"))
        {
            animator.SetBool("IsPushing", false);
            return;
        }

        Dialogo dialogo = other.GetComponentInParent<Dialogo>() ?? other.GetComponentInChildren<Dialogo>();
        if (dialogo != null && dialogo == npcDialogoAtual)
        {
            npcDialogoAtual.EsconderTudo();
            npcDialogoAtual = null;
        }
    }

}
