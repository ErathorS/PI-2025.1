using UnityEngine;
using Photon.Pun;

public class BotaoSincronizacaoFase2 : MonoBehaviourPun
{
    [Header("Configuração")]
    public string tagJogador = "Player";

    private bool jogadorPerto = false;
    private PlayerUIReferences uiDoJogador;
    private bool jaInicializado = false;

    private void Start()
    {
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
        jaInicializado = true;
    }

    private void OnDisable()
    {
        Debug.Log($"[BotaoSincronizacao] {gameObject.name} - OnDisable chamado, botão DESATIVADO");
    }

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

            Debug.Log($"[BotaoSincronizacao] {gameObject.name} - Botão de interação mostrado para Player {pv.Owner.ActorNumber}");
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

        Debug.Log($"[BotaoSincronizacao] {gameObject.name} - Jogador {actorID} interagiu!");

        if (SincronizacaoManager.instancia != null)
        {
            SincronizacaoManager.instancia.RegistrarToque(actorID);
        }

        if (uiDoJogador != null && uiDoJogador.botaoInteracao != null)
        {
            uiDoJogador.botaoInteracao.gameObject.SetActive(false);
            Debug.Log($"[BotaoSincronizacao] {gameObject.name} - Botão de interação escondido após uso");
        }
    }
}