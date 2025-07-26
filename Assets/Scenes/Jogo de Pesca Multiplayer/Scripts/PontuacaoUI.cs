using TMPro;
using UnityEngine;
using Unity.Netcode;

public class PontuacaoUI : MonoBehaviour
{
    public TextMeshProUGUI textoPontuacao; 

    void Start()
    {
        // mudança de pontuação e atualiza o texto inicial
        if (GameManager.Instance != null)
        {
            GameManager.Instance.pontuacaoEquipe.OnValueChanged += AtualizarPontuacao;
            AtualizarPontuacao(0, GameManager.Instance.pontuacaoEquipe.Value);
        }
    }

    // chamado quando a pontuação muda
    void AtualizarPontuacao(int antes, int depois)
    {
        textoPontuacao.text = "Pontuação: " + depois;
    }
}