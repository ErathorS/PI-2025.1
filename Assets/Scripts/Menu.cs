using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro.EditorUtilities;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class Menu : MonoBehaviourPunCallbacks
{
    [SerializeField] private MenuEntrada _menuEntrada;
    [SerializeField] private MenuLobby _menuLobby;
    private void Start()
    {
        _menuEntrada.gameObject.SetActive(false);
        _menuLobby.gameObject.SetActive(false);
    }
    public override void OnConnectedToMaster()
    {
        _menuEntrada.gameObject.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        MudaMenu(_menuLobby.gameObject);
        _menuLobby.AtualizarLista();
    }
    public void MudaMenu(GameObject menu)
    {
        _menuEntrada.gameObject.SetActive(false);
        _menuLobby.gameObject.SetActive(false);
        menu.SetActive(true);
    }
   
}
