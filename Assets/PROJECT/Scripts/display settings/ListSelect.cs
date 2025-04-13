using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ListSelect : MonoBehaviour
{
    public List<string> OptionList = new List<string>();
    public int CurrentIndex = 0;
    public string extraText;

    [SerializeField] TextMeshProUGUI OptionText;

    [SerializeField] Button NextButton;
    [SerializeField] Button PreviousButton;
    public UnityEvent OnPress;


    private void Awake()
    {
        OptionText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        NextButton = transform.GetChild(2).GetComponent<Button>();
        PreviousButton = transform.GetChild(1).GetComponent<Button>();

        NextButton.onClick.AddListener(Next);
        PreviousButton.onClick.AddListener(Previous);
    }

    public void Next()
    {
        CurrentIndex++;
        if (CurrentIndex >= OptionList.Count)
        {
            CurrentIndex = 0;
        }
        UpdateIndexAndText(CurrentIndex);
        OnPress.Invoke();
      
    }

    public void Previous()
    {
        CurrentIndex--;
        if (CurrentIndex < 0)
        {
            CurrentIndex = OptionList.Count - 1;
        }
       
        UpdateIndexAndText(CurrentIndex);
        OnPress.Invoke();
    }

    public void UpdateIndexAndText(int index)
    {
        CurrentIndex = index;
        OptionText.text = OptionList[CurrentIndex] + extraText;
    }
}
