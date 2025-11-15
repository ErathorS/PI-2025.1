using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CaixaDeIngrediente : MonoBehaviourPun
{
    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;

    private bool jogadorPerto = false;
    private PlayerUIReferences uiDoJogador;

    private void Awake()
    {
        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

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

    public void Coletar()
    {
        if (!jogadorPerto) return;

        photonView.RPC("RPC_ColetarCaixa", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_ColetarCaixa()
    {
        gameObject.SetActive(false);
        ColetarCaixasManager coletor = FindObjectOfType<ColetarCaixasManager>();
        coletor.RegistrarColeta();
    }

    public void ResetarEstado()
    {
        transform.position = posicaoInicial;
        transform.rotation = rotacaoInicial;

        gameObject.SetActive(true);
    }
}
