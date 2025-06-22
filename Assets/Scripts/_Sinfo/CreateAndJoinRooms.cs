using UnityEngine;
using TMPro;
using Photon.Pun;

namespace FuroDeNoticia
{
    public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_InputField _input_create;
        [SerializeField] private TMP_InputField _input_join;

        [SerializeField] private string _sceneToLoad;

        public void CreateRoom()
        {
            PhotonNetwork.CreateRoom(_input_create.text);
        }

        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(_input_join.text);
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel(_sceneToLoad);
        }
    }
}
