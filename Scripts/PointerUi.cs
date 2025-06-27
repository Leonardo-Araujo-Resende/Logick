using UnityEngine;
using UnityEngine.EventSystems;

public class PointerUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D handCursor; // arraste aqui a imagem da mãozinha

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(handCursor, new Vector2(9, -1), CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}