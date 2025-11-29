using UnityEngine;
using TMPro;
using Photon.Pun;

public class MissaoFase2Manager : MonoBehaviourPunCallbacks
{
    public static MissaoFase2Manager instancia;

    [Header("UI da Missão")]
    public GameObject painelMissao;
    public TMP_Text textoMissao;

    [Header("Referências")]
    public SincronizacaoManager sincronizacaoManager;
    public DialogoNPC npcImportante; // NPC que inicia a missão

    private bool missaoAtiva = false;
    private bool missaoConcluida = false;

    private void Awake()
    {
        // 🔴 CORREÇÃO: Garantir que só há uma instância
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 🔴 CORREÇÃO: Inicializar UI
        if (painelMissao != null)
            painelMissao.SetActive(false);
    }

    public void IniciarMissao()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        // 🔴 CORREÇÃO: Verificar se o manager está configurado
        if (sincronizacaoManager == null)
        {
            Debug.LogError("[MissaoFase2Manager] sincronizacaoManager não está atribuído!");
            return;
        }

        photonView.RPC("RPC_IniciarMissao", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_IniciarMissao()
    {
        missaoAtiva = true;
        missaoConcluida = false;

        if (painelMissao != null)
        {
            painelMissao.SetActive(true);
            textoMissao.text = "Sincronizem os botões para restaurar a energia!";
        }
        else
        {
            Debug.LogError("[MissaoFase2Manager] painelMissao não está atribuído!");
        }

        // Ativa o sistema de sincronização
        if (sincronizacaoManager != null)
        {
            sincronizacaoManager.IniciarSincronizacao();
        }
        else
        {
            Debug.LogError("[MissaoFase2Manager] sincronizacaoManager é null no RPC!");
        }

        Debug.Log("[MissaoFase2Manager] Missão da Fase 2 iniciada!");
    }

    // ... resto do código permanece igual
    public void MissaoConcluida()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC("RPC_MissaoConcluida", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_MissaoConcluida()
    {
        missaoConcluida = true;
        missaoAtiva = false;

        if (painelMissao != null)
        {
            painelMissao.SetActive(true);
            textoMissao.text = "Energia restaurada! Missão concluída.";
        }

        // Marca progresso no sistema
        var progresso = FindObjectOfType<ProgressaoFaseController>();
        if (progresso != null)
        {
            progresso.NPCImportanteConcluido();
        }
        else
        {
            Debug.LogError("[MissaoFase2Manager] ProgressaoFaseController não encontrado!");
        }

        Debug.Log("[MissaoFase2Manager] Missão da Fase 2 concluída!");
    }

    public void FinalizarMissao()
    {
        photonView.RPC("RPC_FinalizarMissao", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_FinalizarMissao()
    {
        if (painelMissao != null)
        {
            painelMissao.SetActive(false);
            textoMissao.text = "";
        }
    }
}