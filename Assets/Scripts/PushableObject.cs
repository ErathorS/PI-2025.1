using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class PushableObject : MonoBehaviourPun
{
    [Header("Velocidade de Empurrão")]
    public float pushSpeed = 2f;

    // Armazena os jogadores ativos empurrando: [ActorNumber] → Direção
    private Dictionary<int, Vector3> activePushers = new Dictionary<int, Vector3>();

    // Quando um jogador entra em contato com a caixa
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return; // Apenas o dono da caixa processa

        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && !activePushers.ContainsKey(pv.Owner.ActorNumber))
            {
                // Adiciona o jogador à lista de empurradores
                activePushers.Add(pv.Owner.ActorNumber, Vector3.zero);
            }
        }
    }

    // Quando um jogador sai da área de contato
    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && activePushers.ContainsKey(pv.Owner.ActorNumber))
            {
                activePushers.Remove(pv.Owner.ActorNumber);
            }
        }
    }

    // Método chamado por RPC para atualizar a direção de um jogador empurrando
    [PunRPC]
    public void UpdatePushDirection(int actorID, Vector3 direction)
    {
        if (!photonView.IsMine) return;

        if (activePushers.ContainsKey(actorID))
        {
            activePushers[actorID] = direction;
        }
    }

    // Movimenta a caixa com base na média das direções dos empurradores ativos
    private void Update()
    {
        if (!photonView.IsMine) return;

        if (activePushers.Count >= 2)
        {
            Vector3 combinedDirection = Vector3.zero;

            foreach (var dir in activePushers.Values)
            {
                combinedDirection += dir;
            }

            combinedDirection /= activePushers.Count;

            // Só move se a direção for significativa
            if (combinedDirection.magnitude > 0.1f)
            {
                transform.Translate(combinedDirection.normalized * pushSpeed * Time.deltaTime, Space.World);
            }
        }
    }
}
