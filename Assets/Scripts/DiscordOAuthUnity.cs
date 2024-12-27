using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;

public class DiscordOAuthUnity : MonoBehaviour
{
    [SerializeField]
    TMP_Text txt_debug;

    [SerializeField]
    TMP_Text txt_good;
    [SerializeField]
    TMP_Text txt_sub;


    private const string CLIENT_ID = "1319574653697658890";
    private const string CLIENT_SECRET = "GalvUlVOeL4Bw0zYLTIT9MLSmAL6HQ5P";
    private const string REDIRECT_URI = "https://jhuijung.github.io/UnityToyproject_GDG_HostRepo/";

    void Start()
    {
        
    }

    public void BTNS()
    {
        string url = Application.absoluteURL;
        string code = GetCodeFromURL(url);

        if (!string.IsNullOrEmpty(code))
        {
            Debug.Log("Authorization Code: " + code);
            txt_good.text = "Authorization Code: " + code;
            StartCoroutine(RequestAccessToken(code));
        }
        else
        {
            Debug.Log("No Authorization Code found in URL.");
            txt_debug.text = "No Authorization Code found in URL.";
        }
    }

    string GetCodeFromURL(string url)
    {
        Uri uri = new Uri(url);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        return query.Get("code");
    }

    IEnumerator RequestAccessToken(string code)
    {
        string tokenUrl = "https://discord.com/api/oauth2/token";
        WWWForm form = new WWWForm();
        form.AddField("client_id", CLIENT_ID);
        form.AddField("client_secret", CLIENT_SECRET);
        form.AddField("grant_type", "authorization_code");
        form.AddField("code", code);
        form.AddField("redirect_uri", REDIRECT_URI);

        using (UnityWebRequest www = UnityWebRequest.Post(tokenUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Access Token Response: " + www.downloadHandler.text);
                txt_sub.text = www.downloadHandler.text;
                string accessToken = ExtractAccessToken(www.downloadHandler.text);
                StartCoroutine(RequestUserInfo(accessToken));
            }
            else
            {
                Debug.LogError("Token Request Error: " + www.error);
            }
        }
    }

    string ExtractAccessToken(string jsonResponse)
    {
        // 간단한 JSON 파싱 (Unity에서는 JsonUtility를 사용 가능)
        var response = JsonUtility.FromJson<DiscordTokenResponse>(jsonResponse);
        return response.access_token;
    }

    IEnumerator RequestUserInfo(string accessToken)
    {
        string userInfoUrl = "https://discord.com/api/users/@me";

        UnityWebRequest www = UnityWebRequest.Get(userInfoUrl);
        www.SetRequestHeader("Authorization", "Bearer " + accessToken);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("User Info: " + www.downloadHandler.text);
            txt_good.text = www.downloadHandler.text;
        }
        else
        {
            Debug.LogError("User Info Request Error: " + www.error);
        }
    }
}

[Serializable]
public class DiscordTokenResponse
{
    public string access_token;
    public string token_type;
    public string expires_in;
    public string refresh_token;
    public string scope;

    // 사용자 정보 필드 추가
    public string username;
    public string id;
}


