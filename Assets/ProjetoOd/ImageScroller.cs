using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialParallax : MonoBehaviour
{
    [Header("Configurações do Parallax")]
    public float scrollSpeedX = 0.5f;
    public float scrollSpeedY = 0.3f;
    public bool parallaxWithMouse = true;
    public float mouseParallaxAmount = 0.1f;
    
    [Header("Referências")]
    public string texturePropertyName = "_MainTex";
    
    private Material material;
    private Vector2 initialOffset;
    private Vector2 currentOffset;

    void Start()
    {
        // Obtém o material (cria uma instância para não afetar outros objetos)
        material = GetComponent<Renderer>().material;
        initialOffset = material.GetTextureOffset(texturePropertyName);
        currentOffset = initialOffset;
    }

    void Update()
    {
        UpdateParallax();
        ApplyTextureOffset();
    }

    void UpdateParallax()
    {
        // Scroll automático
        currentOffset.x += scrollSpeedX * Time.deltaTime;
        currentOffset.y += scrollSpeedY * Time.deltaTime;

        // Efeito parallax com mouse
        if (parallaxWithMouse && Camera.main != null)
        {
            Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Vector2 mouseOffset = new Vector2(
                (mousePos.x - 0.5f) * mouseParallaxAmount,
                (mousePos.y - 0.5f) * mouseParallaxAmount);
            
            currentOffset += mouseOffset;
        }
    }

    void ApplyTextureOffset()
    {
        // Mantém os valores entre 0 e 1 para loop
        currentOffset.x = Mathf.Repeat(currentOffset.x, 1);
        currentOffset.y = Mathf.Repeat(currentOffset.y, 1);
        
        material.SetTextureOffset(texturePropertyName, currentOffset);
    }

    // Métodos públicos para controle
    public void ResetOffset()
    {
        currentOffset = initialOffset;
    }

    public void SetScrollSpeed(float xSpeed, float ySpeed)
    {
        scrollSpeedX = xSpeed;
        scrollSpeedY = ySpeed;
    }
}