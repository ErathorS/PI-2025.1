using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MontagemController : MonoBehaviour
{
    public TMP_Text textoResultado;

    private HashSet<string> ingredientesCorretos = new HashSet<string>();
    private string[] ingredientesNecessarios = { "camarao", "vatapa", "salada" };

    void Start()
    {
        textoResultado.gameObject.SetActive(false);
    }

    public void MarcarIngrediente(string nome)
    {
        if (!ingredientesCorretos.Contains(nome))
        {
            ingredientesCorretos.Add(nome);
            Debug.Log($"Ingrediente colocado: {nome}");

            if (ingredientesCorretos.Count == ingredientesNecessarios.Length)
            {
                AcarajeCompleto();
            }
        }
    }

    private void AcarajeCompleto()
    {
        Debug.Log("Acarajé montado com sucesso!");
        textoResultado.text = "\u2705 Acarajé montado com sucesso!";
        textoResultado.gameObject.SetActive(true);
    }
}
