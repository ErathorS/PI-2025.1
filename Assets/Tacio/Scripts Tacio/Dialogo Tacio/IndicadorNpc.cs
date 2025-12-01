using UnityEngine;

public class IndicadorNpc : MonoBehaviour
{
    [Header("Configuração do Indicador")]
    public GameObject iconeExclamacao; 
    public int tipoExclamacao = 1;    
    private bool jogadorJaConversou = false;
    private ProgressaoFaseController progressoController;

    void Start()
    {
        progressoController = FindObjectOfType<ProgressaoFaseController>();

        if (!jogadorJaConversou && iconeExclamacao != null)
        {
            iconeExclamacao.SetActive(true);
        }

        if (tipoExclamacao == 2 && progressoController == null)
        {
            Debug.LogWarning($"[IndicadorNpc] NPC importante ({gameObject.name}) mas nenhum ProgressaoFaseController encontrado na cena!");
        }
    }

    public void MarcarComoConversado()
    {
        if (jogadorJaConversou) return;

        jogadorJaConversou = true;

        if (iconeExclamacao != null)
            iconeExclamacao.SetActive(false);

        
        Debug.Log($"[IndicadorNpc] NPC '{gameObject.name}' marcado como conversado. " +
                 $"Tipo: {tipoExclamacao} | Progresso NÃO contado automaticamente.");
    }

    public void AtivarParaEntrega()
    {
        if (iconeExclamacao != null)
        {
            iconeExclamacao.SetActive(true);
            jogadorJaConversou = false; 
            Debug.Log($"[IndicadorNpc] Indicador ativado para entrega - {gameObject.name}");
        }
    }

    public void ReativarIndicador()
    {
        if (iconeExclamacao != null)
        {
            iconeExclamacao.SetActive(true);
            jogadorJaConversou = false;
            Debug.Log($"[IndicadorNpc] Indicador reativado - {gameObject.name}");
        }
    }

    public void DebugEstado()
    {
        Debug.Log($"[IndicadorNpc] {gameObject.name} - " +
                 $"Conversado: {jogadorJaConversou}, " +
                 $"Tipo: {tipoExclamacao}, " +
                 $"Ícone Ativo: {iconeExclamacao != null && iconeExclamacao.activeSelf}");
    }
}