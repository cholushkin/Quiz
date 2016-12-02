using UnityEngine;
using System.Collections;
using Alg;
using Quiz;

public class GameContext : Singleton<GameContext>
{
    public Account CurAccount;


    public int SessionLifesCount;
    public int SessionCorrectAnswerChain;

    public void StartNewSession()
    {
        SessionLifesCount = Setup.LifesCount;
        SessionCorrectAnswerChain = 0;
    }


    [ContextMenu("Reset Save")]
    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();
    }
}
