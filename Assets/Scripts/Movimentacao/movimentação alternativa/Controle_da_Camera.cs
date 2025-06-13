using UnityEngine;

public class Controle_da_Camera : MonoBehaviour
{
    [SerializeField]
    private Transform character;
    public float sensitivity = 2f;
    public float smoothing = 1.5f;

    private Vector2 velocity;
    private Vector2 frameVelocity;
    public FixedJoystick joystickRotation;

    void Reset()
    {
        character = GetComponentInParent<Transform>();
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 mouseDelta = new Vector2(joystickRotation.Horizontal, joystickRotation.Vertical);
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1f / smoothing);
        velocity += frameVelocity;

        // Impede olhar para baixo (limita a rotação a no máximo olhar reto)
        velocity.y = Mathf.Clamp(velocity.y, -25f, 2f);
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}
