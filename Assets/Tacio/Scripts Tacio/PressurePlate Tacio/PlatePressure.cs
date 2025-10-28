using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
public class PlatePressure : MonoBehaviour
{
    [Header("ID da Placa")]
    public int plateID = 1;

    private PlatePressureManager manager;

    void Start()
    {
        manager = FindObjectOfType<PlatePressureManager>();

        if (manager == null)
        {
            Debug.LogError("[PlatePressure] Nenhum PlatePressureManager encontrado na cena!");
            return;
        }

        if (manager.GetComponent<PhotonView>() == null)
        {
            Debug.LogError("[PlatePressure] O PlatePressureManager precisa ter um PhotonView anexado!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && manager != null && manager.photonView != null)
        {
            manager.ReportPlatePressed(plateID, pv.Owner.ActorNumber);
            manager.photonView.RPC("RPC_ReportPlatePressed", RpcTarget.All, plateID, pv.Owner.ActorNumber);
        }
    }
}
