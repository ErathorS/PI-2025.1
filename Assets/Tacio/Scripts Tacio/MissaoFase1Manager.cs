using UnityEngine;
using TMPro;
using Photon.Pun;

public class MissaoFase1Manager : MonoBehaviourPunCallbacks
{
    public static MissaoFase1Manager instancia;

    [Header("UI da Missão")]
    public GameObject painelMissao;
    public TMP_Text textoMissao;
    public TMP_Text textoTimer;

    [Header("Configurações")]
    public float duracaoMissao = 240f; // 4 minutos

    [Header("Reset e Coleta")]
    public GameObject botaoReset;         
    public ColetarCaixasManager coletor;
    public DialogoNPC npcEntrega;

    private bool missaoAtiva = false;
    private bool missaoConcluida = false;
    private float tempoRestante;

    [Header("Caixas da missão")]
    public CaixaDeIngrediente[] caixasDaMissao;

    private void Awake()
    {
        instancia = this;
    }

    // Inicia missão após diálogo do NPC importante
    public void IniciarMissao()
    {
        if (missaoAtiva) return;
        photonView.RPC("RPC_IniciarMissao", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_IniciarMissao()
    {
        missaoAtiva = true;
        missaoConcluida = false;

        tempoRestante = duracaoMissao;

        painelMissao.SetActive(true);
        botaoReset.SetActive(false);

        // textoMissao.text =
        //     "As caixas de ingredientes de Dona Cida foram espalhadas.\n" +
        //     "Procurem pelo Largo e encontrem todas antes que estraguem!\n" +
        //     "Vocês têm 4 minutos.";

        AtivarTodasAsCaixas();
        AtualizarUI();
        coletor.ResetarCaixas(); // reseta UI e contador também
    }

    private void Update()
    {
        if (!missaoAtiva || missaoConcluida) return;

        tempoRestante -= Time.deltaTime;

        if (tempoRestante <= 0)
        {
            tempoRestante = 0;
            missaoAtiva = false;
            MissaoFalhou();
        }

        AtualizarUI();
    }

    private void AtualizarUI()
    {
        int m = Mathf.FloorToInt(tempoRestante / 60);
        int s = Mathf.FloorToInt(tempoRestante % 60);

        textoTimer.text = $"{m:00}:{s:00}";
    }

    private void MissaoFalhou()
    {
        painelMissao.SetActive(true);

        textoMissao.text =
            "O tempo acabou!\n" +
            "O dendê estragou ao sol...\n\n" +
            "Vocês querem tentar novamente?";

        // botão só aparece para o master
        if (PhotonNetwork.IsMasterClient)
            botaoReset.SetActive(true);
    }

    public void BotaoResetarMissao()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC("RPC_ResetarMissao", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_ResetarMissao()
    {
        coletor.ResetarCaixas();
        tempoRestante = duracaoMissao;

        botaoReset.SetActive(false);
        missaoAtiva = true;
        missaoConcluida = false;

        textoMissao.text =
            "Procurem as caixas de ingredientes de Dona Cida antes que estraguem!\n" +
            "Vocês têm 4 minutos.";

        AtualizarUI();
    }

    public void MissaoFinalizada()
    {
        if (missaoConcluida) return;

        missaoConcluida = true;
        missaoAtiva = false;

        painelMissao.SetActive(true);
        botaoReset.SetActive(false);

        textoMissao.text =
            "Excelente trabalho!\n" +
            "Todas as caixas foram recuperadas.\n" +
            "Voltem até o NPC para entregar a missão!";

        npcEntrega.AtivarDialogoDeEntrega();
    }
    private void AtivarTodasAsCaixas()
    {
        foreach (var caixa in caixasDaMissao)
        {
            if (caixa != null)
            {
                caixa.gameObject.SetActive(true);
                caixa.ResetarEstado();
            }
        }
    }
}
