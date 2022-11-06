[System.Serializable]
public class GenomeData
{
    
    public int populationCount = 40;
    public int minesCount = 50;
    
    public int eliteCount = 4;
    public float mutationChance = 0.10f;
    public float mutationRate = 0.01f;
    
    public float[] genome;
    public int inputsCount;
    public int hiddenLayers;
    public int outputsCount;
    public int neuronCountPerHiddenLayer;
    public float bias;
    public float sigmoid;

    public int generationCount;
}
