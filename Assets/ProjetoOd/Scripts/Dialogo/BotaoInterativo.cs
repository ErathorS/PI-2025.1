using UnityEngine;
using Photon.Pun;

public class BotaoInterativo : MonoBehaviourPun
{
    public GameObject Parada;
    public static bool StateSemaforo = false;

    private void Start()
    {
        StateSemaforo = false;
    }

    public void ExecutarAcao()
    {
        photonView.RPC("AlternarSemaforo", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void AlternarSemaforo()
    {
        StateSemaforo = !StateSemaforo;
        Parada.SetActive(StateSemaforo);
    }
}
