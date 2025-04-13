using UnityEngine;

public class Pegar_e_Soltar_Itens : MonoBehaviour
{
    [SerializeField] Transform PlayerCameraTransform;
    [SerializeField] Transform ObjtGrabPointTransform;
    [SerializeField] LayerMask pickupLayerMask;
    ObjetoSeguravel objetoSeguravel;
    public float pickupDistance = 6.5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Verifica se a tecla "E" foi pressionada
        {
            if(objetoSeguravel == null) // Se já estiver segurando um objeto, solta-o
            {
                if (Physics.Raycast(PlayerCameraTransform.position, PlayerCameraTransform.forward, out RaycastHit raycastHit, pickupDistance, pickupLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent(out objetoSeguravel))
                    {
                        objetoSeguravel.pegar(ObjtGrabPointTransform);
                        Debug.Log(raycastHit.transform.name);
                    }
                }
            }
            else // Caso contrário, tenta pegar um novo objeto
            {
                objetoSeguravel.soltar();
                objetoSeguravel = null;
            }
        }
    }
}