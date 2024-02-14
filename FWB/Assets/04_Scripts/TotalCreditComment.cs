using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalCreditComment : MonoBehaviour
{
    public static TotalCreditComment Instance { get; private set; } 
    public Text dayEndMessageText;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        UpdateDayEndMessage();
    }

    public void UpdateDayEndMessage()
    {
        if (GameMgr.In.dayRevenue >= 100) {
            dayEndMessageText.text = "좋아, 오늘은 성공적이야 잘했어!";
        }
        else if (GameMgr.In.dayRevenue >= 1) {
            dayEndMessageText.text = "그렇게 나쁘진 않네! 힘내자!";
        }
        else {
            dayEndMessageText.text = "으음.. 더 노력해야겠는걸?";
        }
    }
}
