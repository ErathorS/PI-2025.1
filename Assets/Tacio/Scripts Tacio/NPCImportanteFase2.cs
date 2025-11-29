using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class NPCImportanteFase2 : MonoBehaviourPun
{
    [Header("Diálogo")]
    [TextArea(2, 5)] public string[] falas;
    public GameObject painelDialogo;
    public TMP_Text textoDialogo;
    public Button botaoAvancar;

    [Header("Sincronização de Luzes")]
    public GameObject[] botoesSincronizacao; // Botões que aparecem após o diálogo

    private int indiceFala = 0;
    private bool emDialogo = false;
    private bool jaConcluiu = false;

    void Start()
    {
        if (painelDialogo != null)
            painelDialogo.SetActive(false);
            
        // Esconde os botões de sincronização inicialmente
        EsconderBotoesSincronizacao();
    }

    public void IniciarDialogo()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (jaConcluiu || falas.Length == 0) return;

        emDialogo = true;
        indiceFala = 0;

        painelDialogo.SetActive(true);
        AtualizarFala();

        botaoAvancar.onClick.RemoveAllListeners();
        botaoAvancar.onClick.AddListener(ProximaFala);
    }

    void AtualizarFala()
    {
        if (textoDialogo != null && indiceFala < falas.Length)
            textoDialogo.text = falas[indiceFala];
    }

    public void ProximaFala()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        indiceFala++;
        if (indiceFala >= falas.Length)
        {
            EncerrarDialogo();
            return;
        }

        AtualizarFala();
    }

    void EncerrarDialogo()
    {
        if (painelDialogo != null)
            painelDialogo.SetActive(false);

        emDialogo = false;

        if (!jaConcluiu && PhotonNetwork.IsMasterClient)
        {
            // Ativa os botões de sincronização para todos os jogadores
            photonView.RPC("RPC_AtivarBotoesSincronizacao", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_AtivarBotoesSincronizacao()
    {
        MostrarBotoesSincronizacao();
        Debug.Log("[NPCImportanteFase2] Botões de sincronização ativados!");
    }

    private void MostrarBotoesSincronizacao()
    {
        foreach (var botao in botoesSincronizacao)
        {
            if (botao != null)
                botao.SetActive(true);
        }
    }

    private void EsconderBotoesSincronizacao()
    {
        foreach (var botao in botoesSincronizacao)
        {
            if (botao != null)
                botao.SetActive(false);
        }
    }

    // Chamado quando a sincronização é concluída
    public void SincronizacaoConcluida()
    {
        jaConcluiu = true;
        EsconderBotoesSincronizacao();
        
        var progresso = FindObjectOfType<ProgressaoFaseController>();
        progresso?.NPCImportanteConcluido();
    }
}