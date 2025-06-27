using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "EnemySO")]
public class EnemySO : ScriptableObject
{
    public string[] movimentPattern;
    public int posicaoInicialX, posicaoInicialY;
    public int posicaoX, posicaoY;
    public Sprite imagem;
    public Color corNomal, corEscolhida;
    private int posicaoMovimento = 0;
    private GameObject[] movimentTiles;
    public void reiniciarPosicao()
    {
        posicaoX = posicaoInicialX;
        posicaoY = posicaoInicialY;
        posicaoMovimento = 0;
    }

    public void movimentarTile()
    {
        movimentTiles[posicaoMovimento].GetComponent<SpriteRenderer>().color = corEscolhida;
        
        int posicaoAnterior = (posicaoMovimento - 1 + movimentPattern.Length) % movimentPattern.Length;
        movimentTiles[posicaoAnterior].GetComponent<SpriteRenderer>().color = corNomal;

        posicaoMovimento = (posicaoMovimento + 1) % movimentPattern.Length;

    }

    public void setMovimentTiles(GameObject[] tiles)
    {
        movimentTiles = tiles;
    }

    public void resetarCores()
    {
        if (movimentTiles == null) return;
        foreach (var item in movimentTiles)
        {
            item.GetComponent<SpriteRenderer>().color = corNomal;
        }
    }
}
