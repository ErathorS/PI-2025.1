using UnityEngine;

public class BotaoPescarUI : MonoBehaviour
{
    private JogadorPescador meuJogador; // Referência ao jogador local

    private void Start()
    {
        // Encontra o jogador local entre todos os jogadores na cena
        foreach (var jp in FindObjectsByType<JogadorPescador>(FindObjectsSortMode.None))
        {
            if (jp.IsOwner)
            {
                meuJogador = jp;
                break;
            }
        }
    }

    // Método chamado pelo botão UI para iniciar a pesca
    public void ChamarPescar()
    {
        if (meuJogador != null)
        {
            meuJogador.PescarViaBotao();
        }
    }
}