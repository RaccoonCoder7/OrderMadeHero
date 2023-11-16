using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TextAsset ta;

    [ContextMenu("test")]
    void test()
    {
        string a = ta.text;
        string b = string.Copy(ta.text);

        // string c = a.repl;
        string d = string.Copy(b);

        a = "a!";
        b = "b!";

        Debug.Log(a);
        Debug.Log(b);
        // Debug.Log(c);
        Debug.Log(d);
    }
}
