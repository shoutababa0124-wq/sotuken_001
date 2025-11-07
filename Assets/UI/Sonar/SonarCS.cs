using UnityEngine;

public class SonarCS : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void doSetPos(SubmarineCS player)
    {
        transform.localPosition = new Vector3(player.transform.position.x, 100, player.transform.position.z);
        transform.localEulerAngles = new Vector3(90, 0 , player.transform.localEulerAngles.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
