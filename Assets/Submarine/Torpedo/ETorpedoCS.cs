using UnityEngine;

public class ETorpedoCS : MonoBehaviour
{
    public float speed;
    float speedTime = 0.0f;
    public int damage;
    float destroyTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destroyTime = 0.0f;
        speedTime = 0.0f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("çUåÇÇéÛÇØÇƒÇ¢Ç‹Ç∑ÅI");
        }
        if(collision.gameObject.tag != "Enemy")
        {

        }
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(speedTime < 1.0f)
        {
            speedTime += Time.deltaTime * 0.5f;
        }
        transform.Translate(Vector3.forward * speed * speedTime);
        destroyTime += Time.deltaTime;
        if(destroyTime > 5.0f)
        {
            Destroy(this.gameObject);
        }
    }
}
