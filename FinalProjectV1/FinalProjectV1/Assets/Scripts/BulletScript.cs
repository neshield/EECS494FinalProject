using UnityEngine;
using System.Collections;


public enum bulletType{PUSH, PULL, JUMP};

public class BulletScript : MonoBehaviour {
	public Material pushMat;
	public Material pullMat;
	public Material jumpMat;
	
	private bulletType btype;
	
	private Vector3 origPosition;
	private float maxDistance;
	
	private PlayerObject playerRef;
	
	public void setPlayerRef(PlayerObject playerRef_){
		playerRef = playerRef_;
	}

	public PlayerObject getPlayerRef(){
		return playerRef;
	}

	
	public void setPush(){
		btype = bulletType.PUSH;
		renderer.material = pushMat;
	}
	
	public void setPull(){
		btype = bulletType.PULL;
		renderer.material = pullMat;
	}
	
	public void setJump(){
		btype = bulletType.JUMP;
		renderer.material = jumpMat;
	}
	
	public bulletType getBulletType(){
		return btype;
	}
	
	public void setVelocity(Vector3 velocity_){
		rigidbody.velocity += velocity_;
	}
	
	public void setPosition(Vector3 position_){
		transform.position = position_;
	}
	
	// Use this for initialization
	void Start () {
		origPosition = transform.position;
		maxDistance = 50;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void FixedUpdate(){
		Vector3 diff = origPosition - transform.position;
		
		if(Mathf.Abs(diff.x) > maxDistance){
			Destroy(gameObject);
		}
	}
	
	void OnTriggerEnter(Collider other){
		DummyScriptNathan dsn = other.GetComponent<DummyScriptNathan> ();
		if(dsn){
			Destroy (gameObject);
		}
	}
}
