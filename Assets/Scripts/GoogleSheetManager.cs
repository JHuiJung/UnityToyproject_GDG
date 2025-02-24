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
    public string order, result, msg, _id,_name,_score,_height,_maxheight;
    public string holdingcake;
    public string isground;
    public string cakenumber;
    public string position;
    public string rotation;

    public string _r1name, _r1height;
    public string _r2name, _r2height;
    public string _r3name, _r3height;
    public string _r4name, _r4height;
    public string _r5name, _r5height;
}

public class GoogleSheetManager : MonoBehaviour
{


    [SerializeField]
    TMP_Text text_reponse;

    [SerializeField]
    TMP_Text text_Debug;

    [SerializeField]
    GameObject panel_Error;

    [SerializeField]
    GameObject panel_Loading;

    const string URL = "https://script.google.com/macros/s/AKfycbziS_bkMyK_13Bx64g5fP44dIQn3kPTreQYNB-OgtTIBNV-ukvlPSnef7pbmia9R5XD/exec";
    public string sheetData = "";
    public GoogleData GD;


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

        //--- 이전 ----
        ////데이터 가져오기
        //string data = www.downloadHandler.text;

        //text.text = GetDataById(data, DiscordManager.Inst.userId);

        ////랭킹 업데이트
        //UIManager.Inst.UpdateRanking(GetTopMaxHeightPlayers(data, 5));


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
        form.AddField("json", "None");

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

    [ContextMenu("Save")]
    public void Save()
    {
        StartCoroutine(CoSave());
    }

    public IEnumerator CoSave()
    {
        string id = DiscordManager.Inst.userId;
        //string cakeJson = "None";
        List<string> string_list = new List<string>();

        string_list = DataManager.Inst.GetSaveData();

        //foreach (string s in string_list)
        //{
        //    print($"*** : {s}");
        //}

        WWWForm form = new WWWForm();
        form.AddField("order", "save");
        form.AddField("id", id);
        form.AddField("score", UIManager.Inst.tokenAmount);
        form.AddField("height", UIManager.Inst.currentHeight.ToString());
        form.AddField("maxheight", UIManager.Inst.peakHeight.ToString());
        form.AddField("holdingcake", string_list[0]);
        form.AddField("isground", string_list[1]);
        form.AddField("cakenumber", string_list[2]);
        form.AddField("position", string_list[3]);
        form.AddField("rotation", string_list[4]);

        yield return StartCoroutine(Post(form));
    }

    [ContextMenu("GetValue")]
    public void GetValue()
    {
        StartCoroutine(CoGetValue());
    }

    public IEnumerator CoGetValue()
    {
        //print(222);
        string id = DiscordManager.Inst.userId;
        WWWForm form = new WWWForm();
        form.AddField("order", "getValue");
        form.AddField("id", id);

        yield return StartCoroutine(Post(form));
    }

    IEnumerator Post(WWWForm form)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.isDone) Response(www.downloadHandler.text);
            else print("웹 응답 없음");
        }
    }

    void Response(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        GD = JsonUtility.FromJson<GoogleData>(json);

        if(GD.result == "ERROR")
        {
            print(GD.order + " 을 실행할 수 없습니다. 에러 메세지 : " + GD.msg);
            //text_reponse.text = GD.order + " 을 실행할 수 없습니다. 에러 메세지 : " + GD.msg;
            return;
        }

        if (GD.result == "OK")
        {
            print(GD.order + " 을 실행했습니다. 메세지 : " + GD.msg);
            //text_reponse.text = GD.order + " 을 실행했습니다. 메세지 : " + GD.msg;
        }

        // 로그인 정보 없음 -> 회원가입
        if(GD.order == "login" && GD.result == "NOLOGINDATA")
        {
            print(GD.order + " 을 실행했습니다. 메세지 : 회원가입해야함 ");
            //text_reponse.text = GD.order + " 을 실행했습니다. 메세지 : 회원가입해야함 ";
            //Register();

            panel_Error.SetActive(true);
            panel_Loading.SetActive(false);
            return;
        }

        if(GD.order == "login" && GD.result == "OK")
        {
            print(GD.order + " 을 실행했습니다. 메세지 : " + GD.msg);
            //text_reponse.text = GD.order + " 을 실행했습니다. 메세지 : " + GD.msg;
            SceneManager.LoadScene("GameScene");
        }

        if(GD.order == "register")
        {
            Login();
        }

        if(GD.order == "getValue" && GD.result == "OK")
        {
            //sheetData = GD._id + GD._name + GD._score+GD._height + GD._maxheight + GD._json+ GD._r1height + GD._r1name
            //    + GD._r2height + GD._r2name + GD._r3height + GD._r3name + GD._r4height + GD._r4name + GD._r5height + GD._r5name;
            //print("Sheet Data : " + sheetData);
            //text_reponse.text = "Sheet Data : " + sheetData;

            UIManager.Inst.preTokenAmount = int.Parse( GD._score );

            List<(string Name, string MaxHeight)> _rankList = new List<(string Name, string MaxHeight)>();

            _rankList.Add((GD._r1name, GD._r1height));
            _rankList.Add((GD._r2name, GD._r2height));
            _rankList.Add((GD._r3name, GD._r3height));
            _rankList.Add((GD._r4name, GD._r4height));
            _rankList.Add((GD._r5name, GD._r5height));

            UIManager.Inst.UpdateRanking(_rankList);
            //text_Debug.text = GD.value;
        }
    }


    private void OnApplicationQuit()
    {

        //InfoUpdate();

        string id = DiscordManager.Inst.userId;
        //string cakeJson = "None";
        List<string> string_list = new List<string>();

        string_list = DataManager.Inst.GetSaveData();

        WWWForm form = new WWWForm();
        form.AddField("order", "save");
        form.AddField("id", id);
        form.AddField("score", UIManager.Inst.tokenAmount);
        form.AddField("height", UIManager.Inst.currentHeight.ToString());
        form.AddField("maxheight", UIManager.Inst.peakHeight.ToString());
        form.AddField("holdingcake", string_list[0]);
        form.AddField("isground", string_list[1]);
        form.AddField("cakenumber", string_list[2]);
        form.AddField("position", string_list[3]);
        form.AddField("rotation", string_list[4]);

        StartCoroutine(Post(form));
    }

    public void InfoUpdate()
    {
        StartCoroutine(CoInfoUpdate());
    }

    public IEnumerator CoInfoUpdate()
    {
        print("1");
        int PreToken = 0;

        PreToken = UIManager.Inst.preTokenAmount;
        int nowToken = UIManager.Inst.tokenAmount;

        int usedToken = PreToken - nowToken > 0 ? PreToken - nowToken : 0;


        yield return StartCoroutine(GoogleSheetManager.Inst.CoGetValue());


        print("2");
        int Aftertoken = int.Parse(GoogleSheetManager.Inst.GD._score);
        int finaltoken = Aftertoken - usedToken > 0 ? Aftertoken - usedToken : 0;
        UIManager.Inst.UpdateToken(finaltoken);
        UIManager.Inst.preTokenAmount = finaltoken;

        yield return StartCoroutine(GoogleSheetManager.Inst.CoSave());


        print("3");
    }

}

