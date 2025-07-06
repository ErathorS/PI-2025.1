using UnityEngine;
using UnityEngine.EventSystems;

public class DropArea : MonoBehaviour, IDropHandler
{
    public GameObject botaoFinal;
    public string[] ingredientesNecessarios;
    private void Start()
    {
        if (botaoFinal != null)
            botaoFinal.SetActive(false); 
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        if (dropped != null)
        {
            dropped.transform.SetParent(transform);
            VerificarIngredientes();
        }
    }

    void VerificarIngredientes()
    {
        int encontrados = 0;

        foreach (string nome in ingredientesNecessarios)
        {
            bool achou = false;

            foreach (Transform filho in transform)
            {
                if (filho.name == nome)
                {
                    achou = true;
                    break;
                }
            }

            if (achou)
            {
                encontrados++;
            }
        }

        if (encontrados == ingredientesNecessarios.Length)
        {
            if (botaoFinal != null && !botaoFinal.activeSelf)
            {
                botaoFinal.SetActive(true);
                Debug.Log(" Todos os ingredientes foram adicionados");
            }
        }
    }
}
