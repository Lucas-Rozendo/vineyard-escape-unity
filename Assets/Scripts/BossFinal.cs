using System;
using System.Collections;
using UnityEngine;

public class BossFinal : MonoBehaviour
{
    public static event Action OnBossMorreu;

    public float velocidade = 1.2f;
    public int vidaMaxima = 30;
    public int vida = 30;
    public int dano = 1;

    [Header("Ataque")]
    public float distanciaAtaque = 2f;
    public float intervaloAtaque = 1.2f;

    [Header("Animação")]
    public float tempoAnimDano = 0.25f;
    public float tempoAnimMorte = 0.8f;

    private Rigidbody2D rb;
    private Transform jogador;
    private Jogador jogadorScript;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool morto = false;
    private bool bloqueadoAnimacao = false;

    private float proximoAtaque = 0f;

    // 0 = Down, 1 = Left, 2 = Right, 3 = Up
    private int facing = 0;

    private string estadoAtual = "";
    private Color corOriginal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            corOriginal = spriteRenderer.color;
        }

        GameObject objJogador = GameObject.FindGameObjectWithTag("Player");

        if (objJogador != null)
        {
            jogador = objJogador.transform;
            jogadorScript = objJogador.GetComponent<Jogador>();
        }
        else
        {
            Debug.LogWarning("Jogador não encontrado. Verifique se ele está com a tag Player.");
        }

        TocarAnimacao("Idle_Down");

        vida = vidaMaxima;

        if (HUDController.Instance != null)
        {
            HUDController.Instance.MostrarBoss(true);
            HUDController.Instance.AtualizarBoss(vida, vidaMaxima);
        }

    }

    void FixedUpdate()
    {
        if (morto)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (jogador == null)
        {
            rb.linearVelocity = Vector2.zero;
            TocarAnimacao("Idle_Down");
            return;
        }

        if (bloqueadoAnimacao)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direcao = jogador.position - transform.position;
        float distancia = direcao.magnitude;

        direcao = direcao.normalized;

        AtualizarFacing(direcao);

        if (distancia <= distanciaAtaque)
        {
            rb.linearVelocity = Vector2.zero;
            TocarAnimacao("Attack_" + SufixoDirecao());
            AtacarJogador();
        }
        else
        {
            rb.linearVelocity = direcao * velocidade;
            TocarAnimacao("Walk_" + SufixoDirecao());
        }
    }

    void AtualizarFacing(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0)
            {
                facing = 2;

                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = false;
                }
            }
            else
            {
                facing = 1;

                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = true;
                }
            }
        }
        else
        {
            if (dir.y > 0)
            {
                facing = 3;
            }
            else
            {
                facing = 0;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    string SufixoDirecao()
    {
        if (facing == 3)
        {
            return "Up";
        }

        if (facing == 0)
        {
            return "Down";
        }

        return "Side";
    }

    void AtacarJogador()
    {
        if (jogadorScript == null)
        {
            return;
        }

        if (Time.time >= proximoAtaque)
        {
            jogadorScript.ReceberDano(dano);
            proximoAtaque = Time.time + intervaloAtaque;
        }
    }

    void TocarAnimacao(string nomeAnimacao)
    {
        if (animator == null)
        {
            return;
        }

        if (estadoAtual == nomeAnimacao)
        {
            return;
        }

        int hash = Animator.StringToHash(nomeAnimacao);

        if (!animator.HasState(0, hash))
        {
            Debug.LogWarning("A animação '" + nomeAnimacao + "' não existe no Animator do boss.");
            return;
        }

        animator.Play(nomeAnimacao);
        estadoAtual = nomeAnimacao;
    }
    
    public void ReceberDano(int danoRecebido)
    {
        if (morto)
        {
            return;
        }

        vida -= danoRecebido;

        Debug.Log("Vida do Boss: " + vida);

        if (HUDController.Instance != null)
        {
            HUDController.Instance.AtualizarBoss(vida, vidaMaxima);
        }

        if (vida <= 0)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.TocarSom(AudioManager.Instance.somBossMorreu);
            }

            StartCoroutine(RotinaMorrer());
        }
        else
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.TocarSom(AudioManager.Instance.somDanoInimigo);
            }

            StartCoroutine(RotinaDano());
        }
    }

    IEnumerator RotinaDano()
    {
        bloqueadoAnimacao = true;

        TocarAnimacao("Hurt_" + SufixoDirecao());

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

        yield return new WaitForSeconds(tempoAnimDano);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = corOriginal;
        }

        bloqueadoAnimacao = false;
    }

    IEnumerator RotinaMorrer()
    {
        morto = true;
        bloqueadoAnimacao = true;

        rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
        {
            col.enabled = false;
        }

        TocarAnimacao("Die_" + SufixoDirecao());

        yield return new WaitForSeconds(tempoAnimMorte);

        if (HUDController.Instance != null)
        {
            HUDController.Instance.MostrarBoss(false);
        }

        Debug.Log("BOSS DERROTADO!");

        OnBossMorreu?.Invoke();

        Destroy(gameObject);
    }
}