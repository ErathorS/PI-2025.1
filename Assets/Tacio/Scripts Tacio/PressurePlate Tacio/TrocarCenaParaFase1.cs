using UnityEngine;
using Photon.Pun;

namespace FuroDeNoticia
{
    public class TrocarCenaParaFase1 : MonoBehaviourPun
    {
        [Header("Cena de destino")]
        [SerializeField] private string _sceneToLoad = "PI Fase 1";

        [Header("Configuração de ativação")]
        [Tooltip("Se verdadeiro, o carregamento só será feito pelo Master Client.")]
        public bool apenasMasterCarrega = true;

        private bool carregando = false;

        public void IrParaFase1()
        {
            if (carregando) return;
            carregando = true;

            if (apenasMasterCarrega)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("[TrocarCenaParaFase1] Master carregando a cena 'PI Fase 1'...");
                    PhotonNetwork.LoadLevel(_sceneToLoad);
                }
                else
                {
                    Debug.Log("[TrocarCenaParaFase1] Esperando o Master carregar a cena...");
                }
            }
            else
            {
                photonView.RPC(nameof(RPC_LoadSceneAll), RpcTarget.All);
            }
        }

        [PunRPC]
        private void RPC_LoadSceneAll()
        {
            if (!carregando)
            {
                carregando = true;
                Debug.Log("[TrocarCenaParaFase1] Carregando cena em todos os clientes...");
                PhotonNetwork.LoadLevel(_sceneToLoad);
            }
        }
    }
}
