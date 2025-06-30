using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;

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
        Debug.Log("Iniciando coroutine para configurar canvas...");
        StartCoroutine(EsperarJogadoresEConfigurarCanvas());
    }

    private IEnumerator EsperarJogadoresEConfigurarCanvas()
    {
        while (GameObject.FindGameObjectsWithTag("Player").Length < 2)
            yield return null;

        GameObject canvas1 = null;
        GameObject canvas2 = null;

        while (canvas1 == null || canvas2 == null)
        {
            canvas1 = GameObject.FindGameObjectWithTag("CanvasP1");
            canvas2 = GameObject.FindGameObjectWithTag("CanvasP2");
            yield return null;
        }

        painelDialogo1 = canvas1.transform.Find("PainelDialogo1").gameObject;
        textoDialogo1 = painelDialogo1.transform.Find("TMP_Text").GetComponent<TMP_Text>();
        botaoDialogo1 = canvas1.transform.Find("BotaoDialogo1").GetComponent<Button>();

        painelDialogo2 = canvas2.transform.Find("PainelDialogo2").gameObject;
        textoDialogo2 = painelDialogo2.transform.Find("TMP_Text").GetComponent<TMP_Text>();
        botaoDialogo2 = canvas2.transform.Find("BotaoDialogo2").GetComponent<Button>();

        Debug.Log("Canvas e elementos encontrados e configurados");

        painelDialogo1?.SetActive(false);
        painelDialogo2?.SetActive(false);
    }

    void OnDestroy()
    {
        botaoDialogo1.onClick.RemoveAllListeners();
        botaoDialogo2.onClick.RemoveAllListeners();
    }

    private void AoClicarNoBotao(string nomeJogador)
    {
        Debug.Log($"Botão clicado por {nomeJogador}");

        int actorID = PhotonNetwork.LocalPlayer.ActorNumber;

        if (!dialogoGlobal)
        {
            if (!falando)
            {
                Debug.Log("Iniciando diálogo local");
                IniciarDialogoLocal(actorID);
            }
            else
            {
                Debug.Log("Avançando diálogo local");
                AvancarDialogoLocal(actorID);
            }
        }
        else
        {
            if (!falando)
            {
                Debug.Log("Iniciando diálogo global");
                photonView.RPC("IniciarDialogoGlobal", RpcTarget.AllBuffered, actorID);
            }
            else if (photonViewDoIniciador != null && photonViewDoIniciador.IsMine)
            {
                Debug.Log("Avançando diálogo global");
                photonView.RPC("AvancarDialogoGlobal", RpcTarget.AllBuffered);
            }
        }
    }

    private void IniciarDialogoLocal(int actorID)
    {
        linhaAtual = 0;
        falando = true;

        Debug.Log($"Diálogo local iniciado pelo jogador {actorID}");

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
            Debug.Log($"Avançando para linha {linhaAtual}");

            if (actorID == 1)
                textoDialogo1.text = linhasDialogo[linhaAtual];
            else if (actorID == 2)
                textoDialogo2.text = linhasDialogo[linhaAtual];
        }
        else
        {
            Debug.Log("Finalizando diálogo local");
            FinalizarDialogoLocal();
        }
    }

    private void FinalizarDialogoLocal()
    {
        painelDialogo1.SetActive(false);
        painelDialogo2.SetActive(false);
        falando = false;
        linhaAtual = 0;

        Debug.Log("Diálogo local finalizado");
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

        Debug.Log($"Diálogo global iniciado por jogador {actorID}");
    }

    [PunRPC]
    private void AvancarDialogoGlobal()
    {
        linhaAtual++;
        if (linhaAtual < linhasDialogo.Length)
        {
            textoDialogo1.text = linhasDialogo[linhaAtual];
            textoDialogo2.text = linhasDialogo[linhaAtual];
            Debug.Log($"Avançando diálogo global para linha {linhaAtual}");
        }
        else
        {
            painelDialogo1.SetActive(false);
            painelDialogo2.SetActive(false);
            falando = false;
            linhaAtual = 0;
            Debug.Log("Diálogo global finalizado");
        }
    }

    public void MostrarBotao(GameObject jogador)
    {
        jogadorPerto = true;
        jogadorAtual = jogador;

        PhotonView view = jogador.GetComponent<PhotonView>();
        int actorID = view.Owner.ActorNumber;

        Debug.Log($"Jogador {actorID} se aproximou do NPC");

        if (actorID == 1)
        {
            botaoDialogo1.onClick.RemoveAllListeners();
            botaoDialogo1.onClick.AddListener(() => AoClicarNoBotao("PI_MC_1"));
            botaoDialogo1.gameObject.SetActive(true);
        }
        else if (actorID == 2)
        {
            botaoDialogo2.onClick.RemoveAllListeners();
            botaoDialogo2.onClick.AddListener(() => AoClicarNoBotao("PI_MC_2"));
            botaoDialogo2.gameObject.SetActive(true);
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

        Debug.Log("Ocultando todos os elementos de diálogo");
    }
}
