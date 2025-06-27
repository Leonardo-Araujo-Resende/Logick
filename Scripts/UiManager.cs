using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class UiManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameObject telaInicial;
    [SerializeField] private GameObject telaConcordar;
    [SerializeField] private GameObject telaMundos;
    [SerializeField] private GameObject telaFases;
    [SerializeField] private GameObject telaSucesso;
    [SerializeField] private GameObject telaTutorial;
    [SerializeField] private GameObject telaPergunta;

    [Header("Tela Mundos")]
    private int indexMundo;
    [SerializeField] private Mundo[] mundos;
    [SerializeField] private UnityEngine.UI.Image imagemMundo;
    [SerializeField] private UnityEngine.UI.Button botaoIniciarMundo;
    [SerializeField] private UnityEngine.UI.Button botaoSetaEsquerda;
    [SerializeField] private UnityEngine.UI.Button botaoSetaDireita;
    [SerializeField] private Text tituloMundo;
    [Header("Tela Fases")]
    [SerializeField] private Text tituloFases;
    [SerializeField] private UnityEngine.UI.Button[] botaoIniciarFase;
    [SerializeField] private Color corPassouFase;
    [SerializeField] private Color corFaseBloqueada;
    [Header("Tela Sucesso")]
    [SerializeField] private UnityEngine.UI.Button proximaFase;
    [Header("Tela Tutorial")]
    [SerializeField] private GameObject panelExplicacaoJogador;
    [SerializeField] private GameObject panelExplicacaoChegada;
    [SerializeField] private GameObject panelExplicacaoMoeda;
    [SerializeField] private GameObject panelExplicacaoInimigo;
    [SerializeField] private GameObject panelExplicacaoMovimentacao;
    [SerializeField] private GameObject panelExplicacaoAcoes;
    private string nivelProg;


    void Start()
    {
        indexMundo = 0;
        gameManager = GetComponent<GameManager>();
        AtualizarMundo(indexMundo);
    }


    public void DesativarTelaInicial() { telaInicial.SetActive(false); }
    public void AtivarTelaInicial() { telaInicial.SetActive(true); }

    public void DesativarTelaMundos() { telaMundos.SetActive(false); }
    public void AtivarTelaTutorial() { telaTutorial.SetActive(true); }
    public void DesativarTelaTutorial() { telaTutorial.SetActive(false); }
    public void AtivarExplicacaoJogador()
    {
        DesativarTelasExplicacao();
        panelExplicacaoJogador.SetActive(true);
    }

    public void AtivarExplicacaoChegada()
    {
        DesativarTelasExplicacao();
        panelExplicacaoChegada.SetActive(true);
    }


    public void AtivarExplicacaoMoeda()
    {
        DesativarTelasExplicacao();
        panelExplicacaoMoeda.SetActive(true);
    }

    public void AtivarExplicacaoInimigo()
    {
        DesativarTelasExplicacao();
        panelExplicacaoInimigo.SetActive(true);
    }

    public void AtivarExplicacaoMovimentacao()
    {
        DesativarTelasExplicacao();
        panelExplicacaoMovimentacao.SetActive(true);
    }
    public void AtivarExplicacaoAcoes()
    {
        DesativarTelasExplicacao();
        panelExplicacaoAcoes.SetActive(true);
    }

    public void DesativarTelasExplicacao()
    {
        panelExplicacaoJogador.SetActive(false);
        panelExplicacaoChegada.SetActive(false);
        panelExplicacaoMoeda.SetActive(false);
        panelExplicacaoInimigo.SetActive(false);
        panelExplicacaoMovimentacao.SetActive(false);
        panelExplicacaoAcoes.SetActive(false);
    }
    public void AtivarTelaMundos()
    {

        DesativarTodasTelas();
        telaMundos.SetActive(true);
    }
    public void AtualizarMundoFrente()
    {
        if (indexMundo + 1 >= mundos.Length) return;
        indexMundo++;
        AtualizarMundo(indexMundo);
    }
    public void AtualizarMundoTras()
    {
        if (indexMundo - 1 < 0) return;

        indexMundo--;
        AtualizarMundo(indexMundo);
    }
    private void AtualizarMundo(int indexMundo)
    {
        imagemMundo.sprite = mundos[indexMundo].imagem;
        tituloMundo.text = mundos[indexMundo].nome;

        botaoIniciarMundo.onClick.RemoveAllListeners();
        botaoIniciarMundo.onClick.AddListener(() => IniciarMundo(indexMundo));

        AtualizarInteratividadeBotoesMundo(indexMundo);
        if (gameManager.PodeIniciarFase(indexMundo, 0))
        {
            botaoIniciarMundo.transform.Find("Cadeado").gameObject.SetActive(false);
            botaoIniciarMundo.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
        else
        {
            botaoIniciarMundo.transform.Find("Cadeado").gameObject.SetActive(true);
            botaoIniciarMundo.GetComponent<UnityEngine.UI.Image>().color = corFaseBloqueada;
        }
    }
    public void IniciarMundo(int mundo)
    {
        DesativarTelaMundos();
        IniciarTelaFases(mundo);
    }

    public void DesativarTelaFases() { telaFases.SetActive(false); }
    public void AtivarTelaFases()
    {
        gameManager.ExcluirObjetosFase();
        DesativarTodasTelas();
        IniciarTelaFases(indexMundo);
    }
    public void IniciarTelaFases(int mundo)
    {
        telaFases.SetActive(true);
        tituloFases.text = mundos[mundo].nome;
        int cont = 0;
        foreach (var item in botaoIniciarFase)
        {
            int tempFase = cont;
            int tempMundo = mundo;
            item.onClick.RemoveAllListeners();
            if (gameManager.PodeIniciarFase(tempMundo, tempFase))
            {
                item.onClick.AddListener(() => IniciarFase(tempMundo, tempFase));
                item.transform.Find("Cadeado").gameObject.SetActive(false);
                item.transform.Find("NumFase").gameObject.SetActive(true);


                NivelStatistics estatistica = gameManager.ObterEstatistica(tempMundo, tempFase);
                if (estatistica != null && estatistica.passouDeFase)
                {
                    item.GetComponent<UnityEngine.UI.Image>().color = corPassouFase;
                }
            }
            else
            {
                item.transform.Find("Cadeado").gameObject.SetActive(true);
                item.transform.Find("NumFase").gameObject.SetActive(false);
                item.GetComponent<UnityEngine.UI.Image>().color = corFaseBloqueada;
            }

            cont++;
        }
    }
    public void IniciarFase(int mundo, int fase)
    {
        if (!gameManager.PodeIniciarFase(mundo, fase)) return;
        DesativarTodasTelas();
        gameManager.iniciarNivel(mundo, fase);
    }

    public void DesativarTelaSucesso() { telaSucesso.SetActive(false); }
    public void AtivarTelaSucesso(int mundo, int fase)
    {
        telaSucesso.SetActive(true);
        if (fase == gameManager.ObterMundo(mundo).Length - 1)
        {
            proximaFase.transform.Find("Texto").GetComponent<Text>().text = "Próximo mundo";
            proximaFase.onClick.RemoveAllListeners();
            proximaFase.onClick.AddListener(gameManager.ExcluirObjetosFase);
            proximaFase.onClick.AddListener(() => AtivarTelaMundos());
        }
        else
        {
            proximaFase.transform.Find("Texto").GetComponent<Text>().text = "Próxima fase";
            proximaFase.onClick.RemoveAllListeners();
            proximaFase.onClick.AddListener(gameManager.ExcluirObjetosFase);
            proximaFase.onClick.AddListener(() => IniciarFase(mundo, fase + 1));
        }

    }
    public void AtivarTelaConcordar() { telaConcordar.SetActive(true); }
    public void DesativarTelaConcordar() { telaConcordar.SetActive(false); }
    public void DesativarTelaPergunta() { telaPergunta.SetActive(false); }
    public void AtivarTelaPergunta() { telaPergunta.SetActive(true); }

    private void DesativarTodasTelas()
    {

        DesativarTelaSucesso();
        DesativarTelaFases();
        DesativarTelaMundos();
        DesativarTelaInicial();
    }

    private void AtualizarInteratividadeBotoesMundo(int mundo)
    {
        if (mundo == 0) botaoSetaEsquerda.interactable = false;
        else botaoSetaEsquerda.interactable = true;

        if (mundo == mundos.Length - 1) botaoSetaDireita.interactable = false;
        else botaoSetaDireita.interactable = true;
    }

    public void NivelProgExcelente() { nivelProg = "excelente"; }
    public void NivelProgBom() { nivelProg = "bom"; }
    public void NivelProgOk() { nivelProg = "ok"; }
    public void NivelProgPouco() { nivelProg = "pouco"; }
    public void NivelProgInexistente() { nivelProg = "nenhum"; }
    
    public string getNivelProg(){ return nivelProg; }

}
