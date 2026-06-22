using UnityEngine;

public class UvaColetavel : MonoBehaviour
{
    public int valorEspecial = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        Jogador jogador = other.GetComponent<Jogador>();

        if (jogador != null)
        {
            jogador.AdicionarEspecial(valorEspecial);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.TocarSom(AudioManager.Instance.somPegarUva);
            }

            Destroy(gameObject);
        }
    }
}