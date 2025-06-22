using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // Todos vão para a mesma cena
        PhotonNetwork.ConnectUsingSettings();        // Conecta ao servidor
    }

    public override void OnConnectedToMaster()
    {
        Debug.LogError("Oi");
        PhotonNetwork.JoinLobby(); // Entra no lobby
    }

    public override void OnJoinedLobby()
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom("Sala1", options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("PI"); // Entra na cena de jogo
    }
}
