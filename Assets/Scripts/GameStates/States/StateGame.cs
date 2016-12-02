using System.Collections;
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
    public LifeProgressBar LifeProgressBar;
    public StarProgressBar StarProgressBar;
    public ClockProgressBar ClockProgressBar;
    private bool isInputAllowed = false;

    public int RightAnswer;

    public override void StateEnter(bool animated)
    {
        LifeProgressBar.Set(2);
        StarProgressBar.Set(0);
        GameContext.Instance.StartNewSession();
        gameObject.SetActive(true);
        SetFieldInitialState();
        NextQuestionStart();
    }

    private void SetFieldInitialState()
    {
        TimeLimit = Setup.QuestionTimeLimit;
        ClockProgressBar.Set(TimeLimit);
        foreach (var answerItem in Answers)
            answerItem.Text.color = new Color(1,1,1,0);
        QuestionText.color = new Color(1, 1, 1, 0);
    }

    private IEnumerator RemoveCards()
    {
        TimeLimit = Setup.QuestionTimeLimit;
        ClockProgressBar.Set(TimeLimit);
        foreach (var answerItem in Answers)
            answerItem.Text.DOFade(0, 1f);
        QuestionText.DOFade(0, 1f);
        yield return new WaitForSeconds(1f);
        NextQuestionStart();
    }

    private IEnumerator ShowCards()
    {
        foreach (var answerItem in Answers)
            answerItem.Text.DOFade(1f, 1f);
        QuestionText.DOFade(1f, 1f);
        yield return new WaitForSeconds(1f);
        isInputAllowed = true;
        TimeLimit = Setup.QuestionTimeLimit;
        ClockProgressBar.Set(TimeLimit);
    }

    private void NextQuestionStart()
    {
        isInputAllowed = false;
        LoadingIcon.Instance.Show(true);
        DataAccessor.Instance.GetRandomData();
    }


    public override void StateLeave(bool animated)
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (DataAccessor.Instance.Data != null)
            if (!DataAccessor.Instance.Data.IsLoading)
            {
                ShowQuestion(DataAccessor.Instance.Data);
            }
        if (DataAccessor.Instance.Data == null && isInputAllowed) // we are inside question state
        {
            if (TimeLimit <= 0f)
            {
                OnAnswerHit(-1);
                return;
            }
            if(!CheatMode)
                TimeLimit -= Time.deltaTime;
            ClockProgressBar.Set(TimeLimit);
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

        QuestionText.text = data.Question.question + (CheatMode?(1+RightAnswer).ToString():"");
        StartCoroutine(ShowCards());
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

    // cheat
    private bool CheatMode = false;
    int clockHitCnt;
    public void OnClockHit()
    {
        clockHitCnt++;
        if (clockHitCnt >= 5)
            CheatMode = true;
    }

    public void OnAnswerHit(int cardID)
    {
        if (!isInputAllowed)
            return;
        if (DataAccessor.Instance.Data != null) // multiple tap check
            return;

        Debug.Log("Card hited: " + cardID);


        if (RightAnswer == cardID) // correct answer
        {
            BlinkBg(Color.green);
            GameContext.Instance.SessionCorrectAnswerChain++;
            StarProgressBar.Set(GameContext.Instance.SessionCorrectAnswerChain);
            if (GameContext.Instance.SessionCorrectAnswerChain >= Setup.WinCountInRaw)
            {
                FinishPlay();
                return;
            }
        }
        else // incorrect answer
        {
            BlinkBg(Color.red);
            GameContext.Instance.SessionLifesCount--;
            GameContext.Instance.SessionCorrectAnswerChain = 0;
            LifeProgressBar.Set(GameContext.Instance.SessionLifesCount);
            StarProgressBar.Set(GameContext.Instance.SessionCorrectAnswerChain);
            if (GameContext.Instance.SessionLifesCount <= 0)
            {
                FinishPlay();
                return;
            }
        }

        isInputAllowed = false;
        StartCoroutine(RemoveCards());
        //NextQuestion();
    }

    private void FinishPlay()
    {
        bool isLose = GameContext.Instance.SessionLifesCount < 1;
        bool isNonPerfectWin = (GameContext.Instance.SessionLifesCount < 2) && !isLose;
        bool isPerfectWin = (GameContext.Instance.SessionLifesCount == Setup.LifesCount) && !isLose;

        Debug.Log("isLose " + isLose);
        Debug.Log("isNonPerfectWin " + isNonPerfectWin);
        Debug.Log("isPerfectWin " + isPerfectWin);

        GameContext.Instance.CurAccount.Data.NonPerfectWins += isNonPerfectWin ? 1 : 0;
        GameContext.Instance.CurAccount.Data.PerfectWins += isPerfectWin ? 1 : 0;
        GameContext.Instance.CurAccount.Data.OvertimeLosses += isLose ? 1 : 0;
        GameContext.Instance.CurAccount.Save();
        var resState = GameManager.Instance.GetMode(typeof (StateResults)) as StateResults;
        resState.SetIsLose(isLose);
        GameManager.Instance.Start(typeof(StateResults), false);
    }

    private void BlinkBg(Color dstColor )
    {
        Bg.DOColor(dstColor, 1f).OnComplete(()=>Bg.DOColor(Color.white, 1f));
    }
}
