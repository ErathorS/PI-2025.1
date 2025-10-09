using UnityEngine;
using System.Collections;

public class GerenciadorDeJogadores : MonoBehaviour
{
    [Header("Referência ao Painel de Espera")]
    public GameObject painelDeEspera;

    private GameObject player1;
    private GameObject player2;

    void Start()
    {
        StartCoroutine(VerificarJogadoresNaCena());
    }

    private IEnumerator VerificarJogadoresNaCena()
    {
        while (true)
        {
            player1 = GameObject.Find("PI_MC_1");
            player2 = GameObject.Find("PI_MC_2");

            if (player1 != null && player2 == null)
            {
                // Apenas o Jogador 1 entrou
                if (painelDeEspera != null)
                    painelDeEspera.SetActive(true);

                Time.timeScale = 0f;
            }
            else if (player1 != null && player2 != null)
            {
                // Ambos jogadores estão presentes
                if (painelDeEspera != null)
                    painelDeEspera.SetActive(false);

                Time.timeScale = 1f;

                // Finaliza o loop, pois os dois já estão na sala
                yield break;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
