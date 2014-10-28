using UnityEngine;
using System.Collections;

public class DummyScript : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "jumpBullet"){
			this.rigidbody.velocity = new Vector3(0,10,0);
			Destroy(col.gameObject);
		}
		if(col.gameObject.tag == "rightBullet"){
			this.rigidbody.velocity = new Vector3(10,0,0);
			Destroy(col.gameObject);
		}
		if(col.gameObject.tag == "leftBullet"){
			
			this.rigidbody.velocity = new Vector3(-10,0,0);
			Destroy(col.gameObject);	
		}
	}
}