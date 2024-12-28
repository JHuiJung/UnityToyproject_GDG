using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Inst { get; private set; }

    [SerializeField]
    GameObject cursorObj; // 커서 오브젝트

    [SerializeField]
    float cursorSpeed = 8f; // 커서 속도

    [SerializeField]
    float rotateSpeed = 60f;

    //[SerializeField]
    //Vector2 boundPos = Vector2.one; // 커서 범위 제한

    //장전된 케이크
    [SerializeField]
    GameObject _HoldingCake;
    public GameObject HoldingCake { get { return _HoldingCake; } private set { _HoldingCake = value; } }

    [SerializeField,Space(10)]
    LineRenderer lineRenderer;

    [Space(10),SerializeField, Header("CakeList")]
    CakeList cakeList;

    [Space(10), SerializeField, Header("Particles")]
    public List<GameObject> particlePrefabs; // 파티클 프리팹 리스트


    private bool isCanDropCake = true;

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
        // 드랍으로 변경
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
        // 탐색 모드로 변경
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            cursorObj.SetActive(false);
            GameManager.Inst.SwitchCamera(CameraType.Search);
            //UIManager.Inst.UpdateModeIcon();
            return;
        }

        // 왼쪽이동
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveCursor(true);
        }

        // 오른쪽 이동
        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveCursor(false);
        }

        // 좌 회전
        if (Input.GetKey(KeyCode.A))
        {
            RotateHoldingCake(true);
        }

        // 우 회전
        if (Input.GetKey(KeyCode.D))
        {
            RotateHoldingCake(false);
        }


        // 케익 드랍
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isCanDropCake)
                return;


            if (HoldingCake != null)
            {
                // 장전 돼있을 때
                
                StartCoroutine(DropCake());
            }
            else
            {
                // 장전 인돼있을 때
                ReloadCake();
            }


        }
    }

    public void ReloadCake()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
    new Vector3(cursorObj.transform.position.x, cursorObj.transform.position.y, Camera.main.nearClipPlane + 1f)
);
        
        // 토큰 업데이트
        UIManager.Inst.UpdateToken();
        CakeInfo newCake = cakeList.GetCakeInfoRandomByWeight();

        //파티클
        PlayParticleByCakeType(newCake.cakeType);

        // 생성
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
        // 파티클 실행이 끝날 때까지 대기
        yield return new WaitWhile(() => particleSystem.IsAlive(true));

        // 파티클 오브젝트 삭제
        Destroy(particleInstance);
    }

    public IEnumerator DropCake()
    {
        // 드랍
        isCanDropCake = false;

        HoldingCake.GetComponent<Cake>().Drop();
        lineRenderer.enabled = false;
        HoldingCake = null;

        UIManager.Inst.UIToggle_InfoReload();

        //떨어지는거 기다리기
        yield return new WaitForSeconds(1f);


        //카메라 위치 부드럽게 업데이트
        GameManager.Inst.UpdateCameraPositionDotween();
        yield return new WaitForSeconds(0.5f);

        //구글 시트에 값 저장 (높이, 최고높이, json )


        // 랭킹 업데이트

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

    // 커서 이동
    public void MoveCursor(bool isMoveLeft)
    {

        RectTransform rectTransform = cursorObj.GetComponent<RectTransform>();

        float minBound = Screen.width*0.05f;
        float maxBound = Screen.width * 0.95f;

        if (isMoveLeft) { 
            //왼쪽으로 이동

            // 커서가 범위를 넘지 않게 조정
            if(rectTransform.position.x - cursorSpeed*Time.deltaTime > minBound)
            {
                rectTransform.position += Vector3.left * cursorSpeed * Time.deltaTime;
                
            }
            //print(1);
        }
        else
        {
            // 오른쪽으로 이동


            // 커서가 범위를 넘지 않게 조정
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

        
        // 1. StartPos 계산
        Vector3 startPos = HoldingCake.transform.position;
        startPos.z = 0f; // 2D 환경에서 Z 좌표 고정

        // 2. Raycast 설정
        Vector2 direction = Vector2.down; // Y축 -방향
        float maxDistance = 100f; // 레이 최대 거리
        int layerMask = LayerMask.GetMask("Ground", "Cake"); // Ground와 Cake 레이어

        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxDistance, layerMask);

        Vector3 endPos = startPos + (Vector3)(direction * maxDistance); // 충돌 없을 경우 기본 endPos

        // 3. 레이캐스트 결과 확인
        if (hit.collider != null)
        {
            endPos = hit.point; // 충돌 지점으로 endPos 설정
        }

        // 4. LineRenderer를 사용해 선 그리기
        lineRenderer.positionCount = 2; // 두 점으로 선 설정
        lineRenderer.SetPosition(0, endPos); // 시작점
        lineRenderer.SetPosition(1, startPos); // 끝점
    }

    public void Setup(int HoldingCakeNumber = 0)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
    new Vector3(cursorObj.transform.position.x, cursorObj.transform.position.y, Camera.main.nearClipPlane + 1f)
);

        // 생성
        HoldingCake = Instantiate(cakeList.cakes[HoldingCakeNumber].cakeObj);
        HoldingCake.GetComponent<Cake>().cakeNumber = HoldingCakeNumber;
        HoldingCake.transform.position = worldPos;
        HoldingCake.transform.GetChild(0).DOPunchScale(Vector3.up * 0.25f, 0.5f).SetEase(Ease.InOutQuad);

        print("실행됨");
    }

//    [ContextMenu("Setup")]
//    public void SetupTest()
//    {
//        int HoldingCakeNumber = 0;

//        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
//    new Vector3(cursorObj.transform.position.x, cursorObj.transform.position.y, Camera.main.nearClipPlane + 1f)
//);

//        // 생성
//        HoldingCake = Instantiate(cakeList.cakes[HoldingCakeNumber].cakeObj);
//        HoldingCake.GetComponent<Cake>().cakeNumber = HoldingCakeNumber;
//        HoldingCake.transform.position = worldPos;
//        HoldingCake.transform.GetChild(0).DOPunchScale(Vector3.up * 0.25f, 0.5f).SetEase(Ease.InOutQuad);

//        print("실행됨");
//    }
}
