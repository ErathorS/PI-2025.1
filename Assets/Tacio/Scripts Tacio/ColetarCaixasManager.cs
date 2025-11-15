using UnityEngine;

public class ColetarCaixasManager : MonoBehaviour
{
    [Header("Configurações")]
    public int caixasTotais = 5;       // Total de caixas da missão
    public int caixasColetadas = 0;    // Contador atual

    [Header("UI")]
    public TMPro.TMP_Text textoContadorCaixas; // arraste o texto "Jornais Coletados", etc

    // Chamado por cada caixa quando for coletada
    public void RegistrarColeta()
    {
        caixasColetadas++;

        // Atualiza o texto da UI
        if (textoContadorCaixas != null)
            textoContadorCaixas.text = $"{caixasColetadas}/{caixasTotais}";

        // Se todas foram coletadas → missão concluída
        if (caixasColetadas >= caixasTotais)
        {
            MissaoFase1Manager.instancia.MissaoFinalizada();
        }
    }

    // Chamado quando o Master clica em "Resetar Missão"
    public void ResetarCaixas()
    {
        caixasColetadas = 0;

        // Atualiza UI
        if (textoContadorCaixas != null)
            textoContadorCaixas.text = $"0/{caixasTotais}";

        // Faz todas as caixas aparecerem novamente
        CaixaDeIngrediente[] caixas = FindObjectsOfType<CaixaDeIngrediente>();

        foreach (var caixa in caixas)
        {
            caixa.ResetarEstado();
        }
    }
}
