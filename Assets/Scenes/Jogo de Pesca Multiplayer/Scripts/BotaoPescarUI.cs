using UnityEngine;

public class BotaoPescarUI : MonoBehaviour
{
    private JogadorPescador meuJogador;

    private void Start()
    {
        foreach (var jp in FindObjectsByType<JogadorPescador>(FindObjectsSortMode.None))
        {
            if (jp.IsOwner)
            {
                meuJogador = jp;
                break;
            }
        }
    }

    public void ChamarPescar()
    {
        if (meuJogador != null)
        {
            meuJogador.PescarViaBotao();
        }
    }
}
