using UnityEngine;
using UnityEngine.UI;


namespace Quiz.GUI
{
    public class AnswerItem : MonoBehaviour
    {
        public Text Text;


        private void Reset()
        {
            Text = transform.FindChild("Text").GetComponent<Text>();
        }
    }
}