using UnityEngine;

public class BotaoPescarUI : MonoBehaviour
{
    private JogadorPescador meuJogador; 

    private void Start()
    {
        // identifica o jogador local 
        foreach (var jp in FindObjectsByType<JogadorPescador>(FindObjectsSortMode.None))
        {
            if (jp.IsOwner)
            {
                meuJogador = jp;
                break;
            }
        }
    }

    // botão UI para iniciar a pesca
    public void ChamarPescar()
    {
        if (meuJogador != null)
        {
            meuJogador.PescarViaBotao();
        }
    }
}