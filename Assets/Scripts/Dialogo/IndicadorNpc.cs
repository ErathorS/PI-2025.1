using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicadorNpc : MonoBehaviour
{
    public GameObject iconeExclamacao; // Referência ao objeto da exclamação
    private bool jogadorJaConversou = false;

    // Chamado pelo script de diálogo quando o jogador iniciar a conversa
    public void MarcarComoConversado()
    {
        jogadorJaConversou = true;
        iconeExclamacao.SetActive(false); // Esconde a exclamação
    }

    void Start()
    {
        if (!jogadorJaConversou && iconeExclamacao != null)
        {
            iconeExclamacao.SetActive(true); // Mostra no início
        }
    }
}
