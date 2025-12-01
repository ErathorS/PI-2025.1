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
        if (pv == null || !pv.IsMine) return;

        if (coletado) return;

        jogadorPerto = true;
        uiDoJogador = other.GetComponentInChildren<PlayerUIReferences>();
        if (uiDoJogador != null)
        {
            uiDoJogador.botaoInteracao.gameObject.SetActive(true);
            uiDoJogador.botaoInteracao.onClick.RemoveAllListeners();
            uiDoJogador.botaoInteracao.onClick.AddListener(() => Coletar(pv.Owner.ActorNumber));
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

        photonView.RPC("RPC_RegistrarNoMaster", RpcTarget.MasterClient, actorId);
    }

    [PunRPC]
    private void RPC_RegistrarNoMaster(int actorId)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (ColetarCaixasManager.instancia != null)
        {
            ColetarCaixasManager.instancia.AdicionarColetaMaster();
        }

        photonView.RPC("RPC_DestruirCaixa", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_DestruirCaixa()
    {
        if (uiDoJogador != null)
            uiDoJogador.botaoInteracao.gameObject.SetActive(false);

        gameObject.SetActive(false);
        
        Debug.Log($"[CaixaDeIngrediente] Caixa coletada e desativada");
    }

    public void ResetarCaixa()
    {
        coletado = false;
        gameObject.SetActive(true);
        
        if (uiDoJogador != null)
        {
            uiDoJogador.botaoInteracao.gameObject.SetActive(false);
            uiDoJogador.botaoInteracao.onClick.RemoveAllListeners();
        }
        
        uiDoJogador = null;
        jogadorPerto = false;
    }
}