using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIReferences : MonoBehaviour
{
    [Header("Referências de Diálogo")]
    public GameObject painelDialogo;
    public TMP_Text textoDialogo;
    public Button botaoDialogo;
    public Button botaoInteracao;

    // 🔹 Método auxiliar que esconde o painel de diálogo no início
    void Start()
    {
        if (painelDialogo != null)
            painelDialogo.SetActive(false);
    }
}
