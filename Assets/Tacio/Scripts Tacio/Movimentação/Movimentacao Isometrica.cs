using UnityEngine;
using Photon.Pun;

public class MovimentacaoIsometrica : MonoBehaviourPun
{
    [Header("Configurações de Movimento")]
    public float moveSpeed = 5f;
    public FixedJoystick joystick;

    [Header("Referências")]
    public Transform cameraTransform; // ← vamos usar a câmera local

    private Rigidbody rb;
    private Animator anim;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        // Se não foi setada manualmente, tenta pegar a câmera principal
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        HandleMovementInput();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void HandleMovementInput()
    {
        if (joystick == null) return;

        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        // Direção do input em relação à câmera
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        // Mantém o movimento no plano (sem subir ou descer)
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // Direção final no mundo (relativa à câmera)
        moveDirection = (camForward * vertical + camRight * horizontal).normalized;
    }

    private void Move()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            // Movimento suave
            Vector3 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);

            // Rotação independente: apenas se houver movimento
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f);

            anim?.SetBool("IsWalking", true);
        }
        else
        {
            anim?.SetBool("IsWalking", false);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
{
    NPCImportante npc = other.GetComponent<NPCImportante>();
    if (npc != null)
    {
        npc.IniciarDialogo();
    }
}

}
