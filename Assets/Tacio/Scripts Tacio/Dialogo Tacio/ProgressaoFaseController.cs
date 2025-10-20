using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ProgressaoFaseController : MonoBehaviourPunCallbacks
{
    [Header("Referências")]
    public Slider barraProgresso;
    public TMP_Text textoObjetivo; // 🆕 Texto do objetivo na tela
    public int npcsImportantesTotais = 3;

    private int npcsConcluidos = 0;
    private float progressoAtual = 0f;
    private float progressoAlvo = 0f;

    [Header("Cores e Velocidade")]
    public Image fillImage;
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

        AtualizarTextoObjetivo(); // Mostra o texto inicial
    }

    public void NPCImportanteConcluido()
    {
        photonView.RPC("RPC_AtualizarProgresso", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_AtualizarProgresso()
    {
        npcsConcluidos++;
        progressoAlvo = (float)npcsConcluidos / npcsImportantesTotais;
        AtualizarTextoObjetivo();
    }

    void Update()
    {
        if (barraProgresso == null) return;

        // Suaviza o preenchimento da barra
        barraProgresso.value = Mathf.Lerp(barraProgresso.value, progressoAlvo, Time.deltaTime * velocidadeLerp);

        // Troca de cor com base no progresso
        if (fillImage != null)
            fillImage.color = Color.Lerp(corInicial, corFinal, barraProgresso.value);
    }

    void AtualizarTextoObjetivo()
    {
        if (textoObjetivo != null)
            textoObjetivo.text = $"Pessoas Entrevistadas {npcsConcluidos}/{npcsImportantesTotais}";
    }
}
