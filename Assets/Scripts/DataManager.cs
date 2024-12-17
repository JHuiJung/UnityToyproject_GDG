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

    public float q = 0f;
    public float w = 0f;
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

                float px = (float) Math.Round(cake.transform.position.x, 4);
                float py = (float)Math.Round(cake.transform.position.y, 4);
                float pz = (float)Math.Round(cake.transform.position.z, 4);
                
                Vector3 pos = new Vector3(px,py,pz);

                float rx = (float)Math.Round(cake.transform.rotation.x, 4);
                float ry = (float)Math.Round(cake.transform.rotation.y, 4);
                float rz = (float)Math.Round(cake.transform.rotation.z, 4);
                float rw = (float)Math.Round(cake.transform.rotation.z, 4);

                Quaternion rot = new Quaternion(rx, ry, rz, rw);
                print(px);
                print(rz);
                _jsonInfo.N = cake.cakeNumber;
                _jsonInfo.G = cake.isGound;

                _jsonInfo.x = px;
                _jsonInfo.y = py;
                _jsonInfo.z = pz;

                _jsonInfo.q = rx;
                _jsonInfo.w = ry;
                _jsonInfo.e = rz;
                _jsonInfo.r = rw;

                //_jsonInfo.P = pos;
                //_jsonInfo.R = rot;
                //

                _saveData.CakeJsonList.Add(_jsonInfo);
            }

            _saveData.CakeJsonList.Sort((a, b) => a.y.CompareTo(b.y));
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
                GameObject cakeObject = Instantiate(CakeList.cakes[cakeInfo.N].cakeObj);

                // Cake ��ũ��Ʈ �߰�
                Cake cakeComponent = cakeObject.GetComponent<Cake>();

                // �Ӽ� ����
                cakeComponent.cakeNumber = cakeInfo.N;
                cakeComponent.isGound = cakeInfo.G;

                Vector3 pos = new Vector3(cakeInfo.x, cakeInfo.y, cakeInfo.z);
                Quaternion rot = new Quaternion(cakeInfo.q, cakeInfo.w, cakeInfo.e, cakeInfo.r);

                // ��ġ�� ȸ�� ����
                cakeObject.transform.position = pos;
                cakeObject.transform.rotation = rot;

                cakeComponent.Setup(cakeInfo.G);
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
