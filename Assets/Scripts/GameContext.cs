using UnityEngine;
using System.Collections;
using Alg;
using Quiz;

public class GameContext : Singleton<GameContext>
{
    public Account CurAccount;

    void Update()
    {

    }


    [ContextMenu("Reset Save")]
    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();
    }
}
