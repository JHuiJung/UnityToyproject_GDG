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
        // �̱��� �ʱ�ȭ
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
        //���� �α����� ����� ���� �������� ����
        // �ִ� ����, ���� ����, ��ū, json�� ������

        //json ����

        JsonData _saveData = new JsonData();

        // ���� �����ϴ� Cake ������Ʈ�� ���� ��� ������Ʈ�� ã��
        Cake[] allCakes = FindObjectsOfType<Cake>();

        // Cake�� ���� ������Ʈ�� �ϳ��� �ִ� ���
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
            Debug.Log("Cake ��ũ��Ʈ�� ���� ������Ʈ�� �������� �ʽ��ϴ�.");
        }
        

        if(PlayerManager.Inst.HoldingCake !=null)
        {
            _saveData.HoldingCakeNumber = PlayerManager.Inst.HoldingCake.GetComponent<Cake>().cakeNumber;
        }

        string json = JsonUtility.ToJson(_saveData, true);
        string path = Path.Combine(Application.dataPath, "database.json");

        File.WriteAllText(path, json);

        // �ִ� ����, ���� ����, ��ū ����
    }

    [ContextMenu("LoadData")]
    public void LoadData()
    {
        string path = Path.Combine(Application.dataPath, "database.json");

        // ������ �����ϴ��� Ȯ��
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            // JSON �����͸� ������ȭ
            JsonData loadedData = JsonUtility.FromJson<JsonData>(json);

            print(loadedData.HoldingCakeNumber);
            // HoldingCake ����
            if ( loadedData.HoldingCakeNumber != -1)
            {
                PlayerManager.Inst.Setup(loadedData.HoldingCakeNumber);
            }

            // ���� ���� �ִ� Cake ������Ʈ�� ���� (�ʿ信 ���� �����ϴ� �ڵ� �߰� ����)
            //Cake[] existingCakes = FindObjectsOfType<Cake>();
            //foreach (Cake cake in existingCakes)
            //{
            //    Destroy(cake.gameObject);
            //}

            // CakeJsonList�� ������� ���ο� Cake ������Ʈ ����
            foreach (CakeJsonInfo cakeInfo in loadedData.CakeJsonList)
            {
                // ���ο� GameObject ����
                GameObject cakeObject = Instantiate(CakeList.cakes[cakeInfo.cakeNumber].cakeObj);

                // Cake ��ũ��Ʈ �߰�
                Cake cakeComponent = cakeObject.GetComponent<Cake>();

                // �Ӽ� ����
                cakeComponent.cakeNumber = cakeInfo.cakeNumber;
                cakeComponent.isGound = cakeInfo.isGround;

                // ��ġ�� ȸ�� ����
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
