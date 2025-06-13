using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player_Move : MonoBehaviourPunCallbacks
{
    [Header("Velocidade")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;

    [Header("Referências")]
    Animator animator;
    private Player _photonPlayer;
    private int _id;
    Rigidbody rb;
    Vector2 moveInput;
    bool isRunning;

    float currentSpeed;
    [PunRPC]
    public void Inicializa(Player player)
    {
        _photonPlayer = player;
        _id = player.ActorNumber;
        ManagerGamer.Instancia.Jogadores.Add(this);
        if (!photonView.IsMine)
            rb.isKinematic = true;
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    void FixedUpdate()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 move = transform.TransformDirection(inputDir) * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        bool isWalking = inputDir.magnitude > 0;
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isWalking && isRunning);
    }
}
