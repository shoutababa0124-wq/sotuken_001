using UnityEngine;

public class PHomingCS : MonoBehaviour
{
    public Transform target;       // ターゲットのトランスフォーム
    public float speed = 10f;      // ミサイルの速度
    public float maxRotationSpeed = 5f; // 最大回転速度
    public float accelerationTime = 0.5f; // 回転速度が最大になるまでの時間
    public float starttime = 0.5f;//回転速度が加算されるまでの時間
    public float maxLifetime = 10f;  // 最大存続時間
    public float hitDistance = 1f;  // ヒットとみなす距離

    private float startTime;
    private float currentRotationSpeed;

    public string TargetName = "LockOnTarget";

    public int damage;
    float destroyTime = 0.0f;
    public TorpedoBomCS torpedoBom;

    void Start()
    {
        startTime = Time.time; // 初期時間を保存
        currentRotationSpeed = 0f; // 初期回転速度を0に設定
    }

    void Update()
    {
        if(target == null)
        {
            return; // ターゲットが設定されていない場合は何もしない
        }
        if(starttime > 0) { starttime -= Time.deltaTime; }
        else
        {
            // 回転速度を徐々に増加
            float elapsedTime = Time.time - startTime;
            currentRotationSpeed = Mathf.Lerp(0f, maxRotationSpeed, elapsedTime / accelerationTime);
        }


        // ターゲットの方向を計算
        Vector3 direction = (target.position - transform.position).normalized;

        // ターゲットの方向に向かって回転
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, currentRotationSpeed * Time.deltaTime);

        // 前進
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // 一定時間経過後に削除
        if(Time.time - startTime >= maxLifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "Enemy":
                Debug.Log("命中！");
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
