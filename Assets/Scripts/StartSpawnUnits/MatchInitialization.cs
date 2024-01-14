using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MatchInitialization : MonoBehaviour
{
    [SerializeField] private GameObject eventManager;

    private void Awake()
    {
        if (CoreBooter.instance != null)
        {
            MatchInitEnd();
        }else{
#if UNITY_EDITOR
            eventManager.SetActive(true);
#endif
        }
    }

    private void MatchInitEnd()
    {
        
        // GameData data = GameData.Instance;
        // if (data == null)
        // {
        //     Object.FindFirstObjectByType<CameraManager>().InitializeBounds();
        EventManager.TriggerEvent("LoadedScene");
        // }
    }
}
