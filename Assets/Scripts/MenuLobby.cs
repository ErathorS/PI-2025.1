using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuLobby : MonoBehaviour
{
    [SerializeField] private Text _listaDeJogadores;
    [SerializeField] private Button _comecaJogo;

    public void AtualizarLista()
    {
        _listaDeJogadores.text = GestorDeRede.Instancia.ObterlistaDeJogadores();
        _comecaJogo.interactable = GestorDeRede.Instancia.DonoDaSala();
    }


}
