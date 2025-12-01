using UnityEngine;
using TMPro;
using Photon.Pun;

public class MissaoFase1Manager : MonoBehaviourPunCallbacks
{
    public static MissaoFase1Manager instancia;

    [Header("UI da Missão")] 
    public GameObject painelMissao; 
    public TMP_Text textoMissao; 
    public TMP_Text textoTimer;

    [Header("Reset e Coleta")] 
    public GameObject botaoReset; 
    public ColetarCaixasManager coletor; 
    public DialogoNPC npcEntrega;

    [Header("Painel Final")]
    public PainelFinalFaseController painelFinalFase;

    private bool missaoAtiva = false; 
    private bool missaoConcluida = false; 
    private float tempoRestante;

    private PhotonView photonView;

    private void Awake() 
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            photonView = gameObject.AddComponent<PhotonView>();
        }
    }

    private void Start()
    {
        if (painelMissao != null)
            painelMissao.SetActive(false);
        
        if (botaoReset != null)
            botaoReset.SetActive(false);
    }

    public void IniciarMissao() 
    {
        if (!PhotonNetwork.IsMasterClient) return; 
        Debug.Log("[MissaoFase1Manager] Iniciando missão via RPC");
        photonView.RPC("RPC_IniciarMissao", RpcTarget.AllBuffered); 
    }

    [PunRPC] 
    private void RPC_IniciarMissao() 
    {
        missaoAtiva = true; 
        missaoConcluida = false;

        if (painelMissao != null)
            painelMissao.SetActive(true); 
        
        if (botaoReset != null)
            botaoReset.SetActive(false);

        tempoRestante = 240f;

        if (coletor != null)
            coletor.AtivarCaixasParaMissao();
        
        AtualizarUI();
        
        Debug.Log("[MissaoFase1Manager] Missão iniciada para TODOS os jogadores!");
    }

    private void Update()
    {
        if (!missaoAtiva || missaoConcluida) return;

        // CORREÇÃO: Apenas o Master controla o timer
        if (PhotonNetwork.IsMasterClient)
        {
            tempoRestante -= Time.deltaTime;

            if (tempoRestante <= 0)
            {
                tempoRestante = 0;
                MissaoFalhou();
            }
        }

        AtualizarUI();
    }

    private void AtualizarUI()
    {
        if (textoTimer != null)
        {
            int m = Mathf.FloorToInt(tempoRestante / 60);
            int s = Mathf.FloorToInt(tempoRestante % 60);
            textoTimer.text = $"{m:00}:{s:00}";
        }
        
        if (textoMissao != null && coletor != null)
        {
            textoMissao.text = $"Caixas: {coletor.caixasColetadas}/{coletor.totalCaixas}\nTempo: {textoTimer.text}";
        }
    }

    private void MissaoFalhou()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        Debug.Log("[MissaoFase1Manager] Missão falhou - enviando para todos");
        photonView.RPC("RPC_MissaoFalhou", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_MissaoFalhou()
    {
        missaoAtiva = false;

        if (painelMissao != null)
            painelMissao.SetActive(true);
            
        if (textoMissao != null)
            textoMissao.text = "O tempo acabou!\nVocês querem tentar novamente?";

        if (PhotonNetwork.IsMasterClient && botaoReset != null)
            botaoReset.SetActive(true);

        Debug.Log("[MissaoFase1Manager] Missão falhou para TODOS os jogadores");
    }

    public void BotaoResetarMissao()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Debug.Log("[MissaoFase1Manager] Resetando missão via RPC");
        photonView.RPC("RPC_ResetarMissao", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_ResetarMissao()
    {
        if (coletor != null)
            coletor.ResetarFase();

        missaoAtiva = true;
        missaoConcluida = false;
        tempoRestante = 240f;

        if (botaoReset != null)
            botaoReset.SetActive(false);
            
        if (textoMissao != null)
            textoMissao.text = "Procurem as caixas!";
            
        AtualizarUI();

        Debug.Log("[MissaoFase1Manager] Missão resetada para TODOS os jogadores");
    }

    // CORREÇÃO: Chamado quando todas as caixas são coletadas
    public void MissaoFinalizada()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Debug.Log("[MissaoFase1Manager] Missão finalizada - enviando para todos");
        photonView.RPC("RPC_MissaoFinalizada", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_MissaoFinalizada()
    {
        missaoConcluida = true;
        missaoAtiva = false; // CORREÇÃO: Para o timer para TODOS os jogadores

        if (painelMissao != null)
            painelMissao.SetActive(true);
            
        if (botaoReset != null)
            botaoReset.SetActive(false);

        if (textoMissao != null)
            textoMissao.text = "Excelente trabalho!\nFalem com o NPC para entregar.";

        // CORREÇÃO: Notificar o NPC que a tarefa está concluída
        if (npcEntrega != null)
        {
            npcEntrega.TarefaConcluida();
            Debug.Log("[MissaoFase1Manager] NPC notificado sobre conclusão da tarefa");
        }
        else
        {
            Debug.LogError("[MissaoFase1Manager] npcEntrega não está atribuído!");
        }

        Debug.Log("[MissaoFase1Manager] Missão finalizada para TODOS os jogadores - timer PARADO");
    }

    // CORREÇÃO: Chamado quando a entrega é feita ao NPC
    public void FinalizarEntrega()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Debug.Log("[MissaoFase1Manager] Finalizando entrega - enviando para todos");
        photonView.RPC("RPC_FinalizarEntrega", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_FinalizarEntrega()
    {
        // Esconder UI da missão
        if (painelMissao != null)
            painelMissao.SetActive(false);
            
        if (botaoReset != null)
            botaoReset.SetActive(false);

        if (textoTimer != null)
            textoTimer.text = "";
            
        if (textoMissao != null)
            textoMissao.text = "";

        // CORREÇÃO: Mostrar painel final da fase
        if (painelFinalFase != null)
        {
            painelFinalFase.MostrarPainelFinal();
            Debug.Log("[MissaoFase1Manager] Painel final da fase exibido");
        }
        else
        {
            Debug.LogError("[MissaoFase1Manager] painelFinalFase não está atribuído!");
        }

        // CORREÇÃO: Notificar progresso do NPC importante
        if (ProgressaoFaseController.instancia != null)
        {
            ProgressaoFaseController.instancia.NPCImportanteConcluido();
            Debug.Log("[MissaoFase1Manager] Progresso do NPC importante registrado");
        }

        Debug.Log("[MissaoFase1Manager] Entrega finalizada para TODOS os jogadores");
    }

    // CORREÇÃO: Método para debug do estado atual
    public void DebugEstado()
    {
        Debug.Log($"[MissaoFase1Manager] === DEBUG ===");
        Debug.Log($"MissaoAtiva: {missaoAtiva}");
        Debug.Log($"MissaoConcluida: {missaoConcluida}");
        Debug.Log($"TempoRestante: {tempoRestante}");
        Debug.Log($"MasterClient: {PhotonNetwork.IsMasterClient}");
        Debug.Log($"Coletor: {coletor != null}");
        Debug.Log($"NPCEntrega: {npcEntrega != null}");
        Debug.Log($"PainelFinal: {painelFinalFase != null}");
        Debug.Log($"=====================");
    }
}