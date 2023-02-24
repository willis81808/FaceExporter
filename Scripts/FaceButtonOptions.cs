using System.Collections;
using System.Collections.Generic;
using UnboundLib;
using UnityEngine;
using UnityEngine.EventSystems;

public class FaceButtonOptions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    internal static Dictionary<CharacterCreatorPortrait, FaceButtonOptions> registeredContextMenus = new Dictionary<CharacterCreatorPortrait, FaceButtonOptions>();

    [SerializeField]
    private GameObject contextForObject;

    private bool mouseInBounds = false;

    internal CharacterCreatorPortrait targetPortrait;

    public void Initialize(CharacterCreatorPortrait targetPortrait)
    {
        this.targetPortrait = targetPortrait;
        contextForObject = targetPortrait.gameObject;
        
        targetPortrait
            .gameObject
            .AddComponent<OnDestroyEvent>()
            .Initialize(() => Destroy(gameObject))
            .gameObject
            .AddComponent<OnDisableEvent>()
            .Initialize(() => Hide());

        registeredContextMenus[targetPortrait] = this;
    }

    private void Start()
    {
        // setup trigger on target button/object
        var trigger = contextForObject.AddComponent<EventTrigger>();
        var e = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        e.callback.AddListener(data => OnTargetClicked((PointerEventData)data));
        trigger.triggers.Add(e);

        Hide();
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !mouseInBounds)
        {
            Hide();
        }
    }

    private void OnDestroy()
    {
        registeredContextMenus.Remove(targetPortrait);
    }

    public void OnTargetClicked(PointerEventData data)
    {
        switch (data.button)
        {
            case PointerEventData.InputButton.Left:
                targetPortrait.ClickButton();
                break;
            case PointerEventData.InputButton.Right:
                Show();
                break;
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        mouseInBounds = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        mouseInBounds = false;
    }

    private void Show()
    {
        gameObject.SetActive(true);
        transform.position = targetPortrait.transform.position;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void Export()
    {
        var json = JsonUtility.ToJson(targetPortrait.myFace);
        GUIUtility.systemCopyBuffer = json;

        Unbound.BuildInfoPopup("Exported face data to clipboard");
        
        Hide();
    }

    public void Import()
    {
        PlayerFace importedFace = null;
        var clipboardData = GUIUtility.systemCopyBuffer;
        var resultModal = Unbound.BuildModal().Title("[ FaceExporter ]");
        try
        {
            importedFace = JsonUtility.FromJson<PlayerFace>(clipboardData);
            if (importedFace == null)
            {
                UnityEngine.Debug.LogError($"[FaceExporter] Failed to deserialize clipboard to a PlayerFace");
                resultModal
                    .Message($"Failed to deserialize clipboard to a <b>PlayerFace</b>\n\nGiven data was:\n{clipboardData}")
                    .Show();
                return;
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogException(e);
            resultModal
                .Message($"An error occurred attempting to interpret clipboard data as a <b>PlayerFace</b>\n\nGiven data was:\n{clipboardData}")
                .ConfirmButton("Copy Error", () => GUIUtility.systemCopyBuffer = $"Exception:\n{e.ToString()}\n\nClipboard Data:\n{clipboardData}")
                .Show();
            return;
        }

        resultModal
            .Message("Are you sure you want to override this face?")
            .CancelButton("Cancel", () => { })
            .ConfirmButton("Yes", () =>
            {
                CharacterCreatorHandler.instance.InvokeMethod("SetFacePreset", targetPortrait.transform.GetSiblingIndex(), importedFace);
            })
            .Show();

        Hide();
    }

    public void EditFace()
    {
        targetPortrait.EditCharacter();
    }
}
