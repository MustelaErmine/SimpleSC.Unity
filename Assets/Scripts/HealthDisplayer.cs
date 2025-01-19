using SimpleSC.Server.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayer : MonoBehaviour
{
    [SerializeField] bool isPlayer;
    [SerializeField] SessionHandler sessionHandler;

    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    public void SetDefaultHealth()
    {
        SetDefaultHealth(GetHealth());
    }

    public void SetDefaultHealth(int health)
    {
        slider.maxValue = health;
        print($"set default health {health}");
    }

    public void UpdateHealth()
    {
        UpdateHealth(GetHealth());
    }

    public void UpdateHealth(int health)
    {
        slider.value = health;
        textMeshProUGUI.text = health.ToString();
        print($"set health {health}");
    }
    int GetHealth()
    {
        return isPlayer ? sessionHandler.CurrentState.health : sessionHandler.CurrentState.enemyHealth;
    }
}
