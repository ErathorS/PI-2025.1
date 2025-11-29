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

    [Header("Reset e Coleta")]
    public GameObject botaoReset;
    public ColetarCaixasManager coletor;
    public DialogoNPC npcEntrega;

    private bool missaoAtiva = false;
    private bool missaoConcluida = false;
    private float tempoRestante;

    private void Awake()
    {
        instancia = this;
    }

    public void IniciarMissao()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC("RPC_IniciarMissao", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_IniciarMissao()
    {
        missaoAtiva = true;
        missaoConcluida = false;

        painelMissao.SetActive(true);
        botaoReset.SetActive(false);

        tempoRestante = 240f;

        coletor.AtivarCaixasParaMissao();
        AtualizarUI();
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
        textoMissao.text = "O tempo acabou!\nVocês querem tentar novamente?";

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
        coletor.ResetarFase();

        missaoAtiva = true;
        missaoConcluida = false;
        tempoRestante = 240f;

        botaoReset.SetActive(false);
        textoMissao.text = "Procurem as caixas!";
        AtualizarUI();
    }

    public void MissaoFinalizada()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC("RPC_MissaoFinalizada", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_MissaoFinalizada()
    {
        missaoConcluida = true;
        missaoAtiva = false;

        painelMissao.SetActive(true);
        botaoReset.SetActive(false);

        textoMissao.text = "Excelente trabalho!\nFalem com Dona Cida para entregar.";

        // 🔴 CORREÇÃO: Verificar se o NPC existe e é da Fase 1
        if (npcEntrega != null)
        {
            // Para Fase 1, usar o método original
            if (!npcEntrega.ehNPCFase2)
            {
                npcEntrega.AtivarDialogoDeEntrega();
            }
            else
            {
                Debug.LogError("[MissaoFase1Manager] NPC de entrega está configurado como Fase 2!");
            }
        }
        else
        {
            Debug.LogError("[MissaoFase1Manager] npcEntrega não está atribuído!");
        }
    }

    public void FinalizarEntrega()
    {
        photonView.RPC("RPC_FinalizarEntrega", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_FinalizarEntrega()
    {
        painelMissao.SetActive(false);
        botaoReset.SetActive(false);

        textoTimer.text = "";
        textoMissao.text = "";

        if (coletor != null)
        {
            coletor.textoCaixas.text = "";
            coletor.textoTempo.text = "";
        }
    }
}