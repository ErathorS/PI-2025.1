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

    [Header("Configuração Fase 2")]
    public bool usarSincronizacao = true;
    public bool autoFinalizarMissao = false;

    private bool missaoAtiva = false;
    private bool missaoConcluida = false;
    private bool tarefaEntregue = false;

    private PhotonView photonView;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("[MissaoFase2Manager] PhotonView não encontrado no GameObject!");
            photonView = gameObject.AddComponent<PhotonView>();
        }
    }

    private void Start()
    {
        if (painelMissao != null)
            painelMissao.SetActive(false);

        if (sincronizacaoManager != null)
        {
            sincronizacaoManager.ehParaFase2 = true;
            sincronizacaoManager.ehParaFase3 = false;
        }
    }

    public void IniciarMissao()
    {
        if (!PhotonNetwork.IsMasterClient) return;

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
        tarefaEntregue = false;

        if (painelMissao != null)
        {
            painelMissao.SetActive(true);
            textoMissao.text = "Sincronizem os botões para restaurar a energia!";
        }
        else
        {
            Debug.LogError("[MissaoFase2Manager] painelMissao não está atribuído!");
        }

        if (usarSincronizacao && sincronizacaoManager != null)
        {
            sincronizacaoManager.IniciarSincronizacao();
        }
        else if (!usarSincronizacao)
        {
            Debug.LogWarning("[MissaoFase2Manager] Sincronização desativada por configuração!");
        }

        Debug.Log("[MissaoFase2Manager] Missão da Fase 2 iniciada!");
    }

    public void MissaoConcluida()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log("[MissaoFase2Manager] Missão concluída - enviando para todos");
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
            textoMissao.text = "Energia restaurada! Missão concluída.\nVolte ao NPC para entregar.";
        }

        if (autoFinalizarMissao)
        {
            FinalizarMissao();
        }

        Debug.Log("[MissaoFase2Manager] Missão da Fase 2 concluída!");
    }

    public void EntregaConcluida()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC("RPC_EntregaConcluida", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_EntregaConcluida()
    {
        tarefaEntregue = true;
        FinalizarMissao();
        Debug.Log("[MissaoFase2Manager] Entrega concluída!");
    }

    public void FinalizarMissao()
    {
        if (photonView != null)
        {
            photonView.RPC("RPC_FinalizarMissao", RpcTarget.AllBuffered);
        }
        else
        {
            Debug.LogError("[MissaoFase2Manager] photonView é null no FinalizarMissao!");
        }
    }

    [PunRPC]
    private void RPC_FinalizarMissao()
    {
        if (painelMissao != null)
        {
            painelMissao.SetActive(false);
            textoMissao.text = "";
        }

        Debug.Log("[MissaoFase2Manager] Missão finalizada para todos os jogadores!");
    }

    public bool IsMissaoAtiva()
    {
        return missaoAtiva;
    }

    public bool IsMissaoConcluida()
    {
        return missaoConcluida;
    }

    public bool IsTarefaEntregue()
    {
        return tarefaEntregue;
    }

    public void DebugEstado()
    {
        Debug.Log($"[MissaoFase2Manager] === DEBUG ===");
        Debug.Log($"MissaoAtiva: {missaoAtiva}");
        Debug.Log($"MissaoConcluida: {missaoConcluida}");
        Debug.Log($"TarefaEntregue: {tarefaEntregue}");
        Debug.Log($"SincronizacaoManager: {sincronizacaoManager != null}");
        Debug.Log($"NPCImportante: {npcImportante != null}");
        Debug.Log($"===================================");
    }
}