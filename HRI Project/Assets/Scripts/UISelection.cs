using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HuggingFace.API;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class UISelection : MonoBehaviour
{
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

    [Space(8)]
    [SerializeField] private Button resetBtn;
    [SerializeField] private Experiment experimentToReset;

    private int m_CurrentRobotIndex = 0;
    private int m_CurrentSlotIndex = 0;
    private int m_CurrentObjectIndex = 0;

    // Variables for voice recognition
    private AudioClip recordingClip;
    private byte[] audioBytes;
    private bool isRecording;

    [SerializeField] private TextMeshProUGUI voiceStatusText;

    private void OnEnable()
    {
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
        resetBtn.onClick.AddListener(experimentToReset.resetObjects);
    }

    private void OnDisable()
    {
        robotSelectionDropdown.onValueChanged.RemoveListener(OnRobotSelectionChanged);
        slotSelectionDropdown.onValueChanged.RemoveListener(OnSlotSelectionChanged);
        objectSelectionDropdown.onValueChanged.RemoveListener(OnObjectSelectionChanged);
        submitBtn.onClick.RemoveListener(OnSubmitButtonClick);

        startRecordingBtn.onClick.RemoveListener(StartRecording);
        stopRecordingBtn.onClick.RemoveListener(StopRecording);
    }



    public void OnVoiceControlToggled(bool toggleValue)
    {
        voiceControlsPanel.SetActive(toggleValue);
        manualControlsPanel.SetActive(!toggleValue);
    }

    private void SetRobotInAction()
    {
        RobotMovement robotScript = robots[m_CurrentRobotIndex];
        robotScript.SetTask(m_CurrentObjectIndex, slotTransforms[m_CurrentSlotIndex]);
    }

    #region Voice Controls Panel Part
    private void StartRecording()
    {
        if (voiceStatusText != null)
        {
            voiceStatusText.text = "Recording...";
        }
        print("start recording");
        startRecordingBtn.interactable = false;
        stopRecordingBtn.interactable = true;

        recordingClip = Microphone.Start(null, false, 10, 44100);
        isRecording = true;
    }

    private void StopRecording()
    {
        print("stop recording");
        try
        {
            if (voiceStatusText != null)
                voiceStatusText.text = "Processing...";

            stopRecordingBtn.interactable = false;

            // Add null checks and error handling
            if (recordingClip == null)
            {
                Debug.LogError("No recording clip available!");
                return;
            }

            var position = Microphone.GetPosition(null);
            if (position <= 0)
            {
                Debug.LogError("No audio data recorded!");
                return;
            }

            Microphone.End(null);

            // Ensure valid array size
            var samples = new float[Mathf.Max(position, 1) * recordingClip.channels];
            bool success = recordingClip.GetData(samples, 0);

            if (!success)
            {
                Debug.LogError("Failed to get audio data!");
                return;
            }

            audioBytes = EncodeAsWAV(samples, recordingClip.frequency, recordingClip.channels);
            isRecording = false;

            ProcessVoiceCommand();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in StopRecording: {e.Message}");
            if (voiceStatusText != null)
                voiceStatusText.text = "Recording Error!";
            startRecordingBtn.interactable = true;
        }
    }

    private void ProcessVoiceCommand()
    {
        if (voiceStatusText != null)
            voiceStatusText.text = "Converting speech to text...";

        // Add retry logic
        StartCoroutine(TryProcessVoiceWithRetry());
    }

    private IEnumerator TryProcessVoiceWithRetry()
    {
        int maxRetries = 5;
        int currentTry = 0;
        bool success = false;
        int loopBreaker = 20;

        while (!success && currentTry < maxRetries && loopBreaker > 0)
        {
            currentTry++;
            if (currentTry > 1)
            {
                if (voiceStatusText != null)
                    voiceStatusText.text = $"Retrying... Attempt {currentTry}/{maxRetries}";
                yield return new WaitForSeconds(1f); // Wait before retry
            }

            try
            {
                HuggingFaceAPI.AutomaticSpeechRecognition(audioBytes,
                    response => {
                        success = true;
                        Debug.Log($"Voice command recognized: {response}");
                        ParseVoiceCommand(response);
                        startRecordingBtn.interactable = true;
                        if (voiceStatusText != null)
                            voiceStatusText.text = "Command received: " + response;
                    },
                    error => {
                        Debug.LogWarning($"Attempt {currentTry}: {error}");
                        if (currentTry >= maxRetries)
                        {
                            startRecordingBtn.interactable = true;
                            if (voiceStatusText != null)
                                voiceStatusText.text = "Service unavailable. Please try manual control.";
                        }
                    });
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error processing voice command: {e.Message}");
                if (currentTry >= maxRetries)
                {
                    startRecordingBtn.interactable = true;
                    if (voiceStatusText != null)
                        voiceStatusText.text = "Error processing voice. Please try manual control.";
                }
            }

            loopBreaker--;
            yield return new WaitForSeconds(2f); // Wait for response
        }

        if(loopBreaker <= 0)
        {
            Debug.LogError("Possible Infinite While Loop in UI Selection While Parsing Voice Command");
        }
    }

    private void ParseVoiceCommand(string command)
    {
        // Convert command to lowercase for easier parsing
        command = command.ToLower();

        // Example command format: "robot 1 pick up red triangle move to slot 2"
        // or "robot 2 get blue sphere slot 3"

        try
        {
            // Parse robot number
            if (command.Contains("robot"))
            {
                string[] parts = command.Split(' ');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i] == "robot" && i + 1 < parts.Length)
                    {
                        if (int.TryParse(Regex.Replace(parts[i + 1], @"[^\d]+", ""), out int robotIndex))
                        {
                            m_CurrentRobotIndex = robotIndex - 1; // Convert to 0-based index
                        }
                        else
                        {
                            print("Failed to parse int from part: " + parts[i + 1] + " Checking for written numbers");
                            if (parts[i + 1].ToLower().Contains("one") | parts[i + 1].ToLower().Contains("won"))
                            {
                                m_CurrentRobotIndex = 0;
                            }
                            else if (parts[i + 1].ToLower().Contains("two") | parts[i + 1].ToLower().Contains("to"))
                            {
                                m_CurrentRobotIndex = 1;
                            }
                            else if (parts[i + 1].ToLower().Contains("three"))
                            {
                                m_CurrentRobotIndex = 2;
                            }
                        }
                    }
                }
            }

            // Parse object (color and shape)
            string[] colors = new[] { "red", "blue", "green" };
            string[] shapes = new[] { "triangle", "sphere", "cube" };

            foreach (string color in colors)
            {
                if (command.Contains(color))
                {
                    foreach (string shape in shapes)
                    {
                        if (command.Contains(shape))
                        {
                            // Set object index based on color and shape
                            m_CurrentObjectIndex = GetObjectIndex(color, shape);
                            break;
                        }
                    }
                }
            }

            // Parse slot number
            if (command.Contains("slot"))
            {
                string[] parts = command.Split(' ');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i] == "slot" && i + 1 < parts.Length)
                    {
                        if (int.TryParse(Regex.Replace(parts[i + 1], @"[^\d]+", ""), out int slotIndex))
                        {
                            m_CurrentSlotIndex = slotIndex - 1; // Convert to 0-based index
                        }
                        else
                        {
                            print("Failed to parse int from part: " + parts[i + 1] + " Checking for written numbers");

                            if (parts[i + 1].ToLower().Contains("one") | parts[i + 1].ToLower().Contains("won"))
                            {
                                m_CurrentRobotIndex = 0;
                            }
                            else if (parts[i + 1].ToLower().Contains("two") | parts[i + 1].ToLower().Contains("to"))
                            {
                                m_CurrentRobotIndex = 1;
                            }
                            else if (parts[i + 1].ToLower().Contains("three"))
                            {
                                m_CurrentRobotIndex = 2;
                            }
                            else if (parts[i + 1].ToLower().Contains("four") | parts[i + 1].ToLower().Contains("for"))
                            {
                                m_CurrentRobotIndex = 3;
                            }
                        }
                    }
                }
            }

            // Execute the command
            SetRobotInAction();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error parsing voice command: " + e.Message);
            if (voiceStatusText != null)
                voiceStatusText.text = "Could not understand command";
        }
    }

    private int GetObjectIndex(string color, string shape)
    {
        Experiment experimentManager = FindObjectOfType<Experiment>();
        if (experimentManager == null)
        {
            Debug.LogError("No Experiment manager found in scene!");
            return 0;
        }

        ExperimentObject[] allObjects = experimentManager.GetAllObjects();
        if (allObjects == null || allObjects.Length == 0)
        {
            Debug.LogError("No objects found in Experiment manager!");
            return 0;
        }

        for (int i = 0; i < allObjects.Length; i++)
        {
            if (allObjects[i].color.ToLower() == color &&
                allObjects[i].shape.ToLower() == shape)
            {
                return i;
            }
        }

        Debug.LogWarning($"No object found with color {color} and shape {shape}. Defaulting to first object.");
        return 0;
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
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
