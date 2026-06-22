using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Jogador : MonoBehaviour
{
    public float velocidade = 5f;

    [Header("Vida")]
    public int vidaMaxima = 3;
    public int vidaAtual;
    public float tempoInvencivel = 1f;

    [Header("Tiro")]
    public GameObject projetilPrefab;
    public Transform pontoTiro;
    public float tempoEntreTiros = 0.25f;
    public float atrasoDisparo = 0.12f;

    [Header("Espada")]
    public int danoEspada = 1;
    public float alcanceEspada = 0.8f;
    public Vector2 tamanhoAreaEspada = new Vector2(0.9f, 0.7f);
    public float tempoEntreGolpes = 0.45f;
    public float tempoAnimEspada = 0.3f;

    private float proximoGolpe;

    [Header("Posição do tiro")]
    public Vector2 pontoTiroBaixo = new Vector2(0f, -0.15f);
    public Vector2 pontoTiroCima = new Vector2(0f, 0.45f);
    public Vector2 pontoTiroLado = new Vector2(0.35f, 0.2f);
     
    [Header("Especial")]
    public int especialAtual = 0;
    public int especialMaximo = 5;
    public float raioEspecial = 3f;
    public int danoEspecial = 999;

    [Header("Efeito do Especial")]
    public GameObject efeitoEspecialPrefab;

    [Header("Animação")]
    public float tempoAnimAtaque = 0.25f;
    public float tempoAnimDano = 0.2f;
    public float tempoAnimMorte = 0.6f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 movimento;
    private Vector2 direcaoOlhar = Vector2.down;

    private float proximoTiro;
    private bool estaInvencivel = false;
    private bool bloqueadoAnimacao = false;
    private bool morto = false;

    private Color corOriginal;

    // 0 = Down, 1 = Left, 2 = Right, 3 = Up
    private int facing = 0;

    private string estadoAtual = "";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        vidaAtual = vidaMaxima;

        if (HUDController.Instance != null)
        {
            HUDController.Instance.AtualizarVida(vidaAtual, vidaMaxima);
            HUDController.Instance.AtualizarEspecial(especialAtual, especialMaximo);
        }

        if (spriteRenderer != null)
        {
            corOriginal = spriteRenderer.color;
        }

        TocarAnimacao("Idle_Down");
    }

    void Update()
    {
        if (morto)
        {
            return;
        }

        if (!bloqueadoAnimacao)
        {
            AtualizarAnimacaoBase();
        }
    }

    public void OnMover(InputValue value)
    {
        if (morto)
        {
            return;
        }

        movimento = value.Get<Vector2>();

        if (movimento.sqrMagnitude > 0.01f)
        {
            direcaoOlhar = movimento.normalized;
            AtualizarFacing(movimento);
        }
    }

    public void OnAtirar(InputValue value)
    {
        if (!value.isPressed || morto)
        {
            return;
        }

        AtualizarDirecaoPeloMouse();

        StartCoroutine(RotinaAtirar());
    }

    public void OnGolpear(InputValue value)
    {
        if (!value.isPressed || morto)
        {
            return;
        }

        StartCoroutine(RotinaGolpear());
    }

    void GolpearComEspada()
    {
        Vector2 direcaoAtaque = direcaoOlhar.normalized;

        if (direcaoAtaque == Vector2.zero)
        {
            direcaoAtaque = Vector2.down;
        }

        Vector2 centroAtaque = (Vector2)transform.position + direcaoAtaque * alcanceEspada;

        Collider2D[] objetosAtingidos = Physics2D.OverlapBoxAll(
            centroAtaque,
            tamanhoAreaEspada,
            0f
        );

        foreach (Collider2D objeto in objetosAtingidos)
        {
            Inimigo inimigo = objeto.GetComponent<Inimigo>();

            if (inimigo != null)
            {
                inimigo.ReceberDano(danoEspada);
            }

            BossFinal boss = objeto.GetComponent<BossFinal>();

            if (boss != null)
            {
                boss.ReceberDano(danoEspada);
            }
        }
    }

    void AtualizarDirecaoPeloMouse()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (Camera.main == null)
        {
            return;
        }

        Vector2 posicaoMouseTela = Mouse.current.position.ReadValue();
        Vector3 posicaoMouseMundo = Camera.main.ScreenToWorldPoint(posicaoMouseTela);

        Vector2 direcao = posicaoMouseMundo - transform.position;

        if (direcao.sqrMagnitude > 0.01f)
        {
            direcaoOlhar = direcao.normalized;
            AtualizarFacing(direcaoOlhar);
        }
    }

    public void OnEspecial(InputValue value)
    {
        if (!value.isPressed || morto)
        {
            return;
        }

        StartCoroutine(RotinaEspecial());
    }

    void FixedUpdate()
    {
        if (morto)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = movimento * velocidade;
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

        AtualizarPontoTiro();
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

    void AtualizarPontoTiro()
    {
        if (pontoTiro == null)
        {
            return;
        }

        if (facing == 3) // cima
        {
            pontoTiro.localPosition = pontoTiroCima;
        }
        else if (facing == 0) // baixo
        {
            pontoTiro.localPosition = pontoTiroBaixo;
        }
        else if (facing == 2) // direita
        {
            pontoTiro.localPosition = pontoTiroLado;
        }
        else if (facing == 1) // esquerda
        {
            pontoTiro.localPosition = new Vector2(-pontoTiroLado.x, pontoTiroLado.y);
        }
    }

    void AtualizarAnimacaoBase()
    {
        if (movimento.sqrMagnitude > 0.01f)
        {
            TocarAnimacao("Walk_" + SufixoDirecao());
        }
        else
        {
            TocarAnimacao("Idle_" + SufixoDirecao());
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
            Debug.LogWarning("A animação '" + nomeAnimacao + "' não existe no Animator.");
            estadoAtual = nomeAnimacao;
            return;
        }

        animator.Play(nomeAnimacao);
        estadoAtual = nomeAnimacao;
    }

    IEnumerator RotinaAtirar()
    {
        if (Time.time < proximoTiro)
        {
            yield break;
        }

        if (projetilPrefab == null)
        {
            Debug.LogWarning("Projétil não foi colocado no Jogador.");
            yield break;
        }

        proximoTiro = Time.time + tempoEntreTiros;

        bloqueadoAnimacao = true;

        TocarAnimacao("Bow_" + SufixoDirecao());

        yield return new WaitForSeconds(atrasoDisparo);

        Atirar();

        yield return new WaitForSeconds(tempoAnimAtaque);

        bloqueadoAnimacao = false;
    }

    void Atirar()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarSom(AudioManager.Instance.somTiro);
        }

        Vector3 origemTiro = transform.position;

        if (pontoTiro != null)
        {
            origemTiro = pontoTiro.position;
        }

        Vector3 posicaoTiro = origemTiro + (Vector3)(direcaoOlhar * 0.6f);

        GameObject novoProjetil = Instantiate(projetilPrefab, posicaoTiro, Quaternion.identity);

        Projetil projetil = novoProjetil.GetComponent<Projetil>();

        if (projetil != null)
        {
            projetil.Configurar(direcaoOlhar);
        }
    }

    public void ReceberDano(int dano)
    {
        if (estaInvencivel || morto)
        {
            return;
        }

        vidaAtual -= dano;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarSom(AudioManager.Instance.somDanoJogador);
        }

        if (HUDController.Instance != null)
        {
            HUDController.Instance.AtualizarVida(vidaAtual, vidaMaxima);
        }

        Debug.Log("Vida do jogador: " + vidaAtual);

        if (vidaAtual <= 0)
        {
            StartCoroutine(RotinaMorrer());
        }
        else
        {
            StartCoroutine(InvencibilidadeTemporaria());
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

    IEnumerator InvencibilidadeTemporaria()
    {
        estaInvencivel = true;

        yield return new WaitForSeconds(tempoInvencivel);

        estaInvencivel = false;
    }

    public void AdicionarEspecial(int quantidade)
    {
        especialAtual += quantidade;

        if (especialAtual > especialMaximo)
        {
            especialAtual = especialMaximo;
        }

        if (HUDController.Instance != null)
        {
            HUDController.Instance.AtualizarEspecial(especialAtual, especialMaximo);
        }

        Debug.Log("Especial: " + especialAtual + "/" + especialMaximo);

        if (especialAtual >= especialMaximo)
        {
            Debug.Log("Especial carregado! Aperte E para usar.");
        }
    }

    IEnumerator RotinaEspecial()
    {
        if (especialAtual < especialMaximo)
        {
            Debug.Log("Especial ainda não está carregado.");
            yield break;
        }

        Debug.Log("ESPECIAL ATIVADO!");

        bloqueadoAnimacao = true;

        TocarAnimacao("Bow_" + SufixoDirecao());

        UsarEspecial();

        yield return new WaitForSeconds(0.3f);

        bloqueadoAnimacao = false;
    }

    void UsarEspecial()
    {

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarSom(AudioManager.Instance.somEspecial);
        }

        if (efeitoEspecialPrefab != null)
        {
            Instantiate(efeitoEspecialPrefab, transform.position, Quaternion.identity);
        }

        Collider2D[] objetosAtingidos = Physics2D.OverlapCircleAll(transform.position, raioEspecial);

        foreach (Collider2D objeto in objetosAtingidos)
        {
            Inimigo inimigo = objeto.GetComponent<Inimigo>();

            if (inimigo != null)
            {
                inimigo.ReceberDano(danoEspecial);
            }

            BossFinal boss = objeto.GetComponent<BossFinal>();

            if (boss != null)
            {
                boss.ReceberDano(10);
            }
        }

        especialAtual = 0;

        if (HUDController.Instance != null)
        {
            HUDController.Instance.AtualizarEspecial(especialAtual, especialMaximo);
        }

        StartCoroutine(EfeitoEspecialVisual());
    }

    IEnumerator EfeitoEspecialVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
        }

        yield return new WaitForSeconds(0.25f);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = corOriginal;
        }
    }

    IEnumerator RotinaGolpear()
    {
        if (Time.time < proximoGolpe)
        {
            yield break;
        }

        proximoGolpe = Time.time + tempoEntreGolpes;

        bloqueadoAnimacao = true;

        TocarAnimacao("Sword_" + SufixoDirecao());

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarSom(AudioManager.Instance.somEspada);
        }

        yield return new WaitForSeconds(0.12f);

        GolpearComEspada();

        yield return new WaitForSeconds(tempoAnimEspada);

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

        Debug.Log("GAME OVER");

        if (HUDController.Instance != null)
        {
            HUDController.Instance.MostrarDerrota();
        }

        gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioEspecial);

        Gizmos.color = Color.red;

        Vector2 dir = direcaoOlhar;

        if (dir == Vector2.zero)
        {
            dir = Vector2.down;
        }

        Vector2 centroAtaque = (Vector2)transform.position + dir.normalized * alcanceEspada;

        Gizmos.DrawWireCube(centroAtaque, tamanhoAreaEspada);
    }
}