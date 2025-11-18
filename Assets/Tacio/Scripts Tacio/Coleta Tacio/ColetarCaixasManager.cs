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

        for (int i = 0; i < totalCaixas; i++)
        {
            posicoesIniciais[i] = caixasOriginais[i].transform.position;
            rotacoesIniciais[i] = caixasOriginais[i].transform.rotation;
            caixasOriginais[i].gameObject.SetActive(false);
        }
    }

    // Chamado apenas pelo MasterClient
    public void AdicionarColetaMaster()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        caixasColetadas++;

        photonView.RPC("RPC_SincronizarUI", RpcTarget.AllBuffered, caixasColetadas);

        if (caixasColetadas >= totalCaixas)
        {
            faseAtiva = false;
            MissaoFase1Manager.instancia.MissaoFinalizada();
        }
    }

    [PunRPC]
    private void RPC_SincronizarUI(int novoValor)
    {
        caixasColetadas = novoValor;
        AtualizarUI();
    }

    // Ativar caixas no início
    public void AtivarCaixasParaMissao()
    {
        caixasColetadas = 0;
        tempoAtual = tempoLimite;
        faseAtiva = true;

        for (int i = 0; i < totalCaixas; i++)
        {
            caixasOriginais[i].transform.position = posicoesIniciais[i];
            caixasOriginais[i].transform.rotation = rotacoesIniciais[i];
            caixasOriginais[i].gameObject.SetActive(true);
        }

        AtualizarUI();
        botaoReset.SetActive(false);
    }

    // Atualizar Relógio
    private void Update()
    {
        if (!faseAtiva) return;

        tempoAtual -= Time.deltaTime;

        if (tempoAtual <= 0)
        {
            tempoAtual = 0;
            faseAtiva = false;

            if (PhotonNetwork.IsMasterClient)
                botaoReset.SetActive(true);
        }

        AtualizarUI();
    }

    private void AtualizarUI()
    {
        if (textoCaixas != null)
            textoCaixas.text = $"Caixas: {caixasColetadas}/{totalCaixas}";

        if (textoTempo != null)
            textoTempo.text = $"{Mathf.RoundToInt(tempoAtual)}s";
    }

    public void ResetarFase()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        caixasColetadas = 0;
        tempoAtual = tempoLimite;
        faseAtiva = true;

        // Reativa todas as caixas e volta elas para posição original
        for (int i = 0; i < totalCaixas; i++)
        {
            caixasOriginais[i].transform.position = posicoesIniciais[i];
            rotacoesIniciais[i] = caixasOriginais[i].transform.rotation;
            caixasOriginais[i].gameObject.SetActive(true);
        }

        photonView.RPC("RPC_SincronizarUI", RpcTarget.AllBuffered, caixasColetadas);

        botaoReset.SetActive(false);
    }

}
