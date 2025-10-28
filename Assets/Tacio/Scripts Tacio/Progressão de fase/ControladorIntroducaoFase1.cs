using UnityEngine;

public class ControladorIntroducaoFase1 : MonoBehaviour
{
    [Header("Bloqueio antes das plataformas")]
    public GameObject bloqueioAcesso;

    [Header("Plataformas cooperativas")]
    public GameObject[] plataformas;

    public void HabilitarPlataformas()
    {
        if (bloqueioAcesso != null)
            bloqueioAcesso.SetActive(false);

        foreach (var plat in plataformas)
        {
            if (plat != null)
                plat.SetActive(true);
        }

        Debug.Log("[ControladorIntroducaoFase1] Plataformas habilitadas!");
    }
}
