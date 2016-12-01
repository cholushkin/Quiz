using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Alg;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Quiz
{
    [Serializable]
    public class Score
    {
        
    }
    [Serializable]
    public class ConvertTempStruc
    {
        public Dictionary<string, object>[] sss;
    }

    //public class RootObject
    //{
    //    public int duid;
    //    public string name;
    //    public int score;
    //}


    public class Topscore
    {
        private static string DbUrl = "digairquiz.firebaseio.com";
        private Firebase DataBase;
        private int CurScore;
        private List<Dictionary<string,object>> ScoreTable = new List<Dictionary<string, object>>();

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
            var tmp = Json.Deserialize(snapshot.RawJson) as List<object>;

            
            //custom covertion
            foreach (var obj in tmp)
            {
                ScoreTable.Add(obj as Dictionary<string,object>);
            }
            ScoreTable[0]["name"] = "asdasd";
            //CurScoreTable = JsonUtility.FromJson<ScoreTable>(snapshot.RawJson);
            //CurScoreTable.scores = CurScoreTable.scores.OrderBy(x => x.score).ToArray();

            //// modify table
            //if (CurScoreTable.scores[0].score < CurScore)
            //{
            //    CurScoreTable.scores[0].score = CurScore;
            //}

            //foreach (var score in CurScoreTable.scores)
            //{
            //    if (score.score < CurScore )
            //        score
            //}

        }

        void SetOKHandler(Firebase sender, DataSnapshot snapshot)
        {
            Debug.Log("[OK] Set from key: <" + sender.FullKey + ">");
        }


        void GetFailHandler(Firebase sender, FirebaseError err)
        {
            Debug.LogError("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");
        }

        public void DbgPrintScore()
        {
            DataBase.Child("0", true).GetValue();
        }

        public void PutScore(string userName, int score)
        {
            CurScore = score;
            ScoreTable = new List<Dictionary<string, object>>();
            FirebaseQueue firebaseQueue = new FirebaseQueue();
            firebaseQueue.AddQueueGet(DataBase.Child("0", true), FirebaseParam.Empty.OrderByKey().LimitToFirst(10));
            firebaseQueue.AddQueueSet(DataBase.Child("0"),ScoreTable);

        }
 
    }




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
        private Dictionary<int, Data> CachedData;
        private int CurrentId;

        public FireBaseDataAcessor()
        {
            DataBase = Firebase.CreateNew(DbUrl);
            DataBase.OnGetSuccess += GetOKHandler;
            DataBase.OnGetFailed += GetFailHandler;

            CachedData = new Dictionary<int, Data>();
        }

        public Data GetData(int id)
        {
            if (CachedData.ContainsKey(id))
                return CachedData[id];

            // create data
            var data = new Data();

            // put in cache
            CachedData.Add(id, data);

            // request to fill data
            CurrentId = id;
            DataBase.Child("1",true).Child("pack00", true).Child(id.ToString(), true).GetValue(FirebaseParam.Empty.OrderByKey().LimitToFirst(6));

            return data;
        }


        void GetOKHandler(Firebase sender, DataSnapshot snapshot)
        {
            Debug.Log("[OK] Get from key: <" + sender.FullKey + ">");
            Debug.Log("[OK] Raw Json: " + snapshot.RawJson);

            int rID = Convert.ToInt32(sender.Key);


            // find in cached quiz questions and fill it up
            Assert.IsTrue(CachedData.ContainsKey(rID));
            var quizQuestion = JsonUtility.FromJson<QuizQuestion>(snapshot.RawJson);
            CachedData[rID].IsLoading = false;
            CachedData[rID].Question = quizQuestion;
        }

        void GetFailHandler(Firebase sender, FirebaseError err)
        {
            Debug.LogError("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");

            // resend request on connetion problems
            DataBase.Child("1", true).Child("pack00", true).Child(CurrentId.ToString(), true).GetValue(FirebaseParam.Empty.OrderByKey().LimitToFirst(6));
        }


    }


    public class DataAccessor : Singleton<DataAccessor>
    {
        private IDataAccessor DAccessor;
        private Topscore TopScoreAccessor;
        [HideInInspector]
        public Data Data; // todo: incapsulate

        void Start()
        {
            DAccessor = new FireBaseDataAcessor();
            TopScoreAccessor = new Topscore();
        }

        [ContextMenu("GetRandomData")]
        public void GetRandomData()
        {
            // todo: make unique random list [0..424]
            Data = DAccessor.GetData(Random.Range(0, 425));
        }

#if DEBUG
        void OnGUI()
        {
            if (GUILayout.Button("get random question"))
                GetRandomData();
            if (GUILayout.Button("print top scores"))
                TopScoreAccessor.PutScore("",666);
            if (Data != null)
                GUILayout.Label(Data.IsLoading ? "Loading..." : JsonUtility.ToJson((object)Data.Question));
        }
#endif

    }

}
