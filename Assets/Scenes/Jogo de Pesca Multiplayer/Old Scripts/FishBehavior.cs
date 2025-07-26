using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //PontuacaoController.instancia.AdicionarPonto();
            Destroy(gameObject);
        }
    }
}
