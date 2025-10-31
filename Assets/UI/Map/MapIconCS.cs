using UnityEngine;

public class MapIconCS : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void doMapping(SubmarineCS player)
    {
        transform.localPosition = new Vector3(player.transform.position.x, player.transform.position.z, 0);
        transform.localEulerAngles = new Vector3(0, 0, -player.transform.localEulerAngles.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
