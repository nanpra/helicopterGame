using DG.Tweening;
using UnityEngine;

public class GroundFollow : MonoBehaviour
{
    public Transform heli;

    // Update is called once per frame
    void Update()
    {   
        if(GameManager.Instance.CurrentState == GameState.Playing)
        {
            transform.position = new Vector3(heli.position.x, transform.position.y, heli.position.z);
        }
    }
}
