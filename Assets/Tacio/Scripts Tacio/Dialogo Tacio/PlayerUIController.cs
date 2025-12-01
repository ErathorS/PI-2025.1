using UnityEngine;
using Photon.Pun;

public class PlayerUIController : MonoBehaviourPun
{
    [Header("Referências de UI")]
    public FixedJoystick joystick;
    public Canvas rootCanvas;

    void Awake()
    {
        if (!photonView.IsMine)
        {
            if (rootCanvas != null)
                rootCanvas.enabled = false;

            if (joystick != null)
                joystick.gameObject.SetActive(false);

            return;
        }

        if (rootCanvas == null)
            rootCanvas = GetComponentInChildren<Canvas>();

        if (rootCanvas != null)
        {
            rootCanvas.enabled = true;

            if (joystick != null)
            {
                joystick.gameObject.SetActive(true);
                Debug.Log("[PlayerUIController] Joystick ativado para o jogador local.");
            }
        }
        else
        {
            Debug.LogWarning("[PlayerUIController] Nenhum Canvas encontrado no jogador!");
        }
    }

    public FixedJoystick GetJoystick()
    {
        return joystick;
    }
}
