using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;


namespace Quiz.GUI
{
    public class LeaderBoardTable : MonoBehaviour
    {
        public void Fill(ScoreData scoreData)
        {
            var sorted = scoreData.ScoreTable.OrderByDescending(x => (System.Int64) x["score"]).ToList();
            for (int i = 0; i < 10; ++i)
            {
                var child = transform.GetChild(i);
                child.Find("Name").GetComponent<Text>().text = (string) sorted[i]["name"];
                child.Find("Score").GetComponent<Text>().text = ((Int64) (sorted[i]["score"])).ToString();
            }
        }
    }
}