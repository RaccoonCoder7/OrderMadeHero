using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// alphaHitTestMinimumThreshold 설정을 위한 컴포넌트
/// </summary>
public class DetailImage : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
}
