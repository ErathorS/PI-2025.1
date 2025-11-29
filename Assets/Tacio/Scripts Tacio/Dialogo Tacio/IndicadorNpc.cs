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

        // Se for NPC importante (tipo 2), registra progresso (com segurança)
        if (tipoExclamacao == 2)
        {
            if (progressoController != null)
            {
                progressoController.NPCImportanteConcluido();
                //Debug.Log($"[IndicadorNpc] NPC importante '{gameObject.name}' marcou como conversado e pediu progresso.");
            }
            else
            {
                //Debug.LogWarning($"[IndicadorNpc] Não foi possível registrar progresso para '{gameObject.name}' porque ProgressaoFaseController estava ausente.");
            }
        }
    }

    public void AtivarParaEntrega()
    {
        if (iconeExclamacao != null)
        {
            iconeExclamacao.SetActive(true);
            // Opcional: mudar cor ou ícone para indicar "entrega"
        }
    }
}
