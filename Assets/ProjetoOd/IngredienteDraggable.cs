using UnityEngine;
using UnityEngine.EventSystems;

public class IngredienteDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 posicaoOriginal;

    public Transform areaAlvo; // Área onde o ingrediente deve ser solto
    public string nomeIngrediente; // Ex: "camarao", "vatapa", etc.
    public MontagemController controladorMontagem; // Arraste o painel com o controlador

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        posicaoOriginal = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        float distancia = Vector3.Distance(transform.position, areaAlvo.position);
        if (distancia < 100f)
        {
            transform.position = areaAlvo.position;
            controladorMontagem.MarcarIngrediente(nomeIngrediente);
        }
        else
        {
            transform.position = posicaoOriginal;
        }
    }
} 