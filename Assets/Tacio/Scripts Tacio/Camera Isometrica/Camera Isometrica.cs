using UnityEngine;
using UnityEngine.EventSystems;

public class CameraIsometricaComRotacao : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;          // alvo da câmera
    public float distance = 10f;      // distância da câmera ao jogador
    public float height = 6f;         // altura da câmera
    public float rotationSpeed = 120f;
    public float followSpeed = 5f;

    private float currentRotationY;
    private Vector2 lastTouchPos;
    private bool isDragging = false;
    private bool initialized = false;

    private Vector3 initialOffset;

    void Start()
    {
        // Calcula o offset inicial com base na posição inicial da câmera e do player
        if (player != null)
        {
            initialOffset = transform.position - player.position;

            // Calcula o ângulo Y inicial baseado na posição da câmera
            Vector3 flatOffset = new Vector3(initialOffset.x, 0f, initialOffset.z);
            currentRotationY = Quaternion.LookRotation(-flatOffset).eulerAngles.y;

            initialized = true;
        }
    }

    void LateUpdate()
    {
        if (!initialized || player == null)
            return;

        HandleTouchRotation();
        UpdateCameraPosition();
    }

    void HandleTouchRotation()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // só rotaciona se tocar do lado direito da tela
            if (touch.position.x > Screen.width / 2)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    lastTouchPos = touch.position;
                    isDragging = true;
                }
                else if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    float deltaX = touch.position.x - lastTouchPos.x;
                    currentRotationY += deltaX * rotationSpeed * Time.deltaTime;
                    lastTouchPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                }
            }
        }
    }

    void UpdateCameraPosition()
    {
        // Calcula posição relativa
        Quaternion rotation = Quaternion.Euler(30f, currentRotationY, 0f);
        Vector3 offset = rotation * new Vector3(0, height, -distance);

        Vector3 targetPos = player.position + offset;

        // Movimento suave até a posição calculada
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}
