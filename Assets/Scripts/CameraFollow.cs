using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Alvo da câmera")]
    public Transform alvo;

    [Header("Configuração")]
    public float suavizacao = 0.15f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Limites do mapa")]
    public bool usarLimites = false;
    public float limiteMinX = -10f;
    public float limiteMaxX = 10f;
    public float limiteMinY = -6f;
    public float limiteMaxY = 6f;

    private Vector3 velocidadeAtual;

    void LateUpdate()
    {
        if (alvo == null)
        {
            return;
        }

        Vector3 posicaoDesejada = alvo.position + offset;

        if (usarLimites)
        {
            posicaoDesejada.x = Mathf.Clamp(posicaoDesejada.x, limiteMinX, limiteMaxX);
            posicaoDesejada.y = Mathf.Clamp(posicaoDesejada.y, limiteMinY, limiteMaxY);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            posicaoDesejada,
            ref velocidadeAtual,
            suavizacao
        );
    }
}