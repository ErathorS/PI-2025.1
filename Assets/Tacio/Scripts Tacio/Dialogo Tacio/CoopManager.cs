using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlatManager : MonoBehaviourPun
{
    [Header("Referências")]
    public PlataformaCoop[] plataformas;
    [SerializeField] private string proximaCena = "Fase1";

    private bool carregandoCena = false;

    public void AtualizarPlataformas()
    {
        if (carregandoCena) return;

        bool p1 = false;
        bool p2 = false;

        foreach (var plat in plataformas)
        {
            if (plat.player1NaPlataforma) p1 = true;
            if (plat.player2NaPlataforma) p2 = true;
        }

        if (p1 && p2)
        {
            photonView.RPC(nameof(IniciarTransicaoCena), RpcTarget.All);
        }
    }

    [PunRPC]
    void IniciarTransicaoCena()
    {
        if (!carregandoCena)
        {
            carregandoCena = true;
            StartCoroutine(Transicao());
        }
    }

    private IEnumerator Transicao()
    {
        Debug.Log("✅ Ambos os jogadores estão nas plataformas! Carregando próxima cena...");
        yield return new WaitForSeconds(1.5f);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(proximaCena);
        }
    }
}
