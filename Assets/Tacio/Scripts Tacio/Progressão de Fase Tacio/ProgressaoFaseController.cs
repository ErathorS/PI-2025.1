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
    public int lugaresTotais = 2; // Quantos lugares precisam ser completados

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

    // =====================================================================
    // 🟢 NPC IMPORTANTE ENTREVISTADO
    // =====================================================================

    public void NPCImportanteConcluido()
    {
        photonView.RPC("RPC_PedirRegistroNPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_PedirRegistroNPC()
    {
        RPC_AtualizarProgressoNPC(); // Executa diretamente no Master
    }

    [PunRPC]
    void RPC_AtualizarProgressoNPC()
    {
        npcsConcluidos++;
        AtualizarProgresso();
        AtualizarTextoObjetivo();
    }

    // =====================================================================
    // 🟡 JORNAL COLETADO
    // =====================================================================

    // Chamado por QUALQUER jogador que pegou o jornal
    public void JornalColetado()
    {
        photonView.RPC("RPC_PedirRegistroJornal", RpcTarget.MasterClient);
    }

    // Apenas o Master executa isso
    [PunRPC]
    void RPC_PedirRegistroJornal()
    {
        RPC_AtualizarProgressoJornal();
    }

    [PunRPC]
    void RPC_AtualizarProgressoJornal()
    {
        if (jornaisColetados < jornaisTotais)
            jornaisColetados++;

        AtualizarProgresso();
        AtualizarTextoJornais();
    }

    // =====================================================================
    // 🔵 LUGAR VISITADO (GRUPO COMPLETO)
    // =====================================================================

    public void LugarVisitadoConcluido()
    {
        photonView.RPC("RPC_PedirRegistroLugar", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_PedirRegistroLugar()
    {
        RPC_AtualizarProgressoLugar();
    }

    [PunRPC]
    void RPC_AtualizarProgressoLugar()
    {
        if (lugaresConcluidos < lugaresTotais)
            lugaresConcluidos++;

        AtualizarProgresso();
        AtualizarTextoLugares();
    }

    // =====================================================================
    // 🎚️ ATUALIZAÇÃO VISUAL DA BARRA DE PROGRESSO
    // =====================================================================

    void Update()
    {
        if (barraProgresso == null) return;

        barraProgresso.value = Mathf.Lerp(barraProgresso.value, progressoAlvo, Time.deltaTime * velocidadeLerp);

        if (fillImage != null)
            fillImage.color = Color.Lerp(corInicial, corFinal, barraProgresso.value);
    }

    // =====================================================================
    // 🧮 CÁLCULO DO PROGRESSO TOTAL
    // =====================================================================

    void AtualizarProgresso()
    {
        float progressoNPC = (float)npcsConcluidos / npcsImportantesTotais;
        float progressoJornal = (float)jornaisColetados / jornaisTotais;
        float progressoLugar = (float)lugaresConcluidos / lugaresTotais;

        // Média dos 3 objetivos
        progressoAlvo = (progressoNPC + progressoJornal + progressoLugar) / 3f;
    }

    // =====================================================================
    // 📝 ATUALIZAÇÃO DOS TEXTOS INDIVIDUAIS
    // =====================================================================

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
