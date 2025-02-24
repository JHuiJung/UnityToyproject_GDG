using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DarkTonic.MasterAudio;

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

    //[SerializeField, Space(10), Header("Mode")]
    //GameObject Obj_DropMode;

    //[SerializeField]
    //GameObject Obj_SearchMode;

    [SerializeField, Space(10), Header("Token")]
    GameObject Icon_Token;
    public TMP_Text txt_Token;
    public int preTokenAmount = 0;
    public int tokenAmount = 0;
    public Transform TF_Panel_ErrorToken_Parent;
    public GameObject Panel_ErrorToken;

    [SerializeField, Space(10), Header("Minimap")]
    GameObject Area_MinimapCam;

    [SerializeField, Space(10), Header("Sound")]
    Slider Slide_Sound;

    [SerializeField, Space(10), Header("Ranking")]
    List<TMP_Text> Txt_RankerNames;

    [SerializeField]
    List<TMP_Text> Txt_RankerHeight;

    [SerializeField, Space(10), Header("InfoReload")]
    GameObject Obj_infoReload;

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

    //public void UpdateModeIcon()
    //{
    //    switch (GameManager.Inst.cameraType)
    //    {
    //        case CameraType.Drop:
    //            Obj_DropMode.SetActive(true);
    //            Obj_SearchMode.SetActive(false);
    //            break;
    //        case CameraType.Search:
    //            Obj_DropMode.SetActive(false);
    //            Obj_SearchMode.SetActive(true);
    //            break;
    //    }
    //}
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
            //GameManager.Inst.UpdateCameraPosition(peakHeight);
        }

        //GameManager.Inst.UpdateCameraPosition(height);
        currentHeight = height;
        txt_CurrentHeight.text = height.ToString("F2") + "m";
    }

    public void UpdateToken(int _token)
    {
        tokenAmount = _token;
        txt_Token.text = tokenAmount.ToString();
        Icon_Token.GetComponent<RectTransform>().DOPunchScale(new Vector3(0f,0.5f,0f), 0.5f).SetEase(Ease.InOutQuad);
    }

    public void UpdateRanking(List<(string Name, string MaxHeight)> rankList)
    {
        print(rankList.Count);

        for (int i = 0; i < rankList.Count; i++) 
        {
            Txt_RankerNames[i].text = rankList[i].Name;

            if(rankList[i].MaxHeight.Length < 4)
            {
                Txt_RankerHeight[i].text = rankList[i].MaxHeight + "m";
            }
            else
            {
                Txt_RankerHeight[i].text = rankList[i].MaxHeight.Substring(0, 4) + "m";
            }
            
        }


    }

    public void UpdateSoundValue()
    {
        float value = Slide_Sound.value;

        MasterAudio.SetBusVolumeByName("SFX", value);
        MasterAudio.SetBusVolumeByName("BGM", value);
    }

    public void UIToggle_InfoReload()
    {
        Obj_infoReload.GetComponent<BTN_Animation>().Toggle();
        MasterAudio.PlaySound("button Click");
    }

    public void UISetUp(int _token, string maxheight)
    {
        tokenAmount = _token;
        peakHeight = float.Parse( maxheight);

        if(maxheight.Length < 4)
        {
            txt_PeakHeight.text = maxheight.ToString() + "m";
        }
        else
        {
            txt_PeakHeight.text = maxheight.Substring(0,4) + "m";
        }

        txt_Token.text = tokenAmount.ToString();
    }

    public void TokenError()
    {
        var np = Instantiate(Panel_ErrorToken);

        
        np.transform.SetParent(TF_Panel_ErrorToken_Parent);
        np.transform.SetAsLastSibling();

        RectTransform rt = np.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero + Vector2.down*50f;
        rt.DOAnchorPos(rt.anchoredPosition + Vector2.up * 50f, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            Destroy(np);
        });
    }
}
