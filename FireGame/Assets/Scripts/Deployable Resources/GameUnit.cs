using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnit : MonoBehaviour
{
	public AIManager spawnUnit;

	[SerializeField]
	private Renderer[] renderers;
	public Shader standardShader;
	public Shader outlineShader;

    public void SelectUnit()
    {
		if (!spawnUnit.unitsSelected.Contains(gameObject))
			spawnUnit.unitsSelected.Add(gameObject);
        foreach (Renderer renderer in renderers)
            renderer.material.shader = outlineShader;
    }

    public void DeselectUnit()
    {
        foreach (Renderer renderer in renderers)
            renderer.material.shader = standardShader;
    }

    public void OnBecameInvisible()
	{
		spawnUnit.unitsVisible.Remove(gameObject);
	}

	public void OnBecameVisible()
	{
		spawnUnit.unitsVisible.Add(gameObject);
	}

	public void OnDestroy()
	{
		spawnUnit.unitsSelected.Remove(gameObject);
		spawnUnit.unitsVisible.Remove(gameObject);
	}
}
