using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialController : MonoBehaviour
{
    public List<GameObject> Panels;
    public TMP_Text txt_pagenumber;
    int currentIndex = 0;

    public void PageCurl_Right()
    {
        currentIndex = currentIndex + 1 < Panels.Count ? currentIndex + 1 : currentIndex;

        for (int i = 0; i < Panels.Count; i++)
        {
            if(i == currentIndex)
            {
                Panels[i].gameObject.SetActive(true);
            }
            else
            {
                Panels[i].gameObject.SetActive(false);

            }

        }
        UpdateText();
    }

    public void PageCurl_Left()
    {

        currentIndex = currentIndex - 1 >= 0 ? currentIndex - 1 : currentIndex;

        for (int i = 0; i < Panels.Count; i++)
        {
            if (i == currentIndex)
            {
                Panels[i].gameObject.SetActive(true);
            }
            else
            {
                Panels[i].gameObject.SetActive(false);

            }

        }

        UpdateText();
    }

    void UpdateText()
    {
        txt_pagenumber.text = (currentIndex+1).ToString() + " / 4";
    }
        

}
