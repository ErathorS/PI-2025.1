using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnergyBarController : MonoBehaviour
{
    [Header("Referência ao Slider da Energia")]
    public Slider barraDeEnergia;

    [Header("Configuração da Energia")]
    public int totalNpcs = 5;
    public float incrementoPorNpc = 1f;
    public float velocidadeExtra = 0.5f;

    private float energiaAtual = 0f;
    private int npcsFalados = 0;
    private bool energiaCompleta = false;

    void Start()
    {
        if (barraDeEnergia != null)
        {
            barraDeEnergia.minValue = 0f;
            barraDeEnergia.maxValue = totalNpcs * incrementoPorNpc;
            barraDeEnergia.value = energiaAtual;
        }
    }

    void Update()
    {
        if (energiaCompleta && energiaAtual < barraDeEnergia.maxValue)
        {
            energiaAtual += velocidadeExtra * Time.deltaTime;
            energiaAtual = Mathf.Clamp(energiaAtual, 0f, barraDeEnergia.maxValue);
            AtualizarBarra();

            // Verifica se está cheia após recarregar
            if (energiaAtual >= barraDeEnergia.maxValue)
            {
                IrParaCenaFinal();
            }
        }
    }
        private void IrParaCenaFinal()
        {
                    SceneManager.LoadScene("CenaFinal"); 
        }

    public void FalouComNpc()
    {
        if (energiaCompleta) return;

        npcsFalados++;
        energiaAtual += incrementoPorNpc;
        energiaAtual = Mathf.Clamp(energiaAtual, 0f, barraDeEnergia.maxValue);
        AtualizarBarra();

        if (npcsFalados >= totalNpcs)
        {
            energiaCompleta = true;
        }
    }

    private void AtualizarBarra()
    {
        if (barraDeEnergia != null)
        {
            barraDeEnergia.value = energiaAtual;
        }
    }
}
