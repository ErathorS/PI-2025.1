using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ProgressaoFaseController : MonoBehaviourPunCallbacks
{
    [Header("Referências")]
    public Slider barraProgresso;
    public TMP_Text textoObjetivo;   // se quiser um título geral, tipo "Objetivos:"
    public TMP_Text textoJornais;
    public TMP_Text textoLugares;
    public TMP_Text textoNpcs;

    [Header("Configurações de Objetivos")]
    public int npcsImportantesTotais = 1;
    public int jornaisTotais = 10;
    public int lugaresTotais = 4;   // ajusta aqui no inspector conforme seu jogo

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

    void Start()
    {
        AtualizarUI();
    }

    void Update()
    {
        // animação suave da barra
        progressoAtual = Mathf.Lerp(progressoAtual, progressoAlvo, Time.deltaTime * velocidadeLerp);

        if (barraProgresso != null)
            barraProgresso.value = progressoAtual;

        if (fillImage != null)
            fillImage.color = Color.Lerp(corInicial, corFinal, progressoAtual);
    }

    // ============================
    //  MÉTODOS PÚBLICOS (CHAMADOS
    //  PELOS OUTROS SCRIPTS)
    // ============================

    public void JornalColetado()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_JornalColetado), RpcTarget.MasterClient);
            return;
        }

        RPC_JornalColetado();
    }

    public void LugarVisitadoConcluido()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_LugarVisitadoConcluido), RpcTarget.MasterClient);
            return;
        }

        RPC_LugarVisitadoConcluido();
    }

    public void NPCImportanteConcluido()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_NPCImportanteConcluido), RpcTarget.MasterClient);
            return;
        }

        RPC_NPCImportanteConcluido();
    }

    // ============================
    //       RPCs NO MASTER
    // ============================

    [PunRPC]
    private void RPC_JornalColetado()
    {
        jornaisColetados = Mathf.Min(jornaisColetados + 1, jornaisTotais);
        SincronizarEstadoParaTodos();
    }

    [PunRPC]
    private void RPC_LugarVisitadoConcluido()
    {
        lugaresConcluidos = Mathf.Min(lugaresConcluidos + 1, lugaresTotais);
        SincronizarEstadoParaTodos();
    }

    [PunRPC]
    private void RPC_NPCImportanteConcluido()
    {
        npcsConcluidos = Mathf.Min(npcsConcluidos + 1, npcsImportantesTotais);
        SincronizarEstadoParaTodos();
    }

    private void SincronizarEstadoParaTodos()
    {
        photonView.RPC(nameof(RPC_SyncEstado), RpcTarget.All,
            npcsConcluidos, jornaisColetados, lugaresConcluidos);
    }

    // ============================
    //     RPC DE SINCRONIZAÇÃO
    // ============================

    [PunRPC]
    private void RPC_SyncEstado(int npcs, int jornais, int lugares)
    {
        npcsConcluidos = npcs;
        jornaisColetados = jornais;
        lugaresConcluidos = lugares;

        AtualizarUI();
    }

    // ============================
    //        LÓGICA DE UI
    // ============================

    private void AtualizarUI()
    {
        // Aqui voltamos ao formato "frase + valor":
        if (textoNpcs != null)
            textoNpcs.text = $"Pessoa Entrevistada: {npcsConcluidos}/{npcsImportantesTotais}";

        if (textoJornais != null)
            textoJornais.text = $"Jornais Coletados: {jornaisColetados}/{jornaisTotais}";

        if (textoLugares != null)
            textoLugares.text = $"Lugares Visitados: {lugaresConcluidos}/{lugaresTotais}";

        float totalPontos = npcsImportantesTotais + jornaisTotais + lugaresTotais;
        float feitos = npcsConcluidos + jornaisColetados + lugaresConcluidos;

        progressoAlvo = totalPontos > 0 ? feitos / totalPontos : 0f;

        // Quando chegar em 100%, mostra painel final
        if (feitos >= totalPontos && painelFinalFase != null)
        {
            painelFinalFase.MostrarPainelFinal();
        }
    }
}
