using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class JsonData
{
    public int HoldingCakeNumber = -1;
    public List<CakeJsonInfo> CakeJsonList = new List<CakeJsonInfo>();
}

[System.Serializable]
public class CakeJsonInfo
{
    public int cakeNumber = 0;
    public Vector3 cakePosition = Vector3.zero;
    public Quaternion cakeRotation = Quaternion.identity;
    public bool isGround = false;
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

    [ContextMenu("SaveData")]
    public void SaveData()
    {
        //현재 로그인한 사람의 행을 기준으로 저장
        // 최대 높이, 현재 높이, 토큰, json을 저장함

        //json 저장

        JsonData _saveData = new JsonData();

        // 씬에 존재하는 Cake 컴포넌트를 가진 모든 오브젝트를 찾음
        Cake[] allCakes = FindObjectsOfType<Cake>();

        // Cake를 가진 오브젝트가 하나라도 있는 경우
        if (allCakes.Length > 0)
        {


            foreach (Cake cake in allCakes)
            {
                if (!cake.isGound)
                    continue;

                CakeJsonInfo _jsonInfo = new CakeJsonInfo();

                _jsonInfo.cakeNumber = cake.cakeNumber;
                _jsonInfo.cakePosition = cake.transform.position;
                _jsonInfo.cakeRotation = cake.transform.rotation;
                _jsonInfo.isGround = cake.isGound;

                _saveData.CakeJsonList.Add(_jsonInfo);
            }

            _saveData.CakeJsonList.Sort((a, b) => a.cakePosition.y.CompareTo(b.cakePosition.y));
        }
        else
        {
            Debug.Log("Cake 스크립트를 가진 오브젝트가 존재하지 않습니다.");
        }
        

        if(PlayerManager.Inst.HoldingCake !=null)
        {
            _saveData.HoldingCakeNumber = PlayerManager.Inst.HoldingCake.GetComponent<Cake>().cakeNumber;
        }

        string json = JsonUtility.ToJson(_saveData, true);
        string path = Path.Combine(Application.dataPath, "database.json");

        File.WriteAllText(path, json);

        // 최대 높이, 현재 높이, 토큰 저장
    }

    [ContextMenu("LoadData")]
    public void LoadData()
    {
        string path = Path.Combine(Application.dataPath, "database.json");

        // 파일이 존재하는지 확인
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            // JSON 데이터를 역직렬화
            JsonData loadedData = JsonUtility.FromJson<JsonData>(json);

            print(loadedData.HoldingCakeNumber);
            // HoldingCake 복원
            if ( loadedData.HoldingCakeNumber != -1)
            {
                PlayerManager.Inst.Setup(loadedData.HoldingCakeNumber);
            }

            // 기존 씬에 있는 Cake 오브젝트를 정리 (필요에 따라 제거하는 코드 추가 가능)
            //Cake[] existingCakes = FindObjectsOfType<Cake>();
            //foreach (Cake cake in existingCakes)
            //{
            //    Destroy(cake.gameObject);
            //}

            // CakeJsonList를 기반으로 새로운 Cake 오브젝트 생성
            foreach (CakeJsonInfo cakeInfo in loadedData.CakeJsonList)
            {
                // 새로운 GameObject 생성
                GameObject cakeObject = Instantiate(CakeList.cakes[cakeInfo.cakeNumber].cakeObj);

                // Cake 스크립트 추가
                Cake cakeComponent = cakeObject.GetComponent<Cake>();

                // 속성 설정
                cakeComponent.cakeNumber = cakeInfo.cakeNumber;
                cakeComponent.isGound = cakeInfo.isGround;

                // 위치와 회전 설정
                cakeObject.transform.position = cakeInfo.cakePosition;
                cakeObject.transform.rotation = cakeInfo.cakeRotation;

                cakeComponent.Setup(cakeInfo.isGround);
                //Debug.Log($"Loaded Cake: {cakeInfo.cakeNumber} at {cakeInfo.cakePosition}");
            }

            Debug.Log("Data successfully loaded!");
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }
    }

}
