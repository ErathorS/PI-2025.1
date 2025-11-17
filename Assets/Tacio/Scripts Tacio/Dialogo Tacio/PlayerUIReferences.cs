using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIReferences : MonoBehaviour
{
    [Header("Referências de Diálogo")]
    public GameObject painelDialogo;
    public TMP_Text textoDialogo;
    public Button botaoDialogo;

    [Header("Interação")]
    public Button botaoInteracao;

    void Start()
    {
        // 🛑 100% garante que nada aparece quando o jogo inicia
        if (painelDialogo != null)
            painelDialogo.SetActive(false);

        if (botaoDialogo != null)
            botaoDialogo.gameObject.SetActive(false);

        if (botaoInteracao != null)
            botaoInteracao.gameObject.SetActive(false);
    }

    // 🔹 Método útil para esconder rapidamente qualquer UI
    public void EsconderTudo()
    {
        if (painelDialogo != null)
            painelDialogo.SetActive(false);

        if (botaoDialogo != null)
            botaoDialogo.gameObject.SetActive(false);

        if (botaoInteracao != null)
            botaoInteracao.gameObject.SetActive(false);
    }
}
