using UnityEngine;

public class GameManagerCS : MonoBehaviour
{
    //進行用変数
    delegate void DoOutGameDelegate();
    DoOutGameDelegate doOutGameDelegate;
    //変数格納オブジェクト用変数
    public ObjectManagerCS ObjM;//3Dオブジェクト用
    public UIManagerCS UIM;//UI用

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ObjM.eneSubmarineManagerCS = new EneSubmarineManagerCS();
        UIM.sonarIconManagerCS = new SonarIconManagerCS(0,1,UIM.sonarIconCS,UIM.sonarCanvas);
        doOutGameDelegate = doInit;
    }

    void doInit()
    {
        ObjM.submarineCS.doInit(0,1);
        ObjM.eneSubmarineManagerCS.doCreateEneSubmarine(ObjM.eneSubmarineCS);
        ObjM.eneSubmarineManagerCS.doInit();
        UIM.airImageCS.doInit(UIM.airGageImage,ObjM.submarineCS.airCount);
        UIM.nTorpedoTextCS.doText(ObjM.submarineCS.doGetNTorpedoCount());
        UIM.hTorpedoTextCS.doText(ObjM.submarineCS.doGetHTorpedoCount());
        doOutGameDelegate = doInGame;
    }

    void doInGame()
    {
        ObjM.submarineCS.doInGame(ObjM,UIM);
        ObjM.eneSubmarineManagerCS.doInGame(ObjM.eTorpedoCS, ObjM.eHomingCS);
        UIM.homingAimCS.doLockOn(ObjM.submarineCS.doGetLockOnFlag());
        UIM.mapIconCS.doMapping(ObjM.submarineCS);
        UIM.sonarImageCS.doRotSonar(ObjM.submarineCS.transform.localEulerAngles);
        UIM.sonarCS.doSetPos(ObjM.submarineCS);
    }

    // Update is called once per frame
    void Update()
    {
        doOutGameDelegate();
    }
}
