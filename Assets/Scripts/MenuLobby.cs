using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class MenuLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text _listaDeJogadores;
    [SerializeField] private Button _comecaJogo;

    public object PhotonView { get; internal set; }

    [PunRPC]
    public void AtualizarLista()
    {
        _listaDeJogadores.text = GestorDeRede.Instancia.ObterlistaDeJogadores();
        _comecaJogo.interactable = GestorDeRede.Instancia.DonoDaSala();
    }


}
