using Newtonsoft.Json;
using SimpleSC.Server.Actions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    [SerializeField] SessionHandler sessionHandler;
    [SerializeField] string code;

    [SerializeField] GameObject darkPanel;
    [SerializeField] TextMeshProUGUI textMesh;
    public void UpdateState()
    {
        UnitAction action = sessionHandler.CurrentState.actions.ToList()
            .Find((UnitAction act) => act.code == code);
        print($"set cooldown {action.currentCooldown} {code}");
        if (action.currentCooldown <= 0)
        {
            darkPanel.SetActive(false);
        } 
        else
        {
            darkPanel.SetActive(true);
            textMesh.text = action.currentCooldown.ToString();
        }
    }
    public void SendAction()
    {
        sessionHandler.Emit("action", code);
    }
}
