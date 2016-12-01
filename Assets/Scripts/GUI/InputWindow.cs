using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputWindow : MonoBehaviour
{
    public InputField InputField;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public void OnApplyInput()
    {
        // todo: check usr input
        var usrName = InputField.text;
        if(usrName.Length < 3 || usrName.Length > 15)
            return;

        GameContext.Instance.CurAccount.Data.Name = usrName;
        GameContext.Instance.CurAccount.Save();
        gameObject.SetActive(false);
        GameManager.Instance.Start(typeof(StateGame),false);
    }

    public void Focus()
    {
        EventSystem.current.SetSelectedGameObject(InputField.gameObject, null);
        //InputField.OnPointerClick(null);
    }
}
