using Unity.Netcode;
using UnityEngine;

public class PlayerFishing : NetworkBehaviour
{
    public GameObject fishPrefab;
    public Transform fishSpawnPoint;

    public void TryFishing()
    {
        if (IsOwner)
        {
            TryFishServerRpc();
        }
    }

    [ServerRpc]
    void TryFishServerRpc(ServerRpcParams rpcParams = default)
    {
        bool sucesso = Random.value < 0.5f; // 50% chance
        if (sucesso)
        {
            GameObject fish = Instantiate(fishPrefab, fishSpawnPoint.position, Quaternion.identity);
            fish.GetComponent<NetworkObject>().Spawn();
            ShowFishFeedbackClientRpc();
        }
    }

    [ClientRpc]
    void ShowFishFeedbackClientRpc()
    {
        // Animação ou som
        Debug.Log("Peixe fisgado!");
    }
}
