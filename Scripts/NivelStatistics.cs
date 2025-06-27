using UnityEngine;

public class NivelStatistics
{
    public bool passouDeFase;
    public int mundo;
    public int nivel;
    public int quantidadeTentativas;
    public float tempoTotal;
    public string nivelProg;
    public int qntMovimentosPassar;

    public NivelStatistics()
    {
        this.passouDeFase = false;
        this.quantidadeTentativas = 0;
        this.tempoTotal = 0;
    }
    public void inicializar(int mundo, int nivel, string nivelProg)
    {
        this.mundo = mundo;
        this.nivel = nivel;
        this.nivelProg = nivelProg;
    }

}
