using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISelection : MonoBehaviour
{
    [SerializeField] private Toggle voiceControlToggle;

    [Space(8)]
    [SerializeField] private GameObject manualControlsPanel;
    [SerializeField] private GameObject voiceControlsPanel;

    [Space(8)]
    [SerializeField] private TMP_Dropdown robotSelectionDropdown;
    [SerializeField] private TMP_Dropdown slotSelectionDropdown;
    [SerializeField] private TMP_Dropdown objectSelectionDropdown;
    [SerializeField] private Button submitBtn;

    [Space(8)]
    [SerializeField] private Button startRecordingBtn;
    [SerializeField] private Button stopRecordingBtn;

    [Space(8)]
    [SerializeField] private RobotMovement[] robots;
    [SerializeField] private Transform[] slotTransforms;

    private int m_CurrentRobotIndex = 0;
    private int m_CurrentSlotIndex = 0;
    private int m_CurrentObjectIndex = 0;

    private void OnEnable()
    {
        voiceControlToggle.onValueChanged.AddListener(OnVoiceControlToggled);

        robotSelectionDropdown.onValueChanged.AddListener(OnRobotSelectionChanged);
        slotSelectionDropdown.onValueChanged.AddListener(OnSlotSelectionChanged);
        objectSelectionDropdown.onValueChanged.AddListener(OnObjectSelectionChanged);
        submitBtn.onClick.AddListener(OnSubmitButtonClick);

        startRecordingBtn.onClick.AddListener(StartRecording);
        stopRecordingBtn.onClick.AddListener(StopRecording);
    }

    private void Start()
    {
        OnVoiceControlToggled(false);
    }

    private void OnDisable()
    {
        voiceControlToggle.onValueChanged.RemoveListener(OnVoiceControlToggled);

        robotSelectionDropdown.onValueChanged.RemoveListener(OnRobotSelectionChanged);
        slotSelectionDropdown.onValueChanged.RemoveListener(OnSlotSelectionChanged);
        objectSelectionDropdown.onValueChanged.RemoveListener(OnObjectSelectionChanged);
        submitBtn.onClick.RemoveListener(OnSubmitButtonClick);

        startRecordingBtn.onClick.RemoveListener(StartRecording);
        stopRecordingBtn.onClick.RemoveListener(StopRecording);
    }

    private void OnVoiceControlToggled(bool toggleValue)
    {
        voiceControlsPanel.SetActive(toggleValue);
        manualControlsPanel.SetActive(!toggleValue);
    }

    private void SetRobotInAction()
    {
        RobotMovement robotScript = robots[m_CurrentRobotIndex];
        robotScript.SetSlotDestinationTransform(slotTransforms[m_CurrentSlotIndex]);
        robotScript.SetShapeDestinationTransform(m_CurrentObjectIndex);
        robotScript.EnableMovement(true);
    }

    #region Voice Controls Panel Part

    private void StartRecording()
    {

    }

    private void StopRecording()
    {
        string voiceStr = "Robot 1, Object 1, slot";

        //Set Values Here
        m_CurrentRobotIndex = 0;
        m_CurrentSlotIndex = 0;
        m_CurrentObjectIndex = 0;

        SetRobotInAction();
    }

    #endregion

    #region Manual Controls Panel Part

    private void OnRobotSelectionChanged(int selectedRobotIndex)
    {
        m_CurrentRobotIndex = selectedRobotIndex;
    }

    private void OnSlotSelectionChanged(int selectedSlotIndex)
    {
        m_CurrentSlotIndex = selectedSlotIndex;
    }

    private void OnObjectSelectionChanged(int selectedObjectIndex)
    {
        m_CurrentObjectIndex = selectedObjectIndex;
    }

    private void OnSubmitButtonClick()
    {
        SetRobotInAction();
    }

    #endregion
}
