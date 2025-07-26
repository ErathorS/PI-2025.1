using Unity.Netcode;
using UnityEngine;

public class ConnectionManagerUI : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host iniciado");
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Cliente conectado");
    }

}
