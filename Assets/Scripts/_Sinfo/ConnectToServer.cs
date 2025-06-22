using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace FuroDeNoticia
{
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string _sceneToLoad;

        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            SceneManager.LoadScene(_sceneToLoad);
        }
    }
}
