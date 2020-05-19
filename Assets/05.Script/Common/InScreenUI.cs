using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InScreenUI:MonoBehaviour
{
    // Start is called before the first frame update
    private Camera enemyCamera;
    private Canvas enemyCanvas;
    private RectTransform rectParent;
    private RectTransform rectHp;

    [HideInInspector] public Vector3 offset = new Vector3(0, 2.2f, 0);
     public Transform targetTr;

    // Use this for initialization
    void Start()
    {
        enemyCanvas = GetComponentInParent<Canvas>();
        enemyCamera = enemyCanvas.worldCamera;
        rectParent = enemyCanvas.GetComponent<RectTransform>();
        rectHp = this.gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset);
        if (screenPos.z < 0.0f)
        {
            screenPos *= -1.0f;
        }
        var localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, enemyCamera, out localPos);
        rectHp.localPosition = localPos;
    }
}
