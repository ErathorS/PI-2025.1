using UnityEngine;
using TMPro;
using Photon.Pun;

public class MissaoFase1Manager : MonoBehaviourPunCallbacks
{
    public static MissaoFase1Manager instancia;

    [Header("UI da Missão")]
    public GameObject painelMissao;
    public TMP_Text textoMissao; // Mostra: Caixas: X/Y e Timer: MM:SS
    public TMP_Text textoExplicacao; // NOVO: Mostra a explicação da missão

    [Header("Reset e Coleta")]
    public GameObject botaoReset;
    public ColetarCaixasManager coletor;
    public DialogoNPC npcEntrega;

    [Header("Painel Final")]
    public PainelFinalFaseController painelFinalFase;

    [Header("Configuração Fase 1")]
    public bool usarProgressoCompletoParaFinalizar = true;
    public bool sincronizarTimerEntreJogadores = true;

    [Header("Textos de Missão")]
    [TextArea(3, 5)]
    public string textoInicial = "Procurem e coletem todas as caixas espalhadas pela área!";
    [TextArea(3, 5)]
    public string textoConcluido = "Excelente trabalho!\nFalem com o NPC para entregar.";
    [TextArea(3, 5)]
    public string textoFalhou = "O tempo acabou!\nVocês querem tentar novamente?";

    private bool missaoAtiva = false;
    private bool missaoConcluida = false;
    private float tempoRestante;

    // Controle de sincronização do timer
    private float tempoUltimaSincronizacao = 0f;
    private float intervaloDeSincronizacao = 1f;

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

        // Inicializa textos
        if (textoExplicacao != null)
            textoExplicacao.text = textoInicial;
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

        tempoRestante = 240f; // 4 minutos
        
        // Sincronizar timer inicial
        if (sincronizarTimerEntreJogadores)
        {
            photonView.RPC("RPC_SincronizarTimer", RpcTarget.All, tempoRestante);
        }

        if (coletor != null)
            coletor.AtivarCaixasParaMissao();

        // Configurar textos iniciais
        if (textoExplicacao != null)
            textoExplicacao.text = textoInicial;
            
        AtualizarUI();

        Debug.Log("[MissaoFase1Manager] Missão iniciada para TODOS os jogadores!");
    }

    private void Update()
    {
        if (!missaoAtiva || missaoConcluida) return;

        // Apenas o Master controla o timer
        if (PhotonNetwork.IsMasterClient)
        {
            tempoRestante -= Time.deltaTime;

            if (tempoRestante <= 0)
            {
                tempoRestante = 0;
                MissaoFalhou();
                return;
            }

            // Sincronizar timer periodicamente
            if (sincronizarTimerEntreJogadores)
            {
                tempoUltimaSincronizacao += Time.deltaTime;
                if (tempoUltimaSincronizacao >= intervaloDeSincronizacao)
                {
                    tempoUltimaSincronizacao = 0f;
                    photonView.RPC("RPC_SincronizarTimer", RpcTarget.Others, tempoRestante);
                }
            }

            // Atualizar UI a cada frame (só no Master)
            AtualizarUI();
        }
    }

    [PunRPC]
    private void RPC_SincronizarTimer(float novoTempo)
    {
        tempoRestante = novoTempo;
        AtualizarUI(); // Atualizar também nos clientes quando recebem sincronização
    }

    private void AtualizarUI()
    {
        if (textoMissao != null && coletor != null)
        {
            // Formatar tempo
            int m = Mathf.FloorToInt(tempoRestante / 60);
            int s = Mathf.FloorToInt(tempoRestante % 60);
            string tempoFormatado = $"{m:00}:{s:00}";
            
            // Atualizar texto de status (contador + timer)
            textoMissao.text = $"Caixas: {coletor.caixasColetadas}/{coletor.totalCaixas}\nTempo: {tempoFormatado}";
        }
    }

    // Método público para quando uma caixa é coletada
    public void CaixaColetada()
    {
        AtualizarUI();
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

        // Atualizar explicação
        if (textoExplicacao != null)
            textoExplicacao.text = textoFalhou;

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

        // Sincronizar timer resetado
        if (sincronizarTimerEntreJogadores)
        {
            photonView.RPC("RPC_SincronizarTimer", RpcTarget.All, tempoRestante);
        }

        if (botaoReset != null)
            botaoReset.SetActive(false);

        // Restaurar texto inicial
        if (textoExplicacao != null)
            textoExplicacao.text = textoInicial;

        AtualizarUI();

        Debug.Log("[MissaoFase1Manager] Missão resetada para TODOS os jogadores");
    }

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
        missaoAtiva = false;

        if (painelMissao != null)
            painelMissao.SetActive(true);

        if (botaoReset != null)
            botaoReset.SetActive(false);

        // Atualizar explicação para texto de conclusão
        if (textoExplicacao != null)
            textoExplicacao.text = textoConcluido;

        // Limpar contador/timer (mostra resultado final)
        if (textoMissao != null && coletor != null)
        {
            int m = Mathf.FloorToInt(tempoRestante / 60);
            int s = Mathf.FloorToInt(tempoRestante % 60);
            string tempoFormatado = $"{m:00}:{s:00}";
            textoMissao.text = $"Caixas: {coletor.totalCaixas}/{coletor.totalCaixas}\nTempo restante: {tempoFormatado}";
        }

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

        if (textoMissao != null)
            textoMissao.text = "";

        if (textoExplicacao != null)
            textoExplicacao.text = "";

        // Notificar progresso do NPC importante
        if (ProgressaoFaseController.instancia != null)
        {
            ProgressaoFaseController.instancia.NPCImportanteConcluido();
            Debug.Log("[MissaoFase1Manager] Progresso do NPC importante registrado");
        }

        Debug.Log("[MissaoFase1Manager] Entrega finalizada para TODOS os jogadores");
    }

    // Método para atualizar explicação em tempo real (se necessário)
    public void AtualizarExplicacao(string novaExplicacao)
    {
        if (textoExplicacao != null)
        {
            textoExplicacao.text = novaExplicacao;
            
            // Sincronizar com outros jogadores
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPC_SincronizarExplicacao", RpcTarget.Others, novaExplicacao);
            }
        }
    }

    [PunRPC]
    private void RPC_SincronizarExplicacao(string novaExplicacao)
    {
        if (textoExplicacao != null)
            textoExplicacao.text = novaExplicacao;
    }

    public void DebugEstado()
    {
        Debug.Log($"[MissaoFase1Manager] === DEBUG ===");
        Debug.Log($"MissaoAtiva: {missaoAtiva}");
        Debug.Log($"MissaoConcluida: {missaoConcluida}");
        Debug.Log($"TempoRestante: {tempoRestante}");
        Debug.Log($"MasterClient: {PhotonNetwork.IsMasterClient}");
        Debug.Log($"Coletor: {coletor != null}");
        Debug.Log($"NPCEntrega: {npcEntrega != null}");
        Debug.Log($"Texto Inicial: {textoInicial}");
        Debug.Log($"================================================");
    }
}