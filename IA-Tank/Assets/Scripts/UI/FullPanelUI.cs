using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FullPanelUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject simulationPanel;

    [Header("Button")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button evolveButton;
    
    [Header("Simulation General")]
    [SerializeField] private TextMeshProUGUI simulationSpeedText;
    [SerializeField] private Slider simulationSpeedSlider;
    [SerializeField] private TextMeshProUGUI generationDurationText;
    [SerializeField] private Slider generationDurationSlider;

    private TextMeshProUGUI _evolveText;
    private TextMeshProUGUI _pauseText;
    private bool _paused = false;
    private bool _evolve = true;

    private void Awake()
    {
        _pauseText = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
        _evolveText = evolveButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        startButton.onClick.AddListener(delegate
        {
            startPanel.SetActive(false);
            simulationPanel.SetActive(true);
            PopulationManager.Instance.StartSimulation();
            simulationSpeedText.text = "Simulation Speed: " + Mathf.Clamp((PopulationManager.Instance.IterationCount / 100.0f) * 50, 1, 50);
        });
        
        stopButton.onClick.AddListener(delegate
        {
            startPanel.SetActive(true);
            simulationPanel.SetActive(false);
            PopulationManager.Instance.StopSimulation();
        });
        
        pauseButton.onClick.AddListener(delegate
        {
            PopulationManager.Instance.PauseSimulation();
            _paused = !_paused;
            _pauseText.text = _paused ? "Unpause" : "Pause";
        });
        
        simulationSpeedText.text = "Simulation Speed: " + Mathf.Clamp((PopulationManager.Instance.IterationCount / 100.0f) * 50, 1, 50);
        simulationSpeedSlider.onValueChanged.AddListener(delegate(float value)
        {
            PopulationManager.Instance.IterationCount = (int)value;
            simulationSpeedText.text = "Simulation Speed: " + Mathf.Clamp((value / 100.0f) * 50, 1, 50);
        });
        
        generationDurationText.text = "Generation Duration: " + PopulationManager.Instance.GenerationDuration;
        generationDurationSlider.onValueChanged.AddListener(delegate(float value)
        {
            PopulationManager.Instance.GenerationDuration = value;
            generationDurationText.text = "Generation Duration: " + PopulationManager.Instance.GenerationDuration;
        });
        
        evolveButton.onClick.AddListener(delegate
        {
            _evolve = !_evolve;
            PopulationManager.Instance.Evolve = _evolve;
            _evolveText.text = _evolve ? "Evolve" : "Compete";
        });
        
    }
}
