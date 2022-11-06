using UnityEngine;
using UnityEngine.UI;

public class StartConfigurationScreen : MonoBehaviour
{

    [SerializeField] private TeamEnum team;
    
    [Header("Configuration")]
    [SerializeField] private Text populationCountTxt;
    [SerializeField] private Slider populationCountSlider;
    [SerializeField] private Text minesCountTxt;
    [SerializeField] private Slider minesCountSlider;
    [SerializeField] private Text eliteCountTxt;
    [SerializeField] private Slider eliteCountSlider;
    [SerializeField] private Text mutationChanceTxt;
    [SerializeField] private Slider mutationChanceSlider;
    [SerializeField] private Text mutationRateTxt;
    [SerializeField] private Slider mutationRateSlider;
    [SerializeField] private Text hiddenLayersCountTxt;
    [SerializeField] private Slider hiddenLayersCountSlider;
    [SerializeField] private Text neuronsPerHiddenLayerCountTxt;
    [SerializeField] private Slider neuronsPerHiddenLayerSlider;
    [SerializeField] private Text biasTxt;
    [SerializeField] private Slider biasSlider;
    [SerializeField] private Text sigmoidSlopeTxt;
    [SerializeField] private Slider sigmoidSlopeSlider;

    [Header("File Management")] 
    [SerializeField] private Button loadFileButton;
    [SerializeField] private TMPro.TMP_InputField loadFilePathInputField;
    [SerializeField] private GameObject[] loadedFileHideObjects;
    [SerializeField] private GameObject[] loadedFileShowObjects;

    string populationText;
    string minesText;
    string elitesText;
    string mutationChanceText;
    string mutationRateText;
    string hiddenLayersCountText;
    string biasText;
    string sigmoidSlopeText;
    string neuronsPerHLCountText;

    void Start()
    {   
        populationCountSlider.onValueChanged.AddListener(OnPopulationCountChange);
        minesCountSlider.onValueChanged.AddListener(OnMinesCountChange);
        eliteCountSlider.onValueChanged.AddListener(OnEliteCountChange);
        mutationChanceSlider.onValueChanged.AddListener(OnMutationChanceChange);
        mutationRateSlider.onValueChanged.AddListener(OnMutationRateChange);
        hiddenLayersCountSlider.onValueChanged.AddListener(OnHiddenLayersCountChange);
        neuronsPerHiddenLayerSlider.onValueChanged.AddListener(OnNeuronsPerHLChange);
        biasSlider.onValueChanged.AddListener(OnBiasChange);
        sigmoidSlopeSlider.onValueChanged.AddListener(OnSigmoidSlopeChange);

        populationText = populationCountTxt.text;
        minesText = minesCountTxt.text;
        elitesText = eliteCountTxt.text;
        mutationChanceText = mutationChanceTxt.text;
        mutationRateText = mutationRateTxt.text;
        hiddenLayersCountText = hiddenLayersCountTxt.text;
        neuronsPerHLCountText = neuronsPerHiddenLayerCountTxt.text;
        biasText = biasTxt.text;
        sigmoidSlopeText = sigmoidSlopeTxt.text;

        populationCountSlider.value = PopulationManager.Instance.GetPopulationCount(team);
        minesCountSlider.value = PopulationManager.Instance.GetMinesCount(team);
        eliteCountSlider.value = PopulationManager.Instance.GetElitesCount(team);
        mutationChanceSlider.value = PopulationManager.Instance.GetMutationChance(team) * 100.0f;
        mutationRateSlider.value = PopulationManager.Instance.GetMutationRate(team) * 100.0f;
        hiddenLayersCountSlider.value = PopulationManager.Instance.GetHiddenLayersAmount(team);
        neuronsPerHiddenLayerSlider.value = PopulationManager.Instance.GetNeuronsCountPerLayers(team);
        biasSlider.value = -PopulationManager.Instance.GetBias(team);
        sigmoidSlopeSlider.value = PopulationManager.Instance.GetSigmoidFactor(team);
   
        loadFileButton.onClick.AddListener(delegate
        {
            var data = SaveSystem.SaveSystem.LoadFromStreamingAssets<GenomeData>(loadFilePathInputField.text);
            if (team == TeamEnum.Blue)
            {
                PopulationManager.Instance.BlueTeamData = data;
                RefreshAllData(TeamEnum.Blue);
            }
            else // if (team == TeamEnum.Red)
            {
                PopulationManager.Instance.RedTeamData = data;
                RefreshAllData(TeamEnum.Red);
            }

            foreach (var hideObject in loadedFileHideObjects)
            {
                hideObject.SetActive(false);
            }
            
            foreach (var showObject in loadedFileShowObjects)
            {
                showObject.SetActive(true);
            }
            
        });
        
    }

    private void RefreshAllData(TeamEnum teamDataToRefresh)
    {
        OnPopulationCountChange(PopulationManager.Instance.GetPopulationCount(teamDataToRefresh));
        OnMinesCountChange(PopulationManager.Instance.GetMinesCount(teamDataToRefresh));
        
        OnEliteCountChange(PopulationManager.Instance.GetElitesCount(teamDataToRefresh));
        OnMutationChanceChange(PopulationManager.Instance.GetMutationChance(teamDataToRefresh));
        OnMutationRateChange(PopulationManager.Instance.GetMutationRate(teamDataToRefresh));
        
        OnHiddenLayersCountChange(PopulationManager.Instance.GetHiddenLayersAmount(teamDataToRefresh));
        OnNeuronsPerHLChange(PopulationManager.Instance.GetNeuronsCountPerLayers(teamDataToRefresh));
        
        OnSigmoidSlopeChange(PopulationManager.Instance.GetSigmoidFactor(teamDataToRefresh));
        OnBiasChange(PopulationManager.Instance.GetBias(teamDataToRefresh));
    }
    
    void OnPopulationCountChange(float value)
    {
        PopulationManager.Instance.SetPopulationCount(team, (int)value);
        populationCountTxt.text = string.Format(populationText, PopulationManager.Instance.GetPopulationCount(team));
        populationCountSlider.SetValueWithoutNotify(value);
    }

    void OnMinesCountChange(float value)
    {
        PopulationManager.Instance.SetMinesCount(team, (int)value);
        minesCountTxt.text = string.Format(minesText, PopulationManager.Instance.GetMinesCount(team));
        minesCountSlider.SetValueWithoutNotify(value);
    }

    void OnEliteCountChange(float value)
    {
        PopulationManager.Instance.SetElitesCount(team, (int)value);
        eliteCountTxt.text = string.Format(elitesText, PopulationManager.Instance.GetElitesCount(team));
    }

    void OnMutationChanceChange(float value)
    {
        PopulationManager.Instance.SetMutationChance(team, value / 100.0f);
        mutationChanceTxt.text = string.Format(mutationChanceText, (int)(PopulationManager.Instance.GetMutationChance(team) * 100));
    }

    void OnMutationRateChange(float value)
    {
        PopulationManager.Instance.SetMutationRate(team, value / 100.0f);
        mutationRateTxt.text = string.Format(mutationRateText, (int)(PopulationManager.Instance.GetMutationRate(team)));
    }

    void OnHiddenLayersCountChange(float value)
    {
        PopulationManager.Instance.SetHiddenLayersAmount(team, (int)value);
        hiddenLayersCountTxt.text = string.Format(hiddenLayersCountText, PopulationManager.Instance.GetHiddenLayersAmount(team));
    }

    void OnNeuronsPerHLChange(float value)
    {
        PopulationManager.Instance.SetNeuronsCountPerLayers(team, (int)value);
        neuronsPerHiddenLayerCountTxt.text = string.Format(neuronsPerHLCountText, PopulationManager.Instance.GetNeuronsCountPerLayers(team));
    }

    void OnBiasChange(float value)
    {
        PopulationManager.Instance.SetBias(team, -value);
        biasTxt.text = string.Format(biasText, PopulationManager.Instance.GetBias(team).ToString("0.00"));
    }

    void OnSigmoidSlopeChange(float value)
    {
        PopulationManager.Instance.SetSigmoidFactor(team, value);
        sigmoidSlopeTxt.text = string.Format(sigmoidSlopeText, PopulationManager.Instance.GetSigmoidFactor(team).ToString("0.00"));
    }
    
}
