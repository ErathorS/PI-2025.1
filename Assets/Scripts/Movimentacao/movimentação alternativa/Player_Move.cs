using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class Player_Move : MonoBehaviourPun
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public bool canRun = true;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("References")]
    public Transform cameraTransform;
    public FixedJoystick joystickMovement;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;

    private Animator animator;
    public bool IsWalking { get; private set; }
    private bool isPushing;

    void Start()
    {
        if (!photonView.IsMine)
        {
            // Desativa componentes locais em jogadores remotos
            enabled = false;
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
        if (!photonView.IsMine) return;

        HandleGroundCheck();
        HandleMovement();
        HandleGravity();
        HandleAnimations();
    }

    private void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleMovement()
    {
        Vector2 input = joystickMovement.Direction;
        Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            Vector3 move = cameraTransform.right * direction.x + cameraTransform.forward * direction.z;
            move.y = 0f;
            currentSpeed = canRun && Input.GetKey(runningKey) ? runSpeed : walkSpeed;
            controller.Move(move * currentSpeed * Time.deltaTime);
        }
    }

    private void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleAnimations()
    {
        // Atualiza parâmetros locais - PhotonAnimatorView cuidará da sincronização
        IsWalking = joystickMovement.Direction.magnitude > 0.1f;
        animator.SetBool("IsWalking", IsWalking);

        // Detecção de empurrar
        isPushing = false;
        if (IsWalking)
        {
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
            Vector3 moveDirection = new Vector3(joystickMovement.Horizontal, 0, joystickMovement.Vertical).normalized;
            
            if (Physics.Raycast(rayOrigin, moveDirection, out hit, 1.5f))
            {
                PushableObject pushable = hit.collider.GetComponent<PushableObject>();
                if (pushable != null)
                {
                    isPushing = true;
                    pushable.photonView.RPC("UpdatePushDirection", RpcTarget.MasterClient,
                        PhotonNetwork.LocalPlayer.ActorNumber, moveDirection);
                }
            }
        }

        animator.SetBool("IsPushing", isPushing);
    }

    public void OnJump()
    {
        if (!photonView.IsMine || !isGrounded) return;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}