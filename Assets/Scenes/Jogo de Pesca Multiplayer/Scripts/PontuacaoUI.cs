using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class PontuacaoUI : MonoBehaviour
{
    public TextMeshProUGUI textoPontuacao;

    void Start()
    {
        GameManager.Instance.pontuacaoEquipe.OnValueChanged += AtualizarUI;
        AtualizarUI(0, GameManager.Instance.pontuacaoEquipe.Value);
    }

    void AtualizarUI(int anterior, int atual)
    {
        textoPontuacao.text = "Pontuação: " + atual;
    }
}
