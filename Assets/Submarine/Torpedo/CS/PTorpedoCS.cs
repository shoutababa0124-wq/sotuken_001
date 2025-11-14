using UnityEngine;

public class PTorpedoCS : MonoBehaviour
{
    public float speed;
    float speedTime = 0.0f;
    public int damage;
    float destroyTime = 0.0f;
    public TorpedoBomCS torpedoBom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destroyTime = 0.0f;
        speedTime = 0.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "Enemy":
                Debug.Log("–½’†I");
                break;
            case "ETorpedo":
                Debug.Log("“G‚Ì‹›—‹‚É–½’†I");
                break;
            case "FTorpedo":
                Debug.Log("–¡•û‚Ì‹›—‹‚É–½’†I");
                break;
        }
        GameObject.Instantiate(torpedoBom, transform.position, transform.rotation);
        Destroy(this.gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        if(speedTime < 1.0f)
        {
            speedTime += Time.deltaTime*0.5f;
        }
        transform.Translate(Vector3.forward * speed * speedTime);
        destroyTime += Time.deltaTime;
        if(destroyTime > 5.0f)
        {
            GameObject.Instantiate(torpedoBom, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
