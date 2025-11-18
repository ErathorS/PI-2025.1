using UnityEngine;
using Photon.Pun;

namespace FuroDeNoticia
{
    public class TrocarCenaFinal : MonoBehaviourPun
    {
        [Header("Cena de destino final")]
        [SerializeField] private string _sceneToLoad = "CutsceneFinal";

        [Header("Carregar apenas pelo Master?")]
        public bool apenasMasterCarrega = true;

        private bool carregando = false;

        public void IrParaCenaFinal()
        {
            if (carregando) return;
            carregando = true;

            if (apenasMasterCarrega)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("[TrocarCenaFinal] Master carregando cena CutsceneFinal...");
                    PhotonNetwork.LoadLevel(_sceneToLoad);
                }
                else
                {
                    Debug.Log("[TrocarCenaFinal] Aguardando Master carregar a cena final...");
                }
            }
            else
            {
                photonView.RPC(nameof(RPC_CarregarTodos), RpcTarget.All);
            }
        }

        [PunRPC]
        private void RPC_CarregarTodos()
        {
            if (!carregando)
            {
                carregando = true;
                Debug.Log("[TrocarCenaFinal] Carregando cena final em todos os jogadores...");
                PhotonNetwork.LoadLevel(_sceneToLoad);
            }
        }
    }
}
