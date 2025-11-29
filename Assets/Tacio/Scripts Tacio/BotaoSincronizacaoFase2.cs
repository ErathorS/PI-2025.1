using UnityEngine;
using Photon.Pun;

public class BotaoSincronizacaoFase2 : MonoBehaviourPun
{
    [Header("Configuração")]
    public string tagJogador = "Player";

    private bool jogadorPerto = false;
    private PlayerUIReferences uiDoJogador;
    private bool jaInicializado = false; // 🔴 NOVO: Evitar múltiplas inicializações

    private void Start()
    {
        // 🔴 CORREÇÃO: Só desativa se não foi ativado pelo SincronizacaoManager
        if (!jaInicializado)
        {
            Debug.Log($"[BotaoSincronizacao] {gameObject.name} - Start chamado, desativando botão (inicial)");
            gameObject.SetActive(false);
            jaInicializado = true;
        }
    }

    private void OnEnable()
    {
        Debug.Log($"[BotaoSincronizacao] {gameObject.name} - OnEnable chamado, botão ATIVADO");
        
        // 🔴 CORREÇÃO: Marcar como inicializado quando ativado pelo manager
        jaInicializado = true;
    }

    private void OnDisable()
    {
        Debug.Log($"[BotaoSincronizacao] {gameObject.name} - OnDisable chamado, botão DESATIVADO");
    }

    // 🔴 NOVO: Método para ativação controlada pelo manager
    public void AtivarParaMissao()
    {
        Debug.Log($"[BotaoSincronizacao] {gameObject.name} - Ativado pelo manager");
        jaInicializado = true;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJogador)) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        jogadorPerto = true;
        uiDoJogador = other.GetComponentInChildren<PlayerUIReferences>();
        
        if (uiDoJogador != null && uiDoJogador.botaoInteracao != null)
        {
            uiDoJogador.botaoInteracao.gameObject.SetActive(true);
            uiDoJogador.botaoInteracao.onClick.RemoveAllListeners();
            uiDoJogador.botaoInteracao.onClick.AddListener(Interagir);
            Debug.Log($"[BotaoSincronizacao] {gameObject.name} - Botão de interação mostrado para Player {pv.OwnerActorNr}");
        }
        else
        {
            Debug.LogError($"[BotaoSincronizacao] {gameObject.name} - UI do jogador não encontrada!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagJogador)) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        jogadorPerto = false;
        
        if (uiDoJogador != null && uiDoJogador.botaoInteracao != null)
        {
            uiDoJogador.botaoInteracao.gameObject.SetActive(false);
            uiDoJogador.botaoInteracao.onClick.RemoveAllListeners();
        }
        
        uiDoJogador = null;
    }

    private void Interagir()
    {
        if (!jogadorPerto) return;

        int actorID = PhotonNetwork.LocalPlayer.ActorNumber;
        
        // ✅ Chama o SincronizacaoManager
        SincronizacaoManager.instancia.RegistrarToque(actorID);

        // Esconde o botão após o uso
        if (uiDoJogador != null && uiDoJogador.botaoInteracao != null)
        {
            uiDoJogador.botaoInteracao.gameObject.SetActive(false);
        }
    }
}