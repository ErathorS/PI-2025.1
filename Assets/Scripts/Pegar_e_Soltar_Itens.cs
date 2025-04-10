using UnityEngine;

public class Pegar_e_Soltar_Itens : MonoBehaviour
{
    [SerializeField] Transform PlayerCameraTransform;
    [SerializeField] LayerMask pickableLayerMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetKey(KeyCode.E)){
        //     float pickupDistance = 2f;
        //     Physics.Raycast(PlayerCameraTransform.position, PlayerCameraTransform.forward, out RaycastHit raycasthit, pickupDistance);
        // }
    }
}
