using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ProgressaoFaseController : MonoBehaviourPunCallbacks
{
    [Header("Referências")]
    public Slider barraProgresso;
    public TMP_Text textoObjetivo;   // NPCs entrevistados
    public TMP_Text textoJornais;    // Jornais coletados
    public TMP_Text textoLugares;    // Lugares visitados
    public Image fillImage;

    [Header("Configurações de Objetivos")]
    public int npcsImportantesTotais = 3;
    public int jornaisTotais = 20;
    public int lugaresTotais = 2;

    private int npcsConcluidos = 0;
    private int jornaisColetados = 0;
    private int lugaresConcluidos = 0;

    private float progressoAlvo = 0f;

    [Header("Cores e Velocidade")]
    public Color corInicial = Color.cyan;
    public Color corFinal = Color.green;
    public float velocidadeLerp = 3f;

    void Start()
    {
        if (barraProgresso != null)
        {
            barraProgresso.minValue = 0f;
            barraProgresso.maxValue = 1f;
            barraProgresso.value = 0f;
        }

        AtualizarTextoObjetivo();
        AtualizarTextoJornais();
        AtualizarTextoLugares();
    }

    // ========== NPC IMPORTANTE ENTREVISTADO ==========

    public void NPCImportanteConcluido()
    {
        // Qualquer jogador chama isso -> pedido vai para o Master
        photonView.RPC("RPC_PedirRegistroNPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_PedirRegistroNPC()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Master autoriza e manda atualizar para todos
        photonView.RPC("RPC_SincronizarNPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_SincronizarNPC()
    {
        if (npcsConcluidos >= npcsImportantesTotais)
            return;

        npcsConcluidos++;
        AtualizarProgresso();
        AtualizarTextoObjetivo();
    }

    // ========== JORNAL COLETADO ==========

    public void JornalColetado()
    {
        photonView.RPC("RPC_PedirRegistroJornal", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_PedirRegistroJornal()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC("RPC_SincronizarJornal", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_SincronizarJornal()
    {
        if (jornaisColetados >= jornaisTotais)
            return;

        jornaisColetados++;
        AtualizarProgresso();
        AtualizarTextoJornais();
    }

    // ========== LUGAR VISITADO (GRUPO COMPLETO) ==========

    public void LugarVisitadoConcluido()
    {
        photonView.RPC("RPC_PedirRegistroLugar", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_PedirRegistroLugar()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC("RPC_SincronizarLugar", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_SincronizarLugar()
    {
        if (lugaresConcluidos >= lugaresTotais)
            return;

        lugaresConcluidos++;
        AtualizarProgresso();
        AtualizarTextoLugares();
    }

    // ========== ATUALIZAÇÃO VISUAL DA BARRA ==========

    void Update()
    {
        if (barraProgresso == null) return;

        barraProgresso.value = Mathf.Lerp(barraProgresso.value, progressoAlvo, Time.deltaTime * velocidadeLerp);

        if (fillImage != null)
            fillImage.color = Color.Lerp(corInicial, corFinal, barraProgresso.value);
    }

    // ========== CÁLCULO DO PROGRESSO TOTAL ==========

    void AtualizarProgresso()
    {
        float progressoNPC = npcsImportantesTotais > 0 ? (float)npcsConcluidos / npcsImportantesTotais : 0f;
        float progressoJornal = jornaisTotais > 0 ? (float)jornaisColetados / jornaisTotais : 0f;
        float progressoLugar = lugaresTotais > 0 ? (float)lugaresConcluidos / lugaresTotais : 0f;

        progressoAlvo = (progressoNPC + progressoJornal + progressoLugar) / 3f;
    }

    // ========== TEXTOS DA UI ==========

    void AtualizarTextoObjetivo()
    {
        if (textoObjetivo != null)
            textoObjetivo.text = $"Pessoas Entrevistadas: {npcsConcluidos}/{npcsImportantesTotais}";
    }

    void AtualizarTextoJornais()
    {
        if (textoJornais != null)
            textoJornais.text = $"Jornais Coletados: {jornaisColetados}/{jornaisTotais}";
    }

    void AtualizarTextoLugares()
    {
        if (textoLugares != null)
            textoLugares.text = $"Lugares Visitados: {lugaresConcluidos}/{lugaresTotais}";
    }
}
