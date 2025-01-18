using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class JsonData
{
    public int HoldingCakeNumber = -1;
    public List<CakeJsonInfo> CakeJsonList = new List<CakeJsonInfo>();
}

[System.Serializable]
public class CakeJsonInfo
{
    public int N = 0;
    //public Vector3 P = Vector3.zero;
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;

    //public float q = 0f;
    //public float w = 0f;
    public float e = 0f;
    public float r = 0f;

    //public Quaternion R = Quaternion.identity;
    public bool G = false;
}

public class DataManager : MonoBehaviour
{

    public static DataManager Inst { get; private set; }

    public CakeList CakeList;

    
    string SheetData = "";
    [SerializeField]
    int rowNumber = 0;

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

    string[] GetStringList(string str)
    {
        string[] parts = str.Split('^');
        return parts;
    }

    [ContextMenu("SaveData")]
    public List<string> GetSaveData()
    {
        List<string> string_list = new List<string>();

        JsonData _saveData = new JsonData();

        string holdingCake = "None";
        string isGround = "None";
        string CakeNumber = "None";
        string Position = "None";
        string Rotation = "None";

        //씬에 존재하는 Cake 컴포넌트를 가진 모든 오브젝트를 찾음
        Cake[] allCakes = FindObjectsOfType<Cake>();

        // Cake를 가진 오브젝트가 하나라도 있는 경우
        if (allCakes.Length > 0)
        {
            CakeNumber = "";
            isGround = "";
            Position = "";
            Rotation = "";

            foreach (Cake cake in allCakes)
            {
                if (!cake.isGound)
                    continue;

                CakeJsonInfo _jsonInfo = new CakeJsonInfo();

                float px = (float)Math.Round(cake.transform.position.x, 4);
                float py = (float)Math.Round(cake.transform.position.y, 4);
                float pz = (float)Math.Round(cake.transform.position.z, 4);

                Vector3 pos = new Vector3(px, py, pz);

                float rx = (float)Math.Round(cake.transform.rotation.x, 4);
                float ry = (float)Math.Round(cake.transform.rotation.y, 4);
                float rz = (float)Math.Round(cake.transform.rotation.z, 4);
                float rw = (float)Math.Round(cake.transform.rotation.w, 4);

                Quaternion rot = new Quaternion(rx, ry, rz, rw);
                //print(px);
                //print(rz);
                //_jsonInfo.N = cake.cakeNumber;
                CakeNumber += cake.cakeNumber.ToString();

                isGround += cake.isGound ? "1" : "0";
                //_jsonInfo.G = cake.isGound;

                //_jsonInfo.x = px;
                //_jsonInfo.y = py;
                //_jsonInfo.z = pz;

                Position += $"{px}_{py}_{pz}";

                //_jsonInfo.q = rx;
                //_jsonInfo.w = ry;
                //_jsonInfo.e = rz;
                //_jsonInfo.r = rw;

                Rotation += $"{rz}_{rw}";

                //_jsonInfo.P = pos;
                //_jsonInfo.R = rot;
                //

                CakeNumber += "^";
                Position += "^";
                Rotation += "^";
                isGround += "^";

                //_saveData.CakeJsonList.Add(_jsonInfo);
            }

            //_saveData.CakeJsonList.Sort((a, b) => a.y.CompareTo(b.y));
        }
        else
        {
            Debug.Log("Cake 스크립트를 가진 오브젝트가 존재하지 않습니다.");
        }


        if (PlayerManager.Inst.HoldingCake != null)
        {
            holdingCake = PlayerManager.Inst.HoldingCake.GetComponent<Cake>().cakeNumber.ToString();
        }


        string_list.Add(holdingCake);
        string_list.Add(isGround);
        string_list.Add(CakeNumber);
        string_list.Add(Position);
        string_list.Add(Rotation);


        return string_list;
    }

    //[ContextMenu("SaveData")]
    //public string GetSaveData()
    //{
    //    //현재 로그인한 사람의 행을 기준으로 저장
    //    // 최대 높이, 현재 높이, 토큰, json을 저장함

    //    //json 저장

    //    JsonData _saveData = new JsonData();



    //    // 씬에 존재하는 Cake 컴포넌트를 가진 모든 오브젝트를 찾음
    //    Cake[] allCakes = FindObjectsOfType<Cake>();

    //    // Cake를 가진 오브젝트가 하나라도 있는 경우
    //    if (allCakes.Length > 0)
    //    {


    //        foreach (Cake cake in allCakes)
    //        {
    //            if (!cake.isGound)
    //                continue;

    //            CakeJsonInfo _jsonInfo = new CakeJsonInfo();

    //            float px = (float) Math.Round(cake.transform.position.x, 4);
    //            float py = (float)Math.Round(cake.transform.position.y, 4);
    //            float pz = (float)Math.Round(cake.transform.position.z, 4);

    //            Vector3 pos = new Vector3(px,py,pz);

    //            float rx = (float)Math.Round(cake.transform.rotation.x, 4);
    //            float ry = (float)Math.Round(cake.transform.rotation.y, 4);
    //            float rz = (float)Math.Round(cake.transform.rotation.z, 4);
    //            float rw = (float)Math.Round(cake.transform.rotation.w, 4);

    //            Quaternion rot = new Quaternion(rx, ry, rz, rw);
    //            //print(px);
    //            //print(rz);
    //            _jsonInfo.N = cake.cakeNumber;
    //            _jsonInfo.G = cake.isGound;

    //            _jsonInfo.x = px;
    //            _jsonInfo.y = py;
    //            _jsonInfo.z = pz;

    //            //_jsonInfo.q = rx;
    //            //_jsonInfo.w = ry;
    //            _jsonInfo.e = rz;
    //            _jsonInfo.r = rw;

    //            //_jsonInfo.P = pos;
    //            //_jsonInfo.R = rot;
    //            //

    //            _saveData.CakeJsonList.Add(_jsonInfo);
    //        }

    //        _saveData.CakeJsonList.Sort((a, b) => a.y.CompareTo(b.y));
    //    }
    //    else
    //    {
    //        Debug.Log("Cake 스크립트를 가진 오브젝트가 존재하지 않습니다.");
    //    }


    //    if(PlayerManager.Inst.HoldingCake !=null)
    //    {
    //        _saveData.HoldingCakeNumber = PlayerManager.Inst.HoldingCake.GetComponent<Cake>().cakeNumber;
    //    }

    //    string json = JsonUtility.ToJson(_saveData, true);
    //    return json;
    //}
    [ContextMenu("LoadData")]
    public void LoadData(List<string> strList)
    {

        string holdingcake = strList[0];
        string isground = strList[1];
        string cakenumber = strList[2];
        string position = strList[3];
        string rotation = strList[4];

        string[] G = GetStringList(isground);
        string[] N = GetStringList(cakenumber);
        string[] P = GetStringList(position);
        string[] R = GetStringList(rotation);

        // holdingcake
        if (holdingcake != "None")
        {
            PlayerManager.Inst.Setup(int.Parse(holdingcake));
        }

        //print(G[0]);
        //print(int.Parse(N[0]));
        //print(P[0]);
        //print(R[0]);
        for (int i = 0; i < N.Length-1; i++)
        {
            bool g = G[i] == "1";
            int n = int.Parse(N[i]);

            string[] p = P[i].Split('_');
            string[] r = R[i].Split('_');

            float x = float.Parse(p[0]);
            float y = float.Parse(p[1]);
            float z = float.Parse(p[2]);
            float e = float.Parse(r[0]);
            float _r = float.Parse(r[1]); ;

            GameObject cakeObject = Instantiate(CakeList.cakes[n].cakeObj);
            Cake cakeComponent = cakeObject.GetComponent<Cake>();

            cakeComponent.cakeNumber = n;
            cakeComponent.isGound = g;

            Vector3 pos = new Vector3(x, y, z);
            Quaternion rot = new Quaternion(0f, 0f, e, _r);

            cakeObject.transform.position = pos;
            cakeObject.transform.rotation = rot;

            cakeComponent.Setup(g);
        }

        Debug.Log("Data successfully loaded!");
    }


    //[ContextMenu("LoadData")]
    //public void LoadData(string _json)
    //{
    //    JsonData loadedData = JsonUtility.FromJson<JsonData>(_json);

    //    //print(loadedData.HoldingCakeNumber);
    //    // HoldingCake 복원
    //    if (loadedData.HoldingCakeNumber != -1)
    //    {
    //        PlayerManager.Inst.Setup(loadedData.HoldingCakeNumber);
    //    }

    //    // 기존 씬에 있는 Cake 오브젝트를 정리 (필요에 따라 제거하는 코드 추가 가능)
    //    //Cake[] existingCakes = FindObjectsOfType<Cake>();
    //    //foreach (Cake cake in existingCakes)
    //    //{
    //    //    Destroy(cake.gameObject);
    //    //}

    //    // CakeJsonList를 기반으로 새로운 Cake 오브젝트 생성
    //    foreach (CakeJsonInfo cakeInfo in loadedData.CakeJsonList)
    //    {
    //        // 새로운 GameObject 생성
    //        GameObject cakeObject = Instantiate(CakeList.cakes[cakeInfo.N].cakeObj);

    //        // Cake 스크립트 추가
    //        Cake cakeComponent = cakeObject.GetComponent<Cake>();

    //        // 속성 설정
    //        cakeComponent.cakeNumber = cakeInfo.N;
    //        cakeComponent.isGound = cakeInfo.G;

    //        Vector3 pos = new Vector3(cakeInfo.x, cakeInfo.y, cakeInfo.z);
    //        //Quaternion rot = new Quaternion(cakeInfo.q, cakeInfo.w, cakeInfo.e, cakeInfo.r);
    //        Quaternion rot = new Quaternion(0f, 0f, cakeInfo.e, cakeInfo.r);

    //        // 위치와 회전 설정
    //        cakeObject.transform.position = pos;
    //        cakeObject.transform.rotation = rot;

    //        cakeComponent.Setup(cakeInfo.G);
    //        //Debug.Log($"Loaded Cake: {cakeInfo.cakeNumber} at {cakeInfo.cakePosition}");
    //    }

    //    Debug.Log("Data successfully loaded!");
    //}

}
