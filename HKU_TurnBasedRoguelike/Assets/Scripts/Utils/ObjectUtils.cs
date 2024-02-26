using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectUtils
{
    public static void EnableObject(GameObject obj)
    {
        obj.SetActive(true);
    }    
    
    public static void DisableObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public static void ToggleObject(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }
}
