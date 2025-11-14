using Unity.VisualScripting;
using UnityEngine;

public class PHomingCS : MonoBehaviour
{
    GameObject target;       // ターゲット
    float speed = 0.015f;      // ミサイルの速度
    float maxRotationSpeed = 3f; // 最大回転速度
    float accelerationTime = 0.3f; // 回転速度が最大になるまでの時間
    float starttime = 0.1f;//回転速度が加算されるまでの時間

    float startTime;
    float currentRotationSpeed;

    public int damage;
    float destroyTime = 0.0f;
    public TorpedoBomCS torpedoBom;

    void Start()
    {
        startTime = Time.deltaTime; // 初期時間を保存
        currentRotationSpeed = 0f; // 初期回転速度を0に設定
    }

    public void doSetTarget(GameObject setTarget)
    {
        target = setTarget;
        Debug.Log("ターゲット設定");
    }

    void Update()
    {
        destroyTime += Time.deltaTime;
        if(destroyTime > 60.0f)
        {
            GameObject.Instantiate(torpedoBom, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
        if(target.IsDestroyed())
        {
            Destroy(this.gameObject);
        }
        if(target == null)
        {
            transform.Translate(Vector3.forward * speed);
            Debug.Log("ロックオン対象なし");
        }
        else
        {
            Debug.Log("ロックオン対象 : " + target.gameObject.name);
            if(starttime > 0) { starttime -= Time.deltaTime; }
            else
            {
                // 回転速度を徐々に増加
                float elapsedTime = Time.deltaTime - startTime;
                currentRotationSpeed = Mathf.Lerp(0f, maxRotationSpeed, elapsedTime / accelerationTime);
            }

            // ターゲットの方向を計算
            Vector3 direction = (target.transform.position - transform.position).normalized;

            // ターゲットの方向に向かって回転
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, currentRotationSpeed * Time.deltaTime);

            // 前進
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "Enemy":
                Debug.Log("ホーミング魚雷命中！");
                break;
            case "ETorpedo":
                Debug.Log("敵の魚雷に命中！");
                break;
            case "FTorpedo":
                Debug.Log("味方の魚雷に命中！");
                break;
        }
        GameObject.Instantiate(torpedoBom, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
