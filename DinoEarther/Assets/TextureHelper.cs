using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureHelper {

	public static Texture2D ChangedFormat(this Texture2D oldTexture, TextureFormat newFormat) {
		Texture2D newTexture = new Texture2D(oldTexture.width, oldTexture.height, newFormat, false);
		newTexture.SetPixels (oldTexture.GetPixels ());
		newTexture.Apply ();
		return newTexture;
	}
}
