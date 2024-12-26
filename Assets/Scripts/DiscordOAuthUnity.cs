using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class DiscordOAuthUnity : MonoBehaviour
{
    public TMP_Text txt; // 결과 표시용 텍스트 UI
    private string serverUrl = "http://localhost:3000/getUserInfo"; // Node.js 서버 주소

    public void FetchUserInfo()
    {
        string code = GetCode();

        if (!string.IsNullOrEmpty(code))
        {
            StartCoroutine(SendCodeToServer(code));
        }
        else
        {
            Debug.LogError("Code is null or empty. Cannot fetch user info.");
            txt.text = "Error: Code not found in URL.";
        }
    }

    private string GetCode()
    {
        string queryKey = "code";
        string codeValue = GetQueryValue(queryKey);

        if (!string.IsNullOrEmpty(codeValue))
        {
            Debug.Log($"Returned Code: {codeValue}");
            return codeValue;
        }
        else
        {
            Debug.LogError("Code value not found in URL.");
            return null;
        }
    }

    private string GetQueryValue(string key)
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

    private IEnumerator SendCodeToServer(string code)
    {
        // 서버로 보낼 데이터 준비
        string json = $"{{\"code\":\"{code}\"}}"; // 명시적 JSON 문자열 생성

        using (UnityWebRequest www = new UnityWebRequest(serverUrl, "POST"))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"User Info: {www.downloadHandler.text}");
                txt.text = www.downloadHandler.text; // 결과를 UI에 표시
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
                txt.text = "Error: Failed to fetch user info.";
            }
        }
    }
}
