using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;

public class DiscordManager : MonoBehaviour
{
    private string clientId = "1319574653697658890"; // Discord Ŭ���̾�Ʈ ID
    private string redirectUri = "https://jhuijung.github.io/UnityToyproject_GDG_HostRepo/"; // ���𷺼� URI
    private string authUrl;

    public TMP_Text Txt_Username;
    public TMP_Text Txt_JsonResponse;

    private string userName = "none";

    // �̱��� �ν��Ͻ�
    public static DiscordManager Inst { get; private set; }

    private void Awake()
    {
        // �̱��� �ʱ�ȭ
        if (Inst != null && Inst != this)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Discord OAuth2 ���� URL ����
        //authUrl = $"https://discord.com/api/oauth2/authorize?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=identify";
        //OnLoginButtonClicked();
    }

    public void BTNTEST()
    {
        print(Application.absoluteURL);
        Txt_Username.text = Application.absoluteURL;

        //if( tmp != "-1")
        //{
        //    StartCoroutine(test(tmp));
        //}
        //else
        //{
        //    Txt_JsonResponse.text = "URL Link No read";
        //}

        string queryKey = "code";
        string codeValue = GetQueryValue(queryKey);

        // �� ���
        if (!string.IsNullOrEmpty(codeValue))
        {
            Debug.Log($"Returned Code: {codeValue}");
            Txt_JsonResponse.text = codeValue;
        }
        else
        {
            Debug.LogError("Code value not found in URL.");
        }
    }

    string GetQueryValue(string key)
    {
        string url = Application.absoluteURL;

        if (string.IsNullOrEmpty(url) || !url.Contains("?"))
        {
            Debug.LogError("Invalid URL or no query string.");
            return null;
        }

        try
        {
            int queryStartIndex = url.IndexOf('?');
            string queryString = url.Substring(queryStartIndex + 1);

            string[] parameters = queryString.Split('&');
            foreach (string param in parameters)
            {
                string[] keyValue = param.Split('=');
                if (keyValue.Length == 2 && keyValue[0] == key)
                {
                    return keyValue[1];
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error parsing URL: {ex.Message}");
        }

        return null;
    }


}
