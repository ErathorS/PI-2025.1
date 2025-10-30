using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class TrocaCenaProximaFase : MonoBehaviourPunCallbacks
{
    [Header("Cena de destino")]
    [SerializeField] private string cenaDestino = "PI Fase 1";

    [Header("Verificação")]
    [SerializeField] private float intervaloVerificacao = 2f;

    private ProgressaoFaseController progresso;
    private bool carregando = false;

    void Start()
    {
        progresso = FindObjectOfType<ProgressaoFaseController>();
        InvokeRepeating(nameof(VerificarProgresso), 1f, intervaloVerificacao);
    }

    void VerificarProgresso()
    {
        if (carregando || progresso == null)
            return;

        bool concluiuNPCs = progresso.npcsImportantesTotais <= progresso.GetNpcsConcluidos();
        bool concluiuJornais = progresso.jornaisTotais <= progresso.GetJornaisColetados();
        bool concluiuLugares = progresso.lugaresTotais <= progresso.GetLugaresConcluidos();

        if (concluiuNPCs && concluiuJornais && concluiuLugares)
        {
            carregando = true;

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("[TrocaCenaProximaFase] Todas as missões completas. Carregando próxima cena...");
                PhotonNetwork.LoadLevel(cenaDestino);
            }
        }
    }
}
