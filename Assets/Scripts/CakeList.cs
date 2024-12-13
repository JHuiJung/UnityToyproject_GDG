using System.Collections.Generic;
using UnityEngine;

public enum CakeType
{
    choco,
    Yellow,
    Red,
    Blue,
    Rainbow
}

[System.Serializable]
public class CakeInfo
{
    public CakeType cakeType;
    public GameObject cakeObj;
    public int cakeNumber = 0;
}

[CreateAssetMenu(fileName = "NewCakeList", menuName = "ScriptableObjects/CakeList", order = 1)]
public class CakeList : ScriptableObject
{
    [SerializeField]
    public readonly int Weight_Choco = 50;
    public readonly int Weight_Yellow = 25;
    public readonly int Weight_Red = 15;
    public readonly int Weight_Blue = 7;
    public readonly int Weight_Rainbow = 3;

    public List<CakeInfo> cakes = new List<CakeInfo>();

    public CakeInfo GetCakeInfo(int num)
    {
        if(num > cakes.Count - 1)
        {

            return null;
        }

        return cakes[num];
    }


    public CakeInfo GetCakeInfoRandomByWeight()
    {
        int totalWeight = Weight_Choco + Weight_Yellow + Weight_Red + Weight_Blue + Weight_Rainbow;
        int randomValue = Random.Range(0, totalWeight);

        if (randomValue < Weight_Choco)
        {
            return cakes[Random.Range(0, 5)]; // 0-4: choco
        }
        else if (randomValue < Weight_Choco + Weight_Yellow)
        {
            return cakes[Random.Range(5, 10)]; // 5-9: yellow
        }
        else if (randomValue < Weight_Choco + Weight_Yellow + Weight_Red)
        {
            return cakes[Random.Range(10, 15)]; // 10-14: red
        }
        else if (randomValue < Weight_Choco + Weight_Yellow + Weight_Red + Weight_Blue)
        {
            return cakes[Random.Range(15, 20)]; // 15-19: blue
        }
        else
        {
            return cakes[Random.Range(20, 25)]; // 20-24: rainbow
        }
    }
}
