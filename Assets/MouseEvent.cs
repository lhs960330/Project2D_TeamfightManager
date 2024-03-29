using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject Champion_hover_red;
    [SerializeField] GameObject champion_slot_red;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (pickRutione == null)
            pickRutione = StartCoroutine(PickRutione());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (champion_slot_red.active == false)
        {
            animator.Play("Idle");
            Champion_hover_red.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (champion_slot_red.active == false)
        {
            animator.Play("Stop");
            Champion_hover_red.SetActive(false);
        }
    }
    Coroutine pickRutione;
    IEnumerator PickRutione()
    {
        animator.Play("BanPickAttack");
        champion_slot_red.SetActive(true);
        yield return null;
    }

    public void BanPickReset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
           transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
