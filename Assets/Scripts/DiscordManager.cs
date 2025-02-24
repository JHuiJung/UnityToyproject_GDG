using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.SceneManagement;


public class DiscordManager : MonoBehaviour
{
    public TMP_Text Txt_Username;
    //public TMP_Text Txt_JsonResponse;

    private string _userName = "none444";
    public string userName { get { return _userName; } private set { _userName = value; } }

    private string _userId = "1237";
    public string userId { get { return _userId; } private set { _userId = value; } }

    private const string CLIENT_ID = "";
    private const string CLIENT_SECRET = "";
    private const string REDIRECT_URI = "";

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

    private void Start()
    {
        BTNS();
    }

    public void BTNS()
    {
        string url = Application.absoluteURL;
        string code = GetCodeFromURL(url);

        if (!string.IsNullOrEmpty(code))
        {
            Debug.Log("Authorization Code: " + code);
            //txt_good.text = "Authorization Code: " + code;
            StartCoroutine(RequestAccessToken(code));
        }
        else
        {
            Debug.LogError("No Authorization Code found in URL.");
            //txt_debug.text = "No Authorization Code found in URL.";
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
                //txt_sub.text = www.downloadHandler.text;

                var tokenResponse = JsonUtility.FromJson<DiscordTokenResponse>(www.downloadHandler.text);
                StartCoroutine(RequestUserInfo(tokenResponse.access_token));
            }
            else
            {
                Debug.LogError("Token Request Error: " + www.error);
                //txt_debug.text = "Token Request Error: " + www.error;
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
            //txt_good.text = www.downloadHandler.text;

            var userInfo = JsonUtility.FromJson<DiscordUserInfo>(www.downloadHandler.text);

            userId = userInfo.id;
            userName = userInfo.username;
            //txt_good.text = $"User: {userInfo.username} (ID: {userInfo.id})";
            Txt_Username.text = userName;
            GoogleSheetManager.Inst.Login();
            
        }
        else
        {
            Debug.LogError("User Info Request Error: " + www.error);
            //txt_debug.text = "User Info Request Error: " + www.error;
        }
    }
}

[Serializable]
public class DiscordTokenResponse
{
    public string access_token;
    public string token_type;
    public int expires_in;
    public string refresh_token;
    public string scope;
}

[Serializable]
public class DiscordUserInfo
{
    public string id;          // 사용자 ID
    public string username;    // 사용자 이름
    public string discriminator; // 태그 번호
    public string avatar;      // 아바타 정보
}