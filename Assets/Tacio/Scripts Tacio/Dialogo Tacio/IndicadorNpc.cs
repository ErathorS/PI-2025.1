using UnityEngine;

public class IndicadorNpc : MonoBehaviour
{
    [Header("Configuração do Indicador")]
    public GameObject iconeExclamacao; // Referência ao objeto da exclamação
    public int tipoExclamacao = 1;     // 1 = NPC normal | 2 = NPC importante

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

    // Chamado pelo DialogoNPC quando o diálogo com esse NPC termina
    public void MarcarComoConversado()
    {
        if (jogadorJaConversou) return;

        jogadorJaConversou = true;

        // Esconde o ícone (local)
        if (iconeExclamacao != null)
            iconeExclamacao.SetActive(false);

        // 🔴 CORREÇÃO CRÍTICA: REMOVER a contagem automática de progresso aqui!
        // O progresso do NPC importante deve ser contado APENAS no DialogoNPC
        // após o diálogo de entrega, não automaticamente aqui.
        
        Debug.Log($"[IndicadorNpc] NPC '{gameObject.name}' marcado como conversado. " +
                 $"Tipo: {tipoExclamacao} | Progresso NÃO contado automaticamente.");
    }

    public void AtivarParaEntrega()
    {
        if (iconeExclamacao != null)
        {
            iconeExclamacao.SetActive(true);
            jogadorJaConversou = false; // 🔴 Permite mostrar o ícone novamente
            Debug.Log($"[IndicadorNpc] Indicador ativado para entrega - {gameObject.name}");
        }
    }

    // 🔴 NOVO: Método para reativar o indicador (usado no reset)
    public void ReativarIndicador()
    {
        if (iconeExclamacao != null)
        {
            iconeExclamacao.SetActive(true);
            jogadorJaConversou = false;
            Debug.Log($"[IndicadorNpc] Indicador reativado - {gameObject.name}");
        }
    }

    // 🔴 NOVO: Método para verificar estado atual (debug)
    public void DebugEstado()
    {
        Debug.Log($"[IndicadorNpc] {gameObject.name} - " +
                 $"Conversado: {jogadorJaConversou}, " +
                 $"Tipo: {tipoExclamacao}, " +
                 $"Ícone Ativo: {iconeExclamacao != null && iconeExclamacao.activeSelf}");
    }
}