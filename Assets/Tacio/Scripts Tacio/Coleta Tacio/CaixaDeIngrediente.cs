using UnityEngine;
using Photon.Pun;

public class CaixaDeIngrediente : MonoBehaviourPun
{
    private PlayerUIReferences uiDoJogador;
    private bool jogadorPerto = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;  // Só o jogador dono vê o botão

        jogadorPerto = true;
        uiDoJogador = other.GetComponentInChildren<PlayerUIReferences>();

        if (uiDoJogador != null)
        {
            uiDoJogador.botaoInteracao.gameObject.SetActive(true);
            uiDoJogador.botaoInteracao.onClick.RemoveAllListeners();
            uiDoJogador.botaoInteracao.onClick.AddListener(Coletar);
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

    private void Coletar()
    {
        if (!jogadorPerto) return;

        // 🔥 Chama coleta sincronizada
        photonView.RPC("RPC_Coletar", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_Coletar()
    {
        // Atualiza contador
        ColetarCaixasManager.instancia.RegistrarColeta();

        // 🔥 Garante que o botão desapareça IMEDIATAMENTE
        if (uiDoJogador != null)
        {
            uiDoJogador.botaoInteracao.gameObject.SetActive(false);
            uiDoJogador.botaoInteracao.onClick.RemoveAllListeners();
        }

        uiDoJogador = null;
        jogadorPerto = false;

        // Desativa a caixa
        gameObject.SetActive(false);
    }
}
