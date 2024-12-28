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

        //데이터 가져오기
        string data = www.downloadHandler.text;

        text.text = GetDataById(data, DiscordManager.Inst.userId);

        //랭킹 업데이트
        UIManager.Inst.UpdateRanking(GetTopMaxHeightPlayers(data, 5));


        //print(data);
    }

    public static string GetDataById(string inputData, string targetId)
    {
        // 데이터를 줄 단위로 나눔
        string[] lines = inputData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // 첫 번째 줄은 헤더이므로 건너뜀
        for (int i = 1; i < lines.Length; i++)
        {
            // 각 줄의 데이터를 탭으로 나눔
            string[] columns = lines[i].Split(new[] { '\t' }, StringSplitOptions.None);

            if (columns.Length < 6) continue; // 데이터 형식이 올바르지 않은 경우 건너뜀

            string id = columns[0];
            string name = columns[1];
            string score = columns[2];
            string height = columns[3];
            string maxHeight = columns[4];
            string json = columns[5];

            // id가 일치하면 해당 데이터를 반환
            if (id == targetId)
            {
                return $"Name: {name}, Score: {score}, Height: {height}, MaxHeight: {maxHeight}, JSON: {json}";
            }
        }

        // 데이터를 찾지 못한 경우
        return "Data not found";
    }

    //가장 높은 5명 리스트 반환코드
    public List<(string Name, float MaxHeight)> GetTopMaxHeightPlayers(string data, int topCount)
    {
        // 데이터를 줄 단위로 나눔
        string[] lines = data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // 첫 줄(헤더)은 무시하고 나머지 데이터 처리
        List<(string Name, float MaxHeight)> players = new List<(string Name, float MaxHeight)>();
        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split('\t');
            if (columns.Length < 5) continue; // 유효하지 않은 데이터 건너뛰기

            string name = columns[1];
            if (float.TryParse(columns[4], out float maxHeight))
            {
                players.Add((name, maxHeight));
            }
        }

        // MaxHeight를 기준으로 내림차순 정렬 후 상위 topCount 추출
        return players.OrderByDescending(p => p.MaxHeight).Take(topCount).ToList();
    }
}

