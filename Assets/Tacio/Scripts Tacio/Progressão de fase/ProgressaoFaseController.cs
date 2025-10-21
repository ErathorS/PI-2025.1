using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ProgressaoFaseController : MonoBehaviourPunCallbacks
{
    [Header("Referências")]
    public Slider barraProgresso;
    public TMP_Text textoObjetivo;   // Texto das pessoas entrevistadas
    public TMP_Text textoJornais;    // 🆕 Texto dos jornais coletados
    public Image fillImage;

    [Header("Configurações de Objetivos")]
    public int npcsImportantesTotais = 3;
    public int jornaisTotais = 10;

    private int npcsConcluidos = 0;
    private int jornaisColetados = 0;
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

    void Update()
    {
        if (barraProgresso == null) return;

        // Suaviza o movimento da barra
        barraProgresso.value = Mathf.Lerp(barraProgresso.value, progressoAlvo, Time.deltaTime * velocidadeLerp);

        // Muda a cor da barra conforme o progresso
        if (fillImage != null)
            fillImage.color = Color.Lerp(corInicial, corFinal, barraProgresso.value);
    }

    // 🧮 Calcula o progresso total (NPCs + Jornais)
    void AtualizarProgresso()
    {
        float progressoNPC = (float)npcsConcluidos / npcsImportantesTotais;
        float progressoJornal = (float)jornaisColetados / jornaisTotais;
        progressoAlvo = (progressoNPC + progressoJornal) / 2f; // média dos dois objetivos
    }

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
}
