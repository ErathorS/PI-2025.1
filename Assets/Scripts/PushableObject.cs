using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviourPun
{
    public float pushForce = 5f;
    private Rigidbody rb;

    private Dictionary<int, Vector3> activePushers = new Dictionary<int, Vector3>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

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

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        if (activePushers.Count >= 2)
        {
            List<Vector3> directions = new List<Vector3>(activePushers.Values);

            // Verifica se as direções são parecidas o suficiente
            if (Vector3.Angle(directions[0], directions[1]) < 45f)
            {
                Vector3 forceDir = (directions[0] + directions[1]).normalized;

                rb.AddForce(forceDir * pushForce, ForceMode.Force);
            }
        }
    }
}
