using UnityEngine;

public class Controle_da_Camera : MonoBehaviour
{
    [SerializeField]
    private Transform character;
    public float sensitivity = 2f;
    public float smoothing = 1.5f;
    private float initialYRotation;

    private Vector2 velocity;
    private Vector2 frameVelocity;
    public FixedJoystick joystickRotation;

    void Reset()
    {
        character = GetComponentInParent<Transform>();
    }

    void Start()
    {
        Cursor.visible = false;
        initialYRotation = character.localEulerAngles.y;
    }

    void Update()
    {
        Vector2 input = new Vector2(joystickRotation.Horizontal, joystickRotation.Vertical);

        if (input.magnitude > 0.1f)
        {
            Vector2 rawFrameVelocity = input * sensitivity;
            frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1f / smoothing);
            velocity += frameVelocity;

            velocity.y = Mathf.Clamp(velocity.y, -16f, 2f);
            transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
            character.localRotation = Quaternion.Euler(0, initialYRotation + velocity.x, 0);
        }
    }

    public void ResetCamera()
    {
        velocity = Vector2.zero;
        character.localRotation = Quaternion.Euler(0, initialYRotation, 0);
        transform.localRotation = Quaternion.identity;
    }
}
