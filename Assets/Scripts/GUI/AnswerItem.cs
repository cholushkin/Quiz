﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnswerItem : MonoBehaviour
{
    public Text Text;

    void Reset()
    {
        Text = transform.FindChild("Text").GetComponent<Text>();
    }
}