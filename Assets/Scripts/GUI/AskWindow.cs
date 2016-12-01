using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class AskWindow : MonoBehaviour
{
    public bool Result;
    private int Slot;

    public void OnModalResult(bool res)
    {
        Result = res;
        gameObject.SetActive(false);

        // todo: use delegate callback. Hardcode for now
        if (Result)
        {
            Debug.Log("delete " + Slot);
            var accState = GameManager.Instance.CurMode as StateAccounts;
            Assert.IsNotNull(accState);
            accState.Accounts[Slot].Delete();
            accState.UpdateViews();
        }
    }

    public void SetOnResult(int slot)
    {
        Slot = slot;
    }
}
