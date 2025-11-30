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
    public DialogoNPC npcImportante;

    private bool missaoAtiva = false;
    private bool missaoConcluida = false;

    private PhotonView photonView; // 🔴 CORREÇÃO: Referência explícita

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

        // 🔴 CORREÇÃO: Obter referência do PhotonView
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("[MissaoFase2Manager] PhotonView não encontrado no GameObject!");
        }

        // Inicializar UI
        if (painelMissao != null)
            painelMissao.SetActive(false);
    }

    public void IniciarMissao()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        if (sincronizacaoManager == null)
        {
            Debug.LogError("[MissaoFase2Manager] sincronizacaoManager não está atribuído!");
            return;
        }

        // 🔴 CORREÇÃO: Usar referência local do PhotonView
        if (photonView != null)
        {
            photonView.RPC("RPC_IniciarMissao", RpcTarget.AllBuffered);
        }
        else
        {
            Debug.LogError("[MissaoFase2Manager] photonView é null!");
        }
    }

    [PunRPC] // 🔴 CORREÇÃO: Garantir que está marcado como PunRPC
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

    public void MissaoConcluida()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        // 🔴 CORREÇÃO: Usar referência local do PhotonView
        if (photonView != null)
        {
            photonView.RPC("RPC_MissaoConcluida", RpcTarget.AllBuffered);
        }
        else
        {
            Debug.LogError("[MissaoFase2Manager] photonView é null no MissaoConcluida!");
        }
    }

    [PunRPC] // 🔴 CORREÇÃO: Garantir que está marcado como PunRPC
    private void RPC_MissaoConcluida()
    {
        missaoConcluida = true;
        missaoAtiva = false;

        if (painelMissao != null)
        {
            painelMissao.SetActive(true);
            textoMissao.text = "Energia restaurada! Missão concluída.";
        }

        Debug.Log("[MissaoFase2Manager] Missão da Fase 2 concluída!");
    }

    public void FinalizarMissao()
    {
        // 🔴 CORREÇÃO: Usar referência local do PhotonView
        if (photonView != null)
        {
            photonView.RPC("RPC_FinalizarMissao", RpcTarget.AllBuffered);
        }
        else
        {
            Debug.LogError("[MissaoFase2Manager] photonView é null no FinalizarMissao!");
        }
    }

    [PunRPC] // 🔴 CORREÇÃO: Garantir que está marcado como PunRPC
    private void RPC_FinalizarMissao()
    {
        if (painelMissao != null)
        {
            painelMissao.SetActive(false);
            textoMissao.text = "";
        }
    }
}