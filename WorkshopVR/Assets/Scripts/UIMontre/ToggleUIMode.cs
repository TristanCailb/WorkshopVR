using UnityEngine;
using VRTK;

public class ToggleUIMode : MonoBehaviour
{
    public bool isInUiMode;
    public GameObject uiMontre; //Menu UI de la montre
    public VRTK_Pointer rcPointer; //Element pointer du controller droit
    public VRTK_BezierPointerRenderer rcBezierPointerRenderer; //Element Bezier renderer du controller droit
    public VRTK_StraightPointerRenderer rcStraightPointerRenderer; //Element Straight pointer renderer du controller droit

    public void ToggleMode()
    {
        isInUiMode = !isInUiMode;
        if(isInUiMode)
        {
            rcPointer.pointerRenderer = rcStraightPointerRenderer;
            rcPointer.enableTeleport = false;
            uiMontre.SetActive(true);
        }
        else
        {
            rcPointer.pointerRenderer = rcBezierPointerRenderer;
            rcPointer.enableTeleport = true;
            uiMontre.SetActive(false);
        }
    }
}
