using UnityEngine;
using Photon.Pun;

public class PlayerUIController : MonoBehaviourPun
{
    [Header("Referências de UI")]
    public FixedJoystick joystick;
    public Canvas rootCanvas;

    void Awake()
    {
        if (FindObjectsOfType<PlayerUIController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }   
        if (rootCanvas == null)
            rootCanvas = GetComponentInChildren<Canvas>();

        // 🔹 Garante que apenas o jogador local tenha a UI ativa
        if (rootCanvas != null)
        {
            if (photonView.IsMine)
            {
                rootCanvas.enabled = true;
                if (joystick != null)
                {
                    joystick.gameObject.SetActive(true);
                    Debug.Log($"[PlayerUIController] Joystick ativado para jogador local");
                }
            }
            else
            {
                rootCanvas.enabled = false;
                if (joystick != null)
                    joystick.gameObject.SetActive(false);
            }
        }
    }

    public FixedJoystick GetJoystick()
    {
        return joystick;
    }
}