using System.Collections;


public static class Utility {

	//obstacle array
	public static M[] ShuffleArray<M>(M[] array, int startNum){
		System.Random numberGenerator = new System.Random (startNum);

		for(int i =0; i<array.Length-1; i++){
			int randIndx = numberGenerator.Next (i, array.Length);
			M tempNum = array [randIndx];
			array [randIndx] = array [i];
			array [i] = tempNum;
		}
		return array;
	}

}
