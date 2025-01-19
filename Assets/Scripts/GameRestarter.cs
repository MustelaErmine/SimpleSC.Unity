using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRestarter : MonoBehaviour
{
    [SerializeField] SessionHandler sessionHandler;
    public void OnGameEnd()
    {
        sessionHandler.RestartGame();
    }
}
