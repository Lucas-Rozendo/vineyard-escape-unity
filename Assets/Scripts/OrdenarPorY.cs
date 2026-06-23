using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OrdenarPorY : MonoBehaviour
{
    public int ordemBase = 5000;
    public int multiplicador = 100;
    public int ajuste = 0;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        spriteRenderer.sortingOrder = ordemBase + Mathf.RoundToInt(-transform.position.y * multiplicador) + ajuste;
    }
}