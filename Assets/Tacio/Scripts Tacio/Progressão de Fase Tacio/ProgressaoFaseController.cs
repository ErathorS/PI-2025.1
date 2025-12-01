using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;

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
    public int npcsImportantesTotais = 1; // NOVO: 2 NPCs para Fase 3
    public int jornaisTotais = 8;         // Ajuste conforme Fase 3
    public int lugaresTotais = 3;         // Ajuste conforme Fase 3

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
    private bool inicializado = false;

    private PhotonView photonView;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            photonView = GetComponent<PhotonView>();
            if (photonView == null)
            {
                photonView = gameObject.AddComponent<PhotonView>();
                photonView.ViewID = 999;
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

        if (!inicializado)
        {
            ResetarProgressoInicial();
            inicializado = true;
        }

        AtualizarUI();

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[ProgressaoFaseController] Master iniciando - sincronizando estado inicial");
            SincronizarEstadoParaTodos();
        }
    }

    private void ResetarProgressoInicial()
    {
        npcsConcluidos = 0;
        jornaisColetados = 0;
        lugaresConcluidos = 0;
        npcImportanteJaConcluido = false;
        progressoAlvo = 0f;
        progressoAtual = 0f;

        Debug.Log("[ProgressaoFaseController] Progresso inicial RESETADO para zero");
    }

    void Update()
    {
        if (Mathf.Abs(progressoAtual - progressoAlvo) > 0.01f)
        {
            progressoAtual = Mathf.Lerp(progressoAtual, progressoAlvo, Time.deltaTime * velocidadeLerp);
        }
        else
        {
            progressoAtual = progressoAlvo;
        }

        if (barraProgresso != null)
            barraProgresso.value = progressoAtual;

        if (fillImage != null)
            fillImage.color = Color.Lerp(corInicial, corFinal, progressoAtual);
    }

    // ---
    // MÉTODOS PÚBLICOS
    // ---

    public void JornalColetado()
    {
        Debug.Log($"[ProgressaoFaseController] JornalColetado chamado - Master: {PhotonNetwork.IsMasterClient}");

        if (PhotonNetwork.IsMasterClient)
        {
            RPC_JornalColetado();
        }
        else
        {
            photonView.RPC("RPC_JornalColetado", RpcTarget.MasterClient);
        }
    }

    public void LugarVisitadoConcluido()
    {
        Debug.Log($"[ProgressaoFaseController] LugarVisitadoConcluido chamado - Master: {PhotonNetwork.IsMasterClient}");

        if (PhotonNetwork.IsMasterClient)
        {
            RPC_LugarVisitadoConcluido();
        }
        else
        {
            photonView.RPC("RPC_LugarVisitadoConcluido", RpcTarget.MasterClient);
        }
    }

    public void NPCImportanteConcluido()
    {
        Debug.Log($"[ProgressaoFaseController] NPCImportanteConcluido chamado - Master: {PhotonNetwork.IsMasterClient}");

        if (npcImportanteJaConcluido)
        {
            Debug.LogWarning("[ProgressaoFaseController] NPC importante JÁ FOI CONCLUÍDO! Ignorando chamada dupla.");
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            RPC_NPCImportanteConcluido();
        }
        else
        {
            photonView.RPC("RPC_NPCImportanteConcluido", RpcTarget.MasterClient);
        }
    }

    // ===============================
    // RPCs
    // ===============================

    [PunRPC]
    private void RPC_JornalColetado()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            jornaisColetados = Mathf.Min(jornaisColetados + 1, jornaisTotais);
            Debug.Log($"[ProgressaoFaseController] ▼️ Master - Jornal coletado! Total: {jornaisColetados}/{jornaisTotais}");

            SincronizarEstadoParaTodos();
        }

        AtualizarUILocal();
    }

    [PunRPC]
    private void RPC_LugarVisitadoConcluido()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            lugaresConcluidos = Mathf.Min(lugaresConcluidos + 1, lugaresTotais);
            Debug.Log($"[ProgressaoFaseController] ▼️ Master - Lugar visitado! Total: {lugaresConcluidos}/{lugaresTotais}");

            SincronizarEstadoParaTodos();
        }

        AtualizarUILocal();
    }

    [PunRPC]
    private void RPC_NPCImportanteConcluido()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (npcImportanteJaConcluido)
            {
                Debug.LogWarning("[ProgressaoFaseController] Master: NPC importante já concluído!");
                return;
            }

            npcImportanteJaConcluido = true;
            npcsConcluidos = Mathf.Min(npcsConcluidos + 1, npcsImportantesTotais);

            Debug.Log($"[ProgressaoFaseController] ▼▼▼ Master - NPC IMPORTANTE CONCLUIDO! Progresso: {npcsConcluidos}/{npcsImportantesTotais}");

            SincronizarEstadoParaTodos();
        }

        AtualizarUILocal();
    }

    // ---
    // SINCRONIZAÇÃO E UI
    // ---

    private void SincronizarEstadoParaTodos()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[ProgressaoFaseController] Apenas Master pode sincronizar estado!");
            return;
        }

        Debug.Log($"[ProgressaoFaseController] 💬️ Sincronizando estado para todos: NPCs={npcsConcluidos}, Jornais={jornaisColetados}, Lugares={lugaresConcluidos}");

        photonView.RPC("RPC_SincronizarEstado", RpcTarget.All, npcsConcluidos, jornaisColetados, lugaresConcluidos);
    }

    [PunRPC]
    private void RPC_SincronizarEstado(int npcs, int jornais, int lugares)
    {
        Debug.Log($"[ProgressaoFaseController] 💬️ Recebendo estado sincronizado: NPCs={npcs}, Jornais={jornais}, Lugares={lugares} | Jogador: {PhotonNetwork.LocalPlayer.ActorNumber}");

        npcsConcluidos = npcs;
        jornaisColetados = jornais;
        lugaresConcluidos = lugares;
        npcImportanteJaConcluido = (npcsConcluidos >= npcsImportantesTotais);

        AtualizarUILocal();

        Debug.Log($"[ProgressaoFaseController] 💬️ Estado sincronizado e UI atualizada");
    }

    private void AtualizarUILocal()
    {
        if (textoNpcs != null)
            textoNpcs.text = $"Pessoas Entrevistadas: {npcsConcluidos}/{npcsImportantesTotais}";

        if (textoJornais != null)
            textoJornais.text = $"Jornais Coletados: {jornaisColetados}/{jornaisTotais}";

        if (textoLugares != null)
            textoLugares.text = $"Lugares Visitados: {lugaresConcluidos}/{lugaresTotais}";

        float totalPontos = npcsImportantesTotais + jornaisTotais + lugaresTotais;
        float feitos = npcsConcluidos + jornaisColetados + lugaresConcluidos;

        progressoAlvo = totalPontos > 0 ? feitos / totalPontos : 0f;

        Debug.Log($"[ProgressaoFaseController] || UI Atualizada: {progressoAlvo:P0} ({feitos}/{totalPontos})");

        if (PhotonNetwork.IsMasterClient && feitos >= totalPontos && painelFinalFase != null)
        {
            painelFinalFase.MostrarPainelFinal();
            Debug.Log("[ProgressaoFaseController] % TODOS OBJETIVOS CONCLUÍDOS!");
        }
    }

    private void AtualizarUI()
    {
        AtualizarUILocal();
    }

    // ================================
    // MÉTODOS ADICIONAIS
    // ================================

    public void ForcarSincronizacao()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[ProgressaoFaseController] % Forçando sincronização manual...");
            SincronizarEstadoParaTodos();
        }
        else
        {
            Debug.Log("[ProgressaoFaseController] 🟹 Solicitando sincronização ao Master...");
            photonView.RPC("RPC_SolicitarSincronizacao", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void RPC_SolicitarSincronizacao()
    {
        Debug.Log("[ProgressaoFaseController] 🟹 Solicitação de sincronização recebida");
        SincronizarEstadoParaTodos();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"[ProgressaoFaseController] 🟹 Novo jogador entrou: {newPlayer.ActorNumber}");

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[ProgressaoFaseController] 🟹 Sincronizando estado com novo jogador...");
            StartCoroutine(SincronizarComNovoJogador());
        }
    }

    private IEnumerator SincronizarComNovoJogador()
    {
        yield return new WaitForSeconds(1f);
        SincronizarEstadoParaTodos();
    }

    public void VerificarESincronizarProgresso()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[ProgressaoFaseController] 🟹 Verificando e sincronizando progresso...");
            SincronizarEstadoParaTodos();
        }
    }

    // ===============================
    // MÉTODOS DE DEBUG
    // ===============================

    public void DebugEstadoAtual()
    {
        Debug.Log($"[ProgressaoFaseController] === DEBUG PROGRESSO ===");
        Debug.Log($"NPCs: {npcsConcluidos}/{npcsImportantesTotais}");
        Debug.Log($"Jornais: {jornaisColetados}/{jornaisTotais}");
        Debug.Log($"Lugares: {lugaresConcluidos}/{lugaresTotais}");
        Debug.Log($"NPC Importante Concluído: {npcImportanteJaConcluido}");
        Debug.Log($"Progresso: {progressoAtual:P2} -> {progressoAlvo:P2}");
        Debug.Log($"Master Client: {PhotonNetwork.IsMasterClient}");
        Debug.Log($"Jogador Local: {PhotonNetwork.LocalPlayer.ActorNumber}");
        Debug.Log($"===================================");
    }

    [PunRPC]
    public void RPC_DebugEstado()
    {
        DebugEstadoAtual();
    }

    public void DebugSincronizarTodos()
    {
        Debug.Log("[ProgressaoFaseController] ![Forçando debug e sincronização...]");
        DebugEstadoAtual();
        ForcarSincronizacao();
    }

    // NOVO: Método para forçar verificação de lugares (debug)
    public void ForcarVerificacaoLugares()
    {
        LugarVisitadoManager lugarManager = FindObjectOfType<LugarVisitadoManager>();
        if (lugarManager != null)
        {
            lugarManager.VerificarEAtualizarProgresso();
            lugarManager.DebugEstadoAtual();
            Debug.Log("[ProgressaoFaseController] Verificação de lugares forçada!");
        }
        else
        {
            Debug.LogError("[ProgressaoFaseController] LugarVisitadoManager não encontrado!");
        }
    }
}