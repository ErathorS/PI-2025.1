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

    private bool missaoAtiva = false;
    private float tempoRestante;

    private void Awake()
    {
        instancia = this;
    }

    // 🔹 Chamado pelo NPC importante após diálogo
    public void IniciarMissao()
    {
        if (missaoAtiva) return;

        photonView.RPC("RPC_IniciarMissao", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_IniciarMissao()
    {
        missaoAtiva = true;
        tempoRestante = duracaoMissao;
        painelMissao.SetActive(true);

        textoMissao.text =
            "As caixas de ingredientes de Dona Cida foram espalhadas.\n" +
            "Procurem pelo Largo e encontrem todas antes que estraguem!\n" +
            "Vocês têm 4 minutos.";

        AtualizarUI();
    }

    private void Update()
    {
        if (!missaoAtiva) return;

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
        int minutos = Mathf.FloorToInt(tempoRestante / 60);
        int segundos = Mathf.FloorToInt(tempoRestante % 60);

        textoTimer.text = $"{minutos:00}:{segundos:00}";
    }

    private void MissaoFalhou()
    {
        painelMissao.SetActive(true);
        textoMissao.text = "O tempo acabou!\nO dendê estragou ao sol... tentem novamente!";
    }
}
