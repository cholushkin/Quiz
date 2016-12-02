using UnityEngine;
using System.Collections;
using System;

public class StateResults : IAppState
{
    public override void StateEnter(bool animated)
    {
        gameObject.SetActive(true);
    }

    public override void StateLeave(bool animated)
    {
        gameObject.SetActive(false);
    }

    public void OnRestartTap()
    {
        GameManager.Instance.Start(typeof(StateGame),false);
    }

    public void SetIsLose(bool isLose)
    {
    }
}
