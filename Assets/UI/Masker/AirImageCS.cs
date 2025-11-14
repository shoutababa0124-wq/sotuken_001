using System.Collections.Generic;
using UnityEngine;

public class AirImageCS : MonoBehaviour
{
    List<GameObject> airList;

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
        int count = 0;
        for(int i=0; i < airList.Count; i++)
        {
            if(airList[i].activeSelf == true)
            {
                count++;
            }
        }
        for(int i = 0; i < maskerCount; i++)
        {
            airList[count - 1 - i].SetActive(false);
        }
    }

    public void doHealAir(int i)//エアの補充
    {
        airList[i - 1].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
