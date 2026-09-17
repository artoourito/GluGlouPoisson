using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private TMP_Text timerTxt;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timerTxt.text = GameManager.Instance.Timer.ToString("0");
    }
}
