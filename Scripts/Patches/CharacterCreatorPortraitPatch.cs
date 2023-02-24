using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using UnboundLib;
using InControl;

[HarmonyPatch(typeof(CharacterCreatorPortrait))]
public class CharacterCreatorPortraitPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    private static void Start(CharacterCreatorPortrait __instance)
    {
        var parentMenu = __instance.GetComponentInParent<ListMenuPage>();
        if (parentMenu == null || parentMenu.gameObject.name != "Online")
        {
            return;
        }

        UnityEngine.Debug.Log($"Added additional context menu to face selection button {__instance.gameObject.name}");
        var faceButtonOptions = GameObject.Instantiate(Assets.faceButtonOptions, __instance.GetComponentInParent<Canvas>().transform);
        faceButtonOptions.Initialize(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch("EditCharacter")]
    private static bool EditCharacter(CharacterCreatorPortrait __instance)
    {
        // ignore override if controller is in use
        for (int i = 0; i < InputManager.ActiveDevices.Count; i++)
        {
            if (InputManager.ActiveDevices[i].Action4.WasPressed && ((HoverEvent)__instance.GetFieldValue("hoverEvent")).isSelected)
            {
                return true;
            }
        }

        // override default behavior unless custom context menu is shown
        if (FaceButtonOptions.registeredContextMenus.TryGetValue(__instance, out var contextMenu) && !contextMenu.gameObject.activeInHierarchy)
        {
            return false;
        }

        //return GameObject.FindObjectsOfType<FaceButtonOptions>().Count(fb => fb.targetPortrait == __instance && fb.gameObject.activeInHierarchy) > 0;
        return true;
    }
}

public class OnDestroyEvent : MonoBehaviour
{
    private Action callback;

    public OnDestroyEvent Initialize(Action callback)
    {
        this.callback = callback;
        return this;
    }

    private void OnDestroy()
    {
        callback?.Invoke();
    }
}

public class OnDisableEvent : MonoBehaviour
{
    private Action callback;

    public OnDisableEvent Initialize(Action callback)
    {
        this.callback = callback;
        return this;
    }

    private void OnDisable()
    {
        callback?.Invoke();
    }
}
