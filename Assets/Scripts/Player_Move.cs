using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class Player_Move : MonoBehaviour
{
    [Header("Velocidade")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Referências")]
    [SerializeField] Animator animator;
    [SerializeField] CinemachineFreeLook freeLookCamera;

    Rigidbody rb;
    float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        bool isRunning = Input.GetKey(runningKey);
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Rotaciona o personagem para a direção da camera
        if (inputDir.magnitude > 0.1f)
        {
            // Obtém a orientação da camera no eixo Y
            Vector3 camForward = freeLookCamera.transform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = freeLookCamera.transform.right;
            camRight.y = 0;
            camRight.Normalize();

            Vector3 moveDir = camForward * v + camRight * h;
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 10f * Time.fixedDeltaTime));

            // Movimento
            Vector3 move = moveDir.normalized * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }

        // Animações
        bool isWalking = inputDir.magnitude > 0;
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isWalking && isRunning);
    }
    // Script para controlar a camera com Cinemachine FreeLook opcionalmente
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class Controle_da_Camera : MonoBehaviour
    {
        CinemachineFreeLook freeLookCam;

        void Awake()
        {
            freeLookCam = GetComponent<CinemachineFreeLook>();
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            freeLookCam.m_XAxis.Value += mouseX;
            freeLookCam.m_YAxis.Value -= mouseY;
        }
    }

}