using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class PushableObject : MonoBehaviourPun
{
    public float pushSpeed = 2f;

    private Dictionary<int, Vector3> activePushers = new Dictionary<int, Vector3>();

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PhotonView pv = collision.gameObject.GetComponent<PhotonView>();
            if (pv != null && !activePushers.ContainsKey(pv.Owner.ActorNumber))
            {
                activePushers.Add(pv.Owner.ActorNumber, Vector3.zero);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PhotonView pv = collision.gameObject.GetComponent<PhotonView>();
            if (pv != null && activePushers.ContainsKey(pv.Owner.ActorNumber))
            {
                activePushers.Remove(pv.Owner.ActorNumber);
            }
        }
    }

    [PunRPC]
    public void UpdatePushDirection(int actorID, Vector3 direction)
    {
        if (!photonView.IsMine) return;

        if (activePushers.ContainsKey(actorID))
        {
            activePushers[actorID] = direction;
        }
    }

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

            if (combinedDirection.magnitude > 0.1f)
            {
                transform.Translate(combinedDirection.normalized * pushSpeed * Time.deltaTime, Space.World);
            }
        }
    }
}
