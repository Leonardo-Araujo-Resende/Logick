using Unity.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemySO dados;
    private int movimentIndex;
    private GridManager gridManager;
    //Movimentacao
    private bool isMovimentando;
    private Vector2 posFinal;
    private float velocidade;

    public void inicializar(EnemySO dados, GridManager gridManager, float velocidade)
    {
        this.velocidade = velocidade;
        this.dados = dados;
        movimentIndex = 0;
        this.gridManager = gridManager;
        dados.reiniciarPosicao();
        var (podeMover, novaPosicao) = gridManager.getPosition(dados.posicaoInicialX, dados.posicaoInicialY);
        transform.position = novaPosicao;
        rotacionarDependerMovimento(dados.movimentPattern[movimentIndex]);
    }

    void Update()
    {
        movimentacaoLinear();
    }
    public void movimentar()
    {
        bool podeMover;
        Vector3 novaPosicao;
        rotacionarDependerMovimento(dados.movimentPattern[movimentIndex]);
        switch (dados.movimentPattern[movimentIndex])
        {
            case "C":
                (podeMover, novaPosicao) = gridManager.getPosition(dados.posicaoX, dados.posicaoY - 1);
                if (podeMover)
                {
                    iniciarMovimentacao(novaPosicao);
                    dados.posicaoY--;
                }
                break;

            case "D":
                (podeMover, novaPosicao) = gridManager.getPosition(dados.posicaoX + 1, dados.posicaoY);
                if (podeMover)
                {
                    iniciarMovimentacao(novaPosicao);
                    dados.posicaoX++;
                }
                break;

            case "B":
                (podeMover, novaPosicao) = gridManager.getPosition(dados.posicaoX, dados.posicaoY + 1);
                if (podeMover)
                {
                    iniciarMovimentacao(novaPosicao);
                    dados.posicaoY++;
                }
                break;

            case "E":
                (podeMover, novaPosicao) = gridManager.getPosition(dados.posicaoX - 1, dados.posicaoY);
                if (podeMover)
                {
                    iniciarMovimentacao(novaPosicao);
                    dados.posicaoX--;
                }
                break;
        }

        updateMovimentIndex();
        dados.movimentarTile();
    }



    void updateMovimentIndex()
    {
        movimentIndex = (movimentIndex + 1) % dados.movimentPattern.Length;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gridManager.colidiuInimigo();
        }
    }

    public void reiniciar()
    {
        isMovimentando = false;
        movimentIndex = 0;
        dados.reiniciarPosicao();
        var (podeMover, novaPosicao) = gridManager.getPosition(dados.posicaoInicialX, dados.posicaoInicialY);
        transform.position = novaPosicao;
        dados.resetarCores();
    }

    private void rotacionarDependerMovimento(string movimento)
    {
        switch (movimento)
        {
            case "C":
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case "B":
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case "E":
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case "D":
                transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
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

            if (Vector2.Distance(transform.position, posFinal) < 0.01f)
            {
                isMovimentando = false;
                transform.position = posFinal;
            }
        }

    }

    private int menorOuMaior(float a, float b)
    {
        if (a < b) return 1;
        if (a > b) return -1;
        return 0;
    }

    public void colidiuPlayer()
    {
        isMovimentando = false;
    }
}
