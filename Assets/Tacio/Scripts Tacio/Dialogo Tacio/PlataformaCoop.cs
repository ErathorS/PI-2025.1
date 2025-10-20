using UnityEngine;
using Photon.Pun;

public class PlataformaCoop : MonoBehaviourPun
{
    public bool player1NaPlataforma;
    public bool player2NaPlataforma;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null)
        {
            if (pv.Owner.ActorNumber == 1)
                player1NaPlataforma = true;
            else if (pv.Owner.ActorNumber == 2)
                player2NaPlataforma = true;

            photonView.RPC(nameof(AtualizarEstadoPlataforma), RpcTarget.All, player1NaPlataforma, player2NaPlataforma);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null)
        {
            if (pv.Owner.ActorNumber == 1)
                player1NaPlataforma = false;
            else if (pv.Owner.ActorNumber == 2)
                player2NaPlataforma = false;

            photonView.RPC(nameof(AtualizarEstadoPlataforma), RpcTarget.All, player1NaPlataforma, player2NaPlataforma);
        }
    }

    [PunRPC]
    void AtualizarEstadoPlataforma(bool p1, bool p2)
    {
        player1NaPlataforma = p1;
        player2NaPlataforma = p2;

        if (player1NaPlataforma && player2NaPlataforma)
        {
            Debug.Log("✅ Ambos os jogadores estão na plataforma!");
            // Aqui você pode chamar o PhotonNetwork.LoadLevel("CenaFase1");
        }
    }
}
