using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMainCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

}
