using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int posicaoX, posicaoY;
    private GridManager gridManager;
    public string[] movimentos;
    private int indexMovimento = 0;
    //Movimentacao
    private bool isMovimentando;
    private Vector2 posFinal;
    private float velocidade;

    private void Update()
    {
        movimentacaoLinear();
    }

    public void movimentar()
    {
        bool podeMover;
        Vector2 novaPosicao;
        switch (movimentos[indexMovimento])
        {
            case "esquerda" + "_0":
                (podeMover, novaPosicao) = gridManager.getPosition(posicaoX - 1, posicaoY);
                if (podeMover)
                {
                    iniciarMovimentacao(novaPosicao);
                    posicaoX--;
                }
                else StartCoroutine(movimentacaoImpossivel());
                break;

            case "direita" + "_0":
                (podeMover, novaPosicao) = gridManager.getPosition(posicaoX + 1, posicaoY);
                if (podeMover)
                {
                    iniciarMovimentacao(novaPosicao);
                    posicaoX++;
                }
                else StartCoroutine(movimentacaoImpossivel());
                break;

            case "cima" + "_0":
                (podeMover, novaPosicao) = gridManager.getPosition(posicaoX, posicaoY - 1);
                if (podeMover)
                {
                    iniciarMovimentacao(novaPosicao);
                    posicaoY--;
                }
                else StartCoroutine(movimentacaoImpossivel());

                break;

            case "baixo" + "_0":
                (podeMover, novaPosicao) = gridManager.getPosition(posicaoX, posicaoY + 1);
                if (podeMover)
                {
                    iniciarMovimentacao(novaPosicao);
                    posicaoY++;
                }
                else StartCoroutine(movimentacaoImpossivel());
                break;

            default:
                Debug.Log(movimentos[indexMovimento] + " Movimento inválido");
                break;
        }
        atualizaIndexMovimento();
    }

    private void atualizaIndexMovimento()
    {
        indexMovimento = indexMovimento + 1;
    }
    public void inicializar(Vector3 posisaoInicial, int x, int y, GridManager gridManager, float velocidade)
    {
        this.velocidade = velocidade;
        transform.position = posisaoInicial;
        posicaoX = x;
        posicaoY = y;
        this.gridManager = gridManager;
    }

    public void setMovimentos(string[] movimentos)
    {
        this.movimentos = movimentos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bolinha"))
        {
            gridManager.playerColetouBolinha();
            Destroy(other.gameObject);
        }
    }

    private void iniciarMovimentacao(Vector2 posFinal)
    {
        if (isMovimentando) transform.position = posFinal;

        this.posFinal = posFinal;
        isMovimentando = true;
    }

    private void movimentacaoLinear()
    {
        if (isMovimentando)
        {
            float x = transform.position.x + velocidade * Time.deltaTime * menorOuMaior(transform.position.x, posFinal.x);
            float y = transform.position.y + velocidade * Time.deltaTime * menorOuMaior(transform.position.y, posFinal.y);
            transform.position = new Vector3(x, y, transform.position.z);
        }
        if (Vector2.Distance(transform.position, posFinal) < 0.01f)
        {
            isMovimentando = false;
            transform.position = posFinal;
        }
    }

    private int menorOuMaior(float a, float b)
    {
        if (a < b) return 1;
        if (a > b) return -1;
        return 0;
    }

    public void colidiuInimigo()
    {
        isMovimentando = false;
    }

    private IEnumerator movimentacaoImpossivel()
    {
        Vector2 posOriginal = transform.position;
        for (int i = 0; i < 6; i++)
        {
            if (i % 2 == 0) transform.position = new Vector2(posOriginal.x + 0.05f, posOriginal.y);
            else transform.position = new Vector2(posOriginal.x - 0.05f, posOriginal.y);

            yield return new WaitForSeconds(0.05f);
        }

        transform.position = posOriginal;
    }

    public int getMovimentoAtual()
    {
        return indexMovimento;
    }



}
