using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerObject : MonoBehaviour {
	static public PlayerObject P;
	
	public Vector3 startPosition;
	public Vector3 velocity;
	public Vector3 currentLinePosition;
	public float xSpeed = 4.0f; //HAVE TO CHANGE THIS IN INSPECTOR, DINGUS! 4.0 seems good
	public float jumpSpeed = 12.0f;
	public static Vector3 playerGravity = new Vector3(0.0f, -15f, 0.0f);
	
	public bool playing;
	
	public List<Collider> groundList;
	public List<Collider> leftWallList;
	public List<Collider> rightWallList;
	
	bool ________________________________________;
	//Physics stuff
	public int gravityDirection = -1;
	private int hitGroundTimer = 0;
	
	Vector3	curGroundRightCornerPos;
	Vector3 curGroundLeftCornerPos;
	Vector3 prevGroundLeftCornerPos;
	Vector3 prevGroundRightCornerPos;
	Vector3	curTopRightCornerPos;
	Vector3 curTopLeftCornerPos;
	Vector3 prevTopLeftCornerPos;
	Vector3 prevTopRightCornerPos;
	
	// Use this for initialization
	void Start () {
		P = this;
		RestoreDefaults ();
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void FixedUpdate(){
		if(playing){
			Vector3 pos = this.transform.position;
			float oldXSpeed = velocity.x;
			
			//Apply gravity if the player is not on a platform.
			if(groundList.Count == 0){
				velocity += playerGravity * Time.deltaTime;
			}
			
			if(rightWallList.Count != 0 && velocity.x > 0){
				velocity.x = 0;
			}
			if(leftWallList.Count != 0 && velocity.x < 0){
				velocity.x = 0;
			}
			
			
			pos += velocity * Time.deltaTime;
			this.transform.position = pos;
			currentLinePosition = pos;
			velocity.x = oldXSpeed;
			
			if (this && LevelLoader.ll && this.transform.position.y <= -4.0f) {
				RestoreDefaults();
			}
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
	
	public void OnTriggerEnter(Collider other){
		
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
		
		if(playerGravity.y < 0){
			gravityDirection = -1;
		} else {
			gravityDirection = 1;
		}
		
		
		float groundYPos = other.transform.position.y + (other.transform.localScale.y / 2.0f * -gravityDirection);
		float ceilYPos = other.transform.position.y + (other.transform.localScale.y / 2.0f * gravityDirection);
		
		float leftSideWallPos = other.transform.position.x - (other.transform.localScale.x / 2.0f);
		float rightSideWallPos = other.transform.position.x + (other.transform.localScale.x / 2.0f);
		
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
			
			if(UtilityFunctions.isApproximate(hitInfo.point.y, groundYPos, 0.3f)){
				shouldMoveOnTop = true;
			} else if (UtilityFunctions.isApproximate(hitInfo.point.x, rightSideWallPos, 0.3f)){
				shouldAssignAsLeftWall = true;
			}
			
		} else if (IsInside (other.collider, curGroundRightCornerPos)) {
			Vector3 cornerPosChangeDirection = curGroundRightCornerPos - prevGroundRightCornerPos;
			
			RaycastHit hitInfo;
			Physics.Raycast(prevGroundRightCornerPos, cornerPosChangeDirection, out hitInfo);
			
			if(UtilityFunctions.isApproximate(hitInfo.point.y, groundYPos, 0.3f)){
				shouldMoveOnTop = true;
			} else if (UtilityFunctions.isApproximate(hitInfo.point.x, leftSideWallPos, 0.3f)){
				shouldAssignAsRightWall = true;
			}
		} else if (IsInside (other.collider, curTopLeftCornerPos) && IsInside (other.collider, curTopRightCornerPos)) {
			shouldBounceOffBottom = true;
		} else if (IsInside (other.collider, curTopLeftCornerPos)) {
			//Only the left corner is in the ground
			Vector3 cornerPosChangeDirection = curTopLeftCornerPos - prevTopLeftCornerPos;
			
			RaycastHit hitInfo;
			Physics.Raycast(prevTopLeftCornerPos, cornerPosChangeDirection, out hitInfo);
			
			if(UtilityFunctions.isApproximate(hitInfo.point.y, ceilYPos, 0.3f)){
				shouldBounceOffBottom = true;
			} else if (UtilityFunctions.isApproximate(hitInfo.point.x, rightSideWallPos, 0.3f)){
				shouldAssignAsLeftWall = true;
			}
		} else if (IsInside (other.collider, curTopRightCornerPos)) {
			//Only the left corner is in the ground
			Vector3 cornerPosChangeDirection = curTopRightCornerPos - prevTopRightCornerPos;
			
			RaycastHit hitInfo;
			Physics.Raycast(prevTopRightCornerPos, cornerPosChangeDirection, out hitInfo);
			
			if(UtilityFunctions.isApproximate(hitInfo.point.y, ceilYPos, 0.3f)){
				shouldBounceOffBottom = true;
			} else if (UtilityFunctions.isApproximate(hitInfo.point.x, leftSideWallPos, 0.3f)){
				shouldAssignAsRightWall = true;
			}
		}
		
		if (shouldMoveOnTop) {
			if(Mathf.Sign(velocity.y) != Mathf.Sign(gravityDirection))
				return;
			
			
			if (Mathf.Approximately (other.bounds.min.x, this.collider.bounds.max.x)
			    || Mathf.Approximately (other.bounds.max.x, this.collider.bounds.min.x)) {
				return;
			}
			HitGround ();
			if(!groundList.Contains(other)){
				groundList.Add(other);
			}
			float radiAdd = (this.transform.lossyScale.y) / 2.0f + (other.transform.localScale.y) / 2.0f;
			Vector3 oldPos = this.transform.position;
			Vector3 newPos = new Vector3 (oldPos.x, oldPos.y, oldPos.z);
			Vector3 groundPos = other.transform.position;
			newPos.y = groundPos.y + (radiAdd * -gravityDirection);
			this.transform.position = newPos;
		} 
		else if (shouldAssignAsLeftWall){
			//currentLeftWall = other.gameObject.GetComponent<SolidObject>();
			if(!leftWallList.Contains(other)){
				leftWallList.Add(other);
			}
			float radiAdd = (this.transform.lossyScale.x) / 2.0f + (other.transform.localScale.x) / 2.0f;
			Vector3 oldPos = this.transform.position;
			Vector3 newPos = new Vector3 (oldPos.x, oldPos.y, oldPos.z);
			Vector3 groundPos = other.transform.position;
			newPos.x = groundPos.x + radiAdd;
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
			newPos.x = groundPos.x - radiAdd;
			this.transform.position = newPos;
		} 
		else if (shouldBounceOffBottom){
			float radiAdd = (this.transform.lossyScale.y) / 2.0f + (other.transform.localScale.y) / 2.0f;
			Vector3 oldPos = this.transform.position;
			Vector3 newPos = new Vector3 (oldPos.x, oldPos.y, oldPos.z);
			Vector3 groundPos = other.transform.position;
			newPos.y = groundPos.y + (radiAdd * gravityDirection) + (0.05f * gravityDirection);
			velocity.y = 0.0f;
			this.transform.position = newPos;
		}
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
		
		velocity.x = xSpeed;
		playing = false;
		hitGroundTimer = 0;
		currentLinePosition = this.transform.position;	
	}
	
	
	//BOOKKEEPING
	private Vector3 getGroundLeftCorner(){
		Vector3 pos = this.transform.position;
		pos.x -= (this.transform.localScale.x) / 2.0f;
		pos.y += ((this.transform.localScale.y) / 2.0f) * gravityDirection;
		return pos;
	}
	
	private Vector3 getGroundRightCorner(){
		Vector3 pos = this.transform.position;
		pos.x += (this.transform.localScale.x) / 2.0f;
		pos.y += ((this.transform.localScale.y) / 2.0f) * gravityDirection;
		return pos;
	}
	
	private Vector3 getTopLeftCorner(){
		Vector3 pos = this.transform.position;
		pos.x -= (this.transform.localScale.x) / 2.0f;
		pos.y += ((this.transform.localScale.y) / 2.0f) * -gravityDirection;
		return pos;
	}
	
	private Vector3 getTopRightCorner(){
		Vector3 pos = this.transform.position;
		pos.x += (this.transform.localScale.x) / 2.0f;
		pos.y += ((this.transform.localScale.y) / 2.0f) * -gravityDirection;
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
