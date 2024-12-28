using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using System.Linq;

public class GoogleSheetManager : MonoBehaviour
{


    [SerializeField]
    TMP_Text text;
    
    const string URL = "https://docs.google.com/spreadsheets/d/1nkbak1ng5qjt4n6Hf9cYUO45CTvvzKRDOmRSK1WRAwk/export?format=tsv";

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
        UpdateGameDataBySheet();
    }

    public void UpdateGameDataBySheet()
    {
        StartCoroutine(StartUpdate());
    }

    private IEnumerator StartUpdate()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        //������ ��������
        string data = www.downloadHandler.text;

        text.text = GetDataById(data, DiscordManager.Inst.userId);

        //��ŷ ������Ʈ
        UIManager.Inst.UpdateRanking(GetTopMaxHeightPlayers(data, 5));


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
}

