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

            string prefabName = "PI_MC_1"; // padrão para o primeiro jogador

            // Define um prefab diferente para o segundo jogador em diante
            if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                prefabName = "PI_MC_2";
            }

            Vector3 spawnPos = new Vector3(Random.Range(-2, 2), 0f, Random.Range(-2, 2));
            PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity);
        }
    }
}