using UnityEngine;
using Photon.Pun;

namespace FuroDeNoticia
{
    public class SpawnPlayers : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;

        private void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity);
                return;
            }

            Debug.LogError("Photon Network is not connected. Cannot spawn player.");
        }
    }
}
