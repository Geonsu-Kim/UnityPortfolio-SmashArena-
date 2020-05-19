using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFrame : MonoBehaviour
{
    public float l;
    public float w;
    public float doorNSWidth;
    public float doorEWWidth;
    public Transform[] WallNS = new Transform[4];
    public Transform[] DoorNS = new Transform[2];
    public Transform[] WallEW = new Transform[4];
    public Transform[] DoorEW = new Transform[2];
    private void OnDrawGizmos()
    {
        for (int i = 0; i < 2; i++)
        {
            DoorNS[i].localScale = new Vector3(doorNSWidth, 6f, 1f);
            DoorEW[i].localScale = new Vector3(1f, 6f, doorEWWidth);
        }
        for (int i = 0; i < 4; i++)
        {
            WallNS[i].localScale = new Vector3((w - doorNSWidth) * 0.5f, 6, 1);
            WallEW[i].localScale = new Vector3(1, 6, (l - doorEWWidth) * 0.5f);
        }

        DoorNS[0].position = transform.position+new Vector3(0,3f,0.5f+l*0.5f);
        DoorNS[1].position = transform.position + new Vector3(0, 3f, -(0.5f + l * 0.5f));
        DoorEW[0].position = transform.position + new Vector3(0.5f + w * 0.5f, 3f,0 );
        DoorEW[1].position = transform.position + new Vector3(-(0.5f + w * 0.5f), 3f, 0);

        WallNS[0].position = transform.position + new Vector3((w + doorNSWidth) *0.25f, 3f, 0.5f + l * 0.5f);
        WallNS[1].position = transform.position + new Vector3(-(w + doorNSWidth) * 0.25f, 3f, 0.5f + l * 0.5f);
        WallNS[2].position = transform.position + new Vector3((w + doorNSWidth) * 0.25f, 3f, -(0.5f + l * 0.5f));
        WallNS[3].position = transform.position + new Vector3(-(w + doorNSWidth) * 0.25f, 3f, -(0.5f + l * 0.5f));


        WallEW[0].position = transform.position + new Vector3(0.5f + w * 0.5f, 3f, (l + doorEWWidth) * 0.25f);
        WallEW[1].position = transform.position + new Vector3(0.5f + w * 0.5f, 3f, -(l + doorEWWidth) * 0.25f);
        WallEW[2].position = transform.position + new Vector3(-(0.5f + w * 0.5f), 3f, (l + doorEWWidth) * 0.25f);
        WallEW[3].position = transform.position + new Vector3(-(0.5f + w * 0.5f), 3f, -(l + doorEWWidth) * 0.25f);

    }
}
