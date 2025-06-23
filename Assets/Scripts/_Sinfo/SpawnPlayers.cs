using UnityEngine;
using Photon.Pun;

namespace FuroDeNoticia
{
    public class SpawnPlayers : MonoBehaviour
    {
        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("Photon Network is not connected. Cannot spawn player.");
                return;
            }

            string prefabName = "PI MC 1"; // padrão para o primeiro jogador

            // Define um prefab diferente para o segundo jogador em diante
            if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                prefabName = "PI MC 2";
            }

            Vector3 spawnPos = new Vector3(Random.Range(-5, 5), 1f, Random.Range(-5, 5));
            PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity);
        }
    }
}