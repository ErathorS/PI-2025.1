using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class DialogoNPC : MonoBehaviourPun
{
    [Header("Referências")]
    public IndicadorNpc indicadorNPC;
    [TextArea(2, 4)] public string[] linhasDialogo;

    [Header("Configuração de Acesso")]
    public bool apenasMasterPodeDialogar = false;

    [Header("Diálogos Alternativos (Entrega)")]
    public string[] dialogoAposEntrega;
    private bool dialogoDeEntregaAtivo = false;

    [Header("Configuração Fase 2")]
    public bool ehNPCFase2 = false;
    public bool tarefaConcluida = false;
    private bool missaoIniciada = false;

    private int linhaAtual = 0;
    private bool dialogoAtivo = false;
    private bool missaoJaEntregue = false;

    private GameObject jogadorAtual;
    private TMP_Text textoDialogo;
    private GameObject painelDialogo;
    private Button botaoDialogo;

    private void Start()
    {
        if (painelDialogo != null)
            painelDialogo.SetActive(false);
    }

    private void FinalizarDialogo()
    {
        
        // 🔴 ADICIONE ESTE DEBUG PARA VERIFICAR
        Debug.Log($"[DialogoNPC] FinalizarDialogo chamado - " +
        $"EntregaAtivo: {dialogoDeEntregaAtivo}, " +
        $"Fase2: {ehNPCFase2}, " +
        $"MissaoEntregue: {missaoJaEntregue}, " +
        $"TarefaConcluida: {tarefaConcluida}");

        painelDialogo.SetActive(false);
        dialogoAtivo = false;
        painelDialogo.SetActive(false);
        dialogoAtivo = false;

        // Marca como já conversado
        if (indicadorNPC != null)
        {
            indicadorNPC.MarcarComoConversado();
            photonView.RPC("DesativarIndicadorGlobal", RpcTarget.AllBuffered);
        }

        // --- SE FOR DIÁLOGO DE ENTREGA ---
        if (dialogoDeEntregaAtivo)
        {
            missaoJaEntregue = true;
            
            // 🔴 CORREÇÃO: Só conta progresso na fase 2 após entrega
            if (ehNPCFase2)
            {
                ContarProgressoNPCImportante();
            }
            else if (MissaoFase1Manager.instancia != null)
            {
                MissaoFase1Manager.instancia.FinalizarEntrega();
            }
            return;
        }

        // --- SE FOR NPC DA FASE 2 (diálogo inicial) ---
        if (ehNPCFase2 && !missaoJaEntregue && !missaoIniciada)
        {
            // Inicia a tarefa de sincronização
            if (MissaoFase2Manager.instancia != null)
            {
                MissaoFase2Manager.instancia.IniciarMissao();
                missaoIniciada = true;
                
                // 🔴 CORREÇÃO CRÍTICA: NÃO contar progresso aqui!
                // Apenas marcar que iniciou, sem chamar ProgressaoFaseController
                Debug.Log("[DialogoNPC] Missão da Fase 2 iniciada via manager. Progresso NÃO contado.");
            }
            else
            {
                Debug.LogError("[DialogoNPC] MissaoFase2Manager.instancia é null!");
            }
            return;
        }

        // --- SE ESTE NPC INICIA A MISSÃO (lógica original da Fase 1) ---
        if (indicadorNPC != null && indicadorNPC.tipoExclamacao == 2 && !ehNPCFase2)
        {
            if (missaoJaEntregue) return;

            var dialogoIntroducao = FindObjectOfType<FuroDeNoticia.DialogoNPCIntroducao>();
            if (dialogoIntroducao != null)
            {
                int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
                dialogoIntroducao.MarcarDialogoConcluido(playerID);
            }

            if (PhotonNetwork.IsMasterClient && MissaoFase1Manager.instancia != null)
            {
                MissaoFase1Manager.instancia.IniciarMissao();
            }
        }
    }

    // No método ContarProgressoNPCImportante, adicione uma verificação extra:
    private void ContarProgressoNPCImportante()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        var progresso = FindObjectOfType<ProgressaoFaseController>();
        if (progresso != null)
        {
            progresso.NPCImportanteConcluido();
            Debug.Log("[DialogoNPC] ✅✅✅ Progresso do NPC importante contado APÓS ENTREGA DA TAREFA!");
            
            // 🔴 CORREÇÃO: Forçar sincronização imediatamente após contar progresso
            progresso.ForcarSincronizacao();
        }
        else
        {
            Debug.LogError("[DialogoNPC] ProgressaoFaseController não encontrado!");
        }
    }

    // 🔴 Chamado quando a sincronização é concluída
    public void TarefaConcluida()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        photonView.RPC("RPC_TarefaConcluida", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_TarefaConcluida()
    {
        tarefaConcluida = true;
        Debug.Log("[DialogoNPC] Tarefa de sincronização concluída! Agora pode fazer diálogo de entrega.");
        
        // Atualiza o indicador para mostrar que pode conversar novamente
        if (indicadorNPC != null)
        {
            indicadorNPC.AtivarParaEntrega();
        }
    }

    // 🔴 Método para Fase 1 (mantido para compatibilidade)
    public void AtivarDialogoDeEntrega()
    {
        if (!ehNPCFase2) // Só ativa para NPCs da Fase 1
        {
            dialogoDeEntregaAtivo = true;
            Debug.Log("[DialogoNPC] Diálogo de entrega ativado para Fase 1");
        }
        else
        {
            Debug.LogWarning("[DialogoNPC] Tentou ativar diálogo de entrega da Fase 1 em NPC da Fase 2");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            if (apenasMasterPodeDialogar && !PhotonNetwork.IsMasterClient) 
                return;

            jogadorAtual = other.gameObject;
            
            // 🔴 CORREÇÃO: Verifica se é diálogo de entrega
            if (ehNPCFase2 && tarefaConcluida && !missaoJaEntregue)
            {
                // Prepara para diálogo de entrega
                dialogoDeEntregaAtivo = true;
                Debug.Log("[DialogoNPC] Preparando para diálogo de ENTREGA (tarefa concluída)");
            }
            else if (ehNPCFase2 && missaoIniciada && !tarefaConcluida)
            {
                Debug.Log("[DialogoNPC] Tarefa em andamento... Mostrando diálogo inicial");
            }
            
            MostrarBotaoDialogo(jogadorAtual, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            MostrarBotaoDialogo(other.gameObject, false);
            jogadorAtual = null;
            
            // 🔴 CORREÇÃO: Só reseta se não estiver em diálogo
            if (!dialogoAtivo)
            {
                dialogoDeEntregaAtivo = false;
            }
        }
    }

    private void MostrarBotaoDialogo(GameObject jogador, bool mostrar)
    {
        var ui = jogador.GetComponentInChildren<PlayerUIReferences>();
        if (ui == null) return;

        ui.botaoDialogo.gameObject.SetActive(mostrar);

        if (mostrar)
        {
            ui.botaoDialogo.onClick.RemoveAllListeners();
            ui.botaoDialogo.onClick.AddListener(() => IniciarDialogo(jogador));
        }
        else
        {
            ui.botaoDialogo.onClick.RemoveAllListeners();
        }
    }

    private void IniciarDialogo(GameObject jogador)
    {
        if (dialogoAtivo) return;
        if (apenasMasterPodeDialogar && !PhotonNetwork.IsMasterClient) return;

        // 🔴 CORREÇÃO: Não inicia diálogo se tarefa está em andamento mas não concluída
        if (ehNPCFase2 && missaoIniciada && !tarefaConcluida && !dialogoDeEntregaAtivo)
        {
            Debug.Log("[DialogoNPC] Tarefa em andamento, aguarde conclusão...");
            return;
        }

        linhaAtual = 0;
        dialogoAtivo = true;

        var ui = jogador.GetComponentInChildren<PlayerUIReferences>();
        if (ui == null) return;

        painelDialogo = ui.painelDialogo;
        textoDialogo = ui.textoDialogo;
        botaoDialogo = ui.botaoDialogo;

        painelDialogo.SetActive(true);

        if (dialogoDeEntregaAtivo)
        {
            textoDialogo.text = dialogoAposEntrega[linhaAtual];
            Debug.Log("[DialogoNPC] Iniciando diálogo de ENTREGA");
        }
        else
        {
            textoDialogo.text = linhasDialogo[linhaAtual];
            Debug.Log("[DialogoNPC] Iniciando diálogo INICIAL");
        }

        StartCoroutine(EsperarToqueParaAvancar());
    }

    private IEnumerator EsperarToqueParaAvancar()
    {
        while (dialogoAtivo)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                AvancarDialogo();
                yield return new WaitForSeconds(0.2f);
            }

            if (Input.GetMouseButtonDown(0))
            {
                AvancarDialogo();
                yield return new WaitForSeconds(0.2f);
            }
            yield return null;
        }
    }

    private void AvancarDialogo()
    {
        linhaAtual++;

        string[] dialogoAtual = dialogoDeEntregaAtivo ? dialogoAposEntrega : linhasDialogo;

        if (linhaAtual < dialogoAtual.Length)
        {
            textoDialogo.text = dialogoAtual[linhaAtual];
        }
        else
        {
            FinalizarDialogo();
        }
    }

    [PunRPC]
    private void DesativarIndicadorGlobal()
    {
        if (indicadorNPC != null && indicadorNPC.iconeExclamacao != null)
            indicadorNPC.iconeExclamacao.SetActive(false);
    }
}