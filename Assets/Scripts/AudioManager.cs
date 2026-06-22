using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fontes de áudio")]
    public AudioSource efeitosSource;
    public AudioSource musicaSource;

    [Header("Música de fundo")]
    public AudioClip musicaFundoMenu;
    public AudioClip musicaFundoJogo;

    [Header("Sons do jogador")]
    public AudioClip somTiro;
    public AudioClip somEspada;
    public AudioClip somEspecial;
    public AudioClip somDanoJogador;

    [Header("Sons de itens")]
    public AudioClip somPegarUva;

    [Header("Sons de inimigos")]
    public AudioClip somDanoInimigo;
    public AudioClip somMorteInimigo;
    public AudioClip somBossApareceu;
    public AudioClip somBossMorreu;

    [Header("Sons finais")]
    public AudioClip somVitoria;
    public AudioClip somDerrota;

    [Header("Volume")]
    [Range(0f, 1f)] public float volumeGeral = 1f;
    [Range(0f, 1f)] public float volumeMusica = 0.5f;
    [Range(0f, 1f)] public float volumeEfeitos = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (efeitosSource == null)
        {
            efeitosSource = GetComponent<AudioSource>();
        }

        CarregarVolumes();
        AplicarVolumes();
    }

    public void TocarSom(AudioClip clip)
    {
        if (clip == null || efeitosSource == null)
        {
            return;
        }

        efeitosSource.PlayOneShot(clip);
    }

    public void TocarMusica(AudioClip musica)
    {
        if (musica == null || musicaSource == null)
        {
            return;
        }

        if (musicaSource.clip == musica && musicaSource.isPlaying)
        {
            return;
        }

        musicaSource.clip = musica;
        musicaSource.loop = true;
        musicaSource.Play();
    }

    public void TocarMusicaMenu()
    {
        TocarMusica(musicaFundoMenu);
    }

    public void TocarMusicaJogo()
    {
        TocarMusica(musicaFundoJogo);
    }

    public void AlterarVolumeGeral(float valor)
    {
        volumeGeral = valor;
        SalvarVolumes();
        AplicarVolumes();
    }

    public void AlterarVolumeMusica(float valor)
    {
        volumeMusica = valor;
        SalvarVolumes();
        AplicarVolumes();
    }

    public void AlterarVolumeEfeitos(float valor)
    {
        volumeEfeitos = valor;
        SalvarVolumes();
        AplicarVolumes();
    }

    void AplicarVolumes()
    {
        AudioListener.volume = volumeGeral;

        if (musicaSource != null)
        {
            musicaSource.volume = volumeMusica;
        }

        if (efeitosSource != null)
        {
            efeitosSource.volume = volumeEfeitos;
        }
    }

    void SalvarVolumes()
    {
        PlayerPrefs.SetFloat("VolumeGeral", volumeGeral);
        PlayerPrefs.SetFloat("VolumeMusica", volumeMusica);
        PlayerPrefs.SetFloat("VolumeEfeitos", volumeEfeitos);
        PlayerPrefs.Save();
    }

    void CarregarVolumes()
    {
        volumeGeral = PlayerPrefs.GetFloat("VolumeGeral", 1f);
        volumeMusica = PlayerPrefs.GetFloat("VolumeMusica", 0.5f);
        volumeEfeitos = PlayerPrefs.GetFloat("VolumeEfeitos", 1f);
    }
}