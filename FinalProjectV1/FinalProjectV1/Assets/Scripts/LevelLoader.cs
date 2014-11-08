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
		string[] lineSplit1 = wholeSplit[0].Split(' ');
		int maxXpos = lineSplit1.Length;
		int maxYpos = wholeSplit.Length;

		string[,] mapGrid = new string[lineSplit1.Length, wholeSplit.Length];

		//Create grid
		GameObject whatToMake = null;
		GameObject newObj;
		int counter = 0;
		int xpos = 0;
		int ypos = wholeSplit.Length;

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
			if(ypos < 0){
				break;
			}
			
			string[] lineSplit = line.Split(' ');
			foreach(string obj in lineSplit){
				Vector3 position = Vector3.zero;
				whatToMake = null;
				if(xpos >= maxXpos){
					break;
				}

				mapGrid[xpos, ypos] = obj;
				xpos++;
				 if (obj == "S"){
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

				position.x = (float)(xpos - 1);
				position.y = (float)ypos;
				
				newObj.transform.position = position;
				
				allObjs.Add(newObj);

			}
			xpos = 0;
		}
		//We have the entire map file in a grid now. Make vertical walls
		xpos = 0;
		ypos = 0;
		//Top and bottom coordinates for each column
		int topYPos = 0;
		int botYPos = 0;
		for (xpos = 0; xpos < maxXpos; xpos++) {
			for(ypos = 0; ypos < maxYpos; ypos++){
				if(mapGrid[xpos, ypos] == "G"){
					//This is a ground object, lets' make the column
					//mapGrid[xpos, ypos] = "0";
					botYPos = ypos;
					for(int tempY = ypos; tempY <= maxYpos; tempY++){
						//We've reached the top of the wall, so let's instantiate it.
						if( tempY == maxYpos || mapGrid[xpos, tempY] != "G"){

							if(tempY != maxYpos){
								if(mapGrid[xpos, tempY] == "G"){
									mapGrid[xpos, tempY] = "0";
								}
							}

							int wallHeight = topYPos - botYPos + 1;
							float wallMidpoint = (topYPos + botYPos)/2.0f;

							newObj = Instantiate(groundPrefab) as GameObject;
							Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
							Vector3 scale = new Vector3(0.0f, 0.0f, 0.0f);
							position.x = (float)xpos;
							position.y = (float)wallMidpoint;
							position.z = 0.0f;

							scale.x = 1.0f;
							scale.y = (float)wallHeight;
							scale.z = 1.0f;

							newObj.transform.position = position;
							newObj.transform.localScale = scale;
							topYPos = 0;
							botYPos = 0;
							break;
						} else if (mapGrid[xpos, tempY] == "G"){
							topYPos = tempY;
							mapGrid[xpos, tempY] = "0";
						}
					}
				}
			}
		}


	}

	
	// Update is called once per frame
	void Update () {
		
	}
	
	void FixedUpdate(){
		
	}
}