using TMPro;
using UnityEngine;

public class HTorpedoTextCS : MonoBehaviour
{
    int torpedoCount = 0;
    public TextMeshProUGUI tmp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void doText(int torpedoCount)
    {
        this.torpedoCount = torpedoCount;
        tmp.text = this.torpedoCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
