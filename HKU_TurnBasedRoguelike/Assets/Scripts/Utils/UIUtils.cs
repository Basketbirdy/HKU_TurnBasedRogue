using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIUtils
{
    public static void EnableAll(UIObject[] uiObjects)
    {
        foreach (UIObject obj in uiObjects)
        {
            obj.obj.SetActive(true);
        }
    }    
    
    public static void DisableAll(UIObject[] uiObjects)
    {
        foreach (UIObject obj in uiObjects)
        {
            obj.obj.SetActive(false);
        }
    }

    public static void EnableSpecified(UIObject[] uiObjects, string name)
    {
        foreach (UIObject obj in uiObjects)
        {
            if (obj.objName != name) { continue; }
            obj.obj.SetActive(true);
        }
    } 
    
    public static void DisableSpecified(UIObject[] uiObjects, string name)
    {
        foreach (UIObject obj in uiObjects)
        {
            if (obj.objName != name) { continue; }
            obj.obj.SetActive(false);
        }
    }
}
