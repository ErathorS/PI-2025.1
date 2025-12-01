using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class NPCImportante : MonoBehaviourPun
{
    [Header("Diálogo")]
    [TextArea(2, 5)] public string[] falas;
    public GameObject painelDialogo;
    public TMP_Text textoDialogo;
    public Button botaoAvancar;

    private int indiceFala = 0;
    private bool emDialogo = false;
    private bool jaConcluiu = false;

    private ProgressaoFaseController progressoController;

    void Start()
    {
        progressoController = FindObjectOfType<ProgressaoFaseController>();

        if (painelDialogo != null)
            painelDialogo.SetActive(false);
    }

    public void IniciarDialogo()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[NPCImportante] Apenas o MasterClient pode iniciar este diálogo.");
            return;
        }

        if (jaConcluiu || falas.Length == 0) return;

        emDialogo = true;
        indiceFala = 0;

        painelDialogo.SetActive(true);
        AtualizarFala();

        botaoAvancar.onClick.RemoveAllListeners();
        botaoAvancar.onClick.AddListener(ProximaFala);
    }

    void AtualizarFala()
    {
        if (textoDialogo != null && indiceFala < falas.Length)
            textoDialogo.text = falas[indiceFala];
    }

    public void ProximaFala()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        indiceFala++;

        if (indiceFala >= falas.Length)
        {
            EncerrarDialogo();
            return;
        }

        AtualizarFala();
    }

    void EncerrarDialogo()
    {
        if (painelDialogo != null)
            painelDialogo.SetActive(false);

        emDialogo = false;

        if (!jaConcluiu && PhotonNetwork.IsMasterClient && progressoController != null)
        {
            progressoController.NPCImportanteConcluido();
            jaConcluiu = true;

            Debug.Log($"[NPCImportante] NPC '{gameObject.name}' concluído pelo MASTER e progresso registrado!");
        }
    }
}
