using UnityEngine;
using UnityEngine.Assertions;

namespace Quiz.GUI
{
    // todo: rewrite, just provide result 
    public class AskWindow : MonoBehaviour
    {
        public bool Result;
        private int Slot;


        public void OnModalResult(bool res)
        {
            Result = res;
            gameObject.SetActive(false);

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
}