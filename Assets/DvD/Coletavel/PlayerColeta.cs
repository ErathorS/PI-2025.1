using UnityEngine;

public class PlayerColeta : MonoBehaviour
{
  public int itensColetados = 0; // contador de itens

    private void OnTriggerEnter(Collider other)
    {
        // verifica se o objeto tem a tag "Item"
        if (other.CompareTag("Item"))
        {
            itensColetados++; // soma +1
            Destroy(other.gameObject); // destrói o item coletado
            Debug.Log("Itens coletados: " + itensColetados);
        }
    }
}
