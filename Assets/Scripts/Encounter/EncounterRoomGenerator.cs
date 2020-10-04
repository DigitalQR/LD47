using DQR.Types;
using DQR.Voxel;
using DQR.Voxel.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class EncounterRoomGenerator : MonoBehaviour
{
	[SerializeField]
	private Vector3Int m_RoomSize;

	[SerializeField]
	private VoxelModelGenerationSettings m_ModelSettings = default;

	[Header("Materials")]
	[SerializeField]
	private float m_MinBlendAmount = 0.5f;

	[SerializeField]
	private float m_BaseBlendThreshold = 0.3f;

	[SerializeField]
	private VoxelBrush m_BaseFloorMaterial = null;

	[SerializeField]
	private WeightedCollection<VoxelBrush> m_FloorMaterials = null;

	[SerializeField]
	private VoxelBrush m_BaseWallMaterial = null;

	[SerializeField]
	private WeightedCollection<VoxelBrush> m_WallMaterials = null;

	[SerializeField]
	private NoiseSampler m_BlendSampler = null;

	[SerializeField]
	private NoiseSampler m_MaterialSampler = null;


	private bool m_IsGenerating = false;
	private VoxelCommonMaterialResolver m_MaterialResolver = null;

	private void Start()
    {
		KickoffGenerate();
	}
	
	public void KickoffGenerate()
	{
		if (!m_IsGenerating)
		{
			m_IsGenerating = true;

			IVoxelVolume volume = GenerateVolume();
			var req = VoxelModelGenerationRequest.NewModelRequestSync(volume, m_MaterialResolver, m_ModelSettings);
			
			MeshFilter filter = GetComponent<MeshFilter>();
			filter.mesh = req.GetMeshOutput();

			m_MaterialResolver = null;
		}
	}

	private IVoxelVolume GenerateVolume()
	{
		BoundsInt bounds = new BoundsInt();
		bounds.SetMinMax(Vector3Int.zero, m_RoomSize - Vector3Int.one);

		StaticArrayVoxelVolume volume = new StaticArrayVoxelVolume(bounds);
		m_MaterialResolver = new VoxelCommonMaterialResolver();

		for (int x = 0; x < m_RoomSize.x; ++x)
			for (int z = 0; z < m_RoomSize.z; ++z)
			{
				float noise = m_BlendSampler.SampleNoise(transform, x, z);

				float height = 1.0f;
				bool isWall = false;

				if (z == 0 || z == m_RoomSize.z - 1)
				{
					isWall = true;
					height = m_RoomSize.y;
				}

				for (int y = 0; y < height; ++y)
				{
					m_MaterialResolver.SetVoxelMaterial(volume, x, y, z, (isWall ? m_BaseWallMaterial : m_BaseFloorMaterial).Material);

					if (noise >= m_BaseBlendThreshold)
					{
						float blendNoise = (noise - m_BaseBlendThreshold) / (1.0f - m_BaseBlendThreshold);

						float selectNoise = m_MaterialSampler.SampleNoise(transform, x, z);
						VoxelBrush blend = (isWall ? m_WallMaterials : m_FloorMaterials).SelectValue(selectNoise);

						if (blend != null)
							m_MaterialResolver.BlendVoxelMaterial(volume, new Vector3Int(x, y, z), blend.Material, Mathf.Max(blendNoise, m_MinBlendAmount));
					}
				}
			}

		return volume;
	}
}
