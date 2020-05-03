using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitSpawnController : MonoBehaviour
{
	public UnitData unitData;
	public GlobalLevelData levelData;
	public TextMeshProUGUI unitsRemainingText;
	public Image cooldownOverlay;

	private int maxUnits;
	private int unitsRemaining;
	private float cooldownRemaining;

	public void decrementUnits()
	{
		unitsRemaining -= 1;
		unitsRemainingText.text = unitsRemaining.ToString();
		cooldownRemaining += 1;
	}

	void Start()
	{
		maxUnits = levelData.actionchoices[unitData.index];
		unitsRemaining = maxUnits;
		unitsRemainingText.text = unitsRemaining.ToString();
		cooldownRemaining = 0;
	}
	
	void Update()
	{
		if (cooldownRemaining > 0)
		{
			cooldownRemaining -= (Time.deltaTime / unitData.unitCooldown * maxUnits * 0.33f);
			unitsRemaining = maxUnits - Mathf.CeilToInt(cooldownRemaining);
			unitsRemainingText.text = unitsRemaining.ToString();
			cooldownOverlay.fillAmount = cooldownRemaining % 1f;
		}
		if (Input.GetButtonDown(unitData.buttonToSummon))
		{
			if (unitsRemaining > 0)
			{
				levelData.SpawnUnitType = unitData.index;
				levelData.PlayerState = State.Selected;
			}
		}
	}
}
