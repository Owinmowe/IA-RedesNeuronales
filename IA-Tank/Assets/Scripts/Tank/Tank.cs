using UnityEngine;

public class Tank : TankBase
{
    float fitness = 0;
    public override void Reset()
    {
        fitness = 1;
    }

    protected override void OnThink(float dt) 
	{
        Transform ownTransform = transform;
        Vector3 ownPosition = ownTransform.position;
        Vector3 dir = ownTransform.forward;

        Vector3 closestAllyMinePosition = nearestAllyTeamMine.transform.position;
        
        float distanceToClosestAllyMine = (ownPosition - closestAllyMinePosition).sqrMagnitude;
        Vector3 dirToClosestAllyMine = GetDirToMine(nearestAllyTeamMine);
        float allyDistanceToClosestAllyMine =
            (nearestAllyTeamTankToAllyMine.transform.position - closestAllyMinePosition).sqrMagnitude;
        float enemyDistanceToClosestAllyMine =
            (nearestEnemyTeamTankToAllyMine.transform.position - closestAllyMinePosition).sqrMagnitude;
        
        
        Vector3 closestEnemyMinePosition = nearestEnemyTeamMine.transform.position;
        
        float distanceToClosestEnemyMine = (ownPosition - closestEnemyMinePosition).sqrMagnitude;
        Vector3 dirToClosestEnemyMine = GetDirToMine(nearestEnemyTeamMine);
        float allyDistanceToClosestEnemyMine =
            (nearestAllyTeamTankToEnemyMine.transform.position - closestEnemyMinePosition).sqrMagnitude;
        float enemyDistanceToClosestEnemyMine =
            (nearestEnemyTeamTankToEnemyMine.transform.position - closestEnemyMinePosition).sqrMagnitude;
        
        inputs[0] = dir.x;
        inputs[1] = dir.z;
        inputs[2] = distanceToClosestAllyMine;
        inputs[3] = dirToClosestAllyMine.x;
        inputs[4] = dirToClosestAllyMine.z;
        inputs[5] = allyDistanceToClosestAllyMine;
        inputs[6] = enemyDistanceToClosestAllyMine;
        inputs[7] = distanceToClosestEnemyMine;
        inputs[8] = dirToClosestEnemyMine.x;
        inputs[9] = dirToClosestEnemyMine.z;
        inputs[10] = allyDistanceToClosestEnemyMine;
        inputs[11] = enemyDistanceToClosestEnemyMine;

        float[] output = brain.Synapsis(inputs);

        SetForces(output[0], output[1], dt);
	}
    
    protected override void OnTakeMine(GameObject mine)
    {
        fitness += PopulationManager.Instance.GetMineTeam(mine) == team ? 10 : 50;
        genome.fitness = fitness;
    }
}
