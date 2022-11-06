using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PopulationManager : MonoBehaviour
{

    public Action<TeamEnum> OnGenerationEnd;
    
    [Header("General Configurations")]
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private Vector3 sceneHalfExtents = new Vector3 (20.0f, 0.0f, 20.0f);
    [SerializeField] private float generationDuration = 20.0f;
    [SerializeField] private int iterationCount = 1;

    [Header("Red Team Configuration")]
    [SerializeField] private int redPopulationCount = 40;
    [SerializeField] private int redMinesCount = 50;
                      
    [SerializeField] private int redEliteCount = 4;
    [SerializeField] private float redMutationChance = 0.10f;
    [SerializeField] private float redMutationRate = 0.01f;
                      
    [SerializeField] private int redInputsCount = 4;
    [SerializeField] private int redHiddenLayers = 1;
    [SerializeField] private int redOutputsCount = 2;
    [SerializeField] private int redNeuronsCountPerHiddenLayer = 7;
    [SerializeField] private float redBias = 1f;
    [SerializeField] private float redSigmoidFactor = 0.5f;
    
    [Header("Blue Team Configuration")]
    [SerializeField] private int bluePopulationCount = 40;
    [SerializeField] private int blueMinesCount = 50;
                      
    [SerializeField] private int blueEliteCount = 4;
    [SerializeField] private float blueMutationChance = 0.10f;
    [SerializeField] private float blueMutationRate = 0.01f;
                      
    [SerializeField] private int blueInputsCount = 4;
    [SerializeField] private int blueHiddenLayers = 1;
    [SerializeField] private int blueOutputsCount = 2;
    [SerializeField] private int blueNeuronsCountPerHiddenLayer = 7;
    [SerializeField] private float blueBias = 1f;
    [SerializeField] private float blueSigmoidFactor = 0.5f;


    public int IterationCount
    {
        get => iterationCount;
        set => iterationCount = value;
    }
    
    private bool _isRedTeamDataLoaded = false;
    private GeneticAlgorithm _redGeneticAlgorithm;
    private List<Tank> _redTanksList = new List<Tank>();
    private List<Genome> _redPopulation = new List<Genome>();
    private List<NeuralNetwork> _redBrains = new List<NeuralNetwork>();
    private List<GameObject> _redMinesList = new List<GameObject>();
    private int _redGenerationCount = 0;
    private Genome _bestRedTankGenome = new Genome();
    private GenomeData _redTeamData;
    private float _redBestFitness;
    private float _redAverageFitness;
    private float _redWorstFitness;
    
    private bool _isBlueTeamDataLoaded = false;
    private GeneticAlgorithm _blueGeneticAlgorithm;
    private List<Tank> _blueTanksList = new List<Tank>();
    private List<Genome> _bluePopulation = new List<Genome>();
    private List<NeuralNetwork> _blueBrains = new List<NeuralNetwork>();
    private List<GameObject> _blueMinesList = new List<GameObject>();
    private int _blueGenerationCount = 0;
    private Genome _bestBlueTankGenome = new Genome();
    private GenomeData _blueTeamData;
    private float _blueBestFitness;
    private float _blueAverageFitness;
    private float _blueWorstFitness;

    private float _timeElapsed = 0;
    private bool _isRunning = false;
    private bool _evolve = true;

    #region GETTERS_AND_SETTER

    public GenomeData RedTeamData
    {
        get
        {
            _redTeamData = GetTeamData(TeamEnum.Red);
            return _redTeamData;
        }
        set
        {
            _redTeamData = value;
            _isRedTeamDataLoaded = true;
        }
    }
    public GenomeData BlueTeamData
    {
        get
        {
            _blueTeamData = GetTeamData(TeamEnum.Blue);
            return _blueTeamData;
        }
        set
        {
            _blueTeamData = value;
            _isBlueTeamDataLoaded = true;
        }
    }
    public float GenerationDuration
    {
        get => generationDuration;
        set => generationDuration = 20 + (value * 20);
    }

    public bool Evolve
    {
        get => _evolve;
        set => _evolve = value;
    }
    
    public Genome GetBestTankGenome(TeamEnum team) => team == TeamEnum.Red ? _bestRedTankGenome : _bestBlueTankGenome;
    public void SetGenerationCount(TeamEnum team, int value)
    {
        if (team == TeamEnum.Red)
            _redGenerationCount = value;
        else 
            _blueGenerationCount = value;
        
    }
    public int GetGenerationCount(TeamEnum team) => team == TeamEnum.Red ? _redGenerationCount : _blueGenerationCount;
    public void SetPopulationCount(TeamEnum team, int value)
    {
        if (team == TeamEnum.Red)
        {
            if (_isRedTeamDataLoaded)
            {
                _redTeamData.populationCount = value;
            }
            else
            {
                redPopulationCount = value;
            }
        }
        else // if (team == TeamEnum.Blue)
        {
            if (_isBlueTeamDataLoaded)
            {
                _blueTeamData.populationCount = value;
            }
            else
            {
                bluePopulationCount = value;
            }
        }
    }

    public int GetPopulationCount(TeamEnum team)
    {
        if (team == TeamEnum.Red)
        {
            return _isRedTeamDataLoaded ? _redTeamData.populationCount : redPopulationCount;
        }
        else // if (team == TeamEnum.Blue)
        {
            return _isBlueTeamDataLoaded ? _blueTeamData.populationCount : bluePopulationCount;
        }
    }
    
    public void SetMinesCount(TeamEnum team, int value)
    {
        if (team == TeamEnum.Red)
        {
            if (_isRedTeamDataLoaded)
            {
                _redTeamData.minesCount = value;
            }
            else
            {
                redMinesCount = value;
            }
        }
        else // if (team == TeamEnum.Blue)
        {
            if (_isBlueTeamDataLoaded)
            {
                _blueTeamData.minesCount = value;
            }
            else
            {
                blueMinesCount = value;
            }
        }
    }
    public int GetMinesCount(TeamEnum team)
    {
        if (team == TeamEnum.Red)
        {
            return _isRedTeamDataLoaded ? _redTeamData.minesCount : redMinesCount;
        }
        else // if (team == TeamEnum.Blue)
        {
            return _isBlueTeamDataLoaded ? _blueTeamData.minesCount : blueMinesCount;
        }
    }
    public void SetElitesCount(TeamEnum team, int value)
    {
        if (team == TeamEnum.Red)
            redEliteCount = value;
        else 
            blueEliteCount = value;
        
    }
    public int GetElitesCount(TeamEnum team) => team == TeamEnum.Red ? redEliteCount : blueEliteCount;
    public void SetMutationChance(TeamEnum team, float value)
    {
        if (team == TeamEnum.Red)
            redMutationChance = value;
        else 
            blueMutationChance = value;
        
    }
    public float GetMutationChance(TeamEnum team) => team == TeamEnum.Red ? redMutationChance : blueMutationChance;
    public void SetMutationRate(TeamEnum team, float value)
    {
        if (team == TeamEnum.Red)
            redMutationRate = value;
        else 
            blueMutationRate = value;
        
    }
    public float GetMutationRate(TeamEnum team) => team == TeamEnum.Red ? redMutationRate : blueMutationRate;
    public int GetInputsAmount(TeamEnum team) => team == TeamEnum.Red ? redInputsCount : blueInputsCount;
    public void SetInputsAmount(TeamEnum team, int value)
    {
        if (team == TeamEnum.Red)
            redInputsCount = value;
        else 
            blueInputsCount = value;
        
    }
    public void SetHiddenLayersAmount(TeamEnum team, int value)
    {
        if (team == TeamEnum.Red)
            redHiddenLayers = value;
        else 
            blueHiddenLayers = value;
        
    }
    public int GetHiddenLayersAmount(TeamEnum team) => team == TeamEnum.Red ? redHiddenLayers : blueHiddenLayers;
    public int GetOutputsAmount(TeamEnum team) => team == TeamEnum.Red ? redOutputsCount : blueOutputsCount;
    public void SetOutputsAmount(TeamEnum team, int value)
    {
        if (team == TeamEnum.Red)
            redOutputsCount = value;
        else 
            blueOutputsCount = value;
        
    }
    public void SetNeuronsCountPerLayers(TeamEnum team, int value)
    {
        if (team == TeamEnum.Red)
            redNeuronsCountPerHiddenLayer = value;
        else 
            blueNeuronsCountPerHiddenLayer = value;
        
    }
    public int GetNeuronsCountPerLayers(TeamEnum team) => team == TeamEnum.Red ? redNeuronsCountPerHiddenLayer : blueNeuronsCountPerHiddenLayer;
    public void SetBias(TeamEnum team, float value)
    {
        if (team == TeamEnum.Red)
            redBias = value;
        else 
            blueBias = value;
        
    }
    public float GetBias(TeamEnum team) => team == TeamEnum.Red ? redBias : blueBias;
    public void SetSigmoidFactor(TeamEnum team, float value)
    {
        if (team == TeamEnum.Red)
            redSigmoidFactor = value;
        else 
            blueSigmoidFactor = value;
        
    }
    public float GetSigmoidFactor(TeamEnum team) => team == TeamEnum.Red ? redSigmoidFactor : blueSigmoidFactor;
    private float GetBestFitness(TeamEnum team)
    {

        List<Genome> population = team == TeamEnum.Red ? _redPopulation : _bluePopulation;
        Genome bestGenome = team == TeamEnum.Red ? _bestRedTankGenome : _bestBlueTankGenome;
        
        float fitness = 0;
        foreach(Genome g in population)
        {
            if (fitness < g.fitness)
            {
                fitness = g.fitness;
                if (g.fitness > bestGenome.fitness)
                {
                    bestGenome.genome = g.genome;
                }
            }
        }

        return fitness;
    }

    public float GetWorstFitnessCached(TeamEnum team) =>
        team == TeamEnum.Blue ? _blueWorstFitness : _redWorstFitness;
    
    public float GetAverageFitnessCached(TeamEnum team) =>
        team == TeamEnum.Blue ? _blueAverageFitness : _redAverageFitness;
    
    public float GetBestFitnessCached(TeamEnum team) =>
        team == TeamEnum.Blue ? _blueBestFitness : _redBestFitness;
    
    private float GetAverageFitness(TeamEnum team)
    {
        
        List<Genome> population = team == TeamEnum.Red ? _redPopulation : _bluePopulation;
        
        float fitness = 0;
        foreach(Genome g in population)
        {
            fitness += g.fitness;
        }

        return fitness / population.Count;
    }

    private float GetWorstFitness(TeamEnum team)
    {
        
        List<Genome> population = team == TeamEnum.Red ? _redPopulation : _bluePopulation;
        
        float fitness = float.MaxValue;
        foreach(Genome g in population)
        {
            if (fitness > g.fitness)
                fitness = g.fitness;
        }

        return fitness;
    }
    
    #endregion 
    
    public static PopulationManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void StartSimulation()
    {
        // Create and configure the Genetic Algorithm
        _redGeneticAlgorithm = new GeneticAlgorithm(redEliteCount, redMutationChance, redMutationRate);
        _blueGeneticAlgorithm = new GeneticAlgorithm(blueEliteCount, blueMutationChance, blueMutationRate);

        GenerateInitialPopulation();
        CreateMines();

        _isRunning = true;
    }

    public void PauseSimulation()
    {
        _isRunning = !_isRunning;
    }

    public void StopSimulation()
    {
        _isRunning = false;

        _redGenerationCount = 0;
        _blueGenerationCount = 0;

        // Destroy previous tanks (if there are any)
        DestroyTanks();

        // Destroy all mines
        DestroyMines();
    }

    // Generate the random initial population
    private void GenerateInitialPopulation()
    {

        // Destroy previous tanks (if there are any)
        DestroyTanks();

        if (_isRedTeamDataLoaded)
        {
            SetTeamData(_redTeamData, TeamEnum.Red);
        }

        for (int i = 0; i < redPopulationCount; i++)
        {
            NeuralNetwork brain = CreateBrain(TeamEnum.Red);

            Genome genome = new Genome(brain.GetTotalWeightsCount());

            if (_isRedTeamDataLoaded)
                genome.genome = _redTeamData.genome;
                
            brain.SetWeights(genome.genome);
            _redBrains.Add(brain);

            _redPopulation.Add(genome);

            Tank redTank = CreateTank(genome, brain);
            redTank.SetTeam(TeamEnum.Red);
            _redTanksList.Add(redTank);
        }

        if (_isBlueTeamDataLoaded)
        {
            SetTeamData(_blueTeamData, TeamEnum.Blue);
        }
        
        for (int i = 0; i < bluePopulationCount; i++)
        {
            NeuralNetwork brain = CreateBrain(TeamEnum.Blue);
                
            Genome genome = new Genome(brain.GetTotalWeightsCount());

            if (_isBlueTeamDataLoaded)
                genome.genome = _blueTeamData.genome;
            
            brain.SetWeights(genome.genome);
            _blueBrains.Add(brain);

            _bluePopulation.Add(genome);
                
            Tank blueTank = CreateTank(genome, brain);
            blueTank.SetTeam(TeamEnum.Blue);
            _blueTanksList.Add(blueTank);
        }
        
        _timeElapsed = 0.0f;
    }

    // Creates a new NeuralNetwork
    NeuralNetwork CreateBrain(TeamEnum team)
    {
        NeuralNetwork brain = new NeuralNetwork();
        
        if (team == TeamEnum.Red)
        {
            // Add first neuron layer that has as many neurons as inputs
            brain.AddFirstNeuronLayer(redInputsCount, redBias, redSigmoidFactor);

            for (int i = 0; i < redHiddenLayers; i++)
            {
                // Add each hidden layer with custom neurons count
                brain.AddNeuronLayer(redNeuronsCountPerHiddenLayer, redBias, redSigmoidFactor);
            }

            // Add the output layer with as many neurons as outputs
            brain.AddNeuronLayer(redOutputsCount, redBias, redSigmoidFactor);
        }
        else //if (team == TeamEnum.Blue)
        {
            // Add first neuron layer that has as many neurons as inputs
            brain.AddFirstNeuronLayer(blueInputsCount, blueBias, blueSigmoidFactor);

            for (int i = 0; i < blueHiddenLayers; i++)
            {
                // Add each hidden layer with custom neurons count
                brain.AddNeuronLayer(blueNeuronsCountPerHiddenLayer, blueBias, blueSigmoidFactor);
            }

            // Add the output layer with as many neurons as outputs
            brain.AddNeuronLayer(blueOutputsCount, blueBias, blueSigmoidFactor);
        }

        return brain;
    }

    // Evolve!!!
    void Epoch()
    {
        _redBestFitness = GetBestFitness(TeamEnum.Red);
        _redAverageFitness = GetAverageFitness(TeamEnum.Red);
        _redWorstFitness = GetWorstFitness(TeamEnum.Red);
        
        _blueBestFitness = GetBestFitness(TeamEnum.Blue);
        _blueAverageFitness = GetAverageFitness(TeamEnum.Blue);
        _blueWorstFitness = GetWorstFitness(TeamEnum.Blue);

        //Debug.Log("~~~~~~~~~~~~~~~~~~~~~~EPOCH~~~~~~~~~~~~~~~~~~~~~~");
        //Debug.Log("Red Average Fitness: " + _redAverageFitness);
        //Debug.Log("Blue Average Fitness: " + _blueAverageFitness);
        //string winTeam = _blueAverageFitness > _redAverageFitness ? "Blue Team Win! Red Team Evolves" : "Red Team Win! Blue Team Evolves";
        //Debug.Log(winTeam);
        //Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

        TeamEnum winningTeam = _blueAverageFitness > _redAverageFitness ? TeamEnum.Blue : TeamEnum.Red;
        
        OnGenerationEnd?.Invoke(winningTeam);

        if (Evolve)
        {
            if (winningTeam == TeamEnum.Blue)
            {
                EvolveTeam(TeamEnum.Red);
                _redGenerationCount++;
            }
            else// if (winningTeam == TeamEnum.Red)
            {
                EvolveTeam(TeamEnum.Blue);
                _blueGenerationCount++;
            }
        }
        

        ResetTanks();
    }

    private void EvolveTeam(TeamEnum team)
    {
        if (team == TeamEnum.Blue)
        {
            // Evolve each genome and create a new array of genomes
            Genome[] newGenomes = _blueGeneticAlgorithm.Epoch(_bluePopulation.ToArray());

            // Clear current population
            _bluePopulation.Clear();

            // Add new population
            _bluePopulation.AddRange(newGenomes);

            // Set the new genomes as each NeuralNetwork weights
            for (int i = 0; i < bluePopulationCount; i++)
            {
                NeuralNetwork brain = _blueBrains[i];

                brain.SetWeights(newGenomes[i].genome);

                _blueTanksList[i].SetBrain(newGenomes[i], brain);
            }
        }
        else //if (team == TeamEnum.Red)
        {
            // Evolve each genome and create a new array of genomes
            Genome[] newGenomes = _redGeneticAlgorithm.Epoch(_redPopulation.ToArray());

            // Clear current population
            _redPopulation.Clear();

            // Add new population
            _redPopulation.AddRange(newGenomes);

            // Set the new genomes as each NeuralNetwork weights
            for (int i = 0; i < redPopulationCount; i++)
            {
                NeuralNetwork brain = _redBrains[i];

                brain.SetWeights(newGenomes[i].genome);

                _redTanksList[i].SetBrain(newGenomes[i], brain);
            }
        }
    }

    private void ResetTanks()
    {
        for (int i = 0; i < bluePopulationCount; i++)
        {
            _blueTanksList[i].transform.position = GetRandomPos();
            _blueTanksList[i].transform.rotation = GetRandomRot();
            _blueTanksList[i].Reset();
        }
        for (int i = 0; i < redPopulationCount; i++)
        {
            _redTanksList[i].transform.position = GetRandomPos();
            _redTanksList[i].transform.rotation = GetRandomRot();
            _redTanksList[i].Reset();
        }
    }
    

    // Update is called once per frame
    void FixedUpdate () 
	{
        if (!_isRunning)
            return;
        
        float dt = Time.fixedDeltaTime;

        for (int i = 0; i < Mathf.Clamp((float)(iterationCount / 100.0f) * 50, 1, 50); i++)
        {
            foreach (Tank t in _redTanksList)
            {
                var tankPosition = t.transform.position;
                
                GameObject allyMine = GetNearestMine(TeamEnum.Red, tankPosition);
                Tank nearestAllyTankToAllyMine = GetNearestTankToMine(tankPosition, t, TeamEnum.Red);
                Tank nearestEnemyTankToAllyMine = GetNearestTankToMine(tankPosition, t, TeamEnum.Blue);

                t.SetNearestOwnTeamMine(allyMine, nearestAllyTankToAllyMine, nearestEnemyTankToAllyMine);

                GameObject enemyMine = GetNearestMine(TeamEnum.Blue, tankPosition);
                Tank nearestAllyTankToEnemyMine = GetNearestTankToMine(tankPosition, t, TeamEnum.Red);
                Tank nearestEnemyTankToEnemyMine = GetNearestTankToMine(tankPosition, t, TeamEnum.Blue);
                
                t.SetNearestEnemyTeamMine(enemyMine, nearestAllyTankToEnemyMine, nearestEnemyTankToEnemyMine);

                // Think!! 
                t.Think(dt);

                // Just adjust tank position when reaching world extents
                Vector3 pos = t.transform.position;
                if (pos.x > sceneHalfExtents.x)
                    pos.x -= sceneHalfExtents.x * 2;
                else if (pos.x < -sceneHalfExtents.x)
                    pos.x += sceneHalfExtents.x * 2;

                if (pos.z > sceneHalfExtents.z)
                    pos.z -= sceneHalfExtents.z * 2;
                else if (pos.z < -sceneHalfExtents.z)
                    pos.z += sceneHalfExtents.z * 2;

                // Set tank position
                t.transform.position = pos;
            }

            foreach (Tank t in _blueTanksList)
            {
                var tankPosition = t.transform.position;
                
                GameObject allyMine = GetNearestMine(TeamEnum.Blue, tankPosition);
                Tank nearestAllyTankToAllyMine = GetNearestTankToMine(tankPosition, t, TeamEnum.Blue);
                Tank nearestEnemyTankToAllyMine = GetNearestTankToMine(tankPosition, t, TeamEnum.Red);

                t.SetNearestOwnTeamMine(allyMine, nearestAllyTankToAllyMine, nearestEnemyTankToAllyMine);

                GameObject enemyMine = GetNearestMine(TeamEnum.Red, tankPosition);
                Tank nearestAllyTankToEnemyMine = GetNearestTankToMine(tankPosition, t, TeamEnum.Blue);
                Tank nearestEnemyTankToEnemyMine = GetNearestTankToMine(tankPosition, t, TeamEnum.Red);
                
                t.SetNearestEnemyTeamMine(enemyMine, nearestAllyTankToEnemyMine, nearestEnemyTankToEnemyMine);

                // Think!! 
                t.Think(dt);

                // Just adjust tank position when reaching world extents
                Vector3 pos = t.transform.position;
                if (pos.x > sceneHalfExtents.x)
                    pos.x -= sceneHalfExtents.x * 2;
                else if (pos.x < -sceneHalfExtents.x)
                    pos.x += sceneHalfExtents.x * 2;

                if (pos.z > sceneHalfExtents.z)
                    pos.z -= sceneHalfExtents.z * 2;
                else if (pos.z < -sceneHalfExtents.z)
                    pos.z += sceneHalfExtents.z * 2;

                // Set tank position
                t.transform.position = pos;
            }
            
            // Check the time to evolve
            _timeElapsed += dt;
            if (_timeElapsed >= generationDuration)
            {
                _timeElapsed -= generationDuration;
                Epoch();
                break;
            }
        }
	}

#region Helpers
    Tank CreateTank(Genome genome, NeuralNetwork brain)
    {
        Vector3 position = GetRandomPos();
        GameObject go = Instantiate<GameObject>(tankPrefab, position, GetRandomRot());
        Tank t = go.GetComponent<Tank>();
        t.SetBrain(genome, brain);
        return t;
    }

    void DestroyMines()
    {
        foreach (GameObject go in _blueMinesList)
            Destroy(go);

        foreach (GameObject go in _redMinesList)
            Destroy(go);
        
        _blueMinesList.Clear();
        _redMinesList.Clear();
    }

    void DestroyTanks()
    {
        foreach (Tank go in _redTanksList)
            Destroy(go.gameObject);
        
        foreach (Tank go in _blueTanksList)
            Destroy(go.gameObject);

        _redTanksList.Clear();
        _redPopulation.Clear();
        _redBrains.Clear();
        
        _blueTanksList.Clear();
        _bluePopulation.Clear();
        _blueBrains.Clear();
        
    }

    void CreateMines()
    {
        // Destroy previous created mines
        DestroyMines();

        for (int i = 0; i < redMinesCount; i++)
        {
            Vector3 position = GetRandomPos();
            GameObject go = Instantiate<GameObject>(minePrefab, position, Quaternion.identity);
            SetMineTeam(TeamEnum.Red, go);
        }
        
        for (int i = 0; i < blueMinesCount; i++)
        {
            Vector3 position = GetRandomPos();
            GameObject go = Instantiate<GameObject>(minePrefab, position, Quaternion.identity);
            SetMineTeam(TeamEnum.Blue, go);
        }
        
    }

    void SetMineTeam(TeamEnum team, GameObject go)
    {
        if (team == TeamEnum.Blue)
        {
            go.GetComponent<Renderer>().material.color = Color.blue;
            _blueMinesList.Add(go);
        }
        else
        {
            go.GetComponent<Renderer>().material.color = Color.red;
            _redMinesList.Add(go);
        }

    }

    public TeamEnum GetMineTeam(GameObject mine) => _blueMinesList.Contains(mine) ? TeamEnum.Blue : TeamEnum.Red;

    public void RelocateMine(GameObject mine)
    {
        TeamEnum mineTeam;
        if (_blueMinesList.Contains(mine))
        {
            _blueMinesList.Remove(mine);
            mineTeam = TeamEnum.Blue;
        }
        else
        {
            _redMinesList.Remove(mine);
            mineTeam = TeamEnum.Red;
        }
        SetMineTeam(mineTeam, mine);
        mine.transform.position = GetRandomPos();
    }

    Vector3 GetRandomPos()
    {
        return new Vector3(Random.value * sceneHalfExtents.x * 2.0f - sceneHalfExtents.x, 0.0f, Random.value * sceneHalfExtents.z * 2.0f - sceneHalfExtents.z); 
    }

    Quaternion GetRandomRot()
    {
        return Quaternion.AngleAxis(Random.value * 360.0f, Vector3.up);
    }

    GameObject GetNearestMine(TeamEnum team, Vector3 pos)
    {
        List<GameObject> mines = team == TeamEnum.Blue ? _blueMinesList : _redMinesList;

        float distance = float.MaxValue;
        GameObject nearestMine = null;
        
        foreach (GameObject go in mines)
        {
            float newDist = (go.transform.position - pos).sqrMagnitude;
            if (newDist < distance)
            {
                distance = newDist;
                nearestMine = go;
            }
        }

        return nearestMine;
    }

    Tank GetNearestTankToMine(Vector3 position, Tank ownTank, TeamEnum team)
    {
        List<Tank> tanksList = team == TeamEnum.Blue ? _blueTanksList : _redTanksList;
        
        Tank tank = default;
        float distanceToMine = float.MaxValue;
        foreach (var tankUnit in tanksList)
        {
            if (ownTank != tankUnit)
            {
                float newDistance = (tankUnit.transform.position - position).sqrMagnitude;
                if (newDistance < distanceToMine)
                {
                    distanceToMine = newDistance;
                    tank = tankUnit;
                }
            }
        }
        return tank;
    }

    private void SetTeamData(GenomeData data, TeamEnum team)
    {
        if (team == TeamEnum.Blue)
        {
            bluePopulationCount = data.populationCount;
            blueMinesCount = data.minesCount;
        }
        else
        {
            redPopulationCount = data.populationCount;
            redMinesCount = data.minesCount;
        }
        
        SetElitesCount(team, data.eliteCount);
        SetMutationChance(team, data.mutationChance);
        SetMutationRate(team, data.mutationRate);
        
        SetInputsAmount(team, data.inputsCount);
        SetHiddenLayersAmount(team, data.hiddenLayers);
        SetOutputsAmount(team, data.outputsCount);
        SetNeuronsCountPerLayers(team, data.neuronCountPerHiddenLayer);
        
        SetBias(team, data.bias);
        SetSigmoidFactor(team, data.sigmoid);
        
        SetGenerationCount(team, data.generationCount);
    }

    private GenomeData GetTeamData(TeamEnum team)
    {
        GenomeData data = new GenomeData();
        data.populationCount = GetPopulationCount(team);
        data.minesCount = GetMinesCount(team);

        data.eliteCount = GetElitesCount(team);
        data.mutationChance = GetMutationChance(team);
        data.mutationRate = GetMutationRate(team);

        data.inputsCount = GetInputsAmount(team);
        data.hiddenLayers = GetHiddenLayersAmount(team);
        data.outputsCount = GetOutputsAmount(team);
        data.neuronCountPerHiddenLayer = GetNeuronsCountPerLayers(team);

        data.bias = GetBias(team);
        data.sigmoid = GetSigmoidFactor(team);

        data.generationCount = GetGenerationCount(team);

        data.genome = GetBestTankGenome(team).genome;

        return data;
    }
    
    
    #endregion

}
