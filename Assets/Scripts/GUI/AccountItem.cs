using UnityEngine;
using System.Collections;
using Quiz;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class AccountItem : MonoBehaviour
{
    private Account Acc;
    public Text UserName;
    public Text UserScore;
    public Button CloseButton;

    public void Reset()
    {
        UserName = transform.FindChild("LabelName").GetComponent<Text>();
        UserScore = transform.FindChild("LabelScore").GetComponent<Text>();
        CloseButton = transform.FindChild("DeleteButton").GetComponent<Button>();
    }

    public void Set(Account acc)
    {
        Acc = acc;
        UpdateView();
    }

    public void UpdateView()
    {
        if (Acc.IsEmpty())
        {
            UserName.text = "New account";
            UserName.gameObject.SetActive(true);
            UserScore.gameObject.SetActive(false);
            CloseButton.gameObject.SetActive(false);
        }
        else
        {
            // fill with data
            UserName.text = Acc.Data.Name;
            UserScore.text = Acc.Data.CalcScores().ToString();
            UserName.gameObject.SetActive(true);
            UserScore.gameObject.SetActive(true);
        }
    }
}
