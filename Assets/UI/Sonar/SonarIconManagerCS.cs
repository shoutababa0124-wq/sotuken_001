using UnityEngine;

public class SonarIconManagerCS
{
    SonarIconCS[] sonarIconCS;
    GameObject sonarIconParent;
    int freCount, eneCount;

    public void doSonar(int i, bool flag, Vector3 pos)
    {
        if(i < freCount)
        {
            //sonarIconCS[i].doSonar(flag, freSubmarineManagerCS.doGetFreSubmarineCS()[i].transform.position);
        }
        else
        {
            sonarIconCS[i].doSonar(flag, pos);
        }
    }

    void doCreateSonarIconCS(SonarIconCS sonarIconCS,Canvas sonarIconCanvas)
    {
        for(int i = 0; i < freCount; i++)
        {
            //this.sonarIconCS[i] = GameObject.Instantiate(sonarIconCS, freSubmarineCS[i].transform);
        }
        for(int i = freCount; i < eneCount + freCount; i++)
        {
            //this.sonarIconCS[i] = GameObject.Instantiate(sonarIconCS, sonarIconParent.transform);
            this.sonarIconCS[i] = GameObject.Instantiate(sonarIconCS, sonarIconCanvas.transform);
        }
    }

    public SonarIconManagerCS(int freCount,int eneCount,SonarIconCS sonarIconCS, Canvas sonarIconCanvas)
    {
        this.freCount = freCount;
        this.eneCount = eneCount;
        this.sonarIconCS = new SonarIconCS[freCount+eneCount];
        //sonarIconParent = new GameObject("SonarIconParent");
        //sonarIconParent.transform.SetParent(sonarCanvas.transform);
        doCreateSonarIconCS(sonarIconCS,sonarIconCanvas);
    }
}
