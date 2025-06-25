using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Dialogo : MonoBehaviourPun
{
    [Header("Referências")]
    public Button botaoDialogo1;
    public Button botaoDialogo2;
    public GameObject painelDialogo1;
    public GameObject painelDialogo2;
    public TMP_Text textoDialogo1;
    public TMP_Text textoDialogo2;
    public IndicadorNpc indicadorNPC;

    [Header("Configuração")]
    public bool dialogoGlobal = false;

    [Header("Linhas do Diálogo")]
    [TextArea(2, 4)]
    [SerializeField]
    private string[] linhasDialogo = {
        "",
        "Olá, viajante!",
        "Você está pronto para uma nova aventura?",
        "Boa sorte no seu caminho!"
    };

    private int linhaAtual = 0;
    private bool falando = false;
    private bool jogadorPerto = false;

    private GameObject jogadorAtual;
    private PhotonView photonViewDoIniciador;

    void Start()
{
    // Procurar Canvas do Jogador 1
    GameObject canvas1 = GameObject.FindGameObjectWithTag("CanvasP1");
    if (canvas1 == null)
    {
        Debug.LogError("❌ CanvasP1 não encontrado!");
    }
    else
    {
        painelDialogo1 = canvas1.transform.Find("PainelDialogo1")?.gameObject;
        textoDialogo1 = painelDialogo1?.transform.Find("TMP_Text")?.GetComponent<TMP_Text>();
        botaoDialogo1 = canvas1.transform.Find("BotaoDialogo1")?.GetComponent<Button>();

        if (painelDialogo1 == null) Debug.LogError("❌ PainelDialogo1 não encontrado dentro de CanvasP1");
        if (textoDialogo1 == null) Debug.LogError("❌ TMP_Text não encontrado dentro de PainelDialogo1");
        if (botaoDialogo1 == null) Debug.LogError("❌ BotaoDialogo1 não encontrado dentro de CanvasP1");

        botaoDialogo1?.onClick.AddListener(() => AoClicarNoBotao("PI_MC_1"));
        painelDialogo1.SetActive(false);
    }

    // Procurar Canvas do Jogador 2
    GameObject canvas2 = GameObject.FindGameObjectWithTag("CanvasP2");
    if (canvas2 == null)
    {
        Debug.LogError("❌ CanvasP2 não encontrado!");
    }
    else
    {
        painelDialogo2 = canvas2.transform.Find("PainelDialogo2")?.gameObject;
        textoDialogo2 = painelDialogo2?.transform.Find("TMP_Text")?.GetComponent<TMP_Text>();
        botaoDialogo2 = canvas2.transform.Find("BotaoDialogo2")?.GetComponent<Button>();

        if (painelDialogo2 == null) Debug.LogError("❌ PainelDialogo2 não encontrado dentro de CanvasP2");
        if (textoDialogo2 == null) Debug.LogError("❌ TMP_Text não encontrado dentro de PainelDialogo2");
        if (botaoDialogo2 == null) Debug.LogError("❌ BotaoDialogo2 não encontrado dentro de CanvasP2");

        botaoDialogo2?.onClick.AddListener(() => AoClicarNoBotao("PI_MC_2"));
        painelDialogo2.SetActive(false);
    }
}



    void OnDestroy()
    {
        botaoDialogo1.onClick.RemoveAllListeners();
        botaoDialogo2.onClick.RemoveAllListeners();
    }
    private void AoClicarNoBotao(string nomeJogador)
    {
        int actorID = PhotonNetwork.LocalPlayer.ActorNumber;

        if (!dialogoGlobal)
        {
            if (!falando) IniciarDialogoLocal(actorID);
            else AvancarDialogoLocal(actorID);
        }
        else
        {
            if (!falando)
            {
                photonView.RPC("IniciarDialogoGlobal", RpcTarget.AllBuffered, actorID);
            }
            else if (photonViewDoIniciador != null && photonViewDoIniciador.IsMine)
            {
                photonView.RPC("AvancarDialogoGlobal", RpcTarget.AllBuffered);
            }
        }
    }
    private void IniciarDialogoLocal(int actorID)
    {
        linhaAtual = 0;
        falando = true;

        if (actorID == 1)
        {
            painelDialogo1.SetActive(true);
            textoDialogo1.text = linhasDialogo[linhaAtual];
        }
        else if (actorID == 2)
        {
            painelDialogo2.SetActive(true);
            textoDialogo2.text = linhasDialogo[linhaAtual];
        }

        indicadorNPC?.MarcarComoConversado();
    }

    private void AvancarDialogoLocal(int actorID)
    {
        linhaAtual++;

        if (linhaAtual < linhasDialogo.Length)
        {
            if (actorID == 1)
                textoDialogo1.text = linhasDialogo[linhaAtual];
            else if (actorID == 2)
                textoDialogo2.text = linhasDialogo[linhaAtual];
        }
        else
        {
            FinalizarDialogoLocal();
        }
    }

    private void FinalizarDialogoLocal()
    {
        painelDialogo1.SetActive(false);
        painelDialogo2.SetActive(false);
        falando = false;
        linhaAtual = 0;
    }

    [PunRPC]
    private void IniciarDialogoGlobal(int actorID)
    {
        linhaAtual = 0;
        falando = true;
        painelDialogo1.SetActive(true);
        painelDialogo2.SetActive(true);
        textoDialogo1.text = linhasDialogo[linhaAtual];
        textoDialogo2.text = linhasDialogo[linhaAtual];

        photonViewDoIniciador = PhotonView.Find(actorID);
    }

    [PunRPC]
    private void AvancarDialogoGlobal()
    {
        linhaAtual++;
        if (linhaAtual < linhasDialogo.Length)
        {
            textoDialogo1.text = linhasDialogo[linhaAtual];
            textoDialogo2.text = linhasDialogo[linhaAtual];
        }
        else
        {
            painelDialogo1.SetActive(false);
            painelDialogo2.SetActive(false);
            falando = false;
            linhaAtual = 0;
        }
    }
    public void MostrarBotao(GameObject jogador)
    {
        jogadorPerto = true;
        jogadorAtual = jogador;

        PhotonView view = jogador.GetComponent<PhotonView>();
        int actorID = view.Owner.ActorNumber;

        if (actorID == 1)
        {
            painelDialogo1.SetActive(true);
            textoDialogo1.text = linhasDialogo[linhaAtual];
        }
        else if (actorID == 2)
        {
            painelDialogo2.SetActive(true);
            textoDialogo2.text = linhasDialogo[linhaAtual];
        }
    }

    public void EsconderTudo()
    {
        jogadorPerto = false;
        botaoDialogo1.gameObject.SetActive(false);
        botaoDialogo2.gameObject.SetActive(false);
        painelDialogo1.SetActive(false);
        painelDialogo2.SetActive(false);
        falando = false;
        linhaAtual = 0;
    }
}
