using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;
using UnityEngine.SceneManagement;

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

    [Header("Montagem de Acarajé (somente baiana)")]
    public bool ativaMontagemAcaraje = false;
    public GameObject painelMontagemP1;
    public GameObject painelMontagemP2;

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
        int actorID = PhotonNetwork.LocalPlayer.ActorNumber;

        if (!dialogoGlobal)
        {
            if (!falando)
                IniciarDialogoLocal(actorID);
            else
                AvancarDialogoLocal(actorID);
        }
        else
        {
            if (!falando)
                photonView.RPC("IniciarDialogoGlobal", RpcTarget.AllBuffered, actorID);
            else if (photonViewDoIniciador != null && photonViewDoIniciador.IsMine)
                photonView.RPC("AvancarDialogoGlobal", RpcTarget.AllBuffered);
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

           if (CompareTag("Baiana"))
         {
               SceneManager.LoadScene("MecanicaAcaraje");
         }
        
        FindObjectOfType<EnergyBarController>()?.FalouComNpc();
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
    }
}
