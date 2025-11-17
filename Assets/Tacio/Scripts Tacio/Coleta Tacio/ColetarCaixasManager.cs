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

    // Chamado pelo MissaoFase1Manager (RPC) quando a missão começa
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

    // Chamado pela CaixaDeIngrediente quando um jogador coleta
    public void RegistrarColeta()
    {
        // Qualquer jogador pede para o Master registrar
        photonView.RPC("RPC_PedirRegistrarColeta", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_PedirRegistrarColeta()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Master manda sincronizar para todos
        photonView.RPC("RPC_RegistrarColeta", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_RegistrarColeta()
    {
        caixasColetadas++;
        AtualizarUI();

        if (caixasColetadas >= totalCaixas)
        {
            faseAtiva = false;

            if (MissaoFase1Manager.instancia != null)
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

            if (PhotonNetwork.IsMasterClient)
                botaoReset.SetActive(true);
        }

        AtualizarUI();
    }

    // Reset total da fase (sem destruir nada)
    public void ResetarFase()
    {
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
        if (textoCaixas != null)
            textoCaixas.text = $"Caixas: {caixasColetadas}/{totalCaixas}";

        if (textoTempo != null)
            textoTempo.text = $"{Mathf.RoundToInt(tempoAtual)}s";
    }
}
