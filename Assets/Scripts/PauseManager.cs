using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject painelPause;

    [Header("Cenas")]
    public string nomeCenaMenu = "MenuPrincipal";

    private bool pausado = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (painelPause != null)
        {
            painelPause.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            AlternarPause();
        }
    }

    public void AlternarPause()
    {
        if (pausado)
        {
            Continuar();
        }
        else
        {
            Pausar();
        }
    }

    public void Pausar()
    {
        pausado = true;
        Time.timeScale = 0f;

        if (painelPause != null)
        {
            painelPause.SetActive(true);
        }
    }

    public void Continuar()
    {
        pausado = false;
        Time.timeScale = 1f;

        if (painelPause != null)
        {
            painelPause.SetActive(false);
        }
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VoltarMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaMenu);
    }
}