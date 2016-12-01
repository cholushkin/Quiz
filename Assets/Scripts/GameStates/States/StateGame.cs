using DG.Tweening;
using Quiz;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class StateGame : IAppState
{
    public Image Bg;
    private float TimeLimit;
    public AnswerItem[] Answers;
    public Text QuestionText;
    public int RightAnswer;

    public override void StateEnter(bool animated)
    {
        gameObject.SetActive(true);
        NextQuestion();
    }

    private void NextQuestion()
    {
        LoadingIcon.Instance.Show(true);
        DataAccessor.Instance.GetRandomData();

    }

    public override void StateLeave(bool animated)
    {
    }

    private void Update()
    {
        if (DataAccessor.Instance.Data != null)
            if (!DataAccessor.Instance.Data.IsLoading)
            {
                ShowQuestion(DataAccessor.Instance.Data);
            }


    }

    private void ShowQuestion(Data data)
    {
        RightAnswer = ConvertAlphabet2CardId(DataAccessor.Instance.Data.Question.answer);
        LoadingIcon.Instance.Show(false);
        DataAccessor.Instance.Data = null;

        Answers[0].Text.text = data.Question.A;
        Answers[1].Text.text = data.Question.B;
        Answers[2].Text.text = data.Question.C;
        Answers[3].Text.text = data.Question.D;

        QuestionText.text = data.Question.question;
    }

    private int ConvertAlphabet2CardId(string answer)
    {
        Assert.IsTrue(answer=="A" || answer == "B" || answer == "C" || answer == "D");
        if (answer == "B")
            return 1;
        if (answer == "C")
            return 2;
        if (answer == "D")
            return 3;
        return 0; // A
    }

    public void OnAnswerHit(int cardID)
    {
        if (DataAccessor.Instance.Data != null) // multiple tap check
            return;

        Debug.Log("Card hited: " + cardID);


        if (RightAnswer == cardID) // correct answer
        {
            BlinkBg(Color.green);
        }
        else // incorrect answer
        {
            BlinkBg(Color.red);
        }

        NextQuestion();
    }

    private void BlinkBg(Color dstColor )
    {
        Bg.DOColor(dstColor, 1f).OnComplete(()=>Bg.DOColor(Color.white, 1f));
    }
}
