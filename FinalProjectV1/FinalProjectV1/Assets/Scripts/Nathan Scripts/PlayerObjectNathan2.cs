using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerObjectNathan2 : MonoBehaviour {
	static public PlayerObjectNathan2 P;
	
	public Vector3 startPosition;
	public Vector3 velocity;
	public float xSpeed = 4.0f; 
	public float jumpSpeed = 12.0f;
	public static Vector3 playerGravity = new Vector3(0.0f, -15f, 0.0f);
	
	public bool playing;
	
	public List<Collider> groundList;
	public List<Collider> leftWallList;
	public List<Collider> rightWallList;
	
	bool ________________________________________;
	//Physics stuff
	private int hitGroundTimer = 0;
	private Vector3 spawnPos = new Vector3(7.0f, 7.1f, 0.0f);
	
	Vector3	curGroundRightCornerPos;
	Vector3 curGroundLeftCornerPos;
	Vector3 prevGroundLeftCornerPos;
	Vector3 prevGroundRightCornerPos;
	Vector3	curTopRightCornerPos;
	Vector3 curTopLeftCornerPos;
	Vector3 prevTopLeftCornerPos;
	Vector3 prevTopRightCornerPos;
	
	bool jumpQueued;
	
	
	//SCOTT AND MATT SHOOTING STUFF
	public GameObject leftBullet;
	public GameObject rightBullet;
	public GameObject jumpBullet;
	public GameObject currentBullet;
	int facing = 0;
	
	
	// Use this for initialization
	void Start () {
		P = this;
		RestoreDefaults ();
		playing = true;
		
		jumpQueued = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton ("Jump2")) {
			jumpQueued = true;
		}
		
		//SCOTT AND MATT
		if(Input.GetKeyDown("j")){
			currentBullet = Instantiate (leftBullet) as GameObject;
			if (facing == 1) {
				currentBullet.transform.position = transform.position;
				currentBullet.rigidbody.velocity += new Vector3 (25, 0, 0);
				
			}
			
			if (facing == -1) {
				currentBullet.transform.position = transform.position;
				currentBullet.rigidbody.velocity += new Vector3 (-25, 0, 0);
			}
		}
		else if(Input.GetKeyDown("l")){
			currentBullet = Instantiate (rightBullet) as GameObject;
			if (facing == 1) {
				currentBullet.transform.position = transform.position;
				currentBullet.rigidbody.velocity += new Vector3 (25, 0, 0);
				
			}
			
			if (facing == -1) {
				currentBullet.transform.position = transform.position;
				currentBullet.rigidbody.velocity += new Vector3 (-25, 0, 0);
			}
		}
		else if(Input.GetKeyDown("i")){
			currentBullet = Instantiate (jumpBullet) as GameObject;
			if (facing == 1) {
				currentBullet.transform.position = transform.position;
				currentBullet.rigidbody.velocity += new Vector3 (25, 0, 0);
				
			}
			
			if (facing == -1) {
				currentBullet.transform.position = transform.position;
				currentBullet.rigidbody.velocity += new Vector3 (-25, 0, 0);
			}
		}
	}
	
	void handleXMovement(){
		float xMovement = Input.GetAxisRaw("Horizontal2");
		Vector3 pos = this.transform.position;
		Vector3 change = new Vector3 (0, 0);
		
		float horizontalChange = 0.15f;
		
		if(xMovement > 0){
			change.x = horizontalChange;
			facing = 1;
		}
		else if(xMovement < 0){
			change.x = -horizontalChange;
			facing = -1;
		}
		
		//if i'm on a wall and going in that direction don't
		//have any change
		if(rightWallList.Count != 0 && change.x > 0){
			change.x = 0;
		}
		if(leftWallList.Count != 0 && change.x < 0){
			change.x = 0;
		}
		
		pos = pos + (change);
		
		this.transform.position = pos;
	}
	
	void handleYMovement (){
		if(jumpQueued && groundList.Count != 0){
			Jump ();
			jumpQueued = false;
		}
		else{
			jumpQueued = false;
		}
	}
	
	public void queueJump(){
		jumpQueued = true;
	}
	
	void FixedUpdate(){
		if(playing){
			
			handleXMovement();
			handleYMovement();
			
			Vector3 pos = this.transform.position;
			float oldXSpeed = velocity.x;
			
			//Apply gravity if the player is not on a platform.
			if(groundList.Count == 0){
				velocity += playerGravity * Time.deltaTime;
			} else if (velocity.y <= 0.0f){
				velocity.y = 0.0f;
			}
			//applies the gravity here
			pos += velocity * Time.deltaTime;
			this.transform.position = pos;
			velocity.x = oldXSpeed;
			
		}
		prevGroundLeftCornerPos = curGroundLeftCornerPos;
		prevGroundRightCornerPos = curGroundRightCornerPos;
		curGroundLeftCornerPos = getGroundLeftCorner ();
		curGroundRightCornerPos = getGroundRightCorner ();
		prevTopLeftCornerPos = curTopLeftCornerPos;
		prevTopRightCornerPos = curTopRightCornerPos;
		curTopLeftCornerPos = getTopLeftCorner ();
		curTopRightCornerPos = getTopRightCorner ();
		hitGroundTimer--;
	}
	
	void OnTriggerEnter(Collider other){
		HandleBulletCollision (other);
		HandleGroundCollision (other);
		HandleSpikeCollision (other);
	}
	
	public void HitGround(){
		if (hitGroundTimer > 0) {
			return;
		}
		Vector3 groundForce = Vector3.zero;
		groundForce.y = -velocity.y;
		velocity += groundForce;
		hitGroundTimer = 10;
	}
	
	void OnTriggerStay(Collider other){
		OnTriggerEnter(other);
	}
	
	void OnTriggerExit(Collider other){
		if(other.GetComponent<GroundObject>()){
			groundList.Remove(other);
			rightWallList.Remove(other);
			leftWallList.Remove(other);
		}
	}
	
	void HandleBulletCollision(Collider other){
		BulletScript bs = other.GetComponent<BulletScript> ();
		float pushPullScaling = 5.0f;
		if(bs){
			if(bs.getBulletType() == bulletType.PULL){
				Vector3 diff = transform.position - bs.getPlayerRef().transform.position;
				diff = diff.normalized * pushPullScaling;
				this.velocity +=new Vector3(diff.x, diff.y, diff.z);
				Destroy(other.gameObject);
			}
			else if(bs.getBulletType() == bulletType.PUSH){
				Vector3 diff = bs.getPlayerRef().transform.position - transform.position;
				diff = diff.normalized * pushPullScaling;
				this.velocity += new Vector3(diff.x, diff.y * 2, diff.z);
				Destroy(other.gameObject);	
			}
			else if(bs.getBulletType() == bulletType.JUMP){
				this.velocity += new Vector3(0,16,0);
				Destroy(other.gameObject);
			}
		}
	}
	
	void HandleGroundCollision(Collider other){
		if(!other.GetComponent<GroundObject>()){
			return;
		}
		if(groundList.Contains(other)){
			return;
		}
		if(rightWallList.Contains(other)){
			return;
		}
		if(leftWallList.Contains(other)){
			return;
		}
		
		
		float groundYPos = other.transform.position.y + (other.transform.localScale.y / 2.0f * 1);
		float ceilYPos = other.transform.position.y + (other.transform.localScale.y / 2.0f * -1);
		
		float leftSideWallPos = other.transform.position.x - (other.transform.localScale.x / 2.0f);
		float rightSideWallPos = other.transform.position.x + (other.transform.localScale.x / 2.0f);
		
		float approxVal = 0.1f;
		
		bool shouldMoveOnTop = false;
		bool shouldBounceOffBottom = false;
		bool shouldAssignAsLeftWall = false;
		bool shouldAssignAsRightWall = false;
		
		//This section 
		if (IsInside (other.collider, curGroundLeftCornerPos) && IsInside (other.collider, curGroundRightCornerPos)) {
			shouldMoveOnTop = true;
		} else if (IsInside (other.collider, curGroundLeftCornerPos)) {
			Vector3 cornerPosChangeDirection = curGroundLeftCornerPos - prevGroundLeftCornerPos;
			
			RaycastHit hitInfo;
			Physics.Raycast(prevGroundLeftCornerPos, cornerPosChangeDirection, out hitInfo);
			
			if(UtilityFunctions.isApproximate(hitInfo.point.y, groundYPos, approxVal)){
				shouldMoveOnTop = true;
			} else if (UtilityFunctions.isApproximate(hitInfo.point.x, rightSideWallPos, approxVal)){
				shouldAssignAsLeftWall = true;
			}
			
		} else if (IsInside (other.collider, curGroundRightCornerPos)) {
			Vector3 cornerPosChangeDirection = curGroundRightCornerPos - prevGroundRightCornerPos;
			
			RaycastHit hitInfo;
			Physics.Raycast(prevGroundRightCornerPos, cornerPosChangeDirection, out hitInfo);
			
			if(UtilityFunctions.isApproximate(hitInfo.point.y, groundYPos, approxVal)){
				shouldMoveOnTop = true;
			} else if (UtilityFunctions.isApproximate(hitInfo.point.x, leftSideWallPos, approxVal)){
				shouldAssignAsRightWall = true;
			}
		} else if (IsInside (other.collider, curTopLeftCornerPos) && IsInside (other.collider, curTopRightCornerPos)) {
			shouldBounceOffBottom = true;
		} else if (IsInside (other.collider, curTopLeftCornerPos)) {
			//Only the left corner is in the ground
			Vector3 cornerPosChangeDirection = curTopLeftCornerPos - prevTopLeftCornerPos;
			
			RaycastHit hitInfo;
			Physics.Raycast(prevTopLeftCornerPos, cornerPosChangeDirection, out hitInfo);
			
			if(UtilityFunctions.isApproximate(hitInfo.point.y, ceilYPos, approxVal)){
				shouldBounceOffBottom = true;
			} else if (UtilityFunctions.isApproximate(hitInfo.point.x, rightSideWallPos, approxVal)){
				shouldAssignAsLeftWall = true;
			}
		} else if (IsInside (other.collider, curTopRightCornerPos)) {
			//Only the left corner is in the ground
			Vector3 cornerPosChangeDirection = curTopRightCornerPos - prevTopRightCornerPos;
			
			RaycastHit hitInfo;
			Physics.Raycast(prevTopRightCornerPos, cornerPosChangeDirection, out hitInfo);
			
			if(UtilityFunctions.isApproximate(hitInfo.point.y, ceilYPos, approxVal)){
				shouldBounceOffBottom = true;
			} else if (UtilityFunctions.isApproximate(hitInfo.point.x, leftSideWallPos, approxVal)){
				shouldAssignAsRightWall = true;
			}
		}
		
		if (shouldMoveOnTop && (Mathf.Sign(velocity.y) == -1)) {
			if (Mathf.Approximately (other.bounds.min.x, this.collider.bounds.max.x)
			    || Mathf.Approximately (other.bounds.max.x, this.collider.bounds.min.x)) {
				return;
			}
			//ONLY HIT GROUND IF VEL IS GOING DOWN
			HitGround ();
			if(!groundList.Contains(other)){
				groundList.Add(other);
			}
			float radiAdd = (this.transform.lossyScale.y) / 2.0f + (other.transform.localScale.y) / 2.0f;
			Vector3 oldPos = this.transform.position;
			Vector3 newPos = new Vector3 (oldPos.x, oldPos.y, oldPos.z);
			Vector3 groundPos = other.transform.position;
			newPos.y = groundPos.y + radiAdd;
			this.transform.position = newPos;
		} 
		else if (shouldAssignAsLeftWall){
			if(!leftWallList.Contains(other)){
				leftWallList.Add(other);
			}
			float radiAdd = (this.transform.lossyScale.x) / 2.0f + (other.transform.localScale.x) / 2.0f;
			Vector3 oldPos = this.transform.position;
			Vector3 newPos = new Vector3 (oldPos.x, oldPos.y, oldPos.z);
			Vector3 groundPos = other.transform.position;
			newPos.x = groundPos.x + radiAdd + 0.01f;
			this.transform.position = newPos;
		} 
		else if (shouldAssignAsRightWall){
			//currentRightWall = other.gameObject.GetComponent<SolidObject>();
			if(!rightWallList.Contains(other)){
				rightWallList.Add(other);
			}
			float radiAdd = (this.transform.lossyScale.x) / 2.0f + (other.transform.localScale.x) / 2.0f;
			Vector3 oldPos = this.transform.position;
			Vector3 newPos = new Vector3 (oldPos.x, oldPos.y, oldPos.z);
			Vector3 groundPos = other.transform.position;
			newPos.x = groundPos.x - radiAdd - 0.01f;
			this.transform.position = newPos;
		} 
		else if (shouldBounceOffBottom && (Mathf.Sign(velocity.y) == 1)){
			float radiAdd = (this.transform.lossyScale.y) / 2.0f + (other.transform.localScale.y) / 2.0f;
			Vector3 oldPos = this.transform.position;
			Vector3 newPos = new Vector3 (oldPos.x, oldPos.y, oldPos.z);
			Vector3 groundPos = other.transform.position;
			newPos.y = groundPos.y - radiAdd;// - 0.05f;
			velocity.y = 0.0f;
			this.transform.position = newPos;
		}
	}
	
	void HandleSpikeCollision(Collider other){
		if (other.gameObject.tag == "spike") {
			Die ();
		} else {
			return;
		}
	}
	
	void Die(){
		this.transform.position = spawnPos;
		RestoreDefaults ();
	}
	
	//ACTIONS
	
	public void Jump(){
		velocity.y = jumpSpeed;
	}
	
	public void Reverse(){
		velocity.x = -velocity.x;
	}
	
	public void RestoreDefaults(){
		groundList = new List<Collider> ();
		rightWallList = new List<Collider> ();
		leftWallList = new List<Collider> ();
		velocity = Vector3.zero;
		
		hitGroundTimer = 0;
	}
	
	
	
	//BOOKKEEPING
	private Vector3 getGroundLeftCorner(){
		Vector3 pos = this.transform.position;
		pos.x -= (this.transform.localScale.x) / 2.0f;
		pos.y += ((this.transform.localScale.y) / 2.0f) * -1;
		return pos;
	}
	
	private Vector3 getGroundRightCorner(){
		Vector3 pos = this.transform.position;
		pos.x += (this.transform.localScale.x) / 2.0f;
		pos.y += ((this.transform.localScale.y) / 2.0f) * -1;
		return pos;
	}
	
	private Vector3 getTopLeftCorner(){
		Vector3 pos = this.transform.position;
		pos.x -= (this.transform.localScale.x) / 2.0f;
		pos.y += ((this.transform.localScale.y) / 2.0f);
		return pos;
	}
	
	private Vector3 getTopRightCorner(){
		Vector3 pos = this.transform.position;
		pos.x += (this.transform.localScale.x) / 2.0f;
		pos.y += ((this.transform.localScale.y) / 2.0f);
		return pos;
	}
	
	//http://answers.unity3d.com/questions/163864/test-if-point-is-in-collider.html
	//Answer by DarkharStudio
	static public bool IsInside ( Collider test, Vector3 point)
	{
		Vector3    center;
		Vector3    direction;
		Ray        ray;
		RaycastHit hitInfo;
		bool       hit;
		
		// Use collider bounds to get the center of the collider. May be inaccurate
		// for some colliders (i.e. MeshCollider with a 'plane' mesh)
		center = test.bounds.center;
		
		// Cast a ray from point to center
		direction = center - point;
		ray = new Ray(point, direction);
		hit = test.Raycast(ray, out hitInfo, direction.magnitude);
		
		// If we hit the collider, point is outside. So we return !hit
		return !hit;
	}
}
