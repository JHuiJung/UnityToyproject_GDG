using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [SerializeField]
    GameObject cursorObj; // 커서 오브젝트

    [SerializeField]
    float cursorSpeed = 8f; // 커서 속도

    [SerializeField]
    float rotateSpeed = 60f;

    [SerializeField]
    Vector2 boundPos = Vector2.one; // 커서 범위 제한

    //장전된 케이크
    [SerializeField]
    GameObject HoldingCake;

    [SerializeField,Space(10)]
    LineRenderer lineRenderer;

    [Space(10),SerializeField, Header("CakeList")]
    CakeList cakeList;

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
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cursorObj.SetActive(true);
            GameManager.Inst.SwitchCamera(CameraType.Drop);
            UIManager.Inst.UpdateModeIcon();
            return;
        }

        if(Input.GetKey(KeyCode.UpArrow))
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
            UIManager.Inst.UpdateModeIcon();
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
        if (Input.GetKey(KeyCode.Q))
        {
            RotateHoldingCake(true);
        }

        // 우 회전
        if (Input.GetKey(KeyCode.E))
        {
            RotateHoldingCake(false);
        }


        // 케익 드랍
        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (HoldingCake != null)
            {
                // 장전 돼있을 때
                DropCake();
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
        
        UIManager.Inst.UpdateToken();
        HoldingCake = Instantiate(cakeList.GetCakeInfoRandomByWeight().cakeObj);
        HoldingCake.transform.position = worldPos;
        lineRenderer.enabled = true;
    }

    public void DropCake()
    {
        HoldingCake.GetComponent<Cake>().Drop();
        lineRenderer.enabled = false;
        HoldingCake = null;
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

        if (isMoveLeft) { 
            //왼쪽으로 이동

            // 커서가 범위를 넘지 않게 조정
            if(rectTransform.position.x - cursorSpeed*Time.deltaTime > boundPos.x)
            {
                rectTransform.position += Vector3.left * cursorSpeed * Time.deltaTime;
                
            }
            //print(1);
        }
        else
        {
            // 오른쪽으로 이동


            // 커서가 범위를 넘지 않게 조정
            if (rectTransform.position.x + cursorSpeed * Time.deltaTime < boundPos.y)
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
}
