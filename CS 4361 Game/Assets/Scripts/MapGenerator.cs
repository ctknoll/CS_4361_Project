using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
	
    public Map[] maps;
    public int mapIndex;
    
	public Transform tile;
	public Transform obstacle;
    public Transform mapFloor;
    public Transform navmeshMask;
	
    public Vector2 maxSizeOfMap;
	

	[Range(0,1)]
	public float outline;

    public float tileSize;
	List<Coordinate> tileCoordinates;
	Queue<Coordinate> randomTileCoordinates;

    public Map currentMap;
    
	void Start(){
		CreateMap ();
	}
    
	public void CreateMap(){
        
        //setup current map by getting the map index of the array
        currentMap = maps[mapIndex];
        //randomize the start num
        System.Random prng = new System.Random(currentMap.startNum);

        //
        GetComponent<BoxCollider>().size = new Vector3(currentMap.sizeOfMap.x * tileSize, .05f, currentMap.sizeOfMap.y * tileSize);
        
        //size of map tiles coordinates
        tileCoordinates = new List<Coordinate> ();
		for (int x = 0; x < currentMap.sizeOfMap.x; x++) {
			for (int y = 0; y < currentMap.sizeOfMap.y; y++) {
				tileCoordinates.Add (new Coordinate (x, y));
			}
		}

		randomTileCoordinates = new Queue<Coordinate> (Utility.ShuffleArray (tileCoordinates.ToArray (), currentMap.startNum));
		
        
	//map holder object
        string holderName = "Generate Map";
		if (transform.Find (holderName)) {
			DestroyImmediate (transform.Find(holderName).gameObject);
		}

		Transform mapHold = new GameObject (holderName).transform;
		mapHold.parent = transform;

		//multiplying tiles
		for (int x = 0; x<currentMap.sizeOfMap.x; x++) {
			for (int y = 0; y<currentMap.sizeOfMap.y; y++) {
				//left edge and centered tile map
				Vector3 tPosition = CoordinatePosition(x,y);

				Transform newTile = Instantiate (tile, tPosition, Quaternion.Euler (Vector3.right * 90)) as Transform;
				newTile.localScale = Vector3.one * (1 - outline)* tileSize;
				newTile.parent = mapHold;
			}
		}
        

		//multiplying obstacles
		bool[,] obstacleLocation = new bool[(int)currentMap.sizeOfMap.x,(int)currentMap.sizeOfMap.y];

		//obstacle position and count
		int obstacleCount = (int)(currentMap.sizeOfMap.x*currentMap.sizeOfMap.y*currentMap.obstacleAmount);
		int currentObstacleAmount = 0;

		for (int i = 0; i < obstacleCount; i++) {
			Coordinate randCoordinate = getRandCoordinate ();
			obstacleLocation [randCoordinate.x, randCoordinate.y] = true;
			currentObstacleAmount++;
			if (randCoordinate != currentMap.centerOfMap && accessible(obstacleLocation, currentObstacleAmount)) {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());

                Vector3 obstaclePosition = CoordinatePosition (randCoordinate.x, randCoordinate.y);

				Transform newObstacle = Instantiate (obstacle, obstaclePosition, Quaternion.identity) as Transform;
				newObstacle.parent = mapHold;
                newObstacle.localScale = new Vector3((1 - outline) * tileSize, obstacleHeight, (1 - outline) * tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randCoordinate.y / (float)currentMap.sizeOfMap.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;
            }
            else {
				obstacleLocation [randCoordinate.x, randCoordinate.y] = false;
				currentObstacleAmount--;
			}

		}
        //mask off sides of floor for enemies to not go out of the board
        Transform maskLeft = Instantiate (navmeshMask, Vector3.left * (currentMap.sizeOfMap.x + maxSizeOfMap.x) / 4f * tileSize, Quaternion.identity) as Transform;
		maskLeft.parent = mapHold;
		maskLeft.localScale = new Vector3 ((maxSizeOfMap.x - currentMap.sizeOfMap.x) / 2f, 1, currentMap.sizeOfMap.y) * tileSize;
        
        Transform maskRight = Instantiate (navmeshMask, Vector3.right * (currentMap.sizeOfMap.x + maxSizeOfMap.x) / 4f * tileSize, Quaternion.identity) as Transform;
		maskRight.parent = mapHold;
		maskRight.localScale = new Vector3 ((maxSizeOfMap.x - currentMap.sizeOfMap.x) / 2f, 1, currentMap.sizeOfMap.y) * tileSize;
        
        Transform maskForward = Instantiate (navmeshMask, Vector3.forward * (currentMap.sizeOfMap.y + maxSizeOfMap.y) / 4f * tileSize, Quaternion.identity) as Transform;
		maskForward.parent = mapHold;
		maskForward.localScale = new Vector3 (maxSizeOfMap.x, 1, (maxSizeOfMap.y-currentMap.sizeOfMap.y)/2f) * tileSize;

		Transform maskBack = Instantiate (navmeshMask, Vector3.back * (currentMap.sizeOfMap.y + maxSizeOfMap.y) / 4f * tileSize, Quaternion.identity) as Transform;
		maskBack.parent = mapHold;
		maskBack.localScale = new Vector3 (maxSizeOfMap.x, 1, (maxSizeOfMap.y-currentMap.sizeOfMap.y)/2f) * tileSize;
       
        
        //change the size of the navmesh floor in map
        mapFloor.localScale = new Vector3(maxSizeOfMap.x, maxSizeOfMap.y) * tileSize;
	}


	//check accesibility of the obstacles in the map
	bool accessible(bool[,] obstacleLocation, int currentObstacleAmount){
		bool[,] flags = new bool[obstacleLocation.GetLength (0), obstacleLocation.GetLength (1)];
		Queue<Coordinate> coordinateQueue= new Queue<Coordinate> ();
		coordinateQueue.Enqueue (currentMap.centerOfMap);
		flags [currentMap.centerOfMap.x, currentMap.centerOfMap.y] = true;
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
		int targetAccessibleTiles = (int)(currentMap.sizeOfMap.x * currentMap.sizeOfMap.y - currentObstacleAmount);
		return targetAccessibleTiles == accesibleTileAmount;
	}

	//Coordinate position
	Vector3 CoordinatePosition(int x, int y){
		return new Vector3(-currentMap.sizeOfMap.x/2f+ 0.5f +x, 0, -currentMap.sizeOfMap.y/2f+0.5f+y)* tileSize;
	}

	//random coordinate position
	public Coordinate getRandCoordinate(){
		Coordinate randomCoordinateNumber = randomTileCoordinates.Dequeue ();
		randomTileCoordinates.Enqueue (randomCoordinateNumber);
		return randomCoordinateNumber;
	}

	//store coordinates 
	[System.Serializable]
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
    
    //save map size
    [System.Serializable]
    public class Map{
        public Coordinate sizeOfMap;
        [Range(0,1)]
        public float obstacleAmount;
        public int startNum;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;
        
        public Coordinate centerOfMap{
            get{
                return new Coordinate(sizeOfMap.x/2, sizeOfMap.y/2);
            }
        }
    }
}