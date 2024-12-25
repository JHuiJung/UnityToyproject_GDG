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
        authUrl = $"https://discord.com/api/oauth2/authorize?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=identify";
        //OnLoginButtonClicked();
    }

    public void BTNTEST()
    {
        print(Application.absoluteURL);
        Txt_Username.text = Application.absoluteURL;
        string tmp = ExtractCodeFromURL(Application.absoluteURL);

        if( tmp != "-1")
        {
            StartCoroutine(test(tmp));
        }
        else
        {
            Txt_JsonResponse.text = "URL Link No read";
        }
    }

    IEnumerator test(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // �������� ���� ����� ���� ó��
                string jsonResponse = request.downloadHandler.text;
                UserInfo userInfo = JsonUtility.FromJson<UserInfo>(jsonResponse);
                //Debug.Log("User info: " + userInfo.username);
                userName = userInfo.username;
                Txt_Username.text = userInfo.username;
                Txt_JsonResponse.text = jsonResponse;
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                Txt_Username.text = "Failed to fetch user info: " + request.error;
                //Debug.LogError("Failed to fetch user info: " + request.error);
            }
        }
    }

    // ��ư Ŭ�� �� ȣ��
    public void OnLoginButtonClicked()
    {
        // ����ڸ� Discord ���� �������� ���𷺼�
        //Application.OpenURL(authUrl);
    }

    // ���� �Ϸ� �� �����κ��� ����� ���� ��û
    public void GetUserInfoFromServer(string code)
    {
        StartCoroutine(GetUserInfoCoroutine(code));
    }

    private string ExtractCodeFromURL(string url)
    {
        try
        {
            Uri uri = new Uri(url);
            string query = uri.Query;

            if (!string.IsNullOrEmpty(query))
            {
                // "code=" ���� ���� �����մϴ�.
                string[] queryParams = query.Split('&');
                foreach (string param in queryParams)
                {
                    if (param.StartsWith("code="))
                    {
                        // "code=" ���� ���� ��ȯ�մϴ�.
                        return param.Substring("code=".Length);
                    }
                }
            }

            return "-1"; // "code=" �Ķ���Ͱ� ���� ��� �⺻ �޽���
        }
        catch (UriFormatException ex)
        {
            Debug.LogError("Invalid URL format: " + ex.Message);
            return "-1"; // ���ܰ� �߻��� ��� ó��
        }
    }

    private IEnumerator GetUserInfoCoroutine(string code)
    {
        // �������� ����� ������ ��û
        string serverUrl = $"https://yourserver.com/callback?code={code}";
        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // �������� ���� ����� ���� ó��
                string jsonResponse = request.downloadHandler.text;
                UserInfo userInfo = JsonUtility.FromJson<UserInfo>(jsonResponse);
                //Debug.Log("User info: " + userInfo.username);
                userName = userInfo.username;
                Txt_Username.text = userInfo.username;
                Txt_JsonResponse.text = jsonResponse;
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                Txt_Username.text = "Failed to fetch user info: " + request.error;
                //Debug.LogError("Failed to fetch user info: " + request.error);
            }
        }


    }

    [System.Serializable]
    public class UserInfo
    {
        public string username;
    }
}
