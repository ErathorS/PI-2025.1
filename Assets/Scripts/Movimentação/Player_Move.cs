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
    private Animator animator;
    private Player _photonPlayer;
    private int _id;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isRunning;

    private float currentSpeed;

    [PunRPC]
    public void Inicializa(Player player)
    {
        _photonPlayer = player;
        _id = player.ActorNumber;

        if (photonView.IsMine)
        {
            ManagerGamer.Instancia.Jogadores.Add(this);
        }
        else
        {
            rb.isKinematic = true;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogWarning("Animator não encontrado no jogador: " + gameObject.name);
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
        if (!photonView.IsMine) return; // Somente o jogador local controla

        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 move = transform.TransformDirection(inputDir) * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        if (animator != null)
        {
            bool isWalking = inputDir.magnitude > 0;
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isRunning", isWalking && isRunning);
        }
    }
}
