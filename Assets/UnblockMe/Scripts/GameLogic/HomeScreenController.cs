using UnityEngine;
using System.Collections;

public class HomeScreenController : MonoBehaviour {
     public GameObject Easybtn, Mediumbtn, hardbtn;
	// Use this for initialization
	void Start () {
        Easybtn.GetComponent<Button>().OnButtonClick =
        delegate
        {
            GameController.currDifficulty = GameController.Difficulty.Easy;
            Application.LoadLevel("UnblockMe");
        };

        Mediumbtn.GetComponent<Button>().OnButtonClick =
       delegate
       {
           GameController.currDifficulty = GameController.Difficulty.Medium;
           Application.LoadLevel("UnblockMe");
       };

        hardbtn.GetComponent<Button>().OnButtonClick =
        delegate
        {
            GameController.currDifficulty = GameController.Difficulty.Hard;
            Application.LoadLevel("UnblockMe");
        };


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
