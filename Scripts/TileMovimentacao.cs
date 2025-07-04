using UnityEngine;
using System;
using System.Collections;

public class TileMovimentacao : MonoBehaviour
{

    public ObjectsMoviment objectsMoviment;
    public bool isMouseTocando;
    public Texture2D handCursor; // arraste aqui a imagem da mãozinha

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isMouseTocando && objectsMoviment != null)
        {
            objectsMoviment.addMovimentoPlayer(GetComponent<SpriteRenderer>().sprite);
            StartCoroutine(rotinaClique());
        }
    }

    public void inicializar(ObjectsMoviment objectsMoviment)
    {
        this.objectsMoviment = objectsMoviment;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mouse"))
        {
            isMouseTocando = true;
            Cursor.SetCursor(handCursor, new Vector2(9, -1), CursorMode.Auto);
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Mouse"))
        {
            isMouseTocando = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    
        IEnumerator rotinaClique()
    {
        GetComponent<SpriteRenderer>().color = Color.black;
        yield return new WaitForSeconds(0.05f);
        if (isMouseTocando) GetComponent<SpriteRenderer>().color = Color.gray;
        else GetComponent<SpriteRenderer>().color = Color.white;
    }



}
