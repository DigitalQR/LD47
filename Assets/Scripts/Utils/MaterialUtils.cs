using DQR.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MaterialUtils
{
	public static void AssignTints(Material mat, params Color[] tints)
	{
		for (int i = 0; i < 4; ++i)
		{
			string prop = "Tint" + i.ToString();
			Color c = i < tints.Length ? tints[i] : Color.white;

			if(mat.HasProperty(prop))
				mat.SetColor(prop, c);
		}
	}

	public static void AssignTints(GameObject obj, params Color[] tints)
	{
		foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
		{
			Material mat = new Material(renderer.material);
			AssignTints(mat, tints);
			renderer.material = mat;
		}
	}
}

[System.Serializable]
public class TintVariationCollection
{
	[SerializeField]
	private WeightedCollection<Color>[] m_Tints = null;

	public void ApplyVariationTo(GameObject obj)
	{
		var tintOutput = m_Tints.Select((col) => col.SelectRandom());
		MaterialUtils.AssignTints(obj, tintOutput.ToArray());
	}
}