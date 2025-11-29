using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ProgressaoFaseController : MonoBehaviourPunCallbacks
{
    public static ProgressaoFaseController instancia;

    [Header("Referências")]
    public Slider barraProgresso;
    public TMP_Text textoObjetivo;
    public TMP_Text textoJornais;
    public TMP_Text textoLugares;
    public TMP_Text textoNpcs;

    [Header("Configurações de Objetivos")]
    public int npcsImportantesTotais = 1;
    public int jornaisTotais = 10;
    public int lugaresTotais = 4;

    [Header("Cores e Velocidade")]
    public Image fillImage;
    public Color corInicial = Color.white;
    public Color corFinal = Color.blue;
    public float velocidadeLerp = 4f;

    [Header("Painel Final")]
    public PainelFinalFaseController painelFinalFase;

    // estados atuais
    private int npcsConcluidos = 0;
    private int jornaisColetados = 0;
    private int lugaresConcluidos = 0;

    private float progressoAlvo = 0f;
    private float progressoAtual = 0f;
    
    // Controle rigoroso
    private bool npcImportanteJaConcluido = false;
    private bool npcImportanteIniciado = false;
    private bool inicializado = false;

    private PhotonView photonView;

    // Se não tiver PhotonView, adicione este código no Awake():
    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            photonView = GetComponent<PhotonView>();
            
            // 🔴 CORREÇÃO: Se não tiver PhotonView, adicionar automaticamente
            if (photonView == null)
            {
                photonView = gameObject.AddComponent<PhotonView>();
                Debug.Log("[ProgressaoFaseController] PhotonView adicionado automaticamente");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Inicializar com valores zerados
        if (!inicializado)
        {
            ResetarProgressoInicial();
            inicializado = true;
        }
        
        AtualizarUI();
        
        // 🔴 CORREÇÃO: Sincronizar estado com todos os jogadores ao iniciar
        if (PhotonNetwork.IsMasterClient)
        {
            SincronizarEstadoParaTodos();
        }
    }

    // Método para resetar progresso inicial
    private void ResetarProgressoInicial()
    {
        npcsConcluidos = 0;
        jornaisColetados = 0;
        lugaresConcluidos = 0;
        npcImportanteJaConcluido = false;
        npcImportanteIniciado = false;
        progressoAlvo = 0f;
        progressoAtual = 0f;
        
        Debug.Log("[ProgressaoFaseController] Progresso inicial RESETADO para zero");
    }

    void Update()
    {
        progressoAtual = Mathf.Lerp(progressoAtual, progressoAlvo, Time.deltaTime * velocidadeLerp);

        if (barraProgresso != null)
            barraProgresso.value = progressoAtual;

        if (fillImage != null)
            fillImage.color = Color.Lerp(corInicial, corFinal, progressoAtual);
    }

    // ============================
    //  MÉTODOS PÚBLICOS
    // ============================

    public void JornalColetado()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"[ProgressaoFaseController] JornalColetado enviado para Master");
            photonView.RPC(nameof(RPC_JornalColetado), RpcTarget.MasterClient);
            return;
        }
        RPC_JornalColetado();
    }

    public void LugarVisitadoConcluido()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"[ProgressaoFaseController] LugarVisitadoConcluido enviado para Master");
            photonView.RPC(nameof(RPC_LugarVisitadoConcluido), RpcTarget.MasterClient);
            return;
        }
        RPC_LugarVisitadoConcluido();
    }

    public void NPCImportanteConcluido()
    {
        // Verificações rigorosas
        if (npcImportanteJaConcluido)
        {
            Debug.LogWarning("[ProgressaoFaseController] NPC importante JÁ FOI CONCLUÍDO! Ignorando chamada dupla.");
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"[ProgressaoFaseController] Cliente enviando NPCImportanteConcluido para Master");
            photonView.RPC(nameof(RPC_NPCImportanteConcluido), RpcTarget.MasterClient);
            return;
        }
        
        Debug.Log($"[ProgressaoFaseController] Master processando NPCImportanteConcluido");
        RPC_NPCImportanteConcluido();
    }

    // ============================
    //       RPCs NO MASTER
    // ============================

    [PunRPC]
    private void RPC_JornalColetado()
    {
        jornaisColetados = Mathf.Min(jornaisColetados + 1, jornaisTotais);
        Debug.Log($"[ProgressaoFaseController] Jornal coletado! Total: {jornaisColetados}/{jornaisTotais}");
        SincronizarEstadoParaTodos();
    }

    [PunRPC]
    private void RPC_LugarVisitadoConcluido()
    {
        lugaresConcluidos = Mathf.Min(lugaresConcluidos + 1, lugaresTotais);
        Debug.Log($"[ProgressaoFaseController] Lugar visitado! Total: {lugaresConcluidos}/{lugaresTotais}");
        SincronizarEstadoParaTodos();
    }

    [PunRPC]
    private void RPC_NPCImportanteConcluido()
    {
        // Verificação dupla no Master
        if (npcImportanteJaConcluido)
        {
            Debug.LogWarning("[ProgressaoFaseController] Master: NPC importante já concluído! Ignorando.");
            return;
        }

        npcImportanteJaConcluido = true;
        npcsConcluidos = Mathf.Min(npcsConcluidos + 1, npcsImportantesTotais);
        
        Debug.Log($"[ProgressaoFaseController] ✅✅✅ NPC IMPORTANTE CONCLUÍDO APÓS ENTREGA! Progresso: {npcsConcluidos}/{npcsImportantesTotais}");
        
        SincronizarEstadoParaTodos();
    }

    private void SincronizarEstadoParaTodos()
    {
        Debug.Log($"[ProgressaoFaseController] SincronizarEstadoParaTodos - Enviando estado para todos: NPCs={npcsConcluidos}, Jornais={jornaisColetados}, Lugares={lugaresConcluidos}");
        
        // 🔴 CORREÇÃO: Usar AllBuffered para garantir que novos jogadores recebam o estado
        photonView.RPC(nameof(RPC_SyncEstado), RpcTarget.AllBuffered, 
            npcsConcluidos, jornaisColetados, lugaresConcluidos);
    }

    // ============================
    //     RPC DE SINCRONIZAÇÃO
    // ============================

    [PunRPC]
    private void RPC_SyncEstado(int npcs, int jornais, int lugares)
    {
        Debug.Log($"[ProgressaoFaseController] RPC_SyncEstado recebido - NPCs={npcs}, Jornais={jornais}, Lugares={lugares} | Sou o jogador {PhotonNetwork.LocalPlayer.ActorNumber}");

        // Só atualizar se for diferente
        if (npcs != npcsConcluidos || jornais != jornaisColetados || lugares != lugaresConcluidos)
        {
            npcsConcluidos = npcs;
            jornaisColetados = jornais;
            lugaresConcluidos = lugares;

            // Atualizar flag baseado no estado atual
            npcImportanteJaConcluido = (npcsConcluidos > 0);

            Debug.Log($"[ProgressaoFaseController] Estado sincronizado - NPCs: {npcsConcluidos}, Jornais: {jornaisColetados}, Lugares: {lugaresConcluidos}");
            
            AtualizarUI();
        }
        else
        {
            Debug.Log($"[ProgressaoFaseController] Estado recebido é igual ao atual, ignorando.");
        }
    }

    // ============================
    //        LÓGICA DE UI
    // ============================

    private void AtualizarUI()
    {
        Debug.Log($"[ProgressaoFaseController] AtualizarUI chamado - NPCs: {npcsConcluidos}");

        if (textoNpcs != null)
            textoNpcs.text = $"Pessoa Entrevistada: {npcsConcluidos}/{npcsImportantesTotais}";

        if (textoJornais != null)
            textoJornais.text = $"Jornais Coletados: {jornaisColetados}/{jornaisTotais}";

        if (textoLugares != null)
            textoLugares.text = $"Lugares Visitados: {lugaresConcluidos}/{lugaresTotais}";

        float totalPontos = npcsImportantesTotais + jornaisTotais + lugaresTotais;
        float feitos = npcsConcluidos + jornaisColetados + lugaresConcluidos;

        progressoAlvo = totalPontos > 0 ? feitos / totalPontos : 0f;

        Debug.Log($"[ProgressaoFaseController] Progresso atual: {progressoAlvo:P0} ({feitos}/{totalPontos})");

        if (feitos >= totalPontos && painelFinalFase != null)
        {
            painelFinalFase.MostrarPainelFinal();
            Debug.Log("[ProgressaoFaseController] TODOS OBJETIVOS CONCLUÍDOS! Mostrando painel final...");
        }
    }

    // 🔴 NOVO: Método para forçar sincronização manual
    public void ForcarSincronizacao()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[ProgressaoFaseController] Forçando sincronização manual...");
            SincronizarEstadoParaTodos();
        }
    }

    // 🔴 NOVO: Chamado quando um jogador entra na sala
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"[ProgressaoFaseController] Novo jogador entrou na sala: {newPlayer.ActorNumber}");
        
        // Sincronizar estado com o novo jogador
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[ProgressaoFaseController] Sincronizando estado com novo jogador...");
            SincronizarEstadoParaTodos();
        }
    }

    // Método para debug
    public void DebugEstadoAtual()
    {
        Debug.Log($"[ProgressaoFaseController] DEBUG - NPCs: {npcsConcluidos}/{npcsImportantesTotais}, " +
                 $"Jornais: {jornaisColetados}/{jornaisTotais}, Lugares: {lugaresConcluidos}/{lugaresTotais}, " +
                 $"FlagNPCConcluido: {npcImportanteJaConcluido}");
    }
}