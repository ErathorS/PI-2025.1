using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Dialogo : MonoBehaviour
{
    public GameObject botaoInteragir;
    public GameObject painelDialogo;
    public TMP_Text textoDialogo;

    private string[] linhasDialogo = {
        "Olá, viajante!",
        "Você está pronto para uma nova aventura?",
        "Boa sorte no seu caminho!"
        
    };

    private int linhaAtual = 0;
    private bool jogadorPerto = false;
    private bool falando = false;

    private InputAction toque;

    private float tempoUltimoDialogo = -999f; // tempo do último início de diálogo
    private float intervaloDialogo = 2f;      // intervalo mínimo entre diálogos (em segundos)

    void Awake()
    {
       
    }

    void OnEnable() => toque.Enable();
    void OnDisable() => toque.Disable();

    void Start()
    {
        botaoInteragir.SetActive(false);
        painelDialogo.SetActive(false);
        botaoInteragir.GetComponent<Button>().onClick.AddListener(IniciarDialogo);
    }

    public void IniciarDialogo()
    {
        Debug.Log("A desgraça n aparece");
        if (Time.time - tempoUltimoDialogo < intervaloDialogo)
            return; // ainda dentro do tempo de espera

        tempoUltimoDialogo = Time.time;

        botaoInteragir.SetActive(false);
        painelDialogo.SetActive(true);
        linhaAtual = 0;
        textoDialogo.text = linhasDialogo[linhaAtual];
        falando = true;
    }

    public void OnToque(InputAction.CallbackContext context)
    {
        if (!context.performed || !falando) return;

        linhaAtual++;
        if (linhaAtual < linhasDialogo.Length)
        {
            textoDialogo.text = linhasDialogo[linhaAtual];
        }
        else
        {
            painelDialogo.SetActive(false);
            falando = false;
            if (jogadorPerto)
                botaoInteragir.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Npc"))
        {
            jogadorPerto = true;
            if (!falando)
                botaoInteragir.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Npc"))
        {
            jogadorPerto = false;
            botaoInteragir.SetActive(false);
            painelDialogo.SetActive(false);
            falando = false;
            linhaAtual = 0;
        }
    }
}
