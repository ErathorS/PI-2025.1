using UnityEngine;
using UnityEngine.SceneManagement;

public class HQSceneManager : MonoBehaviour
{
    [Header("Referência para o Controler_HQ")]
    public Controler_HQ controladorHQ; 

    [Header("Configuração de Cena")]
    public string nomeProximaCena; 
    public float delayTransicao = 1f; 
    private bool hqConcluida = false;

    void Update()
    {
        // Verifica se a HQ acabou (quadroAtual >= total de quadros) E se já não foi marcada como concluída
        if (!hqConcluida && controladorHQ != null)
        {
        
            var quadrosField = typeof(Controler_HQ).GetField("quadrosHQ", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var quadroAtualField = typeof(Controler_HQ).GetField("quadroAtual", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (quadrosField != null && quadroAtualField != null)
            {
                GameObject[] quadros = (GameObject[])quadrosField.GetValue(controladorHQ);
                int quadroAtual = (int)quadroAtualField.GetValue(controladorHQ);

                if (quadroAtual >= quadros.Length)
                {
                    hqConcluida = true;
                    Invoke("CarregarCena", delayTransicao);
                }
            }
        }
    }

    void CarregarCena()
    {
        if (!string.IsNullOrEmpty(nomeProximaCena))
        {
            SceneManager.LoadScene(nomeProximaCena);
        }
        else
        {
            Debug.LogError("Nome da próxima cena não definido!");
        }
    }
}