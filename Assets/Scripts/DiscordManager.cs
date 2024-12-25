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
                // 서버에서 받은 사용자 정보 처리
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

    // 버튼 클릭 시 호출
    public void OnLoginButtonClicked()
    {
        // 사용자를 Discord 인증 페이지로 리디렉션
        //Application.OpenURL(authUrl);
    }

    // 인증 완료 후 서버로부터 사용자 정보 요청
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
                // "code=" 뒤의 값을 추출합니다.
                string[] queryParams = query.Split('&');
                foreach (string param in queryParams)
                {
                    if (param.StartsWith("code="))
                    {
                        // "code=" 뒤의 값을 반환합니다.
                        return param.Substring("code=".Length);
                    }
                }
            }

            return "-1"; // "code=" 파라미터가 없을 경우 기본 메시지
        }
        catch (UriFormatException ex)
        {
            Debug.LogError("Invalid URL format: " + ex.Message);
            return "-1"; // 예외가 발생한 경우 처리
        }
    }

    private IEnumerator GetUserInfoCoroutine(string code)
    {
        // 서버에서 사용자 정보를 요청
        string serverUrl = $"https://yourserver.com/callback?code={code}";
        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 서버에서 받은 사용자 정보 처리
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
