using UnityEngine;
using Photon.Pun;

namespace FuroDeNoticia
{
    public class ControladorIntroducaoFase1 : MonoBehaviourPun
    {
        [Header("Referências")]
        public GameObject bloqueio;
        public PlataformaCoop plataforma1;
        public PlataformaCoop plataforma2;
        public string nomeCenaProxima = "Fase1";

        private bool acessoLiberado = false;
        private bool faseIniciada = false;

        public void LiberarAcesso()
        {
            photonView.RPC("RPC_LiberarAcesso", RpcTarget.All);
        }

        [PunRPC]
        private void RPC_LiberarAcesso()
        {
            acessoLiberado = true;
            if (bloqueio != null)
                bloqueio.SetActive(false);
        }


        [PunRPC]
        private void RPC_CarregarProximaCena()
        {
            PhotonNetwork.LoadLevel(nomeCenaProxima);
        }
    }
}
