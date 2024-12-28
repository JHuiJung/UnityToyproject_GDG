using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Inst { get; private set; }

    [SerializeField]
    GameObject cursorObj; // Ŀ�� ������Ʈ

    [SerializeField]
    float cursorSpeed = 8f; // Ŀ�� �ӵ�

    [SerializeField]
    float rotateSpeed = 60f;

    //[SerializeField]
    //Vector2 boundPos = Vector2.one; // Ŀ�� ���� ����

    //������ ����ũ
    [SerializeField]
    GameObject _HoldingCake;
    public GameObject HoldingCake { get { return _HoldingCake; } private set { _HoldingCake = value; } }

    [SerializeField,Space(10)]
    LineRenderer lineRenderer;

    [Space(10),SerializeField, Header("CakeList")]
    CakeList cakeList;

    [Space(10), SerializeField, Header("Particles")]
    public List<GameObject> particlePrefabs; // ��ƼŬ ������ ����Ʈ


    private bool isCanDropCake = true;

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

    private void Update()
    {
        switch (GameManager.Inst.cameraType)
        {
            case CameraType.Drop:
                PlayerDropInput();
                break;
            case CameraType.Search:
                PlayerSearchInput();
                break;
        }
        
        DrawRay();
    }
    void PlayerSearchInput()
    {
        // ������� ����
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cursorObj.SetActive(true);
            GameManager.Inst.SwitchCamera(CameraType.Drop);
            //UIManager.Inst.UpdateModeIcon();
            return;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            GameManager.Inst.SearchCameraMove(Vector3.up);
            //print(2);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            GameManager.Inst.SearchCameraMove(Vector3.down);
        }

    }
    void PlayerDropInput()
    {
        // Ž�� ���� ����
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            cursorObj.SetActive(false);
            GameManager.Inst.SwitchCamera(CameraType.Search);
            //UIManager.Inst.UpdateModeIcon();
            return;
        }

        // �����̵�
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveCursor(true);
        }

        // ������ �̵�
        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveCursor(false);
        }

        // �� ȸ��
        if (Input.GetKey(KeyCode.A))
        {
            RotateHoldingCake(true);
        }

        // �� ȸ��
        if (Input.GetKey(KeyCode.D))
        {
            RotateHoldingCake(false);
        }


        // ���� ���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isCanDropCake)
                return;


            if (HoldingCake != null)
            {
                // ���� ������ ��
                
                StartCoroutine(DropCake());
            }
            else
            {
                // ���� �ε����� ��
                ReloadCake();
            }


        }
    }

    public void ReloadCake()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
    new Vector3(cursorObj.transform.position.x, cursorObj.transform.position.y, Camera.main.nearClipPlane + 1f)
);
        
        // ��ū ������Ʈ
        UIManager.Inst.UpdateToken();
        CakeInfo newCake = cakeList.GetCakeInfoRandomByWeight();

        //��ƼŬ
        PlayParticleByCakeType(newCake.cakeType);

        // ����
        HoldingCake = Instantiate(newCake.cakeObj);
        HoldingCake.GetComponent<Cake>().cakeNumber = newCake.cakeNumber;
        HoldingCake.transform.position = worldPos;
        //HoldingCake.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        //HoldingCake.transform.rotation = new Quaternion(0f,0f,Random.Range(0f,360f),0f);
        HoldingCake.transform.GetChild(0).DOPunchScale(Vector3.up*0.25f, 0.5f).SetEase(Ease.InOutQuad);
        
        lineRenderer.enabled = true;
    }

    public void PlayParticleByCakeType(CakeType cakeType)
    {
        if (particlePrefabs.Count == 0)
            return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
    new Vector3(cursorObj.transform.position.x, cursorObj.transform.position.y, Camera.main.nearClipPlane + 1f)
);

        GameObject particleInstance;

        switch (cakeType) 
        {
            case CakeType.choco:
                particleInstance = Instantiate(particlePrefabs[0], worldPos, Quaternion.identity);
                MasterAudio.PlaySound("reload choco");
                break;
            case CakeType.Yellow:
                particleInstance = Instantiate(particlePrefabs[1], worldPos, Quaternion.identity);
                MasterAudio.PlaySound("reload choco");
                break;
            case CakeType.Red:
                particleInstance = Instantiate(particlePrefabs[2], worldPos, Quaternion.identity);
                MasterAudio.PlaySound("reload red");
                break;
            case CakeType.Blue:
                particleInstance = Instantiate(particlePrefabs[3], worldPos, Quaternion.identity);
                MasterAudio.PlaySound("reload red");
                break;
            case CakeType.Rainbow:
                particleInstance = Instantiate(particlePrefabs[4], new Vector3(0f, worldPos.y, -1f) , Quaternion.identity);
                MasterAudio.PlaySound("reload rainbow");
                break;
            default:
                particleInstance = Instantiate(particlePrefabs[0], worldPos, Quaternion.identity);
                break;
        }


        ParticleSystem particleSystem = particleInstance.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            StartCoroutine(DestroyAfterLifetime(particleInstance, particleSystem));
        }
        else
        {
            Debug.LogWarning("Assigned prefab does not have a ParticleSystem component!");
            Destroy(particleInstance);
        }

    }

    private IEnumerator DestroyAfterLifetime(GameObject particleInstance, ParticleSystem particleSystem)
    {
        // ��ƼŬ ������ ���� ������ ���
        yield return new WaitWhile(() => particleSystem.IsAlive(true));

        // ��ƼŬ ������Ʈ ����
        Destroy(particleInstance);
    }

    public IEnumerator DropCake()
    {
        // ���
        isCanDropCake = false;

        HoldingCake.GetComponent<Cake>().Drop();
        lineRenderer.enabled = false;
        HoldingCake = null;

        UIManager.Inst.UIToggle_InfoReload();

        //�������°� ��ٸ���
        yield return new WaitForSeconds(1f);


        //ī�޶� ��ġ �ε巴�� ������Ʈ
        GameManager.Inst.UpdateCameraPositionDotween();
        yield return new WaitForSeconds(0.5f);

        //���� ��Ʈ�� �� ���� (����, �ְ����, json )


        // ��ŷ ������Ʈ

        UIManager.Inst.UIToggle_InfoReload();
        isCanDropCake = true;
    }

    public void RotateHoldingCake(bool isRotateLeft)
    {
        if (HoldingCake == null)
            return;

        if(isRotateLeft)
        {
            HoldingCake.transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
        }
        else
        {

            HoldingCake.transform.Rotate(Vector3.back, rotateSpeed * Time.deltaTime);
        }


    }

    // Ŀ�� �̵�
    public void MoveCursor(bool isMoveLeft)
    {

        RectTransform rectTransform = cursorObj.GetComponent<RectTransform>();

        float minBound = Screen.width*0.05f;
        float maxBound = Screen.width * 0.95f;

        if (isMoveLeft) { 
            //�������� �̵�

            // Ŀ���� ������ ���� �ʰ� ����
            if(rectTransform.position.x - cursorSpeed*Time.deltaTime > minBound)
            {
                rectTransform.position += Vector3.left * cursorSpeed * Time.deltaTime;
                
            }
            //print(1);
        }
        else
        {
            // ���������� �̵�


            // Ŀ���� ������ ���� �ʰ� ����
            if (rectTransform.position.x + cursorSpeed * Time.deltaTime < maxBound)
            {
                rectTransform.position += Vector3.right * cursorSpeed * Time.deltaTime;
            }
            //print(0);
        }

        if(HoldingCake != null)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(
    new Vector3(cursorObj.transform.position.x, cursorObj.transform.position.y, Camera.main.nearClipPlane + 1f)
);
            HoldingCake.transform.position = worldPos;
        }

        //print(rectTransform.position);
    }

    void DrawRay()
    {
        if (HoldingCake == null)
        {
            
            return;
        }

        
        // 1. StartPos ���
        Vector3 startPos = HoldingCake.transform.position;
        startPos.z = 0f; // 2D ȯ�濡�� Z ��ǥ ����

        // 2. Raycast ����
        Vector2 direction = Vector2.down; // Y�� -����
        float maxDistance = 100f; // ���� �ִ� �Ÿ�
        int layerMask = LayerMask.GetMask("Ground", "Cake"); // Ground�� Cake ���̾�

        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxDistance, layerMask);

        Vector3 endPos = startPos + (Vector3)(direction * maxDistance); // �浹 ���� ��� �⺻ endPos

        // 3. ����ĳ��Ʈ ��� Ȯ��
        if (hit.collider != null)
        {
            endPos = hit.point; // �浹 �������� endPos ����
        }

        // 4. LineRenderer�� ����� �� �׸���
        lineRenderer.positionCount = 2; // �� ������ �� ����
        lineRenderer.SetPosition(0, endPos); // ������
        lineRenderer.SetPosition(1, startPos); // ����
    }

    public void Setup(int HoldingCakeNumber = 0)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
    new Vector3(cursorObj.transform.position.x, cursorObj.transform.position.y, Camera.main.nearClipPlane + 1f)
);

        // ����
        HoldingCake = Instantiate(cakeList.cakes[HoldingCakeNumber].cakeObj);
        HoldingCake.GetComponent<Cake>().cakeNumber = HoldingCakeNumber;
        HoldingCake.transform.position = worldPos;
        HoldingCake.transform.GetChild(0).DOPunchScale(Vector3.up * 0.25f, 0.5f).SetEase(Ease.InOutQuad);

        print("�����");
    }

//    [ContextMenu("Setup")]
//    public void SetupTest()
//    {
//        int HoldingCakeNumber = 0;

//        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
//    new Vector3(cursorObj.transform.position.x, cursorObj.transform.position.y, Camera.main.nearClipPlane + 1f)
//);

//        // ����
//        HoldingCake = Instantiate(cakeList.cakes[HoldingCakeNumber].cakeObj);
//        HoldingCake.GetComponent<Cake>().cakeNumber = HoldingCakeNumber;
//        HoldingCake.transform.position = worldPos;
//        HoldingCake.transform.GetChild(0).DOPunchScale(Vector3.up * 0.25f, 0.5f).SetEase(Ease.InOutQuad);

//        print("�����");
//    }
}
