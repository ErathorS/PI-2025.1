using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameConection : MonoBehaviourPunCallbacks
{
    public Text chatLog;
    private void Awake()
    {
        chatLog.text += "\nconectando ao servidor...";
        PhotonNetwork.LocalPlayer.NickName = "Deus";
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        //Executa comandos que estão na função original
        base.OnConnectedToMaster();
        chatLog.text += "\nconectado ao servidor!";
        if (PhotonNetwork.InLobby)
        {
            chatLog.text += "\nEntrando no lobby...";
            PhotonNetwork.JoinLobby();
        }
    }
    public override void OnJoinedLobby()
    {
        chatLog.text += "\n entrou no loobby!";
        PhotonNetwork.JoinRoom("Desgraça");
        chatLog.text += "\n entrando na sala!";
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        chatLog.text += "\nErro ao entrrar na sala:" + message + " | codigo:" + returnCode;
        if (returnCode == ErrorCode.GameDoesNotExist)
        {
            chatLog.text += "\nCriando Sala...";
            RoomOptions room = new RoomOptions { MaxPlayers = 2 };
            //PhotonNetwork.CreateRoom("GameLoot", roomOptions, null);
        }
    }
    public override void OnJoinedRoom()
    {
        chatLog.text += "\nVocê entrou na sala Desgraça Seu nick name é" + PhotonNetwork.LocalPlayer.NickName;
        // aqui voce instancia o player na tela 
    }
    public override void OnLeftLobby()
    {
        chatLog.text += "\nJogador saiu da  sala";
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        chatLog.text += "\nJogador entrou na sala" + newPlayer.NickName; ;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        chatLog.text += "\nJogador entrou na sala" + otherPlayer.NickName; ;
    }
    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        chatLog.text += "\naconteceu um erro"+ errorInfo.Info;
    }
}

