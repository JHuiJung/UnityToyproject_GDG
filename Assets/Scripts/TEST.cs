using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class TEST : MonoBehaviour
{
    [SerializeField]
    TMP_Text text;
    
    const string URL = "https://docs.google.com/spreadsheets/d/1nkbak1ng5qjt4n6Hf9cYUO45CTvvzKRDOmRSK1WRAwk/export?format=tsv";

    private IEnumerator Start()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;

        text.text = data;

        print(data);
    }
}
