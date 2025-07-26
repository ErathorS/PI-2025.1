using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkUI : MonoBehaviour
{
    // public string jogoCenaNome = "Jogo de Pesca";
    // public string lobbyCenaNome = "Lobby";

    private void Start()
    {
        //NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        // if (NetworkManager.Singleton.StartHost())
        // {
        //     NetworkManager.Singleton.SceneManager.LoadScene(jogoCenaNome, LoadSceneMode.Single);
        // }
    }

    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        //NetworkManager.Singleton.SceneManager.LoadScene(lobbyCenaNome, LoadSceneMode.Single);
    }

    // private void OnClientConnected(ulong clientId)
    // {
    //     if (NetworkManager.Singleton.ConnectedClients.Count > 4)
    //     {
    //         Debug.Log("Limite de jogadores atingido. Desconectando...");
    //         if (NetworkManager.Singleton.LocalClientId == clientId)
    //         {
    //             NetworkManager.Singleton.Shutdown();
    //         }
    //     }
    // }
}
