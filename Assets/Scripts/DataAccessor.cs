using System;
using UnityEngine;
using System.Collections.Generic;
using Alg;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Quiz
{
    #region score
    public class ScoreData
    {
        public bool IsLoading = true;
        public List<Dictionary<string, object>> ScoreTable;
    }

    public class Topscore
    {
        private static string DbUrl = "digairquiz.firebaseio.com";
        private Firebase DataBase;
        private List<Dictionary<string, object>> ScoreTable = new List<Dictionary<string, object>>();
        private ScoreData ScoreDataOutput;
        private Account Acc;

        public Topscore()
        {
            DataBase = Firebase.CreateNew(DbUrl);
            DataBase.OnGetSuccess += GetOKHandler;
            DataBase.OnGetFailed += GetFailHandler;
            DataBase.OnSetSuccess += SetOKHandler;
        }

        void GetOKHandler(Firebase sender, DataSnapshot snapshot)
        {
            Debug.Log("[OK] Get from key: <" + sender.FullKey + ">");
            Debug.Log("[OK] Raw Json: " + snapshot.RawJson);

            // custom convertion
            var tmp = Json.Deserialize(snapshot.RawJson) as List<object>;
            foreach (var obj in tmp)
                ScoreTable.Add(obj as Dictionary<string, object>);

            Modify();
            ScoreDataOutput.IsLoading = false;
        }

        private void Modify()
        {
            // var a = ScoreTable.OrderBy(x => (System.Int64) x["score"]).ToList();
            // analize list
            int minScoreIndex = 0;
            int accIndex = -1;
            Int64 minVal = (System.Int64)(ScoreTable[0]["score"]);
            for(int index = 0; index < ScoreTable.Count; ++index)
            {
                var score = (Int64)ScoreTable[index]["score"];
                if (score < minVal)
                {
                    minVal = score;
                    minScoreIndex = index;
                }

                if ((string)ScoreTable[index]["name"] == Acc.Data.Name && (string)ScoreTable[index]["duid"] == Acc.Data.Duid)
                    accIndex = index;
            }

            bool canPutIn = Acc.Data.CalcScores() > minVal;

            if (canPutIn)
            {
                // prepare dict for the cur acc
                var dic = new Dictionary<string, object>();
                dic["duid"] = Acc.Data.Duid;
                dic["name"] = Acc.Data.Name;
                dic["score"] = Acc.Data.CalcScores();

                // put the new value
                if(accIndex!=-1)
                    ScoreTable.RemoveAt(accIndex);
                else
                    ScoreTable.RemoveAt(minScoreIndex);

                ScoreTable.Add(dic);
            }
        }

        void SetOKHandler(Firebase sender, DataSnapshot snapshot)
        {
            Debug.Log("[OK] Set from key: <" + sender.FullKey + ">");
        }

        void GetFailHandler(Firebase sender, FirebaseError err)
        {
            Debug.LogError("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");
            // SendRequest();
        }

        public ScoreData UpdateLeaderBoard(Account acc)
        {
            Acc = acc;
            ScoreTable = new List<Dictionary<string, object>>();

            ScoreDataOutput = new ScoreData();
            ScoreDataOutput.IsLoading = true;
            ScoreDataOutput.ScoreTable = ScoreTable;

            SendRequest();

            return ScoreDataOutput;
        }

        private void SendRequest()
        {
            FirebaseQueue firebaseQueue = new FirebaseQueue();
            firebaseQueue.AddQueueGet(DataBase.Child("0", true), FirebaseParam.Empty.OrderByKey().LimitToFirst(10));
            firebaseQueue.AddQueueSet(DataBase.Child("0"), ScoreTable);
        }
    }
    #endregion
    
    #region questions
    [Serializable]
    public class Data
    {
        public bool IsLoading = true;
        public QuizQuestion Question;
    }

    interface IDataAccessor
    {
        Data GetData(int id);
    }

    class FireBaseDataAcessor : IDataAccessor
    {
        private static string DbUrl = "digairquiz.firebaseio.com";

        private Firebase DataBase;
        private Data OutputData;
        private int CurrentId;

        public FireBaseDataAcessor()
        {
            DataBase = Firebase.CreateNew(DbUrl);
            DataBase.OnGetSuccess += GetOKHandler;
            DataBase.OnGetFailed += GetFailHandler;
        }

        // get question data
        public Data GetData(int id)
        {
            OutputData = new Data();
            CurrentId = id;
            DataBase.Child("1", true).Child("pack00", true).Child(id.ToString(), true).GetValue(FirebaseParam.Empty.OrderByKey().LimitToFirst(6));
            return OutputData;
        }


        void GetOKHandler(Firebase sender, DataSnapshot snapshot)
        {
            Debug.Log("[OK] Get from key: <" + sender.FullKey + ">");
            Debug.Log("[OK] Raw Json: " + snapshot.RawJson);

            int rID = Convert.ToInt32(sender.Key);

            var quizQuestion = JsonUtility.FromJson<QuizQuestion>(snapshot.RawJson);
            OutputData.IsLoading = false;
            OutputData.Question = quizQuestion;
        }


        void GetFailHandler(Firebase sender, FirebaseError err)
        {
            Debug.LogError("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");

            // resend request on connetion problems
            DataBase.Child("1", true).Child("pack00", true).Child(CurrentId.ToString(), true).GetValue(FirebaseParam.Empty.OrderByKey().LimitToFirst(6));
        }


    }
    #endregion

    #region singleton
    public class DataAccessor : Singleton<DataAccessor>
    {
        private IDataAccessor DAccessor;
        private Topscore TopScoreAccessor;
        [HideInInspector]
        public Data Data; // todo: incapsulate and use delegates instead isLoading

        public ScoreData ScoreData;

        void Start()
        {
            DAccessor = new FireBaseDataAcessor();
            TopScoreAccessor = new Topscore();
        }

        public void UpdateLeaderBoard(Account acc)
        {
            ScoreData = TopScoreAccessor.UpdateLeaderBoard(acc);
        }

        [ContextMenu("GetRandomData")]
        public void GetRandomData()
        {
            // todo: make unique random list [0..424]
            Data = DAccessor.GetData(Random.Range(0, 425));
        }
    }
    #endregion
}
