using UnityEngine;
using UnityEngine.UI;



namespace Quiz.GUI
{
    public class StarProgressBar : MonoBehaviour
    {
        public int Max;
        public int CurVal;
        public Image Image;

        public void Set(int val)
        {
            CurVal = val;
            Image.fillAmount = CurVal/(float) Max;
        }
        // todo: animation
    }
}