using UnityEngine;
using Photon.Pun;

public class ActivateObj : MonoBehaviour, IActivatable
{
    [Header("Configurações de Ativação")]
    public ActivationType activationType = ActivationType.Movement;
    
    [Header("Configurações de Movimento")]
    public float moveSpeed = 2f;
    public Vector3 moveOffset;
    
    [Header("Configurações de Escala")]
    public Vector3 scaleWhenActive;
    
    [Header("Configurações de Rotação")]
    public Vector3 rotationWhenActive;
    
    [Header("Configurações Visuais")]
    public Material activeMaterial;
    private Material originalMaterial;
    private Renderer objectRenderer;
    
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private bool isActive = false;

    public enum ActivationType
    {
        Movement,
        Scale,
        Rotation,
        MaterialChange,
        ToggleActive
    }

    private void Start()
    {
        // Guarda os valores originais
        originalPosition = transform.position;
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
        
        // Configura o renderer para mudança de material
        if (TryGetComponent<Renderer>(out objectRenderer))
        {
            originalMaterial = objectRenderer.material;
        }
    }

    private void Update()
    {
        if (activationType == ActivationType.Movement)
        {
            Vector3 targetPosition = isActive ? 
                originalPosition + moveOffset : 
                originalPosition;
                
            transform.position = Vector3.Lerp(
                transform.position, 
                targetPosition, 
                moveSpeed * Time.deltaTime
            );
        }
    }

    public void Activate(bool state)
    {
        Debug.Log($"Recebido comando de ativação: {state}. Renderer: {objectRenderer != null}");
        isActive = state;
        
        switch (activationType)
        {
            case ActivationType.Movement:
                // O movimento é tratado no Update
                break;
                
            case ActivationType.Scale:
                transform.localScale = isActive ? scaleWhenActive : originalScale;
                break;
                
            case ActivationType.Rotation:
                transform.rotation = isActive ? 
                    Quaternion.Euler(rotationWhenActive) : 
                    originalRotation;
                break;
                
            case ActivationType.MaterialChange:
                if (objectRenderer != null)
                {
                    objectRenderer.material = isActive ? 
                        activeMaterial : 
                        originalMaterial;
                }
                break;
                
            case ActivationType.ToggleActive:
                gameObject.SetActive(isActive);
                break;
        }
        
        // Sincroniza em multiplayer
        if (TryGetComponent<PhotonView>(out var photonView))
        {
            photonView.RPC("SyncActivationState", RpcTarget.Others, isActive);
        }
    }

    [PunRPC]
    private void SyncActivationState(bool state)
    {
        isActive = state;
        // Reaplica o estado para sincronização
        Activate(isActive);
    }
}