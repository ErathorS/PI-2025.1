using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MissaoHUD : MonoBehaviour
{
    [Header("Referências")]
    public Transform painelMissoes;
    public GameObject prefabMissao;

    private List<GameObject> missoesAtuais = new List<GameObject>();

    void Start()
    {
        AdicionarMissao("Fale com a baiana do acarajé");
        AdicionarMissao("Empurre a caixa até o marcador");
        AdicionarMissao("Colete 3 frutas da praça");
    }

    public void AdicionarMissao(string descricao)
    {
        GameObject novaMissao = Instantiate(prefabMissao, painelMissoes);
        TMP_Text texto = novaMissao.GetComponentInChildren<TMP_Text>();
        texto.text = descricao;

        Button botao = novaMissao.GetComponentInChildren<Button>();
        botao.onClick.AddListener(() => ConcluirMissao(novaMissao));

        missoesAtuais.Add(novaMissao);
    }

    public void ConcluirMissao(GameObject missao)
    {
        missoesAtuais.Remove(missao);
        Destroy(missao);
    }

    // 🟢 Método novo — conclui a missão pelo texto
    public void ConcluirMissaoPorDescricao(string descricao)
    {
        foreach (var missao in missoesAtuais)
        {
            TMP_Text texto = missao.GetComponentInChildren<TMP_Text>();
            if (texto.text == descricao)
            {
                ConcluirMissao(missao);
                break;
            }
        }
    }
}
