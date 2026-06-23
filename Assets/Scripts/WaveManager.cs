using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject inimigoPadraoPrefab;
    public GameObject inimigoTankPrefab;
    public GameObject uvaColetavelPrefab;
    public GameObject bossFinalPrefab;

    [Header("Configuração das Waves")]
    public int waveAtual = 0;
    public int totalWaves = 5;
    public int inimigosBase = 2;
    public float tempoEntreWaves = 2f;

    [Header("Spawns dos Inimigos")]
    public Transform[] pontosSpawnInimigos;

    [Header("Spawns das Uvas")]
    public Transform[] pontosSpawnUvas;

    [Header("Boss")]
    public Transform pontoSpawnBoss;
    public Vector2 posicaoBoss = new Vector2(0f, 3f);

    [Header("Área alternativa de Spawn")]
    public Vector2 limiteMinimo = new Vector2(-8f, -4f);
    public Vector2 limiteMaximo = new Vector2(8f, 4f);

    private int inimigosVivos = 0;
    private bool jogoEmAndamento = true;
    private bool bossInvocado = false;

    void OnEnable()
    {
        Inimigo.OnInimigoMorreu += InimigoMorreu;
        BossFinal.OnBossMorreu += BossMorreu;
    }

    void OnDisable()
    {
        Inimigo.OnInimigoMorreu -= InimigoMorreu;
        BossFinal.OnBossMorreu -= BossMorreu;
    }

    void Start()
    {
        if (HUDController.Instance != null)
        {
            HUDController.Instance.AtualizarWave(waveAtual, totalWaves);
        }

        StartCoroutine(IniciarProximaWave());
    }

    IEnumerator IniciarProximaWave()
    {
        yield return new WaitForSeconds(tempoEntreWaves);

        if (!jogoEmAndamento)
        {
            yield break;
        }

        waveAtual++;

        if (waveAtual > totalWaves)
        {
            InvocarBoss();
            yield break;
        }

        Debug.Log("WAVE " + waveAtual + " COMEÇOU!");

        if (HUDController.Instance != null)
        {
            HUDController.Instance.AtualizarWave(waveAtual, totalWaves);
            HUDController.Instance.MostrarMensagem("Wave " + waveAtual + " começou!");
        }

        int quantidadePadrao = inimigosBase + waveAtual;
        int quantidadeTank = 0;

        if (waveAtual >= 2)
        {
            quantidadeTank = waveAtual - 1;
        }

        inimigosVivos = 0;

        for (int i = 0; i < quantidadePadrao; i++)
        {
            if (CriarInimigoPadrao())
            {
                inimigosVivos++;
            }
        }

        for (int i = 0; i < quantidadeTank; i++)
        {
            if (CriarInimigoTank())
            {
                inimigosVivos++;
            }
        }

        CriarUvasDaWave();

        if (inimigosVivos <= 0)
        {
            StartCoroutine(IniciarProximaWave());
        }
    }

    bool CriarInimigoPadrao()
    {
        if (inimigoPadraoPrefab == null)
        {
            Debug.LogWarning("Prefab do inimigo padrão não foi colocado no WaveManager.");
            return false;
        }

        Vector2 posicao = GerarPosicaoDeSpawnInimigo();

        GameObject novoInimigo = Instantiate(inimigoPadraoPrefab, posicao, Quaternion.identity);

        Inimigo inimigo = novoInimigo.GetComponent<Inimigo>();

        if (inimigo != null)
        {
            inimigo.velocidade = 2.0f + (waveAtual * 0.2f);
            inimigo.vida = 1;
            inimigo.dano = 1;
        }

        return true;
    }

    bool CriarInimigoTank()
    {
        if (inimigoTankPrefab == null)
        {
            Debug.LogWarning("Prefab do inimigo tank não foi colocado no WaveManager.");
            return false;
        }

        Vector2 posicao = GerarPosicaoDeSpawnInimigo();

        GameObject novoInimigo = Instantiate(inimigoTankPrefab, posicao, Quaternion.identity);

        Inimigo inimigo = novoInimigo.GetComponent<Inimigo>();

        if (inimigo != null)
        {
            inimigo.velocidade = 1.1f + (waveAtual * 0.1f);
            inimigo.vida = 3 + waveAtual;
            inimigo.dano = 1;
        }

        return true;
    }

    void CriarUvasDaWave()
    {
        if (uvaColetavelPrefab == null)
        {
            Debug.LogWarning("Prefab da uva coletável não foi colocado no WaveManager.");
            return;
        }

        int quantidadeUvas = 3;

        for (int i = 0; i < quantidadeUvas; i++)
        {
            Vector2 posicao = GerarPosicaoDeSpawnUva();
            Instantiate(uvaColetavelPrefab, posicao, Quaternion.identity);
        }
    }

    void InvocarBoss()
    {
        if (bossInvocado)
        {
            return;
        }

        bossInvocado = true;

        Debug.Log("BOSS FINAL APARECEU!");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarSom(AudioManager.Instance.somBossApareceu);
        }

        if (HUDController.Instance != null)
        {
            HUDController.Instance.MostrarMensagem("BOSS FINAL!");
        }

        if (bossFinalPrefab == null)
        {
            Debug.LogWarning("Prefab do Boss Final não foi colocado no WaveManager.");
            return;
        }

        Vector2 posicaoFinalBoss = posicaoBoss;

        if (pontoSpawnBoss != null)
        {
            posicaoFinalBoss = pontoSpawnBoss.position;
        }

        Instantiate(bossFinalPrefab, posicaoFinalBoss, Quaternion.identity);
    }

    void BossMorreu()
    {
        jogoEmAndamento = false;
        Debug.Log("VITÓRIA! Você venceu o jogo!");

        if (HUDController.Instance != null)
        {
            HUDController.Instance.MostrarVitoria();
        }
    }

    Vector2 GerarPosicaoDeSpawnInimigo()
    {
        if (pontosSpawnInimigos != null && pontosSpawnInimigos.Length > 0)
        {
            int indice = Random.Range(0, pontosSpawnInimigos.Length);

            if (pontosSpawnInimigos[indice] != null)
            {
                return pontosSpawnInimigos[indice].position;
            }
        }

        return GerarPosicaoNasBordas();
    }

    Vector2 GerarPosicaoDeSpawnUva()
    {
        if (pontosSpawnUvas != null && pontosSpawnUvas.Length > 0)
        {
            int indice = Random.Range(0, pontosSpawnUvas.Length);

            if (pontosSpawnUvas[indice] != null)
            {
                return pontosSpawnUvas[indice].position;
            }
        }

        return GerarPosicaoDentroDaArena();
    }

    Vector2 GerarPosicaoDentroDaArena()
    {
        float x = Random.Range(limiteMinimo.x + 1f, limiteMaximo.x - 1f);
        float y = Random.Range(limiteMinimo.y + 1f, limiteMaximo.y - 1f);

        return new Vector2(x, y);
    }

    Vector2 GerarPosicaoNasBordas()
    {
        int lado = Random.Range(0, 4);

        float x;
        float y;

        if (lado == 0)
        {
            x = limiteMinimo.x;
            y = Random.Range(limiteMinimo.y, limiteMaximo.y);
        }
        else if (lado == 1)
        {
            x = limiteMaximo.x;
            y = Random.Range(limiteMinimo.y, limiteMaximo.y);
        }
        else if (lado == 2)
        {
            x = Random.Range(limiteMinimo.x, limiteMaximo.x);
            y = limiteMaximo.y;
        }
        else
        {
            x = Random.Range(limiteMinimo.x, limiteMaximo.x);
            y = limiteMinimo.y;
        }

        return new Vector2(x, y);
    }

    void InimigoMorreu()
    {
        inimigosVivos--;

        Debug.Log("Inimigos vivos: " + inimigosVivos);

        if (inimigosVivos <= 0 && jogoEmAndamento)
        {
            Debug.Log("Wave " + waveAtual + " finalizada!");
            StartCoroutine(IniciarProximaWave());
        }
    }
}