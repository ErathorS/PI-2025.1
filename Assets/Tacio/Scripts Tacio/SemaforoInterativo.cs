using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class SemaforoInterativo : MonoBehaviourPun
{
    public SemaforoController semaforo;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        PlayerUIReferences ui = other.GetComponentInChildren<PlayerUIReferences>();
        if (ui == null || ui.botaoInteragir == null) return;

        ui.botaoInteragir.gameObject.SetActive(true);
        ui.botaoInteragir.onClick.RemoveAllListeners();
        ui.botaoInteragir.onClick.AddListener(() => OnPlayerInteract(ui));
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        PlayerUIReferences ui = other.GetComponentInChildren<PlayerUIReferences>();
        if (ui == null || ui.botaoInteragir == null) return;

        ui.botaoInteragir.onClick.RemoveAllListeners();
        ui.botaoInteragir.gameObject.SetActive(false);
    }

    void OnPlayerInteract(PlayerUIReferences ui)
    {
        if (semaforo != null)
            semaforo.PedirAcionarSemaforo();

        ui.botaoInteragir.onClick.RemoveAllListeners();
        ui.botaoInteragir.gameObject.SetActive(false);
    }
}
