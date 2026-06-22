using UnityEngine;

public class TituloAnimado : MonoBehaviour
{
    public float amplitude = 8f;
    public float velocidade = 2f;

    private Vector3 posicaoInicial;

    void Start()
    {
        posicaoInicial = transform.localPosition;
    }

    void Update()
    {
        float movimento = Mathf.Sin(Time.time * velocidade) * amplitude;
        transform.localPosition = posicaoInicial + new Vector3(0f, movimento, 0f);
    }
}