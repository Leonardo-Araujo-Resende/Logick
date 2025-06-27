using NUnit.Framework.Constraints;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    private const string P = "parede";
    private const string C = "chegada";
    private const string B = "bolinha";
    private const string N = "nada";
    private GameManager gameManager;
    [Header("Grid")]
    [SerializeField] private float tempoEntreMovimentacao;
    [SerializeField] private int larguraX;
    [SerializeField] private int alturaY;
    [SerializeField] private string[] matrizString;
    [SerializeField] private GameObject tile;
    [SerializeField] private Color corClara, corEscura, corParede;

    private Transform cameraTransform;
    [Header("Inimigo")]
    [SerializeField] private EnemySO[] inimigos;
    [SerializeField] private GameObject inimigoGO;
    private GameObject[] instanciaInimigos;
    [SerializeField] private float velocidadeInimigo;
    [Header("Player")]
    [SerializeField] private GameObject jogador;
    [SerializeField] private int posicaoInicialPlayerX, posicaoInicialPlayerY;
    private GameObject instanciaJogador;
    [SerializeField] private float velocidadePlayer;

    [Header("Chegada")]
    [SerializeField] private GameObject chegada;
    [SerializeField] private int posicaoChegadaX, posicaoChegadaY;
    private GameObject instanciaChegada;

    private GameObject[,] matrizTiles;
    private string[,] posicaoObjetos;

    [Header("Camera")]
    [SerializeField] private float offSetX;
    [SerializeField] private float offSetY;
    [Header("Bolinha")]
    [SerializeField] private GameObject bolinha;
    private List<GameObject> bolinhasInstancia;
    private int qntBolinhasTotal;
    private int qntBolinhasPegas;
    //Variaveis de controle
    private bool isJogoComecou;


    void Start()
    {
        bolinhasInstancia = new List<GameObject>();
        qntBolinhasPegas = 0;
        GameManager.AdicionarTagDestrutivel(this.gameObject);
        cameraTransform = Camera.main.transform;
        matrizTiles = new GameObject[larguraX, alturaY];
        preencherMatrizApartirString();
        gerarGrid();
        gerarBolinhas();
        atualizarPosicaoCamera();
        inicializarInimigos();
        inicializarJogador();
        inicializaChegada();
    }
    void gerarGrid()
    {
        for (int x = 0; x < larguraX; x++)
        {
            for (int y = 0; y < alturaY; y++)
            {
                GameObject tileSpawned = Instantiate(tile, new Vector3(x, alturaY - 1 - y), quaternion.identity);
                tileSpawned.name = $"Tile {x} {y}";

                SpriteRenderer spriteRendererTile = tileSpawned.GetComponent<SpriteRenderer>();
                spriteRendererTile.color = (x + y) % 2 == 0 ? corClara : corEscura;

                matrizTiles[x, y] = tileSpawned;


                if (posicaoObjetos[x, y] == "parede")
                {
                    spriteRendererTile.color = corParede;
                }
            }
        }
    }
    void gerarBolinhas()
    {
        qntBolinhasTotal = 0;
        for (int x = 0; x < larguraX; x++)
        {
            for (int y = 0; y < alturaY; y++)
            {
                if (posicaoObjetos[x, y] == "bolinha")
                {
                    GameObject obj = Instantiate(bolinha, new Vector3(x, alturaY - 1 - y), quaternion.identity);
                    bolinhasInstancia.Add(obj);
                    qntBolinhasTotal++;

                }
            }
        }
    }

    void atualizarPosicaoCamera()
    {
        cameraTransform.position = new Vector3((larguraX / 2f - 0.5f + ((19 - larguraX) / 2)) + offSetX, (alturaY / 2f - 0.5f) + offSetY, -10);
    }

    public (bool, Vector2) getPosition(int x, int y)
    {
        if (dentroLimitesMatriz(x, y) && posicaoObjetos[x, y] != "parede")
        {
            return (true, matrizTiles[x, y].transform.position);
        }
        else
        {
            return (false, Vector2.zero);
        }
    }

    bool dentroLimitesMatriz(int x, int y)
    {
        return x >= 0 && x < larguraX && y >= 0 && y < alturaY;
    }

    void inicializarInimigos()
    {
        instanciaInimigos = new GameObject[inimigos.Length];
        int index = 0;
        foreach (var inimigo in inimigos)
        {
            GameObject objeto = Instantiate(inimigoGO, new Vector3(0, 0, 0), quaternion.identity);

            SpriteRenderer[] spriteRenderers = objeto.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer item in spriteRenderers) item.color = inimigo.corEscolhida;

            objeto.GetComponent<Enemy>().inicializar(inimigo, this, velocidadeInimigo);
            instanciaInimigos[index] = objeto;
            index++;
        }
    }

    void inicializarJogador()
    {
        GameObject objeto = Instantiate(jogador, new Vector3(0, 0, 0), quaternion.identity);
        objeto.GetComponent<Player>().inicializar(matrizTiles[posicaoInicialPlayerX, posicaoInicialPlayerY].transform.position, posicaoInicialPlayerX, posicaoInicialPlayerY, this, velocidadePlayer);
        instanciaJogador = objeto;
    }

    void inicializaChegada()
    {
        GameObject instanciaChegada = Instantiate(chegada, new Vector3(0, 0, 0), quaternion.identity);
        instanciaChegada.GetComponent<Chegada>().inicializar(finalizarNivelChegada, matrizTiles[posicaoChegadaX, posicaoChegadaY].transform.position, posicaoChegadaX, posicaoChegadaY, this);
    }

    public (EnemySO[], GameObject) getInimigos()
    {
        return (inimigos, inimigoGO);
    }

    public void iniciarMovimentacao()
    {
        isJogoComecou = true;
        gameManager.incrementaTentativaNivel();
        StartCoroutine(rotinaMovimentacao());
    }

    IEnumerator rotinaMovimentacao()
    {
        int qntMovimentos = instanciaJogador.GetComponent<Player>().movimentos.Length;
        GameObject[] movimentosJogador = GetComponent<ObjectsMoviment>().getTilesMovimentoJogador();
        for (int i = 0; i < qntMovimentos; i++)
        {
            yield return new WaitForSeconds(tempoEntreMovimentacao);
            if (!isJogoComecou) break;
            movimentosJogador[i].GetComponent<SpriteRenderer>().color = Color.gray;
            movimentarObjetos();
            if (!isJogoComecou) break;
        }
    }

    void movimentarObjetos()
    {
        instanciaJogador.GetComponent<Player>().movimentar();
        foreach (var inimigo in instanciaInimigos)
        {
            inimigo.GetComponent<Enemy>().movimentar();
        }
    }

    public GameObject getInstanciaJogador()
    {
        return instanciaJogador;
    }

    void preencherMatrizApartirString()
    {
        posicaoObjetos = new string[larguraX, alturaY];
        for (int x = 0; x < larguraX; x++)
        {
            for (int y = 0; y < alturaY; y++)
            {
                posicaoObjetos[x, y] = converteCharParaStringItensTabuleiro(matrizString[y][x]);
                if (x == posicaoChegadaX && y == posicaoChegadaY) posicaoObjetos[x, y] = "chegada";
            }
        }

    }

    private string converteCharParaStringItensTabuleiro(char item)
    {
        switch (item)
        {
            case 'P': return "parede";
            case 'C': return "chegada";
            case 'B': return "bolinha";
            case 'N': return "nada";
        }
        return "nada";
    }

    public float getAltura()
    {
        return (alturaY / 2f - 0.5f) + offSetY;
    }

    public float getLargura()
    {
        return (larguraX / 2f - 0.5f + ((19 - larguraX) / 2)) + offSetX;
    }

    public void reiniciarNivel()
    {
        isJogoComecou = false;
        Destroy(instanciaJogador);
        inicializarJogador();

        foreach (GameObject inimigo in instanciaInimigos) { inimigo.GetComponent<Enemy>().reiniciar(); }
        excluirInstanciaBolinhas();

        qntBolinhasPegas = 0;
        gerarBolinhas();
    }

    public void finalizarNivelChegada()
    {
        if (qntBolinhasPegas == qntBolinhasTotal)
        {
            passouNivel();
        }
        else
        {
            colidiuInimigo();
        }
    }
    public void colidiuInimigo()
    {
        isJogoComecou = false;
        jogador.GetComponent<Player>().colidiuInimigo();
        foreach (GameObject inimigo in instanciaInimigos) { inimigo.GetComponent<Enemy>().colidiuPlayer(); }
    }

    public void passouNivel()
    {
        isJogoComecou = false;
        gameManager.finalizarNivel(instanciaJogador.GetComponent<Player>().getMovimentoAtual());
    }

    public void setGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void playerColetouBolinha()
    {
        qntBolinhasPegas++;
    }

    public bool getSeJogoComecou()
    {
        return isJogoComecou;
    }
    private void excluirInstanciaBolinhas()
    {
        foreach (GameObject obj in bolinhasInstancia)
        {
            Destroy(obj);
        }
        bolinhasInstancia.Clear();
    }
}
