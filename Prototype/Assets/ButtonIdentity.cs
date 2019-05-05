using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonIdentity : MonoBehaviour
{
	public GameObject quadro;
	public MenuControllerScript controller;
	Texture2D texture;
	FileInfo source;


	public void clickEvent()
	{
		quadro.GetComponent<MeshRenderer>().material.mainTexture = texture;
		SceneManager.LoadScene("prototype");
	}

	public Texture2D getTexture() {return texture;}
	public FileInfo getSource() {return source;}

	public void setTexture(Texture2D t) {texture = t;}
	public void setSource(FileInfo f) {source=f;}

}
