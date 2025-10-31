using System.Collections.Generic;
using UnityEngine;

public class AirImageCS : MonoBehaviour
{
    List<GameObject> airList;
    float healTime = 0.0f;

    void Start()
    {
        
    }

    public void doInit(GameObject airImage,int airCount)
    {
        //追加　Listを初期化
        airList = new List<GameObject>(airCount);
        for(int i = 0; i < airCount; i++)
        {
            //追加
            GameObject AirImg = Instantiate(airImage, transform);
            airList.Add(AirImg);
        }
    }

    public void doUseMasker(int maskerCount)//指定された値分エアを消費
    {
        for(int i = 0; i < maskerCount; i++)
        {
            airList[airList.Count - (1 - i)].SetActive(false);
        }
    }

    public void doHealAir(int airCount)//エアの補充
    {
        if(airList.Count < airCount)
        {
            healTime += Time.deltaTime;
            if(healTime >= 1.0f)
            {
                airList[airList.Count].SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
