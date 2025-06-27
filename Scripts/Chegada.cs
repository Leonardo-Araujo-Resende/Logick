using UnityEngine;
using System;
public class Chegada : MonoBehaviour
{
    private Action chegada;
    private int posicaoX, posicaoY;
    private GridManager gridManager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void inicializar(Action chegada, Vector3 posisaoInicial, int x, int y, GridManager gridManager)
    {
        this.chegada = chegada;
        transform.position = posisaoInicial;
        posicaoX = x;
        posicaoY = y;
        this.gridManager = gridManager;
        GameManager.AdicionarTagDestrutivel(this.gameObject);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            chegada.Invoke();
            Destroy(other);
        }
    }

}
