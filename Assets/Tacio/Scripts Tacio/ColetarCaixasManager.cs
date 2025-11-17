using UnityEngine;
using TMPro;
using Photon.Pun;

public class ColetarCaixasManager : MonoBehaviourPun
{
    public static ColetarCaixasManager instancia;

    [Header("Caixas na cena (originais)")]
    public CaixaDeIngrediente[] caixasOriginais;

    private Vector3[] posicoesIniciais;
    private Quaternion[] rotacoesIniciais;

    [Header("UI")]
    public TMP_Text textoCaixas;
    public TMP_Text textoTempo;
    public GameObject botaoReset;

    [Header("Config")]
    public float tempoLimite = 60f;
    private float tempoAtual;
    private int caixasColetadas = 0;
    private int totalCaixas;
    private bool faseAtiva = false;

    private void Awake()
    {
        instancia = this;
    }

    private void Start()
    {
        totalCaixas = caixasOriginais.Length;

        posicoesIniciais = new Vector3[totalCaixas];
        rotacoesIniciais = new Quaternion[totalCaixas];

        // Salva posições originais e deixa TODAS desativadas
        for (int i = 0; i < totalCaixas; i++)
        {
            posicoesIniciais[i] = caixasOriginais[i].transform.position;
            rotacoesIniciais[i] = caixasOriginais[i].transform.rotation;

            caixasOriginais[i].gameObject.SetActive(false);
        }
    }

    // 🔥 Chamada quando o Master inicia a missão
    public void AtivarCaixasParaMissao()
    {
        caixasColetadas = 0;
        tempoAtual = tempoLimite;
        faseAtiva = true;

        // Reativa e reseta TODAS as caixas
        for (int i = 0; i < totalCaixas; i++)
        {
            caixasOriginais[i].transform.position = posicoesIniciais[i];
            caixasOriginais[i].transform.rotation = rotacoesIniciais[i];
            caixasOriginais[i].gameObject.SetActive(true);
        }

        AtualizarUI();
        botaoReset.SetActive(false);
    }

    // 🔹 Chamado por CaixaDeIngrediente quando o jogador coleta
    public void RegistrarColeta()
    {
        caixasColetadas++;
        AtualizarUI();

        if (caixasColetadas >= totalCaixas)
        {
            faseAtiva = false;
            MissaoFase1Manager.instancia.MissaoFinalizada();
        }
    }

    private void Update()
    {
        if (!faseAtiva) return;

        tempoAtual -= Time.deltaTime;

        if (tempoAtual <= 0)
        {
            tempoAtual = 0;
            faseAtiva = false;

            // Mostra botão de reset SOMENTE NO MASTER
            if (PhotonNetwork.IsMasterClient)
                botaoReset.SetActive(true);
        }

        AtualizarUI();
    }

    // 🔁 Reset total da fase (sem destruir nada)
    public void ResetarFase()
    {
        // Reativa e reseta TODAS as caixas
        for (int i = 0; i < totalCaixas; i++)
        {
            caixasOriginais[i].transform.position = posicoesIniciais[i];
            caixasOriginais[i].transform.rotation = rotacoesIniciais[i];
            caixasOriginais[i].gameObject.SetActive(true);
        }

        caixasColetadas = 0;
        tempoAtual = tempoLimite;
        faseAtiva = true;

        botaoReset.SetActive(false);
        AtualizarUI();
    }

    private void AtualizarUI()
    {
        textoCaixas.text = $"Caixas: {caixasColetadas}/{totalCaixas}";
        textoTempo.text = $"{Mathf.RoundToInt(tempoAtual)}s";
    }
}
