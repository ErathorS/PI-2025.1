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
        if (painelDialogo != null)
            painelDialogo.SetActive(false);

        if (botaoDialogo != null)
            botaoDialogo.gameObject.SetActive(false);

        if (botaoInteracao != null)
            botaoInteracao.gameObject.SetActive(false);
    }

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
