using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Paineis")]
    public GameObject painelMenuPrincipal;
    public GameObject painelConfiguracoes;

    [Header("Sliders")]
    public Slider sliderVolumeGeral;
    public Slider sliderVolumeMusica;
    public Slider sliderVolumeEfeitos;

    [Header("Cena")]
    public string nomeCenaJogo = "CenaJogo";

    void Start()
    {
        if (painelMenuPrincipal != null)
        {
            painelMenuPrincipal.SetActive(true);
        }

        if (painelConfiguracoes != null)
        {
            painelConfiguracoes.SetActive(false);
        }

        ConfigurarSliders();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarMusicaMenu();
        }
    }

    void ConfigurarSliders()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        if (sliderVolumeGeral != null)
        {
            sliderVolumeGeral.value = AudioManager.Instance.volumeGeral;
            sliderVolumeGeral.onValueChanged.AddListener(AudioManager.Instance.AlterarVolumeGeral);
        }

        if (sliderVolumeMusica != null)
        {
            sliderVolumeMusica.value = AudioManager.Instance.volumeMusica;
            sliderVolumeMusica.onValueChanged.AddListener(AudioManager.Instance.AlterarVolumeMusica);
        }

        if (sliderVolumeEfeitos != null)
        {
            sliderVolumeEfeitos.value = AudioManager.Instance.volumeEfeitos;
            sliderVolumeEfeitos.onValueChanged.AddListener(AudioManager.Instance.AlterarVolumeEfeitos);
        }
    }

    public void Jogar()
    {
        SceneManager.LoadScene(nomeCenaJogo);
    }

    public void AbrirConfiguracoes()
    {
        if (painelMenuPrincipal != null)
        {
            painelMenuPrincipal.SetActive(false);
        }

        if (painelConfiguracoes != null)
        {
            painelConfiguracoes.SetActive(true);
        }
    }

    public void FecharConfiguracoes()
    {
        if (painelConfiguracoes != null)
        {
            painelConfiguracoes.SetActive(false);
        }

        if (painelMenuPrincipal != null)
        {
            painelMenuPrincipal.SetActive(true);
        }
    }

    public void Sair()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}