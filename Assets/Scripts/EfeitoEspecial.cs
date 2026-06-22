using System.Collections;
using UnityEngine;

public class EfeitoEspecial : MonoBehaviour
{
    public float escalaFinal = 6f;
    public float duracao = 0.35f;

    private SpriteRenderer spriteRenderer;
    private Vector3 escalaInicial;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        escalaInicial = transform.localScale;

        StartCoroutine(AnimarEfeito());
    }

    IEnumerator AnimarEfeito()
    {
        float tempo = 0f;

        Color corInicial = Color.white;

        if (spriteRenderer != null)
        {
            corInicial = spriteRenderer.color;
        }

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;

            float progresso = tempo / duracao;

            float escalaAtual = Mathf.Lerp(escalaInicial.x, escalaFinal, progresso);
            transform.localScale = new Vector3(escalaAtual, escalaAtual, 1f);

            if (spriteRenderer != null)
            {
                Color novaCor = corInicial;
                novaCor.a = Mathf.Lerp(corInicial.a, 0f, progresso);
                spriteRenderer.color = novaCor;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}