using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputNome;
    public TMP_InputField inputSala;
    public Button btnConectar;
    public Button btnCriarSala;
    public TMP_Text statusTxt;

    public GameObject listaSalasContent;
    public GameObject salaPrefab;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        btnConectar.onClick.AddListener(Conectar);
        btnCriarSala.onClick.AddListener(CriarSala);
    }

    void Conectar()
    {
        PhotonNetwork.NickName = inputNome.text;
        PhotonNetwork.ConnectUsingSettings();
        statusTxt.text = "Conectando...";
    }

    public override void OnConnectedToMaster()
    {
        statusTxt.text = "Conectado!";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        statusTxt.text = "No lobby!";
    }

    void CriarSala()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            string nomeSala = inputSala.text;
            RoomOptions op = new RoomOptions { MaxPlayers = 4 };
            PhotonNetwork.CreateRoom(nomeSala, op, TypedLobby.Default);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in listaSalasContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            GameObject novaSala = Instantiate(salaPrefab, listaSalasContent.transform);
            novaSala.GetComponentInChildren<TMP_Text>().text = room.Name;
            novaSala.GetComponent<Button>().onClick.AddListener(() => EntrarSala(room.Name));
        }
    }

    void EntrarSala(string nome)
    {
        PhotonNetwork.JoinRoom(nome);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}
