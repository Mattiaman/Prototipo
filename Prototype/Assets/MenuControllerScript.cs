using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuControllerScript : MonoBehaviour
{
    const string  DIR_NAME= "/ImageARDirectory";

    public GameObject quadro;
    public GameObject griglia;
	public GameObject buttonSample;

	public Text noImageMessage;
    DirectoryInfo appDir;
    FileInfo[] images;
    

    // Start is called before the first frame update
    void Start()
    {
        if (!Directory.Exists(Application.persistentDataPath + DIR_NAME))
        {
            Directory.CreateDirectory(Application.persistentDataPath + DIR_NAME);
        }
        appDir = new DirectoryInfo(Application.persistentDataPath + DIR_NAME);
        images = appDir.GetFiles();
		Debug.Log(appDir.FullName);
		if (images.Length != 0)
		{
			InizializzaLista();
		}
		else
		{
			noImageMessage.enabled = true;
		}
    }

	// Update is called once per frame
	void Update()
    {
        
    }

	private void InizializzaLista()
	{
		foreach (FileInfo f in images)
		{
			Sprite sprite= SpriteManager.LoadSprite(f.FullName);
			GameObject button = (GameObject)Instantiate(buttonSample,griglia.transform);
			int sampleWidth = button.GetComponent<Image>().sprite.texture.width;
			int sampleHeight = button.GetComponent<Image>().sprite.texture.height;
			button.GetComponent<Image>().sprite = sprite;
			button.GetComponent<ButtonIdentity>().setTexture(sprite.texture);
			button.GetComponent<ButtonIdentity>().setSource(f);

			//button.transform.SetParent(griglia.transform,false);
		}
	}


}
