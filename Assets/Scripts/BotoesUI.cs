using UnityEngine;
using UnityEngine.SceneManagement;

public class BotoesUI : MonoBehaviour
{
    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}