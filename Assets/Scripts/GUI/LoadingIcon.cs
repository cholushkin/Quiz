using Alg;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Quiz
{
    public class LoadingIcon : Singleton<LoadingIcon>
    {
        void Start()
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 0f);
            GetComponent<Image>().rectTransform.DORotate(new Vector3(0, 0, -360), 2.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        }

        public void Show(bool flag)
        {
            GetComponent<Image>().color = new Color(1,1,1,flag?0f:1f);
            GetComponent<Image>().DOFade( flag ? 1f : 0f, 1f);
        }
 
    }
}

