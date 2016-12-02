using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Quiz;
using UnityEngine.UI;

public class LeaderBoardTable : MonoBehaviour
{
    public void Fill(ScoreData scoreData)
    {
        var sorted = scoreData.ScoreTable.OrderByDescending(x => (System.Int64)x["score"]).ToList();
        for (int i = 0; i < 10; ++i)
        {
            var child = transform.GetChild(i);
            child.FindChild("Name").GetComponent<Text>().text = (string)sorted[i]["name"];
            child.FindChild("Score").GetComponent<Text>().text = ((Int64)(sorted[i]["score"])).ToString();
        }
    }
}
