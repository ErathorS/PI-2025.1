using Unity.Netcode;
using UnityEngine;

public class ConnectionManagerUI : MonoBehaviour
{
    // Método para iniciar como host
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host iniciado");
    }

    // Método para iniciar como cliente
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Cliente conectado");
    }
}
