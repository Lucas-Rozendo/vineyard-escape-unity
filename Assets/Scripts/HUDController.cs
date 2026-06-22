using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;


    [Header("Boss")]
    public GameObject painelBoss;
    public TMP_Text textoBoss;
    public Image barraBossFill;

    [Header("Vida por imagens")]
    public Image[] imagensVida;

    [Header("Especial por imagens")]
    public Image[] imagensEspecial;


    [Header("Textos")]
    public TMP_Text textoVida;
    public TMP_Text textoWave;
    public TMP_Text textoEspecial;
    public TMP_Text textoMensagem;

    [Header("Paineis")]
    public GameObject painelGameOver;
    public GameObject painelVitoria;

    void Awake()
    {
        Instance = this;

        if (painelGameOver != null)
        {
            painelGameOver.SetActive(false);
        }

        if (painelVitoria != null)
        {
            painelVitoria.SetActive(false);
        }

        if (painelBoss != null)
        {
            painelBoss.SetActive(false);
        }

        AtualizarEspecial(0, imagensEspecial.Length);

        Time.timeScale = 1f;
    }

    void Start()
    {
        AtualizarEspecial(0, imagensEspecial.Length);
    }

    public void AtualizarVida(int vidaAtual, int vidaMaxima)
    {
        if (textoVida != null)
        {
            textoVida.text = "Vida: " + vidaAtual + "/" + vidaMaxima;
        }

        for (int i = 0; i < imagensVida.Length; i++)
        {
            if (imagensVida[i] != null)
            {
                if (i < vidaAtual)
                {
                    imagensVida[i].color = Color.red;
                }
                else
                {
                    imagensVida[i].color = new Color(0.25f, 0.25f, 0.25f, 0.6f);
                }
            }
        }
    }

    public void AtualizarWave(int waveAtual, int totalWaves)
    {
        if (textoWave != null)
        {
            textoWave.text = "WAVE " + waveAtual + " / " + totalWaves;
        }
    }

public void AtualizarEspecial(int especialAtual, int especialMaximo)
{
    if (textoEspecial != null)
    {
        textoEspecial.text = "Especial: " + especialAtual + "/" + especialMaximo;
    }

    especialAtual = Mathf.Clamp(especialAtual, 0, imagensEspecial.Length);

    for (int i = 0; i < imagensEspecial.Length; i++)
    {
        if (imagensEspecial[i] == null)
        {
            continue;
        }

        // Garante que a uva existe e aparece
        imagensEspecial[i].gameObject.SetActive(true);
        imagensEspecial[i].enabled = true;

        if (i < especialAtual)
        {
            // Uva carregada
            imagensEspecial[i].color = Color.white;
        }
        else
        {
            // Uva apagada
            imagensEspecial[i].color = new Color(0.25f, 0.25f, 0.25f, 0.35f);
        }
    }
}

    public void MostrarMensagem(string mensagem)
    {
        if (textoMensagem != null)
        {
            textoMensagem.text = mensagem;
            CancelInvoke(nameof(LimparMensagem));
            Invoke(nameof(LimparMensagem), 2f);
        }
    }

    void LimparMensagem()
    {
        if (textoMensagem != null)
        {
            textoMensagem.text = "";
        }
    }

    public void MostrarDerrota()
    {
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarSom(AudioManager.Instance.somDerrota);
        }

        Time.timeScale = 0f;
    }

    public void MostrarVitoria()
    {
        if (painelVitoria != null)
        {
            painelVitoria.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarSom(AudioManager.Instance.somVitoria);
        }

        Time.timeScale = 0f;
    }

    public void MostrarBoss(bool mostrar)
    {
        if (painelBoss != null)
        {
            painelBoss.SetActive(mostrar);
        }
    }

    public void AtualizarBoss(int vidaAtual, int vidaMaxima)
    {
        if (textoBoss != null)
        {
            textoBoss.text = "BOSS: " + vidaAtual + " / " + vidaMaxima;
        }

        if (barraBossFill != null)
        {
            float porcentagem = (float)vidaAtual / vidaMaxima;
            barraBossFill.fillAmount = porcentagem;
        }
    }

}