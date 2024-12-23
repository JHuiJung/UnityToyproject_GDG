using System.Linq;
using UnityEngine;
using DarkTonic.MasterAudio;

public enum CameraType
{
    Drop,
    Search
}


public class GameManager : MonoBehaviour
{
    [SerializeField,Header("Cameras")]
    Camera mainCamera;

    [SerializeField]
    Camera searchCamera;

    [SerializeField]
    float searchCameraSpeed = 20f;

    [SerializeField]
    CameraType _cameraType = CameraType.Drop;
    public CameraType cameraType { get { return _cameraType; } private set { _cameraType = value; } }

    [SerializeField]
    Vector2 _minMax_CameraY = Vector2.zero;
    public Vector2 minMax_CameraY { get { return _minMax_CameraY; } private set { _minMax_CameraY = value; } }

    // �̱��� �ν��Ͻ�
    public static GameManager Inst { get; private set; }

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

    void Start()
    {
        MasterAudio.PlaySound("BGM_Game");
    }

    [ContextMenu("GetHighestCakeHeight")]
    public float GetHighestCakeHeight()
    {
        // 1. Cake ���̾��� �ε����� �����ɴϴ�.
        int cakeLayer = LayerMask.NameToLayer("Cake");

        // 2. ��� Collider2D�� �����ɴϴ�.
        Collider2D[] colliders = FindObjectsOfType<Collider2D>();

        // 3. Cake ���̾��� ������Ʈ�� ���͸��մϴ�.
        var cakeObjects = colliders.Where(c => c.gameObject.layer == cakeLayer);

        if (!cakeObjects.Any())
        {
            //Debug.Log("Cake ���̾��� ������Ʈ�� �����ϴ�.");
            return -1;
        }

        // 4. ���� ���� Collider2D�� ã���ϴ�.
        Collider2D highestCollider = null;
        float highestPoint = float.MinValue;

        foreach (var collider in cakeObjects)
        {
            Cake cake = collider.gameObject.GetComponent<Cake>();

            if (cake == null)
                continue;

            if (!cake.isGound)
                continue;

            // Collider2D�� Bound�� ���� ���� �� ���
            float topY = collider.bounds.max.y;

            if (topY > highestPoint)
            {
                highestPoint = topY;
                highestCollider = collider;
            }
        }

        // 5. ��� ���
        if (highestCollider != null)
        {
            //Debug.Log($"���� ���� Cake ������Ʈ: {highestCollider.gameObject.name}, ����: {highestPoint}");
            return highestPoint;
        }

        return -1;
    }

    public void UpdateCameraPosition(float peakCakeHight)
    {
        if (peakCakeHight < 4.5f)
            return;

        mainCamera.transform.position = new Vector3(0f,peakCakeHight,-10f);
    }

    public void SwitchCamera(CameraType cameraType)
    {
        switch (cameraType)
        {
            case CameraType.Drop:
                this.cameraType = CameraType.Drop;
                searchCamera.gameObject.SetActive(false);
                searchCamera.transform.position = mainCamera.transform.position;
                mainCamera.gameObject.SetActive(true);
                break;
            case CameraType.Search:
                this.cameraType = CameraType.Search;
                searchCamera.gameObject.SetActive(true);
                searchCamera.transform.position = mainCamera.transform.position;
                mainCamera.gameObject.SetActive(false);
                break;
        }
    }

    public void SearchCameraMove(Vector3 moveVec)
    {
        Vector3 camPos = searchCamera.transform.position;

        //�̵� ���� 4.5 ~ �ִ� ����

        if (camPos.y + moveVec.y * searchCameraSpeed * Time.deltaTime < minMax_CameraY.x)
        {
            searchCamera.transform.position = Vector3.up * minMax_CameraY.x + Vector3.back * 10f;
            return;
        }
        if (camPos.y + moveVec.y * searchCameraSpeed * Time.deltaTime > minMax_CameraY.y)
        {
            searchCamera.transform.position = Vector3.up * minMax_CameraY.y + Vector3.back*10f;
            return;
        }

        searchCamera.transform.position += moveVec * searchCameraSpeed * Time.deltaTime;
    }
}
