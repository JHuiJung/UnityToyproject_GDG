using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class BTN_Animation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    GameObject targetObj;

    [SerializeField]
    Vector3 targetScale = Vector3.one;

    [SerializeField]
    Vector3 OriginPos = Vector3.one;

    [SerializeField]
    Vector3 MovePos = Vector3.one;

    bool isOpen = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetObj == null)
            return;

        targetObj.GetComponent<RectTransform>().DOScale(targetScale, 0.125f);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetObj == null)
            return;

        targetObj.GetComponent<RectTransform>().DOScale(Vector3.one, 0.125f);
    }

    public void Toggle()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        this.GetComponent<RectTransform>().DOAnchorPos(MovePos, 0.25f).SetEase(Ease.InOutQuad);
        isOpen = true;
    }

    public void Close()
    {
        isOpen = false;
        this.GetComponent<RectTransform>().DOAnchorPos(OriginPos, 0.25f).SetEase(Ease.InOutQuad);
    }
}
