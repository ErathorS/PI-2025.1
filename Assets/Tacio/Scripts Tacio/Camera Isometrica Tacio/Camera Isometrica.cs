using UnityEngine;

public class CameraIsometricaComRotacao : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;

    [Header("Configurações")]
    public float distance = 10f;
    public float height = 6f;
    public float rotationSpeed = 120f;
    public float smoothTime = 0.15f;

    private float currentRotationY;
    private Vector2 lastTouchPos;
    private bool isDragging = false;
    private bool initialized = false;

    private Vector3 velocity = Vector3.zero;
    private Vector3 fixedOffset;

    void Start()
    {
        if (player != null)
        {
            currentRotationY = transform.eulerAngles.y;

            fixedOffset = Quaternion.Euler(30f, currentRotationY, 0f) * 
                          new Vector3(0, height, -distance);

            initialized = true;
        }
    }

    void LateUpdate()
    {
        if (!initialized || player == null) return;

        HandleTouchRotation();
        UpdateCameraPosition();
    }

    void HandleTouchRotation()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

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

                    fixedOffset = Quaternion.Euler(30f, currentRotationY, 0f) *
                                  new Vector3(0, height, -distance);

                    lastTouchPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isDragging = false;
                }
            }
        }
    }

    void UpdateCameraPosition()
    {
        Vector3 targetPos = player.position + fixedOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );

        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}
