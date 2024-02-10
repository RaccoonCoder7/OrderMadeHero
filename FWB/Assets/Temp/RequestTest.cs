using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestTest : MonoBehaviour
{
    public Text getVal;
    public Text ReqText;
    public Text Output;
    public Button refBtn;
    public TestState testState;
    public ConditionTable requestTable;

    public enum TestState
    {
        wait,
        ok,
        no
    }

    private void Start()
    {
        var requestList = requestTable.conditionList;
        var request = requestList[UnityEngine.Random.Range(0, requestList.Count)];
        // ReqText.text = request.requestName;
        testState = TestState.wait;
        StartCoroutine(caseMach());
    }
    
    private void Update()
    {
        refBtn.onClick.AddListener(Refresh);
    }

    void Refresh()
    {
        var requestList = requestTable.conditionList;
        var request = requestList[UnityEngine.Random.Range(0, requestList.Count)];
        // ReqText.text = request.requestName;
    }

    private IEnumerator caseMach()
    {
        switch (testState)
        {
            case TestState.wait:
                Output.text = "waiting";
                break;
            case TestState.no:
                Output.text = "fail";
                break;
            case TestState.ok:
                Output.text = "pass";
                break;
        }

        yield return null;
    }
}
