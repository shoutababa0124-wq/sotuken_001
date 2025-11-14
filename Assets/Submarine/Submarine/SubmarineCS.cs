using System.Collections;
using System.Timers;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

//潜水艦の基本操作用スクリプトクラス
public class SubmarineCS : MonoBehaviour
{
    //初期パラメータ
    [Header("海面時の前進速度")] public int onFrontSpeed; //海面前進速度
    [Header("海面時の後退速度")] public int onBackSpeed; //海面後退速度
    [Header("水中時の前進速度")] public int underFrontSpeed; //水中前進速度
    [Header("水中時の後退速度")] public int underBackSpeed; //水中後退速度
    [Header("旋回速度")] public int turnSpeed; //旋回速度
    [Header("潜水浮上速度")] public int diveSpeed; //潜水浮上速度
    [Header("耐久力")] public int durability; //耐久力
    [Header("通常魚雷装填数")] public int nTorpedoCount; //通常魚雷装填数
    [Header("通常魚雷装填時間")] public int reloadSpeed; //通常魚雷装填時間
    [Header("ホーミング魚雷装填数")] public int hTorpedoCount; //ホーミング魚雷装填数
    [Header("エア最大量")] public int airCount; //エア最大量
    [Header("マスカーの消費エア")] public int maskerCount; //マスカーの消費エア
    [Header("魚雷発射位置")]public Vector3 attackRightPos, attackLeftPos;
    [Header("デフォルトの角度")] public Vector3 initRotation;

    float frontSpeed, //前進後退速度
        turningSpeed, //旋回速度
        divingRSpeed, //潜水浮上の角度速度
        divingPSpeed; //潜水浮上の移動速度
    int remainingAir; //エア残量

    int remainingNTorpedo, //通常魚雷残数
        remainingHTorpedo; //ホーミング魚雷残数

    //固定パラメータ
    float playerHP = 100;//プレイヤーのHP

    public PlayerInput playerInput;
    public Renderer myRend;
    public Rigidbody rb;

    bool onTheSeaFlag = false;//海面にいるか
    float frontBackInterval = 0.0f;//前後移動レバーの入力インターバル
    bool frontBackIntervalStart = false;//前後移動レバーの入力確認フラグ
    int frontLever = 0;//前後移動レバーの入力量
    float divingLever = 0.0f;//潜水浮上レバーの入力量
    float turnMax = 0.0f;//現時点の旋回速度の最大値
    float frontAcceleration = 0.0f;//前後移動の加算

    float attackInterval = 0.0f;//魚雷再発射にかかるインターバルのタイマー
    float interValTime;//インターバルにかかる時間
    bool attackFlag = false;//攻撃実行確認フラグ
    float loadingTime = 0.0f;//装填時間のカウント

    bool attackRightLeft = false;//falseでleft:trueでright
    bool lockonFlag = false;//ロックオン中か

    bool maskerUseFlag = false;//マスカー使用中か
    bool maskerFlag = true;//マスカー使用可能か
    bool maskerIntervalStart = false;//マスカーの使用インターバル開始
    float maskerTime = 0.0f;//マスカーの経過時間
    float maskerInterval = 0.0f;//マスカーの使用インターバル
    Color myColor, maskerColor;
    float airHealTime = 0.0f;

    float sonarInterval = 0.0f;
    bool sonarFlag = false;
    Vector3[] pos;//ソナー上に表示する光点の座標

    int freCount, eneCount;//敵味方の人数

    float currentHP = 100;//現在の残りHP
    float tDamageInterval = 0.0f;//無敵時間
    float cDamageInterval = 0.0f;//無敵時間
    bool tDamageFlag = false;//魚雷によるダメージを受けた
    bool cDamageFlag = false;//衝突によるダメージを受けた

    //海面移動時の重力は-9.81、水中では0
    Vector3 onGravity = new Vector3(0.0f, -9.81f, 0.0f), underGravity = Vector3.zero;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void doInit(int freCount, int eneCount)
    {
        this.freCount = freCount;
        this.eneCount = eneCount;
        playerHP = 100;//HPを最大に
        currentHP = playerHP;
        remainingNTorpedo = nTorpedoCount;//通常魚雷の装填数を最大に
        remainingHTorpedo = hTorpedoCount;//ホーミング魚雷の装填数を最大に
        remainingAir = airCount;//マスカーのエアを最大に
        //前進後退の初期化
        frontSpeed = 0.0f; //前進後退速度
        frontLever = 0;//前進後退レバーをニュートラルに
        frontBackInterval = 0.0f;
        frontBackIntervalStart = false;
        //旋回移動の初期化
        turningSpeed = 0.0f;
        transform.rotation = Quaternion.Euler(initRotation);
        //潜水浮上の初期化
        divingRSpeed = 0.0f;
        divingPSpeed = 0.0f;
        //マスカーの初期化
        myRend.material.color = new Color(myRend.material.color.r, myRend.material.color.g, myRend.material.color.b, 1.0f);
        maskerFlag = true;
        maskerTime = 0.0f;
        maskerInterval = 0.0f;
        maskerIntervalStart = false;
        maskerUseFlag = false;
        myColor = myRend.material.color;//カラーをマテリアルに合わせる
        maskerColor = myRend.material.color - new Color(0.0f, 0.0f, 0.0f, 1.0f);
        //ソナーの初期化
        sonarFlag = false;
        sonarInterval = 0.0f;
        pos = new Vector3[this.freCount + this.eneCount];
        //無敵時間の初期化
        tDamageInterval = 0.0f;
        cDamageInterval = 0.0f;
        tDamageFlag = false;
        cDamageFlag = false;
    }

    void doCheckDepth()
    {
        if(transform.position.y >= 0.0f)//海面にいるか
        {
            onTheSeaFlag = true;
            //Debug.Log("海面 前進" + onFrontSpeed + " : 後退" + onBackSpeed);
        }
        else
        {
            onTheSeaFlag = false;
            //Debug.Log("水中 前進" + underFrontSpeed + " : 後退" + underBackSpeed);
        }
    }

    void doSetGravity()
    {
        Vector3 setGravity;
        if(onTheSeaFlag == true)
        {
            setGravity = onGravity;
        }
        else
        {
            setGravity = underGravity;
        }
        rb.AddForce(setGravity, ForceMode.Acceleration);
    }

    //基本移動
    void doMoveFrontBack()
    {
        var frontBack = playerInput.actions["Front&Back"];
        float initFrontSpeed;
        //適応する速度の設定
        if(onTheSeaFlag == true)//海面にいる場合
        {
            if(frontLever > 0)//前進方向に倒した場合
            {
                initFrontSpeed = onFrontSpeed;//前進
            }
            else
            {
                initFrontSpeed = onBackSpeed;//後退
            }
        }
        else//水中にいる場合
        {
            if(frontLever > 0)//前進方向に倒した場合
            {
                initFrontSpeed = underFrontSpeed;//前進
            }
            else
            {
                initFrontSpeed = underBackSpeed;//後退
            }
        }
        if(frontBackIntervalStart == true)//前進後退レバーを倒し始めてからの経過時間
        {
            frontBackInterval += Time.deltaTime;
            if(frontBackInterval > 1.0f / 4.0f)//動かしてから1/4秒後に再度操作可能
            {
                frontBackInterval = 0.0f;
                frontBackIntervalStart = false;
            }
        }
        else
        {
            if(frontBack.ReadValue<float>() != 0)//前進後退いずれかの入力があったら
            {
                if(frontLever < 3 && frontBack.ReadValue<float>() > 0)//前進の最高速度
                {
                    frontLever++;
                    frontSpeed = frontLever * 0.0003f;//入力回数1回ごとに加速
                }
                if(frontLever > -3 && frontBack.ReadValue<float>() < 0)//後退の最高速度
                {
                    frontLever--;
                    frontSpeed = frontLever * 0.0003f;//入力回数1回ごとに加速
                }
            }
            if(frontBack.IsPressed())//前進後退レバーが押されたら
            {
                frontBackIntervalStart = true;//インターバル開始
            }
        }
        if(frontSpeed == 0)
        {
            if(frontAcceleration < frontSpeed)
            {
                frontAcceleration += Time.deltaTime* 0.00015f;
            }
            else if(frontAcceleration > frontSpeed)
            {
                frontAcceleration += -Time.deltaTime * 0.00015f;
            }
            if(Mathf.Abs(frontAcceleration) < 0.00015f) frontAcceleration = 0.0f;
        }
        else if(frontAcceleration < frontSpeed)
        {
            frontAcceleration += Time.deltaTime * 0.002f;
        }
        else if(frontAcceleration > frontSpeed)
        {
            frontAcceleration += -Time.deltaTime * 0.002f;
        }
        transform.Translate(Vector3.forward * (initFrontSpeed+29) * frontAcceleration);
    }

    void doMoveRightLeft()
    {
        var LRUD = playerInput.actions["Left&Right&Up&Down"];
        if(LRUD.ReadValue<Vector2>().x != 0)//旋回の入力があったら
        {
            if(Mathf.Abs(turningSpeed) < 0.05f)
            {
                turningSpeed += Time.deltaTime * LRUD.ReadValue<Vector2>().x * 0.03f;
                turnMax = turningSpeed;
            }
            else if(turningSpeed > 0.05f)
            {
                turningSpeed = 0.05f;
            }
            else if(turningSpeed < -0.05f)
            {
                turningSpeed = -0.05f;
            }
        }
        else
        {
            if(turningSpeed > 0)
            {
                turningSpeed += -Time.deltaTime * turnMax * 1.0f;
            }
            else if(turningSpeed < 0)
            {
                turningSpeed += -Time.deltaTime * turnMax * 1.0f;
            }
            if(Mathf.Abs(turningSpeed) < 0.003f)
            {
                turningSpeed = 0.0f;
                turnMax = 0.0f;
            }
        }
        transform.Rotate(new Vector3(0, (turnSpeed + 3) * turningSpeed, 0),Space.World);
    }

    void doMoveUpDown()
    {
        var LRUD = playerInput.actions["Left&Right&Up&Down"]; 
        if(LRUD.ReadValue<Vector2>().y != 0)//潜水浮上の入力があったら
        {
            //角度
            if(Mathf.Repeat(transform.localEulerAngles.x + 180, 360) - 180 < initRotation.x + 45.0f && LRUD.ReadValue<Vector2>().y < 0)//+45度より上には行かない
            {
                divingRSpeed += -Time.deltaTime * LRUD.ReadValue<Vector2>().y * 3.0f;//潜水
                divingLever += -LRUD.ReadValue<Vector2>().y*0.1f;
            }
            if(Mathf.Repeat(transform.localEulerAngles.x + 180, 360) - 180 > initRotation.x - 45.0f && LRUD.ReadValue<Vector2>().y > 0)//-45度より下には行かない
            {
                divingRSpeed += -Time.deltaTime * LRUD.ReadValue<Vector2>().y * 3.0f;//浮上
                divingLever += -LRUD.ReadValue<Vector2>().y*0.1f;
            }
            //移動
            if(Mathf.Abs(divingPSpeed) <= 0.002f)
            {
                divingPSpeed += Time.deltaTime * LRUD.ReadValue<Vector2>().y * 0.002f;
            }
            else if(divingPSpeed > 0.002f)
            {
                divingPSpeed = 0.002f;
            }
            else if(divingPSpeed < -0.002f)
            {
                divingPSpeed = -0.002f;
            }
        }
        else
        {
            if(Mathf.Abs(divingLever) < 2.0f)
            {
                if(divingRSpeed > 0.0f)
                {
                    divingRSpeed += -Time.deltaTime * 0.3f;
                }
                else if(divingRSpeed < 0.0f)
                {
                    divingRSpeed += Time.deltaTime * 0.3f;
                }
                if(Mathf.Abs(divingRSpeed) < 0.01f)
                {
                    divingRSpeed = 0.0f;
                }
                if(divingPSpeed > 0.0f)
                {
                    divingPSpeed += -Time.deltaTime * 0.001f;
                }
                else if(divingPSpeed < 0.0f)
                {
                    divingPSpeed += Time.deltaTime * 0.001f;
                }
                if(Mathf.Abs(divingPSpeed) < 0.001f)
                {
                    divingPSpeed = 0.0f;
                }
                if(divingRSpeed == 0.0f && divingPSpeed == 0.0f)
                {
                    divingLever = 0.0f;
                }
            }
        }
        //角度
        transform.localEulerAngles=new Vector3(initRotation.x+(diveSpeed+2) * divingRSpeed, transform.localEulerAngles.y, transform.localEulerAngles.z);
        //移動
        transform.position += Vector3.up * divingPSpeed * (diveSpeed+2);
    }

    void doMove()
    {
        doMoveFrontBack();
        doMoveRightLeft();
        doMoveUpDown();
    }

    //通常魚雷
    void doAttack(ObjectManagerCS objM, UIManagerCS uIM)
    {
        var torpedoCS = objM.pTorpedoCS;
        var homingCS = objM.pHomingCS;
        var nTorpedoTextCS = uIM.nTorpedoTextCS;
        var hTorpedoTextCS = uIM.hTorpedoTextCS;
        if(attackFlag == false)
        {
            if(playerInput.actions["NormalAttack"].WasReleasedThisFrame() && remainingNTorpedo > 0)
            {
                attackFlag = true;
                remainingNTorpedo--;
                nTorpedoTextCS.doText(remainingNTorpedo);
                interValTime = 3.0f;
                if(attackRightLeft == false)
                {
                    GameObject.Instantiate(torpedoCS, transform.position + attackLeftPos, Quaternion.Euler(transform.localEulerAngles));
                }
                else
                {
                    GameObject.Instantiate(torpedoCS, transform.position + attackRightPos, Quaternion.Euler(transform.localEulerAngles));
                }
                attackRightLeft = !attackRightLeft;
            }
            else if(playerInput.actions["HomingAttack"].WasReleasedThisFrame() && remainingHTorpedo > 0)
            {
                attackFlag = true;
                remainingHTorpedo--;
                hTorpedoTextCS.doText(remainingHTorpedo);
                interValTime = 4.0f;
                if(attackRightLeft == false)
                {
                    GameObject.Instantiate(homingCS, transform.position + attackLeftPos, Quaternion.Euler(transform.localEulerAngles));
                }
                else
                {
                    GameObject.Instantiate(homingCS, transform.position + attackRightPos, Quaternion.Euler(transform.localEulerAngles));
                }
                attackRightLeft = !attackRightLeft;
            }
        }
        if(attackFlag == true)
        {
            attackInterval += Time.deltaTime;
            if(attackInterval > interValTime)
            {
                attackInterval = 0.0f;
                attackFlag = false;
            }
        }
    }

    void doReload()
    {
        if(remainingNTorpedo == 0)
        {
            loadingTime += Time.deltaTime;
            if(loadingTime > reloadSpeed)
            {
                remainingNTorpedo = nTorpedoCount;
                loadingTime = 0.0f;
            }
        }
    }

    void doRockOn()
    {
        if(remainingHTorpedo > 0)
        {
            // 1. カメラからRayを生成
            // Camera.mainは"MainCamera"タグのついたカメラを取得します
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth/2,Camera.main.pixelHeight/2,0));

            // RaycastHit型の変数を宣言し、衝突した情報を受け取ります
            RaycastHit hit;

            // 2. Rayを投射し、衝突があったか判定
            // Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity)
            if(Physics.Raycast(ray, out hit))
            {
                // 3. 衝突したオブジェクトの情報を取得
                // hit.collider.gameObject で衝突したゲームオブジェクトを取得
                GameObject hitObject = hit.collider.gameObject;

                if(hitObject.tag == "Enemy")
                {
                    Debug.Log("Lock On : " + hitObject.name);
                    lockonFlag = true;
                }
                else
                {
                    lockonFlag = false;
                }
            }
        }
    }

    void doMasker(ObjectManagerCS objM,UIManagerCS uIM)
    {
        if(playerInput.actions["SpecialAction"].IsPressed() && playerInput.actions["Masker"].WasPressedThisFrame() && maskerFlag == true && onTheSeaFlag == false&&remainingAir>=maskerCount)//マスカーの入力+マスカー使用可能
        {
            Debug.Log("Masker");
            remainingAir -= maskerCount;//エアを消費
            uIM.airImageCS.doUseMasker(maskerCount);
            maskerUseFlag = true;
            maskerFlag = false;
            ParticleSystem.Instantiate(objM.maskerParticle,transform.position,objM.maskerParticle.transform.rotation);
        }
        if(maskerUseFlag == true)
        {
            maskerTime += Time.deltaTime;
            if(maskerTime < 1.0f)
            {
                myRend.material.color = Color.Lerp(myColor, maskerColor, maskerTime);
            }
            else if(maskerTime == 1.0f)
            {
                myRend.material.color = maskerColor;
            }
            if(maskerTime >= 10.0f || onTheSeaFlag == true || attackFlag == true)//時間経過or海面浮上or魚雷発射で解除+インターバル開始
            {
                maskerUseFlag = false;
                maskerTime = 0.0f;
                maskerIntervalStart = true;
            }
        }
        if(maskerIntervalStart == true)
        {
            maskerInterval += Time.deltaTime;
            if(maskerInterval < 1.0f)
            {
                myRend.material.color = Color.Lerp(maskerColor, myColor, maskerInterval);
            }
            else if(maskerInterval == 1.0f)
            {
                myRend.material.color = myColor;
            }
            if(maskerInterval >= 4.0f)//時間経過で再使用可能
            {
                maskerInterval = 0.0f;
                maskerIntervalStart= false;
                maskerFlag = true;
            }
        }
        if(onTheSeaFlag == true && airCount > remainingAir)
        {
            airHealTime += Time.deltaTime;
            if(airHealTime >= 1.0f)
            {
                remainingAir++;
                uIM.airImageCS.doHealAir(remainingAir);
                airHealTime = 0.0f;
            }
        }
    }

    void doSonar(EneSubmarineManagerCS eneSubmarineManagerCS, SonarIconManagerCS sonarIconManagerCS)
    {
        if((!playerInput.actions["SpecialAction"].IsPressed()) &&playerInput.actions["Sonar"].WasPressedThisFrame() && sonarFlag == false)
        {
            Debug.Log("ソナー");
            for(int i = 0; i < freCount; i++)
            {
                //味方
            }
            for(int i = freCount; i < freCount + eneCount; i++)
            {
                pos[i] = transform.position - eneSubmarineManagerCS.doGetEneSubmarineCS()[i].transform.position;
            }
            sonarFlag = true;
        }
        if(sonarFlag == true)
        {
            sonarInterval += Time.deltaTime;
            if(sonarInterval > 4.0f)
            {
                sonarFlag = false;
                sonarInterval = 0.0f;
            }
        }
        for(int i = 0; i < freCount; i++)
        {
            //if(freSubmarineCS[i].diveSpeed != 0)//前進後退中
            //{
            //sonarIconManagerCS.doSonar(i, sonarFlag, eneSubmarineManagerCS);
            //}
        }
        for(int i = freCount; i < freCount + eneCount; i++)
        {
            //if(eneSubmarineCS[i].diveSpeed != 0)//前進後退中
            //{
            sonarIconManagerCS.doSonar(i, sonarFlag, new Vector3(-pos[i].x, -pos[i].z, 0));
            //}
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "ETorpedo" && tDamageFlag == false)
        {
            currentHP -= 25;
            tDamageFlag = true;
            Debug.Log("攻撃を受けています！");
        }
        else if(other.gameObject.tag == "Bomb" && tDamageFlag == false)
        {
            currentHP -= 5;
            tDamageFlag = true;
            Debug.Log("被害が甚大です！");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag != "FSubmarine" && cDamageFlag == false)
        {
            currentHP -= 3;
            cDamageFlag = true;
            Debug.Log("衝突しています！");
        }
    }

    public void doDamageInterval()
    {
        if(tDamageFlag == true)
        {
            tDamageInterval += Time.deltaTime;
            if(tDamageInterval > 3.0f)//無敵時間船ごとに分けても良さそう
            {
                tDamageFlag = false;
                tDamageInterval = 0.0f;
            }
        }
        if(cDamageFlag == true)
        {
            cDamageInterval += Time.deltaTime;
            if(cDamageInterval > 2.0f)
            {
                cDamageFlag = false;
                cDamageInterval = 0.0f;
            }
        }
    }

    //変数の取得
    public float doGetSpeed()
    {
        return frontAcceleration + divingPSpeed;
    }

    public float doGetDepth()
    {
        return transform.position.y;
    }

    public float doGetDirection()
    {
        return transform.localEulerAngles.y;
    }
    public bool doGetLockOnFlag()
    {
        return lockonFlag;
    }

    public int doGetAirCount()
    {
        return airCount;//最大エア
    }

    public int doGetMaskerCount()
    {
        return maskerCount;//消費エア
    }

    public int doGetNTorpedoCount()
    {
        return remainingNTorpedo;
    }

    public int doGetHTorpedoCount()
    {
        return remainingHTorpedo;
    }

    public float doGetCurrentHP()
    {
        return currentHP;
    }

    public float doGetMaxHP()
    {
        return playerHP;
    }

    public void doInGame(ObjectManagerCS objM,UIManagerCS uIM) {
        doCheckDepth();
        //doSetGravity();
        doMove();
        doAttack(objM, uIM);
        doReload();
        doRockOn();
        doMasker(objM,uIM);
        doSonar(objM.eneSubmarineManagerCS,uIM.sonarIconManagerCS);
        doDamageInterval();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
