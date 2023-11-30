using UnityEngine;

/// <summary>
/// 게임 플레이 중 각종 이벤트를 정의하는 이벤트플로우 클래스들의 부모 클래스
/// </summary>
public abstract class EventFlow : MonoBehaviour
{
    public string eventKey;
    [HideInInspector]
    public GameSceneMgr mgr;

    /// <summary>
    /// GameSceneMgr에서 StartFlow를 호출하여 제어권을 넘김
    /// </summary>
    public abstract void StartFlow();

    /// <summary>
    /// GameSceneMgr에 제어권을 반환함
    /// </summary>
    public virtual void EndFlow()
    {
        mgr.isEventFlowing = false;
    }
}
