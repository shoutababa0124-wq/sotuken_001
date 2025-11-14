using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBarCS : MonoBehaviour
{
    public Slider slider;

    public void doInit()
    {
        slider.value = 1;
    }

    public void doHP(float currentHP, float maxHP)
    {
        slider.value = currentHP / maxHP;
    }
}