using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMove : MonoBehaviour
{
    [SerializeField]
    List<GameObject> BackGroundObjs;

    [SerializeField]
    float moveSpeed = 5f;

    [SerializeField]
    float CheckHeight = 60f;

    private void Update()
    {
        CheckObj();
        MoveObj();
    }

    void MoveObj()
    {

        foreach (var obj in BackGroundObjs)
        {
            obj.transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }


    }

    void CheckObj()
    {
        foreach (var obj in BackGroundObjs)
        {
            if(obj.transform.position.y > CheckHeight)
            {
                obj.transform.position = new Vector3(0f,-30f,10f);
            }
        }
    }
}
