using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlataformaCooperativa : MonoBehaviourPun
{
    public string proximaCena = "Fase1";
    private static bool player1NaPlataforma = false;
    private static bool player2NaPlataforma = false;

    private static Coroutine contagemCoroutine;
    private static bool carregandoCena = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null) return;

        if (pv.Owner.ActorNumber == 1)
            player1NaPlataforma = true;
        else if (pv.Owner.ActorNumber == 2)
            player2NaPlataforma = true;

        TentarIniciarContagem();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null) return;

        if (pv.Owner.ActorNumber == 1)
            player1NaPlataforma = false;
        else if (pv.Owner.ActorNumber == 2)
            player2NaPlataforma = false;
    }

    private void TentarIniciarContagem()
    {
        if (carregandoCena) return;

        if (player1NaPlataforma && player2NaPlataforma)
        {
            if (contagemCoroutine == null)
            {
                contagemCoroutine = StartCoroutine(IniciarContagem());
            }
        }
    }

    private IEnumerator IniciarContagem()
    {
        float tempo = 3f;
        float timer = 0f;

        Debug.Log("[PlataformaCooperativa] Ambos jogadores na plataforma. Iniciando contagem...");

        while (timer < tempo)
        {
            if (!(player1NaPlataforma && player2NaPlataforma))
            {
                Debug.Log("[PlataformaCooperativa] Um jogador saiu antes do tempo.");
                contagemCoroutine = null;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (PhotonNetwork.IsMasterClient && !carregandoCena)
        {
            carregandoCena = true;
            Debug.Log("[PlataformaCooperativa] 3 segundos completos! Carregando próxima cena...");
            PhotonNetwork.LoadLevel(proximaCena);
        }

        contagemCoroutine = null;
    }
}
