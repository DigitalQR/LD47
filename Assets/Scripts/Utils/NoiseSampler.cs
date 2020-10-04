using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSampler
{
	[SerializeField]
	private Vector2 m_Offset = Vector2.one;
	
	[SerializeField]
	private float m_NoiseScale = 1.0f;

	[SerializeField]
	private float m_NoiseFalloff = 0.9f;

	[SerializeField]
	private int m_NoiseOctaves = 4;

	public float SampleNoise(Transform source, float x, float y)
	{
		return SampleNoise(-source.position.x + x, -source.position.z + y);
	}

	public float SampleNoise(float x, float y)
	{
		float noise = 0.0f;
		float falloff = 1.0f;

		for (int i = 0; i < m_NoiseOctaves; ++i)
		{
			float scale = (1.0f / (i + 1.0f)) * m_NoiseScale;
			float rawNoise = Mathf.PerlinNoise(m_Offset.x + x * scale, m_Offset.y + y * scale);

			noise += ((rawNoise * 2.0f) - 1.0f) * m_NoiseFalloff;

			falloff *= m_NoiseFalloff;
		}

		return Mathf.Clamp01((noise + 1.0f) * 0.5f);
	}
}
