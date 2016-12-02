using UnityEngine;
using UnityEngine.UI;

namespace Quiz.GUI
{
    public class ClockProgressBar : MonoBehaviour
    {
        public float Max;
        public float CurVal;
        public Image Image;

        public void Set(float val)
        {
            CurVal = val;
            Image.fillAmount = CurVal/Max;
        }
    }
}