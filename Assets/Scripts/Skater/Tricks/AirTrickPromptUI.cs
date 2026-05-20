using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirTrickPromptUI : MonoBehaviour
{
    private const float PanelWidth = 280.0f;
    private const float PanelHeight = 170.0f;
    private const float DirectionBoxSize = 44.0f;

    private static AirTrickPromptUI _instance;

    private readonly List<TMP_Text> _directionLabels = new List<TMP_Text>();
    private CanvasGroup _canvasGroup;
    private RectTransform _directionRow;
    private TMP_Text _titleText;
    private TMP_Text _timerText;
    private TMP_Text _resultText;
    private TrickCombo _renderedCombo;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimePrompt()
    {
        if (_instance != null)
        {
            return;
        }

        GameObject promptObject = new GameObject("Air Trick Prompt UI");
        DontDestroyOnLoad(promptObject);
        promptObject.AddComponent<AirTrickPromptUI>();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        BuildUI();
        SetVisible(false);
    }

    private void Update()
    {
        SkaterAirTrickController controller = FindPromptController();
        if (controller == null || !controller.HasVisiblePrompt)
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);
        Render(controller);
    }

    private SkaterAirTrickController FindPromptController()
    {
        var controllers = FindObjectsByType<SkaterAirTrickController>(FindObjectsSortMode.InstanceID);
        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i].HasVisiblePrompt)
            {
                return controllers[i];
            }
        }

        return null;
    }

    private void BuildUI()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920.0f, 1080.0f);
        scaler.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        RectTransform panel = CreateRect("Prompt Panel", transform);
        panel.anchorMin = new Vector2(0.0f, 0.5f);
        panel.anchorMax = new Vector2(0.0f, 0.5f);
        panel.pivot = new Vector2(0.0f, 0.5f);
        panel.anchoredPosition = new Vector2(32.0f, 0.0f);
        panel.sizeDelta = new Vector2(PanelWidth, PanelHeight);

        Image panelImage = panel.gameObject.AddComponent<Image>();
        panelImage.color = new Color(0.04f, 0.05f, 0.06f, 0.78f);

        _titleText = CreateText("Title", panel, "AIR TRICK", 24, TextAlignmentOptions.Left);
        _titleText.rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
        _titleText.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
        _titleText.rectTransform.pivot = new Vector2(0.0f, 1.0f);
        _titleText.rectTransform.anchoredPosition = new Vector2(18.0f, -16.0f);
        _titleText.rectTransform.sizeDelta = new Vector2(-36.0f, 34.0f);

        _directionRow = CreateRect("Directions", panel);
        _directionRow.anchorMin = new Vector2(0.0f, 0.5f);
        _directionRow.anchorMax = new Vector2(1.0f, 0.5f);
        _directionRow.pivot = new Vector2(0.0f, 0.5f);
        _directionRow.anchoredPosition = new Vector2(18.0f, 2.0f);
        _directionRow.sizeDelta = new Vector2(-36.0f, DirectionBoxSize);

        _timerText = CreateText("Timer", panel, "0.00s", 18, TextAlignmentOptions.Left);
        _timerText.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
        _timerText.rectTransform.anchorMax = new Vector2(1.0f, 0.0f);
        _timerText.rectTransform.pivot = new Vector2(0.0f, 0.0f);
        _timerText.rectTransform.anchoredPosition = new Vector2(18.0f, 16.0f);
        _timerText.rectTransform.sizeDelta = new Vector2(-36.0f, 24.0f);

        _resultText = CreateText("Result", panel, "", 18, TextAlignmentOptions.Right);
        _resultText.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
        _resultText.rectTransform.anchorMax = new Vector2(1.0f, 0.0f);
        _resultText.rectTransform.pivot = new Vector2(1.0f, 0.0f);
        _resultText.rectTransform.anchoredPosition = new Vector2(-18.0f, 16.0f);
        _resultText.rectTransform.sizeDelta = new Vector2(-36.0f, 24.0f);
    }

    private void Render(SkaterAirTrickController controller)
    {
        TrickCombo combo = controller.CurrentCombo;
        if (combo != _renderedCombo)
        {
            RebuildDirectionLabels(combo);
            _renderedCombo = combo;
        }

        _titleText.text = combo != null && !string.IsNullOrWhiteSpace(combo.Name) ? combo.Name.ToUpperInvariant() : "AIR TRICK";
        _timerText.text = controller.State == SkaterAirTrickController.TrickSessionState.Active
            ? $"{controller.TimeRemaining:0.00}s"
            : "";

        _resultText.text = GetResultText(controller.State);
        _resultText.color = controller.State == SkaterAirTrickController.TrickSessionState.Completed
            ? new Color(0.35f, 1.0f, 0.45f, 1.0f)
            : new Color(1.0f, 0.3f, 0.24f, 1.0f);

        for (int i = 0; i < _directionLabels.Count; i++)
        {
            TMP_Text label = _directionLabels[i];
            Image boxImage = label.transform.parent.GetComponent<Image>();
            bool completed = i < controller.CurrentStepIndex;
            bool current = i == controller.CurrentStepIndex && controller.State == SkaterAirTrickController.TrickSessionState.Active;

            label.color = completed ? Color.black : Color.white;
            boxImage.color = completed
                ? new Color(0.35f, 1.0f, 0.45f, 1.0f)
                : current ? new Color(1.0f, 0.83f, 0.25f, 1.0f) : new Color(0.18f, 0.2f, 0.24f, 1.0f);
        }
    }

    private void RebuildDirectionLabels(TrickCombo combo)
    {
        for (int i = _directionRow.childCount - 1; i >= 0; i--)
        {
            Destroy(_directionRow.GetChild(i).gameObject);
        }

        _directionLabels.Clear();
        if (combo == null || !combo.IsValid)
        {
            return;
        }

        for (int i = 0; i < combo.StepCount; i++)
        {
            RectTransform box = CreateRect($"Direction {i + 1}", _directionRow);
            box.anchorMin = new Vector2(0.0f, 0.5f);
            box.anchorMax = new Vector2(0.0f, 0.5f);
            box.pivot = new Vector2(0.5f, 0.5f);
            box.anchoredPosition = new Vector2((DirectionBoxSize * 0.5f) + (i * (DirectionBoxSize + 10.0f)), 0.0f);
            box.sizeDelta = new Vector2(DirectionBoxSize, DirectionBoxSize);

            Image image = box.gameObject.AddComponent<Image>();
            image.color = new Color(0.18f, 0.2f, 0.24f, 1.0f);

            TMP_Text label = CreateText("Label", box, combo.GetStep(i).ToInputLabel(), 30, TextAlignmentOptions.Center);
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = Vector2.zero;
            label.rectTransform.offsetMax = Vector2.zero;
            _directionLabels.Add(label);
        }
    }

    private void SetVisible(bool visible)
    {
        _canvasGroup.alpha = visible ? 1.0f : 0.0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    private string GetResultText(SkaterAirTrickController.TrickSessionState state)
    {
        switch (state)
        {
            case SkaterAirTrickController.TrickSessionState.Completed:
                return "BOOST";
            case SkaterAirTrickController.TrickSessionState.Failed:
                return "MISS";
            default:
                return "";
        }
    }

    private RectTransform CreateRect(string objectName, Transform parent)
    {
        GameObject child = new GameObject(objectName);
        child.transform.SetParent(parent, false);
        return child.AddComponent<RectTransform>();
    }

    private TMP_Text CreateText(string objectName, Transform parent, string text, float fontSize, TextAlignmentOptions alignment)
    {
        RectTransform rectTransform = CreateRect(objectName, parent);
        TMP_Text label = rectTransform.gameObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = alignment;
        label.color = Color.white;
        label.raycastTarget = false;
        return label;
    }
}
