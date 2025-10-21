using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ProgressaoFaseController : MonoBehaviourPunCallbacks
{
    [Header("Referências")]
    public Slider barraProgresso;
    public TMP_Text textoObjetivo;   // Pessoas entrevistadas
    public TMP_Text textoJornais;    // Jornais coletados
    public TMP_Text textoLugares;    // 🆕 Lugares visitados
    public Image fillImage;

    [Header("Configurações de Objetivos")]
    public int npcsImportantesTotais = 3;
    public int jornaisTotais = 20;
    public int lugaresTotais = 2; // Quantos lugares precisam ser visitados

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

    // 🟢 Quando um NPC importante for entrevistado
    public void NPCImportanteConcluido()
    {
        photonView.RPC("RPC_AtualizarProgressoNPC", RpcTarget.AllBuffered);
    }

    // 🟡 Quando um jornal for coletado
    public void JornalColetado()
    {
        photonView.RPC("RPC_AtualizarProgressoJornal", RpcTarget.AllBuffered);
    }

    // 🔵 Quando um lugar for visitado
    public void LugarVisitadoConcluido()
    {
        photonView.RPC("RPC_AtualizarProgressoLugar", RpcTarget.AllBuffered);
    }

    // RPCs (sincronizados entre todos os jogadores)
    [PunRPC]
    void RPC_AtualizarProgressoNPC()
    {
        npcsConcluidos++;
        AtualizarProgresso();
        AtualizarTextoObjetivo();
    }

    [PunRPC]
    void RPC_AtualizarProgressoJornal()
    {
        jornaisColetados++;
        AtualizarProgresso();
        AtualizarTextoJornais();
    }

    [PunRPC]
    void RPC_AtualizarProgressoLugar()
    {
        lugaresConcluidos++;
        AtualizarProgresso();
        AtualizarTextoLugares();
    }

    void Update()
    {
        if (barraProgresso == null) return;

        // Suaviza o movimento da barra de progresso
        barraProgresso.value = Mathf.Lerp(barraProgresso.value, progressoAlvo, Time.deltaTime * velocidadeLerp);

        // Muda a cor gradualmente conforme o progresso
        if (fillImage != null)
            fillImage.color = Color.Lerp(corInicial, corFinal, barraProgresso.value);
    }

    // 🧮 Calcula o progresso total (NPCs + Jornais + Lugares)
    void AtualizarProgresso()
    {
        float progressoNPC = (float)npcsConcluidos / npcsImportantesTotais;
        float progressoJornal = (float)jornaisColetados / jornaisTotais;
        float progressoLugar = (float)lugaresConcluidos / lugaresTotais;

        // Média dos três objetivos
        progressoAlvo = (progressoNPC + progressoJornal + progressoLugar) / 3f;
    }

    // Atualiza textos individuais
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
