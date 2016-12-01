using System;
using UnityEngine;
using UnityEngine.Assertions;


namespace Quiz
{
    [Serializable]
    public class Account
    {
        [Serializable]
        public class AccountData
        {
            public int Slot = -1;
            public string Duid;
            public string Name;
            public int OvertimeLosses;
            public int Wins;

            public int CalcScores()
            {
                int gamePlayed = Wins + OvertimeLosses;
                if (gamePlayed == 0)
                    return 0;
                float eff = (OvertimeLosses + (2 * Wins)) / (float)(2 * gamePlayed);
                return (int)(eff * Wins * 1000);
            }
        }


        // todo: incapsulate me
        public AccountData Data = new AccountData();
        public int Slot;

        // Use this for initialization
        public Account(int slot)
        {
            Assert.IsTrue(slot >= 0 && slot <= 3);
            Slot = slot;
            Data.Duid = SystemInfo.deviceUniqueIdentifier;
        }

     
        public Account Load()
        {
            var json = PlayerPrefs.GetString("Slot" + Slot);
            var data = JsonUtility.FromJson<AccountData>(json);
            if (data != null)
                Data = data;
            return this;
        }

        public void Save()
        {
            PlayerPrefs.SetString("Slot" + Slot, JsonUtility.ToJson(Data));
        }

        public void Delete()
        {
            PlayerPrefs.DeleteKey("Slot" + Slot);
            Data = new AccountData();
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Data.Name);
        }
    }


}
