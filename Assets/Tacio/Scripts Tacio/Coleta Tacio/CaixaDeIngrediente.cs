using UnityEngine;
using Photon.Pun;

public class CaixaDeIngrediente : MonoBehaviourPun
{
    private PlayerUIReferences uiDoJogador;
    private bool jogadorPerto = false;
    private bool coletado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return; // Apenas o jogador dono vê UI

        // Se já foi coletado, ignora
        if (coletado) return;

        jogadorPerto = true;
        uiDoJogador = other.GetComponentInChildren<PlayerUIReferences>();

        if (uiDoJogador != null)
        {
            uiDoJogador.botaoInteracao.gameObject.SetActive(true);
            uiDoJogador.botaoInteracao.onClick.RemoveAllListeners();
            uiDoJogador.botaoInteracao.onClick.AddListener(() => Coletar(pv.OwnerActorNr));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        jogadorPerto = false;

        if (uiDoJogador != null)
        {
            uiDoJogador.botaoInteracao.gameObject.SetActive(false);
            uiDoJogador.botaoInteracao.onClick.RemoveAllListeners();
        }

        uiDoJogador = null;
    }

    private void Coletar(int actorId)
    {
        if (!jogadorPerto || coletado) return;

        coletado = true;

        // 🔥 MASTER registra a coleta
        photonView.RPC("RPC_RegistrarNoMaster", RpcTarget.MasterClient, actorId);
    }

    [PunRPC]
    private void RPC_RegistrarNoMaster(int actorId)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Adiciona 1 caixa
        ColetarCaixasManager.instancia.AdicionarColetaMaster();

        // Sincroniza destruição
        photonView.RPC("RPC_DestruirCaixa", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_DestruirCaixa()
    {
        if (uiDoJogador != null)
            uiDoJogador.botaoInteracao.gameObject.SetActive(false);

        Destroy(gameObject);
    }
}
