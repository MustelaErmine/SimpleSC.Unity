using Newtonsoft.Json;
using SimpleSC.Server.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectDisplayer : MonoBehaviour
{
    [SerializeField] SessionHandler sessionHandler;
    [SerializeField] string code;
    [SerializeField] TextMeshProUGUI textMesh;

    [SerializeField] Image image;
    public void UpdateState()
    {
        int max_steps = 0;
        try
        {
            max_steps = sessionHandler.CurrentState.effects.ToList()
                .FindAll((Effect eff) => eff.code == code)
                .Max((Effect eff) => eff.activeStepsCurrent);
        } catch (InvalidOperationException)
        {

        }
        if (max_steps > 0)
        {
            textMesh.text = max_steps.ToString();
        } else
        {
            textMesh.text = "";
        }
        print($"set effect state {max_steps} {code}");
        image.enabled = max_steps > 0;
    }
}
