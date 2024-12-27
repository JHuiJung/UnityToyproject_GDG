using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;

public class DiscordOAuthUnity : MonoBehaviour
{
    [SerializeField] private TMP_Text txt_debug;
    [SerializeField] private TMP_Text txt_good;
    [SerializeField] private TMP_Text txt_sub;

    private const string CLIENT_ID = "1319574653697658890";
    private const string CLIENT_SECRET = "GalvUlVOeL4Bw0zYLTIT9MLSmAL6HQ5P";
    private const string REDIRECT_URI = "https://jhuijung.github.io/UnityToyproject_GDG_HostRepo/";

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

        // Custom query string parsing for Unity WebGL
        string query = uri.Query;
        if (!string.IsNullOrEmpty(query) && query.StartsWith("?"))
        {
            query = query.Substring(1); // Remove '?' at the start
            string[] pairs = query.Split('&');
            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split('=');
                if (keyValue.Length == 2 && keyValue[0] == "code")
                {
                    return keyValue[1];
                }
            }
        }
        return null; // No code found
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

                var tokenResponse = JsonUtility.FromJson<DiscordTokenResponse>(www.downloadHandler.text);
                StartCoroutine(RequestUserInfo(tokenResponse.access_token));
            }
            else
            {
                Debug.LogError("Token Request Error: " + www.error);
                txt_debug.text = "Token Request Error: " + www.error;
            }
        }
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

            var userInfo = JsonUtility.FromJson<DiscordUserInfo>(www.downloadHandler.text);
            txt_good.text = $"User: {userInfo.username} (ID: {userInfo.id})";
        }
        else
        {
            Debug.LogError("User Info Request Error: " + www.error);
            txt_debug.text = "User Info Request Error: " + www.error;
        }
    }
}

//[Serializable]
//public class DiscordTokenResponse
//{
//    public string access_token;
//    public string token_type;
//    public int expires_in;
//    public string refresh_token;
//    public string scope;
//}

//[Serializable]
//public class DiscordUserInfo
//{
//    public string id;          // 사용자 ID
//    public string username;    // 사용자 이름
//    public string discriminator; // 태그 번호
//    public string avatar;      // 아바타 정보
//}
