using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
    public static StatsBar instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [SerializeField]
    private Slider health, stamina;

    public void SetHealth(float amount)
    {
        health.value = amount / 100;
    }
    public void SetStamina(float amount)
    {
        stamina.value = amount / 100;
    }
}
