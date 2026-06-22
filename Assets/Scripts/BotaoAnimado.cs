using UnityEngine;
using UnityEngine.EventSystems;

public class BotaoAnimado : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float escalaNormal = 1f;
    public float escalaSelecionado = 1.08f;
    public float velocidade = 10f;

    private Vector3 escalaAlvo;

    void Start()
    {
        escalaAlvo = Vector3.one * escalaNormal;
        transform.localScale = escalaAlvo;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            escalaAlvo,
            Time.unscaledDeltaTime * velocidade
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        escalaAlvo = Vector3.one * escalaSelecionado;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        escalaAlvo = Vector3.one * escalaNormal;
    }
}