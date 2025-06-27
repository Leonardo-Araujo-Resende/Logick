using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;


public class ObjectsMoviment : MonoBehaviour
{
    private GameManager gameManager;
    [Header("Configuracoes")]
    [SerializeField] private float inicioX;
    [SerializeField] private float inicioY;
    [SerializeField] private float distanciaEntreTiles;
    [SerializeField] private float distanciaEntreObjetos;
    private float tileX = 10, tileY = 10;
    [SerializeField] private int qntTilesFileira;
    [SerializeField] private GameObject tile;
    [SerializeField] private GameObject tileJogador;
    [SerializeField] private GameObject tileBotao;

    [Header("Inimigo")]
    private EnemySO[] inimigos;
    private GameObject inimigoGO;

    [Header("Sprites")]
    [SerializeField] private Sprite spriteJogador;
    [SerializeField] private GameObject background;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite spritePlay;
    [SerializeField] private Sprite spriteParar;
    [SerializeField] private Sprite spriteMenu;
    [SerializeField] private Sprite spriteInfo;
    [SerializeField] private bool mesmaFileiraMovimentoAcao = false;

    [Header("Opcoes de movimento")]
    [SerializeField] private string[] opcoesMovimento;
    [Header("Player")]
    [SerializeField] private int qntMovimentosFase;
    private int contTilesPreenchidos = 0;
    private GameObject[] tilesMovimentoJogador;

    private string[] mapea1 = { "C", "B", "E", "D" };
    private string[] mapea2 = { "cima", "baixo", "esquerda", "direita" };
    private GridManager gridManager;
    private bool podePararNivel;


    void Start()
    {
        podePararNivel = false;
        tileX = inicioX; tileY = inicioY;
        gridManager = GetComponent<GridManager>();
        (inimigos, inimigoGO) = gridManager.getInimigos();


        foreach (EnemySO inimigo in inimigos) gerarMovimentosInimigos(inimigo, inimigo.imagem, inimigo.movimentPattern, inimigo.corNomal);
        gerarMovimentoJogador();
        gerarOpcoesMovimento(opcoesMovimento);
        gerarBotoes();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) addMovimentoPlayer(GetSpritePeloNome( "cima"+ "_0"));
        if (Input.GetKeyDown(KeyCode.DownArrow)) addMovimentoPlayer(GetSpritePeloNome( "baixo"+ "_0"));
        if (Input.GetKeyDown(KeyCode.RightArrow)) addMovimentoPlayer(GetSpritePeloNome( "direita"+ "_0"));
        if (Input.GetKeyDown(KeyCode.LeftArrow)) addMovimentoPlayer(GetSpritePeloNome( "esquerda"+ "_0"));

    }

    void gerarMovimentosInimigos(EnemySO inimigo, Sprite imagem, string[] movimentos, Color cor)
    {
        //GameObject objeto = Instantiate(background, new Vector3(tileX, tileY), quaternion.identity);

        GameObject[] tilesMovimentacao = new GameObject[movimentos.Length];
        int arrayIndex = 0;

        GameObject imagemInimigo = Instantiate(tile, new Vector3(gridManager.getLargura() + tileX - distanciaEntreTiles, gridManager.getAltura() + tileY), Quaternion.identity);
        imagemInimigo.GetComponent<SpriteRenderer>().sprite = imagem;
        imagemInimigo.GetComponent<SpriteRenderer>().color = inimigo.corNomal;

        int contTileFileira = 0;
        int contTilesTotal = 0;
        foreach (string movimento in movimentos)
        {
            contTilesTotal++;
            GameObject objeto = Instantiate(tile, new Vector3(gridManager.getLargura() + tileX, gridManager.getAltura() + tileY), Quaternion.identity);
            tilesMovimentacao[arrayIndex] = objeto;
            arrayIndex++;

            objeto.GetComponent<SpriteRenderer>().sprite = GetSpritePeloCodigo(movimento);
            objeto.GetComponent<SpriteRenderer>().color = cor;

            tileX += distanciaEntreTiles;
            contTileFileira++;
            if (contTileFileira == qntTilesFileira && movimentos.Length != contTilesTotal)
            {
                contTileFileira = 0;
                tileX = inicioX;
                tileY -= distanciaEntreTiles;
            }
        }

        inimigo.setMovimentTiles(tilesMovimentacao);
        tileX = inicioX;
        tileY -= distanciaEntreObjetos;
    }

    void gerarMovimentoJogador()
    {
        GameObject imagemJogador = Instantiate(tile, new Vector3(gridManager.getLargura() + tileX - distanciaEntreTiles, gridManager.getAltura() + tileY), Quaternion.identity);
        imagemJogador.GetComponent<SpriteRenderer>().sprite = spriteJogador;
        int contTiles = 0;

        tilesMovimentoJogador = new GameObject[qntMovimentosFase];

        for (int i = 0; i < qntMovimentosFase; i++)
        {

            GameObject objeto = Instantiate(tile, new Vector3(gridManager.getLargura() + tileX, gridManager.getAltura() + tileY), Quaternion.identity);
            tilesMovimentoJogador[i] = objeto;
            contTiles++;
            tileX += distanciaEntreTiles;

            if (contTiles == qntTilesFileira && i != qntMovimentosFase - 1)
            {
                tileX = inicioX;
                tileY -= distanciaEntreTiles;
                contTiles = 0;
            }
        }
        tileX = inicioX;
        tileY -= distanciaEntreObjetos;
    }

    void gerarOpcoesMovimento(string[] opcoes)
    {
        int contTiles = 0;
        foreach (string opcao in opcoes)
        {
            GameObject objeto = Instantiate(tileJogador, new Vector3(gridManager.getLargura() + tileX, gridManager.getAltura() + tileY), Quaternion.identity);
            objeto.GetComponent<TileMovimentacao>().inicializar(this);
            objeto.GetComponent<SpriteRenderer>().sprite = GetSpritePeloNome(opcao + "_0");

            contTiles++;
            tileX += distanciaEntreTiles;

            if (contTiles == qntTilesFileira && opcoes.Length != contTiles)
            {
                tileX = inicioX;
                tileY -= distanciaEntreTiles;
                contTiles = 0;
            }
        }
        if (!mesmaFileiraMovimentoAcao)
        {
            tileX = inicioX;
            tileY -= distanciaEntreObjetos;
        }
    }

    void gerarBotoes()
    {
        GameObject tilePlay = Instantiate(tileBotao, new Vector3(gridManager.getLargura() + tileX, gridManager.getAltura() + tileY), Quaternion.identity);
        tilePlay.GetComponent<SpriteRenderer>().sprite = spritePlay;
        tilePlay.GetComponent<BotaoAcao>().inicializar(this);

        tileX += distanciaEntreTiles;

        GameObject tileParar = Instantiate(tileBotao, new Vector3(gridManager.getLargura() + tileX, gridManager.getAltura() + tileY), Quaternion.identity);
        tileParar.GetComponent<SpriteRenderer>().sprite = spriteParar;
        tileParar.GetComponent<BotaoAcao>().inicializar(this);

        tileX += distanciaEntreTiles;

        GameObject tileMenu = Instantiate(tileBotao, new Vector3(gridManager.getLargura() + tileX, gridManager.getAltura() + tileY), Quaternion.identity);
        tileMenu.GetComponent<SpriteRenderer>().sprite = spriteMenu;
        tileMenu.GetComponent<BotaoAcao>().inicializar(this);

        tileX += distanciaEntreTiles;

        GameObject tileInfo = Instantiate(tileBotao, new Vector3(gridManager.getLargura() + tileX, gridManager.getAltura() + tileY), Quaternion.identity);
        tileInfo.GetComponent<SpriteRenderer>().sprite = spriteInfo;
        tileInfo.GetComponent<BotaoAcao>().inicializar(this);
    }


    public Sprite GetSpritePeloCodigo(string spriteName)
    {
        int index = System.Array.IndexOf(mapea1, spriteName);

        if (index == -1)
        {
            index = 0;
        }

        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == mapea2[index] + "_0")
            {
                return sprite;
            }
        }
        return null;
    }

    public Sprite GetSpritePeloNome(string spriteName)
    {
        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == spriteName)
            {
                return sprite;
            }
        }
        return null;
    }

    public void addMovimentoPlayer(Sprite sprite)
    {
        if (contTilesPreenchidos >= tilesMovimentoJogador.Length || gridManager.getSeJogoComecou()) return;

        tilesMovimentoJogador[contTilesPreenchidos].GetComponent<SpriteRenderer>().sprite = sprite;
        contTilesPreenchidos++;
    }

    public GameObject[] getTilesMovimentoJogador()
    {
        return tilesMovimentoJogador;
    }
    private void passarMovimentoParaPlayer()
    {
        GameObject jogador = GetComponent<GridManager>().getInstanciaJogador();

        string[] movimentoString = new string[contTilesPreenchidos];

        for (int i = 0; i < contTilesPreenchidos; i++)
        {
            movimentoString[i] = tilesMovimentoJogador[i].GetComponent<SpriteRenderer>().sprite.name;
        }

        jogador.GetComponent<Player>().setMovimentos(movimentoString);

        GetComponent<GridManager>().iniciarMovimentacao();
    }

    public void botaoAcao(String acao)
    {

        switch (acao)
        {
            case "play_0":
                if (podePararNivel || contTilesPreenchidos == 0) return;
                podePararNivel = true;
                passarMovimentoParaPlayer();
                break;

            case "parar_0":
                reiniciarMovimentacaoJogador();
                
                if (podePararNivel)
                {
                    podePararNivel = false;
                    reiniciarNivel();
                }
                break;
            case "menu_0":
                gameManager.VoltarTelaFases();
                break; 
            case "InfoBotao_0":
                gameManager.uiManager.AtivarTelaTutorial();
                break; 
        }
    }

    public void reiniciarNivel()
    {
        gridManager.reiniciarNivel();
    }



    private void reiniciarMovimentacaoJogador()
    {
        
        Sprite padrao = tileJogador.GetComponent<SpriteRenderer>().sprite;
        foreach (GameObject item in tilesMovimentoJogador)
        {
            SpriteRenderer sp = item.GetComponent<SpriteRenderer>();
            sp.sprite = padrao;
            sp.color = Color.white;
            
        }
        contTilesPreenchidos = 0;
    }

    public void destruirObjetosCena()
    {
        foreach (GameObject item in tilesMovimentoJogador)
        {
            Destroy(item);
        }

    }

    public void setGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }



}
