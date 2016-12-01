using UnityEngine;
using System.Collections;


namespace Quiz
{
    public class Account : MonoBehaviour
    {
        public string duid;
        public string Name;
        public int OvertimeLosses;
        public int Wins;
        public int GamePlayed;


        // Use this for initialization
        void Awake()
        {
            duid = SystemInfo.deviceUniqueIdentifier;
        }

        // Update is called once per frame
        int CalcScores()
        {
            float eff = (OvertimeLosses + (2 * Wins)) / (float)(2 * GamePlayed);
            return (int)(eff * Wins * 1000);
        }

        [ContextMenu("DbgCalcScores")]
        void DbgCalcScores()
        {
            Debug.Log(CalcScores());
        }
    }


}
