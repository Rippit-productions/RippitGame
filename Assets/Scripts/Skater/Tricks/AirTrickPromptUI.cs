using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirTrickPromptUI : MonoBehaviour
{
    private static readonly Color PanelColor = new Color(0.04f, 0.05f, 0.06f, 0.78f);
    private static readonly Color EmptyColor = new Color(0.18f, 0.2f, 0.24f, 1.0f);
    private static readonly Color CurrentColor = new Color(1.0f, 0.83f, 0.25f, 1.0f);
    private static readonly Color CompleteColor = new Color(0.35f, 1.0f, 0.45f, 1.0f);
    private static readonly Color FailColor = new Color(1.0f, 0.3f, 0.24f, 1.0f);
    private static readonly Color BarBackgroundColor = new Color(0.14f, 0.15f, 0.18f, 1.0f);

    private const float PanelWidth = 280.0f;
    private const float PanelHeight = 170.0f;
    private const float DirectionBoxSize = 44.0f;
    private const float SpeedPanelWidth = 220.0f;
    private const float SpeedPanelHeight = 92.0f;

    [SerializeField] private bool _ShowSpeedDebug = true;

    private static AirTrickPromptUI _instance;

    private readonly List<TMP_Text> _directionLabels = new List<TMP_Text>();
    private CanvasGroup _promptCanvasGroup;
    private CanvasGroup _speedCanvasGroup;
    private RectTransform _directionRow;
    private RectTransform _stepFillTransform;
    private TMP_Text _titleText;
    private TMP_Text _timerText;
    private TMP_Text _resultText;
    private TMP_Text _speedText;
    private TMP_Text _boostText;
    private Image _stepFillImage;
    private TrickCombo _renderedCombo;
    private bool _promptVisible = true;
    private bool _speedVisible = true;

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
        SetPromptVisible(false);
        SetSpeedVisible(false);
    }

    private void Update()
    {
        // One pass over registered trick controllers supplies both panels.
        GetControllers(out SkaterAirTrickController promptController, out SkaterAirTrickController primaryController);
        if (promptController == null)
        {
            SetPromptVisible(false);
        }
        else
        {
            SetPromptVisible(true);
            Render(promptController);
        }

        RenderSpeedDebug(_ShowSpeedDebug ? primaryController : null);
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

        RectTransform panel = CreateRect("Prompt Panel", transform);
        panel.anchorMin = new Vector2(0.0f, 0.5f);
        panel.anchorMax = new Vector2(0.0f, 0.5f);
        panel.pivot = new Vector2(0.0f, 0.5f);
        panel.anchoredPosition = new Vector2(32.0f, 0.0f);
        panel.sizeDelta = new Vector2(PanelWidth, PanelHeight);

        Image panelImage = panel.gameObject.AddComponent<Image>();
        panelImage.color = PanelColor;
        _promptCanvasGroup = panel.gameObject.AddComponent<CanvasGroup>();

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

        RectTransform stepBar = CreateRect("Step Time", panel);
        stepBar.anchorMin = new Vector2(0.0f, 0.0f);
        stepBar.anchorMax = new Vector2(1.0f, 0.0f);
        stepBar.pivot = new Vector2(0.0f, 0.5f);
        stepBar.anchoredPosition = new Vector2(18.0f, 52.0f);
        stepBar.sizeDelta = new Vector2(-36.0f, 8.0f);

        Image stepBarImage = stepBar.gameObject.AddComponent<Image>();
        stepBarImage.color = BarBackgroundColor;

        _stepFillTransform = CreateRect("Fill", stepBar);
        _stepFillTransform.anchorMin = new Vector2(0.0f, 0.0f);
        _stepFillTransform.anchorMax = new Vector2(0.0f, 1.0f);
        _stepFillTransform.pivot = new Vector2(0.0f, 0.5f);
        _stepFillTransform.anchoredPosition = Vector2.zero;
        _stepFillTransform.sizeDelta = new Vector2(stepBar.rect.width, 0.0f);

        _stepFillImage = _stepFillTransform.gameObject.AddComponent<Image>();
        _stepFillImage.color = CurrentColor;

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

        BuildSpeedDebugUI();
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
            ? $"{controller.StepTimeRemaining:0.00}s"
            : controller.IsBoosting ? $"{controller.BoostTimeRemaining:0.00}s" : "";

        SetStepFill(controller);
        _resultText.text = GetResultText(controller);
        _resultText.color = controller.State == SkaterAirTrickController.TrickSessionState.Completed || controller.IsBoosting
            ? CompleteColor
            : FailColor;

        for (int i = 0; i < _directionLabels.Count; i++)
        {
            TMP_Text label = _directionLabels[i];
            Image boxImage = label.transform.parent.GetComponent<Image>();
            bool completed = i < controller.CurrentStepIndex;
            bool current = i == controller.CurrentStepIndex && controller.State == SkaterAirTrickController.TrickSessionState.Active;

            label.color = completed ? Color.black : Color.white;
            boxImage.color = completed
                ? CompleteColor
                : current ? CurrentColor : EmptyColor;
        }
    }

    private void SetStepFill(SkaterAirTrickController controller)
    {
        float progress = controller.State == SkaterAirTrickController.TrickSessionState.Active
            ? controller.StepProgress
            : controller.IsBoosting ? controller.BoostProgress : 0.0f;

        RectTransform parentRect = _stepFillTransform.parent as RectTransform;
        float parentWidth = parentRect == null ? 0.0f : parentRect.rect.width;
        _stepFillTransform.sizeDelta = new Vector2(parentWidth * progress, 0.0f);
        _stepFillImage.color = controller.IsBoosting ? CompleteColor : CurrentColor;
    }

    private void BuildSpeedDebugUI()
    {
        RectTransform panel = CreateRect("Speed Debug", transform);
        panel.anchorMin = new Vector2(1.0f, 1.0f);
        panel.anchorMax = new Vector2(1.0f, 1.0f);
        panel.pivot = new Vector2(1.0f, 1.0f);
        panel.anchoredPosition = new Vector2(-32.0f, -32.0f);
        panel.sizeDelta = new Vector2(SpeedPanelWidth, SpeedPanelHeight);

        Image panelImage = panel.gameObject.AddComponent<Image>();
        panelImage.color = PanelColor;
        _speedCanvasGroup = panel.gameObject.AddComponent<CanvasGroup>();

        _speedText = CreateText("Speed", panel, "SPEED 0.0", 24, TextAlignmentOptions.Right);
        _speedText.rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
        _speedText.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
        _speedText.rectTransform.pivot = new Vector2(1.0f, 1.0f);
        _speedText.rectTransform.anchoredPosition = new Vector2(-14.0f, -12.0f);
        _speedText.rectTransform.sizeDelta = new Vector2(-28.0f, 32.0f);

        _boostText = CreateText("Boost", panel, "BOOST --", 16, TextAlignmentOptions.Right);
        _boostText.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
        _boostText.rectTransform.anchorMax = new Vector2(1.0f, 0.0f);
        _boostText.rectTransform.pivot = new Vector2(1.0f, 0.0f);
        _boostText.rectTransform.anchoredPosition = new Vector2(-14.0f, 14.0f);
        _boostText.rectTransform.sizeDelta = new Vector2(-28.0f, 34.0f);
    }

    private void RenderSpeedDebug(SkaterAirTrickController controller)
    {
        bool visible = controller != null;
        SetSpeedVisible(visible);

        if (!visible)
        {
            return;
        }

        _speedText.text = $"SPEED {controller.CurrentSpeed:0.0}";
        _boostText.text = controller.IsBoosting
            ? $"BOOST {controller.BoostTimeRemaining:0.00}s  {controller.BoostStartSpeed:0.0}->{controller.BoostEndSpeed:0.0}"
            : "BOOST --";
        _boostText.color = controller.IsBoosting ? CompleteColor : Color.white;
    }

    private void RebuildDirectionLabels(TrickCombo combo)
    {
        // Combos are short, and this only runs when the active combo changes.
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
            image.color = EmptyColor;

            TMP_Text label = CreateText("Label", box, combo.GetStep(i).ToInputLabel(), 30, TextAlignmentOptions.Center);
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = Vector2.zero;
            label.rectTransform.offsetMax = Vector2.zero;
            _directionLabels.Add(label);
        }
    }

    private void GetControllers(out SkaterAirTrickController promptController, out SkaterAirTrickController primaryController)
    {
        promptController = null;
        primaryController = null;

        var controllers = SkaterAirTrickController.ActiveControllers;
        for (int i = 0; i < controllers.Count; i++)
        {
            SkaterAirTrickController controller = controllers[i];
            if (controller == null)
            {
                continue;
            }

            primaryController ??= controller;
            if (promptController == null && controller.HasVisiblePrompt)
            {
                promptController = controller;
            }

            if (promptController != null)
            {
                break;
            }
        }
    }

    private void SetPromptVisible(bool visible)
    {
        SetCanvasVisible(_promptCanvasGroup, visible, ref _promptVisible);
    }

    private void SetSpeedVisible(bool visible)
    {
        SetCanvasVisible(_speedCanvasGroup, visible, ref _speedVisible);
    }

    private void SetCanvasVisible(CanvasGroup canvasGroup, bool visible, ref bool currentVisible)
    {
        // Avoid writing CanvasGroup fields every frame when visibility has not changed.
        if (currentVisible == visible)
        {
            return;
        }

        currentVisible = visible;
        canvasGroup.alpha = visible ? 1.0f : 0.0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    private string GetResultText(SkaterAirTrickController controller)
    {
        if (controller.IsBoosting)
        {
            return "BOOST";
        }

        switch (controller.State)
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
