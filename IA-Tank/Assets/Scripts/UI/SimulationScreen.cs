using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimulationScreen : MonoBehaviour
{
    [SerializeField] private TeamEnum team;
    [Space(10)]
    public Text generationsCountTxt;
    public Text bestFitnessTxt;
    public Text avgFitnessTxt;
    public Text worstFitnessTxt;
    [Header("Compete Panel")]
    [SerializeField] private TextMeshProUGUI ownTeamRoundsText;

    [Header("File Management")] 
    [SerializeField] private TMP_InputField filePathInputField;
    [SerializeField] private Button saveButton;

    private int ownTeamRoundsCount = 0;
    
    string generationsCountText;
    string bestFitnessText;
    string avgFitnessText;
    string worstFitnessText;

    void Start()
    {
        if (string.IsNullOrEmpty(generationsCountText))
            generationsCountText = generationsCountTxt.text;   
        if (string.IsNullOrEmpty(bestFitnessText))
            bestFitnessText = bestFitnessTxt.text;   
        if (string.IsNullOrEmpty(avgFitnessText))
            avgFitnessText = avgFitnessTxt.text;   
        if (string.IsNullOrEmpty(worstFitnessText))
            worstFitnessText = worstFitnessTxt.text;

        PopulationManager.Instance.OnGenerationEnd += UpdateGeneration;

        saveButton.onClick.AddListener(delegate
        {
            var data = team == TeamEnum.Blue
                ? PopulationManager.Instance.BlueTeamData
                : PopulationManager.Instance.RedTeamData;
            SaveSystem.SaveSystem.SaveToStreamingAssets(data, filePathInputField.text);
        });
        
    }

    private void OnDestroy()
    {
        PopulationManager.Instance.OnGenerationEnd -= UpdateGeneration;
    }

    void OnEnable()
    {
        if (string.IsNullOrEmpty(generationsCountText))
            generationsCountText = generationsCountTxt.text;   
        if (string.IsNullOrEmpty(bestFitnessText))
            bestFitnessText = bestFitnessTxt.text;   
        if (string.IsNullOrEmpty(avgFitnessText))
            avgFitnessText = avgFitnessTxt.text;   
        if (string.IsNullOrEmpty(worstFitnessText))
            worstFitnessText = worstFitnessTxt.text;   

        generationsCountTxt.text = string.Format(generationsCountText, 0);
        bestFitnessTxt.text = string.Format(bestFitnessText, 0);
        avgFitnessTxt.text = string.Format(avgFitnessText, 0);
        worstFitnessTxt.text = string.Format(worstFitnessText, 0);

        ownTeamRoundsText.gameObject.SetActive(!PopulationManager.Instance.Evolve);
    }


    void UpdateGeneration(TeamEnum winningTeam)
    {
        generationsCountTxt.text = string.Format(generationsCountText, PopulationManager.Instance.GetGenerationCount(team));
        bestFitnessTxt.text = string.Format(bestFitnessText, PopulationManager.Instance.GetBestFitnessCached(team));
        avgFitnessTxt.text = string.Format(avgFitnessText, PopulationManager.Instance.GetAverageFitnessCached(team));
        worstFitnessTxt.text = string.Format(worstFitnessText, PopulationManager.Instance.GetWorstFitnessCached(team));

        if (!PopulationManager.Instance.Evolve)
        {
            if (team == winningTeam)
            {
                ownTeamRoundsCount++;
                ownTeamRoundsText.text = "Rounds Won: " + ownTeamRoundsCount;
            }
        }
    }

}
