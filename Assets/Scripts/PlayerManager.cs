using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [SerializeField]
    GameObject cursorObj; // Ŀ�� ������Ʈ

    [SerializeField]
    float cursorSpeed = 8f; // Ŀ�� �ӵ�

    [SerializeField]
    float rotateSpeed = 60f;

    [SerializeField]
    Vector2 boundPos = Vector2.one; // Ŀ�� ���� ����

    //������ ����ũ
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
        // ������� ����
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
        // Ž�� ���� ����
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            cursorObj.SetActive(false);
            GameManager.Inst.SwitchCamera(CameraType.Search);
            UIManager.Inst.UpdateModeIcon();
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
        if (Input.GetKey(KeyCode.Q))
        {
            RotateHoldingCake(true);
        }

        // �� ȸ��
        if (Input.GetKey(KeyCode.E))
        {
            RotateHoldingCake(false);
        }


        // ���� ���
        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (HoldingCake != null)
            {
                // ���� ������ ��
                DropCake();
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

    // Ŀ�� �̵�
    public void MoveCursor(bool isMoveLeft)
    {

        RectTransform rectTransform = cursorObj.GetComponent<RectTransform>();

        if (isMoveLeft) { 
            //�������� �̵�

            // Ŀ���� ������ ���� �ʰ� ����
            if(rectTransform.position.x - cursorSpeed*Time.deltaTime > boundPos.x)
            {
                rectTransform.position += Vector3.left * cursorSpeed * Time.deltaTime;
                
            }
            //print(1);
        }
        else
        {
            // ���������� �̵�


            // Ŀ���� ������ ���� �ʰ� ����
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
}
