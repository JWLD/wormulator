using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AspectRatioHandler : MonoBehaviour 
{
	public List<RectTransform> UIPanels = new List<RectTransform>();

	void Start()
	{
		if (Camera.main.aspect >= 1.7)
		{
			// adjusts the UI borders
			for (int i = 0; i < UIPanels.Count; i++)
			{
				UIPanels[i].offsetMin = new Vector2(133, UIPanels[i].offsetMin.y);
				UIPanels[i].offsetMax = new Vector2(-133, UIPanels[i].offsetMax.y);
			}
		}
		else if (Camera.main.aspect >= 1.5)
		{
			// adjusts the UI borders
			for (int i = 0; i < UIPanels.Count; i++)
			{
				UIPanels[i].offsetMin = new Vector2(49.5f, UIPanels[i].offsetMin.y);
				UIPanels[i].offsetMax = new Vector2(-49f, UIPanels[i].offsetMax.y);
			}
		}

	}
}