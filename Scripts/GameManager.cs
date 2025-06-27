using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UiManager uiManager;

    [SerializeField] private GameObject[] mundo1;
    [SerializeField] private GameObject[] mundo2;
    [SerializeField] private GameObject[] mundo3;
    private NivelStatistics[] mundo1Estatistica;
    private NivelStatistics[] mundo2Estatistica;
    private NivelStatistics[] mundo3Estatistica;

    private GameObject faseAtual;
    private NivelStatistics estatisticaAtual;
    private float tempoInicio;
    private int intmundoAtual;
    private int intFaseAtual;
    private DataBaseManager dataBaseManager;


    void Awake()
    {
        uiManager = GetComponent<UiManager>();
    }

    void Start()
    {
        dataBaseManager = GetComponent<DataBaseManager>();
        mundo1Estatistica = new NivelStatistics[mundo1.Length];
        mundo2Estatistica = new NivelStatistics[mundo2.Length];
        mundo3Estatistica = new NivelStatistics[mundo3.Length];
    }

    void Update()
    {

    }
    public void iniciarNivel(int mundo, int fase)
    {
        if (faseAtual) finalizarNivel(-1);

        tempoInicio = Time.time;
        if (ObterEstatistica(mundo, fase) != null)
        {
            estatisticaAtual = ObterEstatistica(mundo, fase);
        }
        else
        {
            estatisticaAtual = new NivelStatistics();
            estatisticaAtual.inicializar(mundo, fase, uiManager.getNivelProg());
            SetEstatistica(mundo, fase, estatisticaAtual);
        }
        intmundoAtual = mundo;
        intFaseAtual = fase;
        faseAtual = Instantiate(ObterFase(mundo, fase), Vector3.zero, quaternion.identity);
        faseAtual.GetComponent<GridManager>().setGameManager(this);
        faseAtual.GetComponent<ObjectsMoviment>().setGameManager(this);
    }

    public void finalizarNivel(int qntMovimentosPassar)
    {
        if (!estatisticaAtual.passouDeFase)
        {
            AtualizaTempoEstatistica();
            estatisticaAtual.passouDeFase = true;
            if(qntMovimentosPassar != -1)estatisticaAtual.qntMovimentosPassar = qntMovimentosPassar;
            dataBaseManager.criarEstatistica(estatisticaAtual);
        }

        faseAtual = null;
        uiManager.AtivarTelaSucesso(intmundoAtual, intFaseAtual);
    }
    public void incrementaTentativaNivel()
    {
        if (!estatisticaAtual.passouDeFase) estatisticaAtual.quantidadeTentativas++;
    }


    private GameObject ObterFase(int mundo, int fase)
    {
        switch (mundo)
        {
            case 0: return mundo1[fase];
            case 1: return mundo2[fase];
            case 2: return mundo3[fase];
            default: return null;
        }
    }
    public GameObject[] ObterMundo(int mundo)
    {
        switch (mundo)
        {
            case 0: return mundo1;
            case 1: return mundo2;
            case 2: return mundo3;
            default: return null;
        }
    }

    private void SetEstatistica(int mundo, int fase, NivelStatistics nivelStatistics)
    {
        switch (mundo)
        {
            case 0: mundo1Estatistica[fase] = nivelStatistics; break;
            case 1: mundo2Estatistica[fase] = nivelStatistics; break;
            case 2: mundo3Estatistica[fase] = nivelStatistics; break;
        }
    }
    public NivelStatistics ObterEstatistica(int mundo, int fase)
    {
        switch (mundo)
        {
            case 0: return mundo1Estatistica[fase];
            case 1: return mundo2Estatistica[fase];
            case 2: return mundo3Estatistica[fase];
            default: return null;
        }
    }
    public void ExcluirObjetosFase()
    {
        GameObject[] todosObjetos = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in todosObjetos)
        {
            if (obj.CompareTag("Destrutivel") || obj.CompareTag("Player") || obj.CompareTag("Enemy") || obj.CompareTag("Destrutivel") || obj.CompareTag("Finish") || obj.CompareTag("Bolinha"))
            {
                Destroy(obj);
            }
        }


    }

    public bool PodeIniciarFase(int mundo, int fase)
    {
        if (mundo == 2) return false;
        if (fase != 0)
        {
            return ObterEstatistica(mundo, fase - 1) != null && ObterEstatistica(mundo, fase - 1).passouDeFase;
        }
        else if (fase == 0 && mundo == 0)
        {
            return true;
        }
        else
        {
            return ObterEstatistica(mundo - 1, ObterMundo(mundo - 1).Length - 1) != null && ObterEstatistica(mundo - 1, ObterMundo(mundo - 1).Length - 1).passouDeFase;
        }
    }

    public void VoltarTelaFases()
    {
        ExcluirObjetosFase();
        AtualizaTempoEstatistica();
        uiManager.AtivarTelaFases();
    }
    private void AtualizaTempoEstatistica()
    {
        if (!estatisticaAtual.passouDeFase) estatisticaAtual.tempoTotal += Time.time - tempoInicio;
    }

    public static void AdicionarTagDestrutivel(GameObject objeto)
    {
        objeto.tag = "Destrutivel";
    }

    public void fecharJogo()
    {
        Application.Quit();
    }
}
