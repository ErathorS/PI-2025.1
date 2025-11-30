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

    [Header("Configuração Fase 3")]
    public bool ehNPCZona1 = false;
    public bool ehNPCZona2 = false;
    
    // CORREÇÃO: Mantendo compatibilidade com Fase 2
    public bool ehNPCFase2 = false;
    
    public bool tarefaConcluida = false;
    private bool missaoIniciada = false;

    // NOVO: Eventos para notificar conclusão de diálogo
    public Action OnDialogoInicialConcluido;
    public Action OnDialogoEntregaConcluido;

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

    // NOVO: Métodos de configuração para Fase 3
    public void ConfigurarComoNPCZona1()
    {
        ehNPCZona1 = true;
        ehNPCZona2 = false;
        ehNPCFase2 = false;
    }

    public void ConfigurarComoNPCZona2()
    {
        ehNPCZona1 = false;
        ehNPCZona2 = true;
        ehNPCFase2 = true; // Compatibilidade com Fase 2
    }

    private void FinalizarDialogo()
    {
        Debug.Log($"[DialogoNPC] FinalizarDialogo - " +
            $"EntregaAtivo: {dialogoDeEntregaAtivo}, " +
            $"Zona1: {ehNPCZona1}, Zona2: {ehNPCZona2}, " +
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

        // 🔴 NOTIFICAR EVENTOS
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

        // 🔴 CORREÇÃO: SÓ conta progresso se for diálogo de ENTREGA
        if (dialogoDeEntregaAtivo && !missaoJaEntregue)
        {
            missaoJaEntregue = true;

            if (ehNPCZona2 || ehNPCFase2)
            {
                Debug.Log("[DialogoNPC] ✅ Diálogo de ENTREGA concluído - Missão 2 finalizada!");
            }
            else if (ehNPCZona1)
            {
                Debug.Log("[DialogoNPC] ✅ Diálogo de ENTREGA concluído - Missão 1 finalizada!");
            }

            dialogoDeEntregaAtivo = false;
            return;
        }

        // --- SE FOR NPC DA ZONA 1 (diálogo inicial) ---
        if (ehNPCZona1 && !missaoJaEntregue && !missaoIniciada)
        {
            missaoIniciada = true;
            Debug.Log("[DialogoNPC] Missão da Zona 1 INICIADA");
            return;
        }

        // --- SE FOR NPC DA ZONA 2 (diálogo inicial) ---
        if ((ehNPCZona2 || ehNPCFase2) && !missaoJaEntregue && !missaoIniciada)
        {
            missaoIniciada = true;
            Debug.Log("[DialogoNPC] Missão da Zona 2 INICIADA");
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
        Debug.Log("[DialogoNPC] Tarefa concluída! Agora pode fazer diálogo de entrega.");
        
        if (indicadorNPC != null)
        {
            indicadorNPC.AtivarParaEntrega();
        }
    }

    public void AtivarDialogoDeEntrega()
    {
        if (ehNPCZona1 || ehNPCZona2 || ehNPCFase2)
        {
            dialogoDeEntregaAtivo = true;
            Debug.Log($"[DialogoNPC] Diálogo de entrega ativado para {(ehNPCZona1 ? "Zona 1" : ehNPCZona2 ? "Zona 2" : "Fase 2")}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        if (apenasMasterPodeDialogar && !PhotonNetwork.IsMasterClient) return;

        jogadorAtual = other.gameObject;

        // CORREÇÃO: Lógica mais clara para determinar o tipo de diálogo
        if (ehNPCZona1 || ehNPCZona2 || ehNPCFase2)
        {
            if (missaoIniciada && !tarefaConcluida)
            {
                // Preparar para diálogo de ENTREGA
                dialogoDeEntregaAtivo = true;
                Debug.Log("[DialogoNPC] Pronto para diálogo de ENTREGA");
            }
            else if (!missaoIniciada)
            {
                // Diálogo inicial
                dialogoDeEntregaAtivo = false;
                Debug.Log("[DialogoNPC] Pronto para diálogo INICIAL");
            }
            else
            {
                // Tarefa em andamento - não mostrar botão de diálogo
                Debug.Log("[DialogoNPC] Tarefa em andamento, não mostrar diálogo");
                return;
            }
        }

        MostrarBotaoDialogo(jogadorAtual, true);
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
        if (apenasMasterPodeDialogar && !PhotonNetwork.IsMasterClient) return;

        // CORREÇÃO: Verificação mais rigorosa do estado
        if (ehNPCZona1 || ehNPCZona2 || ehNPCFase2)
        {
            if (missaoIniciada && !tarefaConcluida)
            {
                Debug.Log("[DialogoNPC] Tarefa em andamento, aguarde conclusão...");
                return;
            }

            // SÓ ativa diálogo de entrega se a tarefa estiver concluída E missão não entregue
            if (tarefaConcluida && !missaoJaEntregue)
            {
                dialogoDeEntregaAtivo = true;
                Debug.Log("[DialogoNPC] Iniciando diálogo de ENTREGA (tarefa concluída)");
            }
            else if (!missaoIniciada)
            {
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
}