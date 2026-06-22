using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float velocidade = 10f;
    public float tempoDeVida = 2f;
    public int dano = 1;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    public void Configurar(Vector2 direcao)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        direcao = direcao.normalized;

        if (direcao == Vector2.zero)
        {
            direcao = Vector2.down;
        }

        rb.linearVelocity = direcao * velocidade;

        // Faz a flecha girar para a direção do tiro
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angulo + 225f);;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Inimigo inimigo = other.GetComponent<Inimigo>();

        if (inimigo != null)
        {
            inimigo.ReceberDano(dano);
            Destroy(gameObject);
            return;
        }

        BossFinal boss = other.GetComponent<BossFinal>();

        if (boss != null)
        {
            boss.ReceberDano(dano);
            Destroy(gameObject);
            return;
        }
    }
}