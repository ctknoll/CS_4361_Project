using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
	
	public Transform tile;
	public Transform obstacle;
	public Vector2 sizeOfMap;
	public int startNum = 15;
	Coordinate centerOfMap;

	[Range(0,1)]
	public float obstacleAmount;

	[Range(0,1)]
	public float outline;

	List<Coordinate> tileCoordinates;
	Queue<Coordinate> randomTileCoordinates;

	void Start(){
		CreateMap ();
	}
	public void CreateMap(){
        
	
        
		tileCoordinates = new List<Coordinate> ();
		for (int x = 0; x < sizeOfMap.x; x++) {
			for (int y = 0; y < sizeOfMap.y; y++) {
				tileCoordinates.Add (new Coordinate (x, y));
			}
		}

		randomTileCoordinates = new Queue<Coordinate> (Utility.ShuffleArray (tileCoordinates.ToArray (), startNum));
		centerOfMap = new Coordinate ((int)sizeOfMap.x / 2, (int)sizeOfMap.y / 2);
			
        
	//For debugging only- to see the board
        string holderName = "Generate Map";
		if (transform.Find (holderName)) {
			DestroyImmediate (transform.Find(holderName).gameObject);
		}

		Transform mapHold = new GameObject (holderName).transform;
		mapHold.parent = transform;
		//create size of map and position
		for (int x = 0; x<sizeOfMap.x; x++) {
			for (int y = 0; y<sizeOfMap.y; y++) {
				//left edge and centered tile map
				Vector3 tPosition = CoordinatePosition(x,y);

				Transform newTile = Instantiate (tile, tPosition, Quaternion.Euler (Vector3.right * 90)) as Transform;
				newTile.localScale = Vector3.one * (1 - outline);
				newTile.parent = mapHold;
			}
		}
        

		//make sure it doesn't block a piece of the map
		bool[,] obstacleLocation = new bool[(int)sizeOfMap.x,(int)sizeOfMap.y];

		//obstacle position and count
		int obstacleCount = (int)(sizeOfMap.x*sizeOfMap.y*obstacleAmount);
		int currentObstacleAmount = 0;

		for (int i = 0; i < obstacleCount; i++) {
			Coordinate randCoordinate = getRandCoordinate ();
			obstacleLocation [randCoordinate.x, randCoordinate.y] = true;
			currentObstacleAmount++;
			if (randCoordinate != centerOfMap && accessible(obstacleLocation, currentObstacleAmount)) {
				Vector3 obstaclePosition = CoordinatePosition (randCoordinate.x, randCoordinate.y);

				Transform newObstacle = Instantiate (obstacle, obstaclePosition, Quaternion.identity) as Transform;
				newObstacle.parent = mapHold;
			} else {
				obstacleLocation [randCoordinate.x, randCoordinate.y] = false;
				currentObstacleAmount--;
			}

		}
	}


	//check accesibility of the obstacles in the map
	bool accessible(bool[,] obstacleLocation, int currentObstacleAmount){
		bool[,] flags = new bool[obstacleLocation.GetLength (0), obstacleLocation.GetLength (1)];
		Queue<Coordinate> coordinateQueue= new Queue<Coordinate> ();
		coordinateQueue.Enqueue (centerOfMap);
		flags [centerOfMap.x, centerOfMap.y] = true;
		int accesibleTileAmount = 1;

		while (coordinateQueue.Count > 0) {
			Coordinate tiles = coordinateQueue.Dequeue ();
			//check all the tiles only once
			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					int nextX = tiles.x + x;
					int nextY = tiles.y + y;
					if (x == 0 || y == 0) {
						if (nextX >= 0 && nextY >= 0 && nextX < obstacleLocation.GetLength (0) && nextY < obstacleLocation.GetLength (1)) {
							if (!flags [nextX, nextY] && !obstacleLocation [nextX, nextY]) {
								flags [nextX, nextY] = true;
								coordinateQueue.Enqueue (new Coordinate (nextX, nextY));
								accesibleTileAmount++;
							}
						}
					}
				}
			}
		}
		int targetAccessibleTiles = (int)(sizeOfMap.x * sizeOfMap.y - currentObstacleAmount);
		return targetAccessibleTiles == accesibleTileAmount;
	}

	//Coordinate position
	Vector3 CoordinatePosition(int x, int y){
		return new Vector3(-sizeOfMap.x/2+ 0.5f +x, 0, -sizeOfMap.y/2+0.5f+y);
	}

	//random coordinate position
	public Coordinate getRandCoordinate(){
		Coordinate randomCoordinateNumber = randomTileCoordinates.Dequeue ();
		randomTileCoordinates.Enqueue (randomCoordinateNumber);
		return randomCoordinateNumber;
	}

	//store coordinates 
	public struct Coordinate{
		public int x;
		public int y;

		public Coordinate(int _x, int _y){
			x= _x;
			y= _y;
		}
		public static bool operator ==(Coordinate c1, Coordinate c2){
			return c1.x == c2.x && c1.y == c2.y;
		}
		public static bool operator !=(Coordinate c1, Coordinate c2){
			return !(c1 == c2);
		}
	}
}