using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Move : MonoBehaviour
{
    private CharacterController controller;
    private Vector2 moveInput;
    private bool isRunning;

    public float walkSpeed = 5f;
    public float runSpeed = 9f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        float speed = isRunning ? runSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);
    }
}
