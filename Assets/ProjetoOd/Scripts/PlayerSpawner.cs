using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPun
{
    public GameObject playerPrefab;

    void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            Vector3 pos = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
            PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);
        }
    }
}
