using DQR.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : SingletonBehaviour<PopupManager>
{
	[SerializeField]
	private PopupText3D m_Popup3DPrefab = null;

	protected override void SingletonInit()
	{
		base.SingletonInit();
		m_Popup3DPrefab.gameObject.SetActive(false);
	}

	public PopupText3D CreatePopup3D(string text, Vector3 location, float scale = 1.0f)
	{
		return CreatePopup3D(text, location, scale, Color.white);
	}

	public PopupText3D CreatePopup3D(string text, Vector3 location, float scale, Color colour, FontStyle style = FontStyle.Normal)
	{
		return m_Popup3DPrefab.Spawn(text, location, scale, colour, style);
	}

	public void CreateHeadingPopup3D(string text, string subHeading, Vector3 location, float scale)
	{
		PopupText3D heading = CreatePopup3D(text, location, scale, Color.white, FontStyle.BoldAndItalic);
		PopupText3D subheading = CreatePopup3D(subHeading, location, scale * 0.5f, Color.grey, FontStyle.Italic);
		subheading.transform.position += Vector3.down * scale * 0.15f;

		heading.ExtendLifetime(2.0f);
		subheading.ExtendLifetime(2.0f);
	}
}
