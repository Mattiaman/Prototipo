using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SpriteManager
{
	static public Sprite LoadSprite(string path)
	{
		Sprite s;
		byte[] byteArray = File.ReadAllBytes(path);
		Texture2D t = new Texture2D(1,1);
		t.LoadImage(byteArray);
		s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero, 1f);

		return s;
	}

}
