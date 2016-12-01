using Alg;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : Singleton<GameManager>
{
    public IAppState[] Modes;

    public IAppState CurMode { get; private set; }

    // Use this for initialization
    void Start ()
    {
       Start(typeof(StateAccounts), false);
    }

    public void Start(Type mode, bool animated)
    {
        if (null != CurMode && CurMode.GetType().Equals(mode))
            return;

        var next = Modes.FirstOrDefault(s => s.GetType().Equals(mode));
        Assert.IsNotNull(next);

        // hope StateLeave won't call Start
        if (null != CurMode)
            CurMode.StateLeave(animated);

        CurMode = next;
        CurMode.StateEnter(animated);
    }

#region modal routines

    public InputWindow InputWindow;
    public AskWindow AskWindow;


    public void ShowInputWindow()
    {
        InputWindow.gameObject.SetActive(true);
        InputWindow.Focus();
    }

    public void ShowAskWindow(int slot)
    {
        AskWindow.SetOnResult(slot); // todo:
        AskWindow.gameObject.SetActive(true);
    }
#endregion


}
