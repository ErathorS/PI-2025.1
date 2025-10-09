using UnityEngine;
using Photon.Pun;

namespace FuroDeNoticia
{
    public class SpawnPlayers : MonoBehaviour
    {
        [Header("Posições de Spawn")]
        public Transform positionPrefabPlayer1;
        public Transform positionPrefabPlayer2;

        [Header("Prefabs dos Jogadores")]
        public GameObject player1Prefab;
        public GameObject player2Prefab;

        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("Photon Network is not connected. Cannot spawn player.");
                return;
            }

            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

            // Verifica qual jogador está conectando
            if (actorNumber == 1 && positionPrefabPlayer1 != null)
            {
                PhotonNetwork.Instantiate(player1Prefab.name, positionPrefabPlayer1.position, player1Prefab.transform.rotation);
            }
            else if (actorNumber == 2 && positionPrefabPlayer2 != null)
            {
                PhotonNetwork.Instantiate(player2Prefab.name, positionPrefabPlayer2.position, player2Prefab.transform.rotation);
            }
            else
            {
                Debug.LogWarning("ActorNumber não mapeado ou posição de spawn não definida.");
            }
        }
    }
}
