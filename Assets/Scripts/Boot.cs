using System.Collections;
using System.Collections.Generic;
using Framework.Asset;
using Framework.Config;
using UnityEngine;

public class Boot : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        YooAssetHelper.Init();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
