using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Dialogo : MonoBehaviour
{
    [Header("Referências")]
    public GameObject botaoInteragir; // Botão "Falar" (opcional)
    public GameObject painelDialogo;  // Painel de diálogo
    public TMP_Text textoDialogo;     // Texto do diálogo
     public IndicadorNpc indicadorNPC;

    [Header("Configuração do Diálogo")]
    [SerializeField] private string[] linhasDialogo = {
        "",
        "Olá, viajante!",
        "Você está pronto para uma nova aventura?",
        "Boa sorte no seu caminho!"
    };

    private int linhaAtual = 0;
    private bool jogadorPerto = false;
    private bool falando = false;

    void Start()
    {
        botaoInteragir.SetActive(false);
        painelDialogo.SetActive(false);
    }

    // Chamado pelo Input System (configurado no Inspector)
    public void OnToque(InputAction.CallbackContext context)
    {
        // Só avança se:
        // 1. O toque foi concluído (performed)
        // 2. O jogador está perto do NPC
        // 3. O diálogo já começou
        if (!context.performed || !jogadorPerto) 
            return;

        if (!falando)
        {
            IniciarDialogo(); // Primeiro toque: inicia o diálogo
        }
        else
        {
            AvancarDialogo(); // Toques seguintes: avança o texto
        }
    }

    private void IniciarDialogo()
    {
       if (falando) return;

              botaoInteragir.SetActive(false);
              painelDialogo.SetActive(true);
              linhaAtual = 0;
            textoDialogo.text = linhasDialogo[linhaAtual];
             falando = true;

        if (indicadorNPC != null)
         indicadorNPC.MarcarComoConversado();
    }

    private void AvancarDialogo()
    {
        linhaAtual++;
        
        if (linhaAtual < linhasDialogo.Length)
        {
            textoDialogo.text = linhasDialogo[linhaAtual];
        }
        else
        {
            FinalizarDialogo();
        }
    }

    private void FinalizarDialogo()
    {
        painelDialogo.SetActive(false);
        falando = false;
        
        // Reativa o botão "Falar" se o jogador ainda estiver perto
        if (jogadorPerto && botaoInteragir != null)
            botaoInteragir.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Npc"))
        {
            jogadorPerto = true;
            
            // Mostra o botão "Falar" (opcional)
            if (botaoInteragir != null && !falando)
                botaoInteragir.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Npc"))
        {
            jogadorPerto = false;
            
            // Esconde o botão e fecha o diálogo se sair do NPC
            if (botaoInteragir != null)
                botaoInteragir.SetActive(false);
            
            if (falando)
                FinalizarDialogo();
        }
    }
}