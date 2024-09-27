using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectNoDrag : ScrollRect
{
    public PuzzleMgr puzzleMgr;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (puzzleMgr)
        {
            puzzleMgr.OnBeginDrag(eventData);
        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (puzzleMgr)
        {
            puzzleMgr.OnDrag(eventData);
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (puzzleMgr)
        {
            puzzleMgr.OnEndDrag(eventData);
        }
    }
}
