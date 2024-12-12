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
    Vector2 boundPos = Vector2.one; // Ŀ�� ���� ����

    //�׽�Ʈ�� ����
    [SerializeField]
    GameObject TestObj;

    private void Update()
    {

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


        // ���� ���
        if(Input.GetKeyDown(KeyCode.Space))
        {
            DropCake();
        }
    }

    public void DropCake()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
    new Vector3(cursorObj.transform.position.x, cursorObj.transform.position.y, Camera.main.nearClipPlane + 1f)
);

        var np = Instantiate(TestObj);
        np.transform.position = worldPos;
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

        //print(rectTransform.position);
    }

}
