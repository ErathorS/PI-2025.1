using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;
using System;

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

    [Header("Configuração por Fase")]
    public bool ehNPCFase1 = false;
    public bool ehNPCFase2 = false;
    public bool ehNPCZona1 = false;
    public bool ehNPCZona2 = false;
    public bool ehNPCIntroducao = false;
    
    public bool tarefaConcluida = false;
    private bool missaoIniciada = false;

    // Eventos para notificar conclusão de diálogo
    public Action OnDialogoInicialConcluido;
    public Action OnDialogoEntregaConcluido;

    private int linhaAtual = 0;
    private bool dialogoAtivo = false;
    private bool missaoJaEntregue = false;

    private GameObject jogadorAtual;
    private TMP_Text textoDialogo;
    private GameObject painelDialogo;
    private Button botaoDialogo;

    // Referência para o script de introdução
    private DialogoNPCIntroducao introManager;

    private void Start()
    {
        if (painelDialogo != null)
            painelDialogo.SetActive(false);

        // Buscar automaticamente o DialogoNPCIntroducao se for NPC de introdução
        if (ehNPCIntroducao)
        {
            introManager = GetComponent<DialogoNPCIntroducao>();
            if (introManager == null)
            {
                introManager = FindObjectOfType<DialogoNPCIntroducao>();
            }
            
            if (introManager != null)
            {
                Debug.Log("[DialogoNPC] DialogoNPCIntroducao encontrado para NPC de introdução");
            }
            else
            {
                Debug.LogWarning("[DialogoNPC] NPC de introdução mas DialogoNPCIntroducao não encontrado");
            }
        }
    }

    // Métodos de configuração para diferentes fases
    public void ConfigurarComoNPCFase1()
    {
        ehNPCFase1 = true;
        ehNPCFase2 = false;
        ehNPCZona1 = false;
        ehNPCZona2 = false;
        ehNPCIntroducao = false;
    }

    public void ConfigurarComoNPCFase2()
    {
        ehNPCFase1 = false;
        ehNPCFase2 = true;
        ehNPCZona1 = false;
        ehNPCZona2 = false;
        ehNPCIntroducao = false;
    }

    public void ConfigurarComoNPCZona1()
    {
        ehNPCFase1 = false;
        ehNPCFase2 = false;
        ehNPCZona1 = true;
        ehNPCZona2 = false;
        ehNPCIntroducao = false;
    }

    public void ConfigurarComoNPCZona2()
    {
        ehNPCFase1 = false;
        ehNPCFase2 = false;
        ehNPCZona1 = false;
        ehNPCZona2 = true;
        ehNPCIntroducao = false;
    }

    public void ConfigurarComoNPCIntroducao()
    {
        ehNPCFase1 = false;
        ehNPCFase2 = false;
        ehNPCZona1 = false;
        ehNPCZona2 = false;
        ehNPCIntroducao = true;
    }

    private void FinalizarDialogo()
    {
        Debug.Log($"[DialogoNPC] FinalizarDialogo - " +
            $"Fase1: {ehNPCFase1}, Fase2: {ehNPCFase2}, " +
            $"Zona1: {ehNPCZona1}, Zona2: {ehNPCZona2}, " +
            $"Introducao: {ehNPCIntroducao}, " +
            $"EntregaAtivo: {dialogoDeEntregaAtivo}, " +
            $"MissaoEntregue: {missaoJaEntregue}, " +
            $"TarefaConcluida: {tarefaConcluida}");

        painelDialogo.SetActive(false);
        dialogoAtivo = false;

        // Marca como já conversado (apenas visual)
        if (indicadorNPC != null)
        {
            indicadorNPC.MarcarComoConversado();
            photonView.RPC("DesativarIndicadorGlobal", RpcTarget.AllBuffered);
        }

        // NOTIFICAR EVENTOS
        if (!dialogoDeEntregaAtivo && OnDialogoInicialConcluido != null)
        {
            OnDialogoInicialConcluido.Invoke();
            Debug.Log("[DialogoNPC] Evento OnDialogoInicialConcluido disparado");
        }

        if (dialogoDeEntregaAtivo && OnDialogoEntregaConcluido != null)
        {
            OnDialogoEntregaConcluido.Invoke();
            Debug.Log("[DialogoNPC] Evento OnDialogoEntregaConcluido disparado");
        }

        // LÓGICA PARA NPC DE INTRODUÇÃO
        if (ehNPCIntroducao && !dialogoDeEntregaAtivo)
        {
            Debug.Log("[DialogoNPC] NPC de introdução - Notificando sistema");
            
            // Notificar o DialogoNPCIntroducao que um jogador conversou
            if (introManager != null)
            {
                int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
                introManager.MarcarDialogoConcluido(playerID);
                Debug.Log($"[DialogoNPC] Notificando DialogoNPCIntroducao sobre jogador {playerID}");
            }
            else
            {
                Debug.LogWarning("[DialogoNPC] NPC de introdução mas introManager não encontrado");
            }
            return;
        }

        // LÓGICA ESPECÍFICA PARA FASE 1 (diálogo inicial)
        if (ehNPCFase1 && !dialogoDeEntregaAtivo && !missaoIniciada)
        {
            missaoIniciada = true;
            Debug.Log("[DialogoNPC] NPC da Fase 1 - Iniciando missão de coleta!");
            
            // Notificar o MissaoFase1Manager para iniciar a missão
            if (MissaoFase1Manager.instancia != null)
            {
                MissaoFase1Manager.instancia.IniciarMissao();
            }
            else
            {
                Debug.LogError("[DialogoNPC] MissaoFase1Manager não encontrado!");
            }
            return;
        }

        // SÓ conta progresso se for diálogo de ENTREGA
        if (dialogoDeEntregaAtivo && !missaoJaEntregue)
        {
            missaoJaEntregue = true;

            if (ehNPCFase1)
            {
                Debug.Log("[DialogoNPC] ✅ Diálogo de ENTREGA concluído - Fase 1 finalizada!");
                if (MissaoFase1Manager.instancia != null)
                {
                    // CORREÇÃO: Chamar FinalizarEntrega
                    MissaoFase1Manager.instancia.FinalizarEntrega();
                }
                
                // CORREÇÃO: Resetar estados após entrega
                missaoIniciada = false;
                tarefaConcluida = false;
                dialogoDeEntregaAtivo = false;
                missaoJaEntregue = true; // IMPORTANTE: Marcar como já entregue
            }
            else if (ehNPCFase2)
            {
                Debug.Log("[DialogoNPC] ✅ Diálogo de ENTREGA concluído - Fase 2 finalizada!");
                // Notificar ProgressaoFaseController para contar o NPC importante
                if (ProgressaoFaseController.instancia != null)
                {
                    ProgressaoFaseController.instancia.NPCImportanteConcluido();
                }
            }
            else if (ehNPCZona2)
            {
                Debug.Log("[DialogoNPC] ✅ Diálogo de ENTREGA concluído - Zona 2 finalizada!");
                // Notificar ProgressaoFaseController para contar o NPC importante
                if (ProgressaoFaseController.instancia != null)
                {
                    ProgressaoFaseController.instancia.NPCImportanteConcluido();
                }
            }
            else if (ehNPCZona1)
            {
                Debug.Log("[DialogoNPC] ✅ Diálogo de ENTREGA concluído - Zona 1 finalizada!");
                // Notificar ProgressaoFaseController para contar o NPC importante
                if (ProgressaoFaseController.instancia != null)
                {
                    ProgressaoFaseController.instancia.NPCImportanteConcluido();
                }
            }

            dialogoDeEntregaAtivo = false;
            return;
        }

        // SE FOR NPC DA FASE 2 (diálogo inicial)
        if (ehNPCFase2 && !missaoJaEntregue && !missaoIniciada)
        {
            missaoIniciada = true;
            Debug.Log("[DialogoNPC] Missão da Fase 2 INICIADA");
            
            // Iniciar missão da Fase 2
            if (MissaoFase2Manager.instancia != null)
            {
                MissaoFase2Manager.instancia.IniciarMissao();
            }
            return;
        }

        // SE FOR NPC DA ZONA 1 (diálogo inicial)
        if (ehNPCZona1 && !missaoJaEntregue && !missaoIniciada)
        {
            missaoIniciada = true;
            Debug.Log("[DialogoNPC] Missão da Zona 1 INICIADA");
            
            // Iniciar missão da Zona 1 (Fase 3)
            if (MissaoFase3Manager.instancia != null)
            {
                MissaoFase3Manager.instancia.IniciarMissao1();
            }
            return;
        }

        // SE FOR NPC DA ZONA 2 (diálogo inicial)
        if (ehNPCZona2 && !missaoJaEntregue && !missaoIniciada)
        {
            missaoIniciada = true;
            Debug.Log("[DialogoNPC] Missão da Zona 2 INICIADA");
            
            // Iniciar missão da Zona 2 (Fase 3)
            if (MissaoFase3Manager.instancia != null)
            {
                MissaoFase3Manager.instancia.IniciarMissao2();
            }
            return;
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
        missaoIniciada = false; // CORREÇÃO: Resetar para permitir novo diálogo
        dialogoDeEntregaAtivo = false; // CORREÇÃO: Resetar estado de diálogo
        
        Debug.Log("[DialogoNPC] Tarefa concluída! Agora pode fazer diálogo de entrega.");
        
        if (indicadorNPC != null)
        {
            indicadorNPC.AtivarParaEntrega();
            Debug.Log("[DialogoNPC] Indicador reativado para entrega");
        }

        // CORREÇÃO: Forçar atualização do estado visual
        photonView.RPC("RPC_AtualizarEstadoVisual", RpcTarget.AllBuffered, tarefaConcluida);
    }

    [PunRPC]
    private void RPC_AtualizarEstadoVisual(bool tarefaConcluida)
    {
        this.tarefaConcluida = tarefaConcluida;
        
        if (indicadorNPC != null)
        {
            indicadorNPC.AtivarParaEntrega();
        }
        
        Debug.Log($"[DialogoNPC] Estado visual atualizado: TarefaConcluida={tarefaConcluida}");
    }

    // Método para verificar se o NPC está interagível
    public bool PodeInteragir()
    {
        if (ehNPCIntroducao)
            return true; // NPC de introdução sempre interagível
        
        if (ehNPCFase1 || ehNPCFase2 || ehNPCZona1 || ehNPCZona2)
        {
            // Pode interagir se:
            // 1. Não iniciou missão ainda, OU
            // 2. Tarefa concluída e não entregue, OU  
            // 3. Missão em andamento (para ver progresso)
            return !missaoIniciada || (tarefaConcluida && !missaoJaEntregue) || (missaoIniciada && !tarefaConcluida);
        }
        
        return true; // NPCs normais sempre interagíveis
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        // NPC de introdução - ambos jogadores podem dialogar
        if (apenasMasterPodeDialogar && !PhotonNetwork.IsMasterClient && !ehNPCIntroducao) 
            return;

        // Verificar se pode interagir
        if (!PodeInteragir())
        {
            Debug.LogWarning($"[DialogoNPC] NPC não está interagível no momento. Estado: MissaoIniciada={missaoIniciada}, TarefaConcluida={tarefaConcluida}, MissaoJaEntregue={missaoJaEntregue}");
            return;
        }

        jogadorAtual = other.gameObject;

        // Lógica para determinar o tipo de diálogo
        if (ehNPCFase1 || ehNPCFase2 || ehNPCZona1 || ehNPCZona2 || ehNPCIntroducao)
        {
            // Verificar se a tarefa está concluída para mostrar botão de entrega
            if (tarefaConcluida && !missaoJaEntregue)
            {
                // Preparar para diálogo de ENTREGA
                dialogoDeEntregaAtivo = true;
                Debug.Log("[DialogoNPC] Tarefa concluída - pronto para diálogo de ENTREGA");
            }
            else if (!missaoIniciada || ehNPCIntroducao)
            {
                // Diálogo inicial (sempre para introdução)
                dialogoDeEntregaAtivo = false;
                Debug.Log("[DialogoNPC] Pronto para diálogo INICIAL");
            }
            else if (missaoIniciada && !tarefaConcluida)
            {
                // Tarefa em andamento - permitir diálogo para ver progresso
                dialogoDeEntregaAtivo = false;
                Debug.Log("[DialogoNPC] Tarefa em andamento - permitindo diálogo para ver progresso");
            }
        }

        MostrarBotaoDialogo(jogadorAtual, true);
        Debug.Log($"[DialogoNPC] Botão de interação mostrado. PodeInteragir: {PodeInteragir()}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        MostrarBotaoDialogo(other.gameObject, false);
        jogadorAtual = null;

        if (!dialogoAtivo)
        {
            dialogoDeEntregaAtivo = false;
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
        
        // NPC de introdução - ambos jogadores podem dialogar
        if (apenasMasterPodeDialogar && !PhotonNetwork.IsMasterClient && !ehNPCIntroducao) 
            return;

        // Verificação mais rigorosa do estado
        if (ehNPCFase1 || ehNPCFase2 || ehNPCZona1 || ehNPCZona2 || ehNPCIntroducao)
        {
            if (missaoIniciada && !tarefaConcluida && !ehNPCIntroducao)
            {
                Debug.Log("[DialogoNPC] Tarefa em andamento, aguarde conclusão...");
                return;
            }

            // SÓ ativa diálogo de entrega se a tarefa estiver concluída E missão não entregue (não para introdução)
            if (tarefaConcluida && !missaoJaEntregue && !ehNPCIntroducao)
            {
                dialogoDeEntregaAtivo = true;
                Debug.Log("[DialogoNPC] Iniciando diálogo de ENTREGA (tarefa concluída)");
            }
            else if (!missaoIniciada || ehNPCIntroducao)
            {
                // Diálogo inicial (sempre para introdução)
                dialogoDeEntregaAtivo = false;
                Debug.Log("[DialogoNPC] Iniciando diálogo INICIAL");
            }
            else
            {
                Debug.Log("[DialogoNPC] Estado inválido para diálogo");
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

        // Escolher o dialogo correto baseado no estado
        if (dialogoDeEntregaAtivo)
        {
            textoDialogo.text = dialogoAposEntrega[linhaAtual];
        }
        else
        {
            textoDialogo.text = linhasDialogo[linhaAtual];
        }

        Debug.Log($"[DialogoNPC] Dialogo iniciado - " +
            $"Tipo: {(dialogoDeEntregaAtivo ? "ENTREGA" : "INICIAL")}, " +
            $"NPC: {(ehNPCFase1 ? "Fase1" : ehNPCFase2 ? "Fase2" : ehNPCZona1 ? "Zona1" : ehNPCZona2 ? "Zona2" : ehNPCIntroducao ? "Introducao" : "Normal")}");

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

    // MÉTODO PARA DEBUG: Resetar estado do NPC
    public void ResetarEstadoNPC()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        missaoJaEntregue = false;
        missaoIniciada = false;
        dialogoDeEntregaAtivo = false;
        tarefaConcluida = false;

        Debug.Log("[DialogoNPC] Estado do NPC resetado!");

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

    public void AtivarDialogoDeEntrega()
    {
        if (ehNPCFase1 || ehNPCFase2 || ehNPCZona1 || ehNPCZona2)
        {
            dialogoDeEntregaAtivo = true;
            string tipoNPC = ehNPCFase1 ? "Fase 1" : 
                            ehNPCFase2 ? "Fase 2" : 
                            ehNPCZona1 ? "Zona 1" : "Zona 2";
            Debug.Log($"[DialogoNPC] Diálogo de entrega ativado para {tipoNPC}");
        }
    }

    // Método para debug
    public void DebugEstado()
    {
        Debug.Log($"[DialogoNPC] === DEBUG ===");
        Debug.Log($"Fase1: {ehNPCFase1}, Fase2: {ehNPCFase2}");
        Debug.Log($"Zona1: {ehNPCZona1}, Zona2: {ehNPCZona2}");
        Debug.Log($"Introducao: {ehNPCIntroducao}");
        Debug.Log($"MissaoIniciada: {missaoIniciada}");
        Debug.Log($"TarefaConcluida: {tarefaConcluida}");
        Debug.Log($"MissaoJaEntregue: {missaoJaEntregue}");
        Debug.Log($"DialogoEntregaAtivo: {dialogoDeEntregaAtivo}");
        Debug.Log($"PodeInteragir: {PodeInteragir()}");
        Debug.Log($"=====================");
    }

    // Método temporário para debug - chame via console ou botão
    public void DebugEstadoInteracao()
    {
        Debug.Log($"[DialogoNPC] === DEBUG INTERAÇÃO ===");
        Debug.Log($"PodeInteragir: {PodeInteragir()}");
        Debug.Log($"MissaoIniciada: {missaoIniciada}");
        Debug.Log($"TarefaConcluida: {tarefaConcluida}"); 
        Debug.Log($"MissaoJaEntregue: {missaoJaEntregue}");
        Debug.Log($"DialogoAtivo: {dialogoAtivo}");
        Debug.Log($"JogadorPerto: {jogadorAtual != null}");
        Debug.Log($"=====================");
    }
}