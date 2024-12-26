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
    private string clientId = "1319574653697658890"; // Discord 클라이언트 ID
    private string redirectUri = "https://jhuijung.github.io/UnityToyproject_GDG_HostRepo/"; // 리디렉션 URI
    private string authUrl;

    public TMP_Text Txt_Username;
    public TMP_Text Txt_JsonResponse;

    private string userName = "none";

    // 싱글톤 인스턴스
    public static DiscordManager Inst { get; private set; }

    private void Awake()
    {
        // 싱글톤 초기화
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
        // Discord OAuth2 인증 URL 생성
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

        // 값 출력
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
