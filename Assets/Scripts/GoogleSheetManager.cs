using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GoogleData
{
    public string order, result, msg, _id,_name,_score,_height,_maxheight,_json;
}

public class GoogleSheetManager : MonoBehaviour
{


    [SerializeField]
    TMP_Text text_reponse;

    [SerializeField]
    TMP_Text text_Debug;

    const string URL = "https://script.google.com/macros/s/AKfycbz1w85NkFyqWb9fUWHLLevrZBAT1lgRG8exwvwrpShXMfwtILMznRcwMhicrtAz3fnL/exec";
    public string sheetData = "";
    public GoogleData GD;


    public static GoogleSheetManager Inst { get; private set; }

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

    private void Start()
    {
        //UpdateGameDataBySheet();
    }

    public void UpdateGameDataBySheet()
    {
        StartCoroutine(StartUpdate());
    }

    private IEnumerator StartUpdate()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();


        string data = www.downloadHandler.text;
        print(data);

        //--- ���� ----
        ////������ ��������
        //string data = www.downloadHandler.text;

        //text.text = GetDataById(data, DiscordManager.Inst.userId);

        ////��ŷ ������Ʈ
        //UIManager.Inst.UpdateRanking(GetTopMaxHeightPlayers(data, 5));


        //print(data);
    }

    public static string GetDataById(string inputData, string targetId)
    {
        // �����͸� �� ������ ����
        string[] lines = inputData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // ù ��° ���� ����̹Ƿ� �ǳʶ�
        for (int i = 1; i < lines.Length; i++)
        {
            // �� ���� �����͸� ������ ����
            string[] columns = lines[i].Split(new[] { '\t' }, StringSplitOptions.None);

            if (columns.Length < 6) continue; // ������ ������ �ùٸ��� ���� ��� �ǳʶ�

            string id = columns[0];
            string name = columns[1];
            string score = columns[2];
            string height = columns[3];
            string maxHeight = columns[4];
            string json = columns[5];

            // id�� ��ġ�ϸ� �ش� �����͸� ��ȯ
            if (id == targetId)
            {
                return $"Name: {name}, Score: {score}, Height: {height}, MaxHeight: {maxHeight}, JSON: {json}";
            }
        }

        // �����͸� ã�� ���� ���
        return "Data not found";
    }

    //���� ���� 5�� ����Ʈ ��ȯ�ڵ�
    public List<(string Name, float MaxHeight)> GetTopMaxHeightPlayers(string data, int topCount)
    {
        // �����͸� �� ������ ����
        string[] lines = data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // ù ��(���)�� �����ϰ� ������ ������ ó��
        List<(string Name, float MaxHeight)> players = new List<(string Name, float MaxHeight)>();
        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split('\t');
            if (columns.Length < 5) continue; // ��ȿ���� ���� ������ �ǳʶٱ�

            string name = columns[1];
            if (float.TryParse(columns[4], out float maxHeight))
            {
                players.Add((name, maxHeight));
            }
        }

        // MaxHeight�� �������� �������� ���� �� ���� topCount ����
        return players.OrderByDescending(p => p.MaxHeight).Take(topCount).ToList();
    }

    [ContextMenu("register")]
    public void Register()
    {
        string id = DiscordManager.Inst.userId;
        string _name = DiscordManager.Inst.userName;

        WWWForm form = new WWWForm();
        form.AddField("order", "register");
        form.AddField("id", id);
        form.AddField("name", _name);
        form.AddField("score", 5);
        form.AddField("height", 0);
        form.AddField("maxheight", 0);
        form.AddField("json", "none");

        StartCoroutine(Post(form));
        
    }

    [ContextMenu("Login")]
    public void Login()
    {
        string id = DiscordManager.Inst.userId;

        WWWForm form = new WWWForm();
        form.AddField("order", "login");
        form.AddField("id", id);

        StartCoroutine(Post(form));
    }

    public void SetValue()
    {
        //WWWForm form = new WWWForm();
        //form.AddField("order", "getValue");

        //StartCoroutine(Post(form));
    }

    [ContextMenu("GetValue")]
    public void GetValue()
    {
        string id = DiscordManager.Inst.userId;
        WWWForm form = new WWWForm();
        form.AddField("order", "getValue");
        form.AddField("id", id);

        StartCoroutine(Post(form));
    }

    IEnumerator Post(WWWForm form)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.isDone) Response(www.downloadHandler.text);
            else print("�� ���� ����");
        }
    }

    void Response(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        GD = JsonUtility.FromJson<GoogleData>(json);

        if(GD.result == "ERROR")
        {
            print(GD.order + " �� ������ �� �����ϴ�. ���� �޼��� : " + GD.msg);
            text_reponse.text = GD.order + " �� ������ �� �����ϴ�. ���� �޼��� : " + GD.msg;
            return;
        }

        if (GD.result == "OK")
        {
            print(GD.order + " �� �����߽��ϴ�. �޼��� : " + GD.msg);
            text_reponse.text = GD.order + " �� �����߽��ϴ�. �޼��� : " + GD.msg;
        }

        // �α��� ���� ���� -> ȸ������
        if(GD.order == "login" && GD.result == "NOLOGINDATA")
        {
            print(GD.order + " �� �����߽��ϴ�. �޼��� : ȸ�������ؾ��� ");
            text_reponse.text = GD.order + " �� �����߽��ϴ�. �޼��� : ȸ�������ؾ��� ";
            Register();
            

            return;
        }

        if(GD.order == "login" && GD.result == "OK")
        {
            print(GD.order + " �� �����߽��ϴ�. �޼��� : " + GD.msg);
            text_reponse.text = GD.order + " �� �����߽��ϴ�. �޼��� : " + GD.msg;
            SceneManager.LoadScene("GameScene");
        }

        if(GD.order == "register")
        {
            Login();
        }

        if(GD.order == "getValue" && GD.result == "OK")
        {
            sheetData = GD._id + GD._name + GD._score+GD._height + GD._maxheight + GD._json;
            print("Sheet Data : " + sheetData);
            text_reponse.text = "Sheet Data : " + sheetData;

            //text_Debug.text = GD.value;
        }
    }


    private void OnApplicationQuit()
    {
        WWWForm form = new WWWForm();
        form.AddField("order", "logout");

        StartCoroutine (Post(form));
    }

}

