using Alg;
using System;
using System.Linq;
using Quiz.GUI;
using UnityEngine.Assertions;


namespace Quiz
{
    public class GameManager : Singleton<GameManager>
    {
        public IAppState[] Modes;
        public IAppState CurMode { get; private set; }


        void Start()
        {
            Start(typeof(StateAccounts), false);
        }


        // todo: support fade in/ fade out on mode changes
        public void Start(Type mode, bool animated)
        {
            if (CurMode != null && CurMode.GetType().Equals(mode))
                return;

            var next = Modes.FirstOrDefault(s => s.GetType().Equals(mode));
            Assert.IsNotNull(next);

            // hope StateLeave will not call Start
            if (CurMode != null)
                CurMode.StateLeave(animated);

            CurMode = next;
            CurMode.StateEnter(animated);
        }


        public IAppState GetMode(Type mode)
        {
            return Modes.FirstOrDefault(s => s.GetType() == mode);
        }


        // todo: move in static methods of window
        #region modal routines

        public InputWindow InputWindow;
        public AskWindow AskWindow;


        public void ShowInputWindow()
        {
            InputWindow.gameObject.SetActive(true);
            InputWindow.Focus();
        }


        public void ShowAskWindow(int slot)
        {
            AskWindow.SetOnResult(slot); // todo:
            AskWindow.gameObject.SetActive(true);
        }
        #endregion
    }
}
