using TMPro;
using UnityEngine;
using Unity.Netcode;

public class PontuacaoUI : MonoBehaviour
{
    public TextMeshProUGUI textoPontuacao; // Referência ao texto UI que mostra a pontuação

    void Start()
    {
        // Inscreve no evento de mudança de pontuação e atualiza o texto inicial
        if (GameManager.Instance != null)
        {
            GameManager.Instance.pontuacaoEquipe.OnValueChanged += AtualizarPontuacao;
            AtualizarPontuacao(0, GameManager.Instance.pontuacaoEquipe.Value);
        }
    }

    // Método chamado quando a pontuação muda
    void AtualizarPontuacao(int antes, int depois)
    {
        textoPontuacao.text = "Pontuação: " + depois;
    }
}