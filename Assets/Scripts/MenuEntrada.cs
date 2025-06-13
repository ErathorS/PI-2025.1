using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuEntrada : MonoBehaviour
{
    [SerializeField] private Text _nomeDoJogador;
    [SerializeField] private Text _nomeDaSala;
    public void CriarSala()
    {
        GestorDeRede.Instancia.CriarSala(_nomeDaSala.text);
        GestorDeRede.Instancia.MudarNick(_nomeDoJogador.text);
    }
    public void EntrarSala()
    {
          GestorDeRede.Instancia.MudarNick(_nomeDoJogador.text);
          GestorDeRede.Instancia.EntrarSala(_nomeDaSala.text);
    }
}
