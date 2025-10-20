using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class MovimentacaoIsometrica : MonoBehaviourPun
{
    [Header("Configurações de Movimento")]
    public float moveSpeed = 5f;
    public FixedJoystick joystick;

    [Header("Referências")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private Animator anim;
    private Vector3 moveDirection;
    private bool referenciasConfiguradas = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        // 🔹 Apenas o jogador local controla o movimento
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        // 🔹 Tenta buscar as referências imediatamente
        BuscarReferenciasAutomaticamente();
    }

    void Update()
    {
        // 🔹 Verifica se as referências estão configuradas antes de mover
        if (!photonView.IsMine || !referenciasConfiguradas)
        {
            // Se ainda não tem referências, tenta buscar novamente
            if (photonView.IsMine && !referenciasConfiguradas)
            {
                BuscarReferenciasAutomaticamente();
            }
            return;
        }

        HandleMovementInput();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine || !referenciasConfiguradas)
            return;

        Move();
    }

    private void BuscarReferenciasAutomaticamente()
    {
        bool encontrouTudo = true;

        // 🔹 Busca o joystick pela tag correta
        if (joystick == null)
        {
            int actorID = PhotonNetwork.LocalPlayer.ActorNumber;
            string canvasTag = actorID == 1 ? "CanvasP1" : "CanvasP2";
            
            GameObject canvasObj = GameObject.FindGameObjectWithTag(canvasTag);
            if (canvasObj != null)
            {
                joystick = canvasObj.GetComponentInChildren<FixedJoystick>();
                if (joystick != null)
                {
                    Debug.Log($"[MovimentacaoIsometrica] Joystick encontrado via tag '{canvasTag}'");
                }
            }
            
            if (joystick == null)
            {
                // Fallback: busca em toda a cena
                joystick = FindObjectOfType<FixedJoystick>();
                if (joystick != null)
                {
                    Debug.Log($"[MovimentacaoIsometrica] Joystick encontrado via busca geral");
                }
            }
        }

        if (joystick == null)
        {
            Debug.LogWarning($"[MovimentacaoIsometrica] Joystick ainda não encontrado para player {PhotonNetwork.LocalPlayer.ActorNumber}");
            encontrouTudo = false;
        }

        // 🔹 Busca a câmera
        if (cameraTransform == null)
        {
            CameraIsometricaComRotacao camScript = FindObjectOfType<CameraIsometricaComRotacao>();
            if (camScript != null)
            {
                cameraTransform = camScript.transform;
                Debug.Log($"[MovimentacaoIsometrica] Câmera encontrada");
            }
        }

        if (cameraTransform == null)
        {
            Debug.LogWarning($"[MovimentacaoIsometrica] Câmera ainda não encontrada");
            encontrouTudo = false;
        }

        referenciasConfiguradas = encontrouTudo;
        
        if (referenciasConfiguradas)
        {
            Debug.Log($"[MovimentacaoIsometrica] Todas as referências configuradas para {gameObject.name}");
        }
    }

    private void HandleMovementInput()
    {
        if (joystick == null || cameraTransform == null) return;

        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * vertical + camRight * horizontal).normalized;
    }

    private void Move()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f);

            anim?.SetBool("IsWalking", true);
        }
        else
        {
            anim?.SetBool("IsWalking", false);
        }
    }

    // 🔹 Método chamado pelo NetworkGameManager
    public void ConfigurarReferencias(FixedJoystick novoJoystick, Transform novaCamera)
    {
        joystick = novoJoystick;
        cameraTransform = novaCamera;
        referenciasConfiguradas = (joystick != null && cameraTransform != null);

        Debug.Log($"[MovimentacaoIsometrica] Referências configuradas manualmente para {gameObject.name}. " +
                 $"Joystick: {joystick != null}, Câmera: {cameraTransform != null}");
    }

    private void OnTriggerEnter(Collider other)
    {
        NPCImportante npc = other.GetComponent<NPCImportante>();
        if (npc != null)
        {
            npc.IniciarDialogo();
        }
    }
}