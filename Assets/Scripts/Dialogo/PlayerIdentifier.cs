using UnityEngine;
using Photon.Pun;

public class PlayerIdentifier : MonoBehaviourPun
{
    public int actorID;

    void Start()
    {
        actorID = photonView.Owner.ActorNumber;
        gameObject.name = $"PI_MC_{actorID}";
    }
}
