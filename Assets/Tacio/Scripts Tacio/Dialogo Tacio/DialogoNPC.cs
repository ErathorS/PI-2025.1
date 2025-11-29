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

    Debug.Log($"[DialogoNPC] FinalizarDialogo - " +
        $"EntregaAtivo: {dialogoDeEntregaAtivo}, " +
        $"Fase2: {ehNPCFase2}, " +
        $"MissaoEntregue: {missaoJaEntregue}, " +
        $"TarefaConcluida: {tarefaConcluida}");

    painelDialogo.SetActive(false);
    dialogoAtivo = false;

    // Marca como já conversado (APENAS visual - sem contar progresso)
    if (indicadorNPC != null)
    {
        indicadorNPC.MarcarComoConversado();
        photonView.RPC("DesativarIndicadorGlobal", RpcTarget.AllBuffered);
    }

    // 🔴 CORREÇÃO: SÓ conta progresso se for diálogo de ENTREGA
    if (dialogoDeEntregaAtivo && !missaoJaEntregue)
    {
        missaoJaEntregue = true;
        
        if (ehNPCFase2)
        {
            Debug.Log("[DialogoNPC] ✅✅✅ Diálogo de ENTREGA concluído - Contando progresso do NPC importante!");
            ContarProgressoNPCImportante();
        }
        else if (MissaoFase1Manager.instancia != null)
        {
            MissaoFase1Manager.instancia.FinalizarEntrega();
        }
        
        dialogoDeEntregaAtivo = false;
        return;
    }

        // --- SE FOR NPC DA FASE 2 (diálogo inicial) ---
        // 🔴 CORREÇÃO: NÃO contar progresso aqui, apenas iniciar missão
        if (ehNPCFase2 && !missaoJaEntregue && !missaoIniciada)
        {
            // Inicia a tarefa de sincronização
            if (MissaoFase2Manager.instancia != null)
            {
                MissaoFase2Manager.instancia.IniciarMissao();
                missaoIniciada = true;
                Debug.Log("[DialogoNPC] Missão da Fase 2 INICIADA - Progresso NÃO contado ainda!");
            }
            return;
        }

        // --- LÓGICA ORIGINAL FASE 1 ---
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

    private void ContarProgressoNPCImportante()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        var progresso = FindObjectOfType<ProgressaoFaseController>();
        if (progresso != null)
        {
            progresso.NPCImportanteConcluido();
            Debug.Log("[DialogoNPC] ✅✅✅ Progresso do NPC importante contado APÓS ENTREGA DA TAREFA!");
            
            progresso.ForcarSincronizacao();
        }
        else
        {
            Debug.LogError("[DialogoNPC] ProgressaoFaseController não encontrado!");
        }
    }

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

    public void AtivarDialogoDeEntrega()
    {
        if (!ehNPCFase2)
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
            
            // 🔴 CORREÇÃO: Lógica mais clara para determinar o tipo de diálogo
            if (ehNPCFase2)
            {
                if (tarefaConcluida && !missaoJaEntregue)
                {
                    // Preparar para diálogo de ENTREGA
                    dialogoDeEntregaAtivo = true;
                    Debug.Log("[DialogoNPC] ✅ Pronto para diálogo de ENTREGA");
                }
                else if (!missaoIniciada)
                {
                    // Diálogo inicial
                    dialogoDeEntregaAtivo = false;
                    Debug.Log("[DialogoNPC] 💬 Pronto para diálogo INICIAL");
                }
                else
                {
                    // Tarefa em andamento - não mostrar botão de diálogo
                    Debug.Log("[DialogoNPC] ⏳ Tarefa em andamento, não mostrar diálogo");
                    return;
                }
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

        // 🔴 CORREÇÃO: Verificação mais rigorosa do estado
        if (ehNPCFase2)
        {
            if (missaoIniciada && !tarefaConcluida)
            {
                Debug.Log("[DialogoNPC] ⏳ Tarefa em andamento, aguarde conclusão...");
                return;
            }
            
            // 🔴 SÓ ativa diálogo de entrega se a tarefa estiver concluída E missão não entregue
            if (tarefaConcluida && !missaoJaEntregue)
            {
                dialogoDeEntregaAtivo = true;
                Debug.Log("[DialogoNPC] 🎯 Iniciando diálogo de ENTREGA (tarefa concluída)");
            }
            else if (!missaoIniciada)
            {
                dialogoDeEntregaAtivo = false;
                Debug.Log("[DialogoNPC] 💬 Iniciando diálogo INICIAL da Fase 2");
            }
            else
            {
                Debug.Log("[DialogoNPC] ❌ Estado inválido para diálogo");
                return;
            }
        }

        linhaAtual = 0;
        dialogoAtivo = true;

        var ui = jogador.GetComponentInChildren<PlayerUIReferences>();
        if (ui == null) return;

        painelDialogo = ui.painelDialogo;
        textoDialogo = ui.textoDialogo;
        botaoDialogo = ui.botaoDialogo;

        painelDialogo.SetActive(true);

        // Escolher o diálogo correto baseado no estado
        if (dialogoDeEntregaAtivo)
        {
            textoDialogo.text = dialogoAposEntrega[linhaAtual];
        }
        else
        {
            textoDialogo.text = linhasDialogo[linhaAtual];
        }

        Debug.Log($"[DialogoNPC] Diálogo iniciado - " +
                  $"Tipo: {(dialogoDeEntregaAtivo ? "ENTREGA" : "INICIAL")}, " +
                  $"TarefaConcluida: {tarefaConcluida}, " +
                  $"MissaoJaEntregue: {missaoJaEntregue}");

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

    // 🔴 MÉTODO PARA DEBUG: Resetar estado do NPC
    public void ResetarEstadoNPC()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        missaoJaEntregue = false;
        missaoIniciada = false;
        dialogoDeEntregaAtivo = false;
        tarefaConcluida = false;
        
        Debug.Log("[DialogoNPC] 🔄 Estado do NPC resetado!");
        
        photonView.RPC("RPC_ResetarEstadoNPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_ResetarEstadoNPC()
    {
        missaoJaEntregue = false;
        missaoIniciada = false;
        dialogoDeEntregaAtivo = false;
        tarefaConcluida = false;
        
        if (indicadorNPC != null)
        {
            indicadorNPC.ReativarIndicador();
        }
        
        Debug.Log($"[DialogoNPC] Estado resetado para jogador {PhotonNetwork.LocalPlayer.ActorNumber}");
    }
}