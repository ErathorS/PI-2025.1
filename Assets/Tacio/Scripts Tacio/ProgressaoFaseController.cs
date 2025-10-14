using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ProgressaoFaseController : MonoBehaviourPunCallbacks
{
    [Header("Referências")]
    public Slider barraProgresso;
    public int npcsImportantesTotais = 3;

    private int npcsConcluidos = 0;

    void Start()
    {
        if (barraProgresso != null)
            barraProgresso.value = 0f;
    }

    // 🔹 Chamado pelos NPCs importantes
    public void NPCImportanteConcluido()
    {
        photonView.RPC("RPC_AtualizarProgresso", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_AtualizarProgresso()
    {
        npcsConcluidos++;
        AtualizarBarra();
    }

    void AtualizarBarra()
    {
        if (barraProgresso == null) return;

        float progresso = (float)npcsConcluidos / npcsImportantesTotais;
        barraProgresso.value = progresso;
        Debug.Log($"[Progresso] NPCs Concluídos: {npcsConcluidos}/{npcsImportantesTotais}");
    }
}
