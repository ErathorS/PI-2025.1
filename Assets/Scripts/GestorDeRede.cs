using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class GestorDeRede : MonoBehaviourPunCallbacks
{
    public static GestorDeRede Instancia { get; private set; }
   
    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            gameObject.SetActive(false);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Deu certo");
    }
    public void CriarSala(string nomeSala)
    {
        PhotonNetwork.CreateRoom(nomeSala);
    }
    public void EntrarSala(string nomeSala)
    {
        PhotonNetwork.JoinRoom(nomeSala);
    }
    public void MudarNick(string nickname)
    {
        PhotonNetwork.NickName = nickname;
    }
    public string ObterlistaDeJogadores()
    {
        var lista = "";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            lista += player.NickName + "\n";
        }
        return lista;
    }
    public bool DonoDaSala()
    {
        return PhotonNetwork.IsMasterClient;
    }
    public void SairDoLobby()
    {
        PhotonNetwork.LeaveRoom();
    }
    [PunRPC]
    public void Comecajogo(string nomeCena)
    {
        PhotonNetwork.LoadLevel(nomeCena);
    }

}
