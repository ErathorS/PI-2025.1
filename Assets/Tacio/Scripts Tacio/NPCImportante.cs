using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class NPCImportante : MonoBehaviourPun
{
    [Header("Diálogo")]
    [TextArea(2, 5)] public string[] falas; // falas específicas desse NPC
    public GameObject painelDialogo;        // painel de diálogo do jogador
    public TMP_Text textoDialogo;           // texto do diálogo exibido
    public Button botaoAvancar;             // botão de avançar diálogo

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
        if (textoDialogo != null)
            textoDialogo.text = falas[indiceFala];
    }

    public void ProximaFala()
    {
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
        painelDialogo.SetActive(false);
        emDialogo = false;

        if (!jaConcluiu && progressoController != null)
        {
            progressoController.NPCImportanteConcluido();
            jaConcluiu = true;
            Debug.Log($"[NPCImportante] {gameObject.name} completado e progresso registrado!");
        }
    }
}
