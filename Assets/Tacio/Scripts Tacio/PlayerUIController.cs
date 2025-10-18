using UnityEngine;
using Photon.Pun;

public class PlayerUIController : MonoBehaviour
{
    public FixedJoystick joystick;
    public Canvas rootCanvas;

    void Awake()
    {
        if (rootCanvas == null)
            rootCanvas = GetComponentInChildren<Canvas>();

        // Marca o canvas como "CanvasP1" ou "CanvasP2" se ainda não estiver
        if (rootCanvas != null && rootCanvas.gameObject.tag != "CanvasP1" && rootCanvas.gameObject.tag != "CanvasP2")
        {
            int actor = PhotonNetwork.LocalPlayer.ActorNumber;
            rootCanvas.gameObject.tag = actor == 1 ? "CanvasP1" : "CanvasP2";
        }
    }
}
