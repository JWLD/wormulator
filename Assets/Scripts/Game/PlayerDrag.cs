using UnityEngine;
using System.Collections;

public class PlayerDrag : MonoBehaviour 
{
	public ClickMover clickMover;
	public MainMenuMover mainMenuMover;

//	#if UNITY_EDITOR

	void OnMouseDown()
	{
		if (clickMover != null)
			clickMover.playerSelect = true;

		if (mainMenuMover != null)
			mainMenuMover.playerSelect = true;
	}

	void OnMouseUp()
	{
		if (clickMover != null)
			clickMover.playerSelect = false;

		if (mainMenuMover != null)
			mainMenuMover.playerSelect = false;
	}

//	#endif
//
//	#if UNITY_IOS
//
//	void OnPointerDown()
//	{
//		if (clickMover != null)
//			clickMover.playerSelect = true;
//
//		if (mainMenuMover != null)
//			mainMenuMover.playerSelect = true;
//	}
//
//	void OnPointerUp()
//	{
//		if (clickMover != null)
//			clickMover.playerSelect = false;
//
//		if (mainMenuMover != null)
//			mainMenuMover.playerSelect = false;
//	}
//
//	#endif
}