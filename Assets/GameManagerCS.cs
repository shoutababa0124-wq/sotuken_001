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
        doOutGameDelegate = doInit;
    }

    void doInit()
    {
        ObjM.submarineCS.doInit();
        ObjM.eneSubmarineCS.doInit();
        UIM.airImageCS.doInit(UIM.airGageImage,ObjM.submarineCS.airCount);
        doOutGameDelegate = doInGame;
    }

    void doInGame()
    {
        ObjM.submarineCS.doInGame(ObjM.pTorpedoCS,ObjM.pHomingCS,UIM.aiming);
        ObjM.eneSubmarineCS.doInGame(ObjM.eTorpedoCS, ObjM.eHomingCS);
        UIM.homingAimCS.doLockOn(ObjM.submarineCS.doGetLockOnFlag());
        UIM.mapIconCS.doMapping(ObjM.submarineCS);
    }

    // Update is called once per frame
    void Update()
    {
        doOutGameDelegate();
    }
}
