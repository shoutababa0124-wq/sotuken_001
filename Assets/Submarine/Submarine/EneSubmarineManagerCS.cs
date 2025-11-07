using UnityEngine;

public class EneSubmarineManagerCS
{
    EneSubmarineCS[] eneSubmarineCS;
    GameObject eneSubmarineParent;
    int enemyCount = 1;

    public EneSubmarineCS[] doGetEneSubmarineCS()
    {
        return eneSubmarineCS;
    }

    public void doSetEnemyCount(int count)
    {
        enemyCount = count;
    }

    public void doInit()
    {
        for(int i = 0; i < enemyCount; i++)
        {
            eneSubmarineCS[i].doInit();
        }
    }

    public void doInGame(ETorpedoCS eTorpedoCS,EHomingCS eHomingCS)
    {
        for (int i = 0;i < enemyCount; i++)
        {
            eneSubmarineCS[i].doInGame(eTorpedoCS, eHomingCS);
        }
    }

    public void doCreateEneSubmarine(EneSubmarineCS eneSubmarineCS)
    {
        for(int i = 0; i < enemyCount; i++)
        {
            this.eneSubmarineCS[i] = GameObject.Instantiate(eneSubmarineCS,eneSubmarineParent.transform);
        }
    }

    public EneSubmarineManagerCS()
    {
        eneSubmarineCS = new EneSubmarineCS[enemyCount];
        eneSubmarineParent = new GameObject("EneSubmarineParent");
    }
}
