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
    Vector2 boundPos = Vector2.one; // 커서 범위 제한

    //장전된 케이크
    [SerializeField]
    GameObject HoldingCake;


    [SerializeField, Header("CakeList")]
    CakeList cakeList;

    private void Update()
    {

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


        // 케익 드랍
        if(Input.GetKeyDown(KeyCode.Space))
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
        HoldingCake = cakeList.GetCakeInfoRandomByWeight().cakeObj;
    }

    public void DropCake()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
    new Vector3(cursorObj.transform.position.x, cursorObj.transform.position.y, Camera.main.nearClipPlane + 1f)
);

        var np = Instantiate(HoldingCake);
        np.transform.position = worldPos;

        HoldingCake = null;
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

        //print(rectTransform.position);
    }

}
