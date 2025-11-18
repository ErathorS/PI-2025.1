using UnityEngine;
using Photon.Pun;

public class PainelFinalFaseController : MonoBehaviourPun
{
    public static PainelFinalFaseController instancia;

    [Header("UI Final")]
    public GameObject painelFimDeFase;

    private void Awake()
    {
        instancia = this;
    }

    // Mostrar o painel para todos
    [PunRPC]
    public void RPC_MostrarPainelFinal()
    {
        painelFimDeFase.SetActive(true);
    }

    // Botão "Sim" → Carrega a próxima cena
    public void ConfirmarIrParaProximaCena()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC(nameof(RPC_CarregarProximaCena), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_CarregarProximaCena()
    {
        PhotonNetwork.LoadLevel("CutsceneFinal");
    }

    // Botão opcional "Não"
    public void Cancelar()
    {
        painelFimDeFase.SetActive(false);
    }
}
