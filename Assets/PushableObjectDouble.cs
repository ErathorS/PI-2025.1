using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
public class PushableObjectDouble : MonoBehaviourPun
{
    public float pushForce = 5f;
    public float angleThreshold = 45f;
    
    private Rigidbody rb;
    private Dictionary<int, Vector3> activePushers = new Dictionary<int, Vector3>();
    private float originalDrag;
    private bool wasBeingPushed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        originalDrag = rb.drag;
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
                if (activePushers.Count < 2)
                {
                    StopPushing();
                }
            }
        }
    }

    [PunRPC]
    public void UpdatePushDirection(int actorID, Vector3 direction)
    {
        if (!photonView.IsMine) return;

        if (activePushers.ContainsKey(actorID))
        {
            activePushers[actorID] = direction.normalized;
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        bool canPush = activePushers.Count >= 2 && CheckPushDirections();

        if (canPush)
        {
            PushObject();
            wasBeingPushed = true;
        }
        else if (wasBeingPushed)
        {
            StopPushing();
            wasBeingPushed = false;
        }
    }

    private bool CheckPushDirections()
    {
        List<Vector3> directions = new List<Vector3>(activePushers.Values);
        return Vector3.Angle(directions[0], directions[1]) <= angleThreshold;
    }

    private void PushObject()
    {
        rb.drag = 0;
        Vector3 combinedDirection = (activePushers[0] + activePushers[1]).normalized;
        Vector3 force = new Vector3(combinedDirection.x, 0, combinedDirection.z) * pushForce;
        rb.velocity = force;
    }

    private void StopPushing()
    {
        rb.drag = originalDrag;
        rb.velocity = Vector3.zero;
    }
}