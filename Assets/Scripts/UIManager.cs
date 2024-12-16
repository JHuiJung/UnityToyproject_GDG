using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    // ΩÃ±€≈Ê ¿ŒΩ∫≈œΩ∫
    public static UIManager Inst { get; private set; }

    private void Awake()
    {
        // ΩÃ±€≈Ê √ ±‚»≠
        if (Inst != null && Inst != this)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
        DontDestroyOnLoad(gameObject);
    }


    [SerializeField, Space(10), Header("Height Score Board Txt")]
    TMP_Text txt_CurrentHeight;
    [SerializeField]
    TMP_Text txt_PeakHeight;


    [Space(10), Header("Height Info")]
    public float currentHeight = 0f;
    public float peakHeight = 0f;

    [SerializeField, Space(10), Header("Mode")]
    GameObject Obj_DropMode;

    [SerializeField]
    GameObject Obj_SearchMode;

    [SerializeField, Space(10), Header("Token")]
    GameObject Icon_Token;

    [SerializeField, Space(10), Header("Minimap")]
    GameObject Area_MinimapCam;

    private void FixedUpdate()
    {
        UpdateHeight();
        MiniMapUI();
    }

    void MiniMapUI()
    {
        float normalized = Mathf.InverseLerp(GameManager.Inst.minMax_CameraY.x, GameManager.Inst.minMax_CameraY.y, Camera.main.transform.position.y);
        float normalizedValue = Mathf.Lerp(-180f, 180f, normalized);
        Area_MinimapCam.GetComponent<RectTransform>().anchoredPosition = Vector3.up * normalizedValue;
    }

    public void UpdateModeIcon()
    {
        switch (GameManager.Inst.cameraType)
        {
            case CameraType.Drop:
                Obj_DropMode.SetActive(true);
                Obj_SearchMode.SetActive(false);
                break;
            case CameraType.Search:
                Obj_DropMode.SetActive(false);
                Obj_SearchMode.SetActive(true);
                break;
        }
    }
    public void UpdateHeight()
    {
        float height = GameManager.Inst.GetHighestCakeHeight();

        //∞™ ∏¯√£¿∏∏È ∏Æ≈œ
        if (height == -1)
            return;

        if (peakHeight < height)
        {
            peakHeight = height;
            txt_PeakHeight.text = peakHeight.ToString("F2") + "m";
            GameManager.Inst.UpdateCameraPosition(peakHeight);
        }
            


        txt_CurrentHeight.text = height.ToString("F2") + "m";
    }

    public void UpdateToken()
    {
        Icon_Token.GetComponent<RectTransform>().DOPunchScale(new Vector3(0f,0.5f,0f), 0.5f).SetEase(Ease.InOutQuad);
    }
}
