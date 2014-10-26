using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour {
	static public LevelLoader ll;
	public GameObject playerPrefab;
	public GameObject groundPrefab;
	public GameObject spikePrefab;

	//TILES
	public GameObject reverseTilePrefab;
	public GameObject jumpTilePrefab;

	bool __________________________;
	
	public Vector3 startPosition = new Vector3(0f, 0f, 0f);
	private int levelToLoad = 0;
	
	private List<string> levelFileList;
	
	private List<GameObject> allObjs;
	private int yposMax;
	
	// Use this for initialization
	void Start () {
		ll = this;
		allObjs = new List<GameObject> ();
		levelFileList = new List<string> ();
		
		levelFileList.Add ("Level1");
		
		levelToLoad = 0;
		loadLevel ();
	}
	
	public void resetLevelCounter(){
		levelToLoad = 0;
	}
	
	public void incrementLevelCounter(){
		levelToLoad++;
		if(levelToLoad >= levelFileList.Count){
			levelToLoad = 0;
		}
	}
	
	public void loadLevel(){

		for(int i = 0; i < allObjs.Count; ++i){
			if(allObjs[i] == null)
				continue;
			Destroy(allObjs[i]);
		}
		allObjs.Clear ();

		TextAsset textObj = Resources.Load(levelFileList[levelToLoad]) as TextAsset;
		
		string[] wholeSplit = textObj.text.Split ('\n');
		
		GameObject whatToMake = null;
		GameObject newObj;
		int counter = 0;
		int xpos = 0;
		int ypos = yposMax = wholeSplit.Length;
		yposMax--;
		
		//int ypos = wholeSplit.Length;
		foreach(string line in wholeSplit){
			//skip comment lines and empty lines
			if(line.Length == 0){
				continue;
			}
			if(line[0] == '#'){
				continue;
			}
			
			ypos--;
			
			string[] lineSplit = line.Split(' ');
			foreach(string obj in lineSplit){
				Vector3 position = Vector3.zero;
				whatToMake = null;
				xpos++;
				if(obj == "G"){
					whatToMake = groundPrefab;
				}
				else if (obj == "S"){
					whatToMake = spikePrefab;
				}
				/*
				else if (obj == "P"){
					whatToMake = playerPrefab;
				}
				else if (obj == "J"){
					whatToMake = jumpTilePrefab;
				}
				else if (obj == "E"){
					whatToMake = exitDoorPrefab;
				}
				else if (obj == "R"){
					whatToMake = reverseTilePrefab;
				}
				*/
				
				if(whatToMake == null){
					continue;
				}
				
				newObj = Instantiate(whatToMake) as GameObject;

				position.x = (float)xpos;
				position.y = (float)ypos;
				
				newObj.transform.position = position;
				
				allObjs.Add(newObj);
			}
			xpos = 0;
		}
	}

	
	// Update is called once per frame
	void Update () {
		
	}
	
	void FixedUpdate(){
		
	}
}