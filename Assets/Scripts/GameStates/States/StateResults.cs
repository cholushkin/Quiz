using System;
using Quiz;
using UnityEngine.UI;

public class StateResults : IAppState
{
    public ImageSwitch ImgSwitcher;
    public Text StatusText;
    public LeaderBoardTable LeaderBoardTbl;

    public override void StateEnter(bool animated)
    {
        gameObject.SetActive(true);
        LoadingIcon.Instance.Show(true);
        DataAccessor.Instance.UpdateLeaderBoard(GameContext.Instance.CurAccount);
    }

    public override void StateLeave(bool animated)
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (DataAccessor.Instance.ScoreData == null)
            return;
        if (DataAccessor.Instance.ScoreData.IsLoading == false)
        {
            LoadingIcon.Instance.Show(false);
            FillView(DataAccessor.Instance.ScoreData);
            DataAccessor.Instance.ScoreData = null;
        }
    }

    private void FillView(ScoreData scoreData)
    {
        LeaderBoardTbl.Fill(scoreData);
    }

    public void OnRestartTap()
    {
        GameManager.Instance.Start(typeof(StateGame), false);
    }

    public void SetIsLose(bool isLose)
    {
        ImgSwitcher.Switch(isLose ? 0 : 1);
        StatusText.text = isLose ? "You lose" : "You win!";
    }
}
