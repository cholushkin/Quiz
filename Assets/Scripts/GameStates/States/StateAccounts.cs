using UnityEngine;
using System.Collections;
using Quiz;

public class StateAccounts : IAppState
{
    [HideInInspector]
    public Account[] Accounts;
    public AccountItem[] AccountObjects;

    void Start()
    {
        Accounts = new Account[3];
        for (int i = 0; i < 3; ++i)
        {
            var acc = new Account(i);
            acc.Load();
            Accounts[i] = acc;
            Debug.Log("loading acc: " + acc);
            AccountObjects[i].Set(acc);
        }
    }

    public void UpdateViews()
    {
        for (int i = 0; i < 3; ++i)
            AccountObjects[i].UpdateView();
    }

    public override void StateEnter(bool animated)
    {
        gameObject.SetActive(true);
    }

    public override void StateLeave(bool animated)
    {
        gameObject.SetActive(false);
    }

    public void OnSlotTap(int slot)
    {
        if (Accounts[slot].IsEmpty()) // create new acc
        {
            var newAcc = new Account(slot);
            Accounts[slot] = newAcc;
            GameContext.Instance.CurAccount = newAcc;
            GameManager.Instance.ShowInputWindow();
        }
        else // choose existing one account
        {
            GameContext.Instance.CurAccount = Accounts[slot];
            GameManager.Instance.Start(typeof(StateGame), false);
        }
    }

    public void OnSlotDeleteTap(int slot)
    {
        Debug.Log("intent to del slot:" + slot);
        GameManager.Instance.ShowAskWindow(slot);
    }

    void OnGUI()
    {
        GUI.contentColor = Color.black;
        GUILayout.Label(Setup.VersionString);
    }
}
