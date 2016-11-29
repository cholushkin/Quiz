using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using SimpleFirebaseUnity;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Quiz
{
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
            DataBase.Child(id.ToString(), true).GetValue(FirebaseParam.Empty.OrderByKey().LimitToFirst(6));

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
        }


    }


    public class DataAccessor : MonoBehaviour
    {
        private IDataAccessor DAccessor;
        private Data Data;

        void Start()
        {
            DAccessor = new FireBaseDataAcessor();
        }

        // Update is called once per frame
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
            if (Data != null)
                GUILayout.Label(Data.IsLoading ? "Loading..." : JsonUtility.ToJson((object)Data.Question));
        }
#endif

    }

}
