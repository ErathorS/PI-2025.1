using TMPro;
using UnityEngine;
using Unity.Netcode;

public class PontuacaoUI : MonoBehaviour
{
    public TextMeshProUGUI textoPontuacao;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.pontuacaoEquipe.OnValueChanged += AtualizarPontuacao;
            AtualizarPontuacao(0, GameManager.Instance.pontuacaoEquipe.Value);
        }
    }

    void AtualizarPontuacao(int antes, int depois)
    {
        textoPontuacao.text = "Pontuação: " + depois;
    }
}
