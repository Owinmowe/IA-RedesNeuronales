using UnityEngine;
using System.Collections;

public class TankBase : MonoBehaviour
{
    public Renderer[] tankRenderers;
    public float Speed = 10.0f;
    public float RotSpeed = 20.0f;

    protected Genome genome;
	protected NeuralNetwork brain;
    
    protected TeamEnum team;
    
    protected GameObject nearestAllyTeamMine;
    protected Tank nearestAllyTeamTankToAllyMine;
    protected Tank nearestEnemyTeamTankToAllyMine;
    
    protected GameObject nearestEnemyTeamMine;
    protected Tank nearestAllyTeamTankToEnemyMine;
    protected Tank nearestEnemyTeamTankToEnemyMine;
    
    protected Tank[] nearestTankToMines = new Tank[4];
    
    protected float[] inputs;

    public void SetBrain(Genome genome, NeuralNetwork brain)
    {
        this.genome = genome;
        this.brain = brain;
        inputs = new float[brain.InputsCount];
        Reset();
    }

    public void SetTeam(TeamEnum newTeam)
    {
        team = newTeam;

        Color newColor = team == TeamEnum.Red ? Color.red : Color.blue;
        
        foreach (var rend in tankRenderers)
        {
            rend.material.color = newColor;
        }
    }
    
    public void SetNearestOwnTeamMine(GameObject mine, Tank closestAllyTank, Tank closestEnemyTank)
    {
        nearestAllyTeamMine = mine;
        nearestAllyTeamTankToAllyMine = closestAllyTank;  
        nearestEnemyTeamTankToAllyMine = closestEnemyTank;
    }    
    
    public void SetNearestEnemyTeamMine(GameObject mine, Tank closestAllyTank, Tank closestEnemyTank)
    {
        nearestEnemyTeamMine = mine;
        nearestAllyTeamTankToEnemyMine = closestAllyTank;  
        nearestEnemyTeamTankToEnemyMine = closestEnemyTank;
    }

    public Vector3 GetDirToMine(GameObject mine)
    {
        return (mine.transform.position - this.transform.position).normalized;
    }
    
    protected bool IsCloseToMine(GameObject mine)
    {
        return (this.transform.position - mine.transform.position).sqrMagnitude <= 2.0f;
    }

    protected void SetForces(float leftForce, float rightForce, float dt)
    {
        Vector3 pos = this.transform.position;
        float rotFactor = Mathf.Clamp((rightForce - leftForce), -1.0f, 1.0f);
        this.transform.rotation *= Quaternion.AngleAxis(rotFactor * RotSpeed * dt, Vector3.up);
        pos += this.transform.forward * Mathf.Abs(rightForce + leftForce) * 0.5f * Speed * dt;
        this.transform.position = pos;
    }

	public void Think(float dt) 
	{
        OnThink(dt);

        if(IsCloseToMine(nearestAllyTeamMine))
        {
            OnTakeMine(nearestAllyTeamMine);
            PopulationManager.Instance.RelocateMine(nearestAllyTeamMine);
        }
        
        if(IsCloseToMine(nearestEnemyTeamMine))
        {
            OnTakeMine(nearestEnemyTeamMine);
            PopulationManager.Instance.RelocateMine(nearestEnemyTeamMine);
        }
	}

    protected virtual void OnThink(float dt)
    {

    }

    protected virtual void OnTakeMine(GameObject mine)
    {
    }

    public virtual void Reset()
    {

    }
}
