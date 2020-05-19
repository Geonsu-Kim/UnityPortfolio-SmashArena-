using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TouchPad : MonoBehaviour
{

    private RectTransform touchPad;
    private int touchID = -1;
    private Vector3 startPos = Vector3.zero;
    private Vector3 diff;
    private float dragRadius = 80f;
    public PlayerMovement player;
    private bool buttonPressed = false;
    // Start is called before the first frame update
    void Start()
    {
        touchPad = GetComponent<RectTransform>();
        startPos = touchPad.position;
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER

        HandleInput(Input.mousePosition);

#endif
        HandleTouchInput();
    }
    public void ButtonDowm()
    {
        buttonPressed = true;
    }
    public void ButtonUp()
    {
        buttonPressed = false;
        HandleInput(Vector3.zero);
    }
    void HandleTouchInput()
    {
        int i = 0;
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                i++;
                Vector3 touchPos = new Vector3(touch.position.x, touch.position.y);
                if (touch.phase == TouchPhase.Began)
                {
                    if (touch.position.x <= (startPos.x + dragRadius))
                    {
                        touchID = i;
                    }
                }
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (touchID == i)
                    {
                        HandleInput(touchPos);
                    }
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    if(touchID==i)
                    {
                        touchID = -1;
                    }
                }
            }
        }
    }
    void HandleInput(Vector3 input)
    {
        if (buttonPressed)
        {
            Vector3 diffVector = (input - startPos);
            if (diffVector.sqrMagnitude > dragRadius * dragRadius)
            {
                diffVector.Normalize();
                touchPad.position = startPos + diffVector * dragRadius;
            }
           
             else
            {
                touchPad.position = input;
            }
        }
        else
        {
            touchPad.position = startPos;
            
        }
        diff = touchPad.position - startPos;
        Vector2 normDiff = new Vector3(diff.x / dragRadius, diff.y / dragRadius);
        if (player != null)
        {
            player.OnStickChanged(normDiff);
        }
    }
}
