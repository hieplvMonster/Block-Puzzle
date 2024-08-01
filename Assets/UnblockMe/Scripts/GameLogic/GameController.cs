using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class GameController : MonoBehaviour {

    public static Difficulty currDifficulty=Difficulty.Easy;

    public enum Difficulty
    {
     Easy,
     Medium,
     Hard
    }
    //assign sprite with top left pivot
    public Sprite PlayerSprite, Hori3Sprite, Vert3Sprite, Hori2Sprite, Vert2Sprite,HintSprite;

    //size of 1 box in 6*6 board for game
    public float boxSize = 76f;

    //Hint Arrow Sprite
    public GameObject HintArrow;

    //Button in UI
    public GameObject previousbtn, Nextbtn, currLevelText, CurrMovelbl;

    //Botton Button
    public GameObject Homebtn,reloadbtn, hintbtn, undobtn;

    //Level clear and Loading Object
    public GameObject LevelClearobj, Solvingobj;


    public  int currLevel = 1;
    public static int TotalMove = 0;

    public static bool HintMode;

   

    //Record each movemetn for undo operation
    public static List<blockdata> blockpositionList = new List<blockdata>();

   

    //Left top position of game to setup
    public Vector2 LeftTopPos;



    public static Vector2 LeftTopPositon;

    public static float BoxSize ;
   
    public static BoardSolution sln = null;

    public static Board GameBoard;

    public static GameObject[] gameObjs;

    public static GameObject hintObj;

    public struct blockdata
    {
       public int index;
      
       public Block block;
       public int SolutionBoardNo;

       public blockdata(int _index, Block _previousblock, Block _nextblock, int _SolutionBoardNo)
       {
           index = _index;
           block = _previousblock;
           SolutionBoardNo = _SolutionBoardNo;
        
       }
    }

    public static GameController GameControllerObj
    {
        get
        {
            return GameObject.Find("GameController").GetComponent<GameController>();
        }
    }

     private int MaxLevel
    {
        get
        {
            if (currDifficulty == Difficulty.Easy)
                return Puzzle.EasyPuzzles().Count();
            else if (currDifficulty == Difficulty.Medium)
                return Puzzle.MediumPuzzles().Count();
            else
                return Puzzle.HardPuzzles().Count();
        }
    }

    void Start()
    {
      
        BoxSize = boxSize;
        LeftTopPositon = LeftTopPos;
       

        currLevelText.GetComponent<TextMesh>().text = currLevel.ToString();
        CurrMovelbl.GetComponent<TextMesh>().text = TotalMove.ToString();
        //Previous button click event
        previousbtn.GetComponent<Button>().OnButtonClick = delegate
        {
            if (currLevel > 1)
            {
                currLevel--;
                SetNewLevel(currLevel - 1);
            }
        };
        //Next button click event
        Nextbtn.GetComponent<Button>().OnButtonClick = delegate
        {
            if (currLevel < MaxLevel)
            {
                currLevel++;
                SetNewLevel(currLevel - 1);

            }
        };
        //Home button
        Homebtn.GetComponent<Button>().OnButtonClick = delegate
        {
            Application.LoadLevel(0);
        };

        //Reload btn click event

        reloadbtn.GetComponent<Button>().OnButtonClick = delegate
        {

            StopAllCoroutines();
            SetNewLevel(currLevel - 1);
        };

        //Hint btn click event
        hintbtn.GetComponent<Button>().OnButtonClick = delegate
        {
            if (!HintMode)
            {
                Solvingobj.SetActive(true);
                TotalMove =0;
                SetCurrMove();
                StartCoroutine(GetSolution());

            }
        };
        //undo btn click event
        undobtn.GetComponent<Button>().OnButtonClick = delegate
        {

           Undo();

        };

        //Intialize game board and setup puzzle in scene
        GameBoard = GetPuzzle(0);

      SetPuzzle(GameBoard);
        
    }


    //intantiate blocks
    public  void SetPuzzle(Board b)
    {
        
        GameObject[]  oldgameObjs = GameObject.FindGameObjectsWithTag("Game");
        foreach (GameObject gTemp in oldgameObjs)
        {
            Destroy(gTemp);
        }
        if (hintObj != null)
        {
            Destroy(hintObj);
        }
        hintObj = InstantiateBlock(HintSprite, b._blocks[0]);
        hintObj.transform.position = new Vector3(500f, 500f, 1f);
        hintObj.GetComponent<SpriteRenderer>().sortingOrder=-1;
        gameObjs = new GameObject[b._blocks.Length];
        for (int i = 0; i < b._blocks.Length; i++)
        {
            GameObject gTemp;
            if (i == 0)
            {
                gTemp = InstantiateBlock(PlayerSprite, b._blocks[i]);
                   
            }
            else
            {
                if (b.Blocks[i].Orientation == BlockOrientation.Orientation.Horizontal)
                {
                    if (b.Blocks[i].Length == 2)
                    {
                        gTemp = InstantiateBlock(Hori2Sprite, b._blocks[i]);
                    }
                    else
                    {
                        gTemp = InstantiateBlock(Hori3Sprite, b._blocks[i]);
                    }
                }
                else
                {
                    if (b.Blocks[i].Length == 2)
                    {
                        gTemp = InstantiateBlock(Vert2Sprite, b._blocks[i]);
                        
                    }
                    else
                    {
                        gTemp = InstantiateBlock(Vert3Sprite, b._blocks[i]);
                    }
                }
            }
        //    gTemp.transform.position = setBlockPosition(b.Blocks[i]);
            gTemp.AddComponent<BlockMovement>();
            gTemp.GetComponent<BlockMovement>().thisBlock = b.Blocks[i];
            gTemp.GetComponent<BlockMovement>().thisblockId =i;
            gTemp.GetComponent<BlockMovement>().startPos = LeftTopPos;
         
            gTemp.name = i.ToString();
           
            gTemp.tag = "Game";
            gameObjs[i] = gTemp;

        }
     
      
    }
   

    //get 3d position according to 6*6 board position
    public static  Vector3 GetBlockPosition(Block _block)
    {
       
        Vector3 Position=Vector3.zero;;
            if (_block.Orientation == BlockOrientation.Orientation.Horizontal)
            {
               Position = new Vector3(LeftTopPositon.x + _block.Column * (BoxSize / 100f), LeftTopPositon.y - _block.Row * (BoxSize/100f), 0f);
            }
            else
            {
               Position = new Vector3(LeftTopPositon.x +_block.Column * (BoxSize / 100f), LeftTopPositon.y -_block.Row * (BoxSize/100f), 0f);
            }
            
        

        return Position;
    }

  

    public static GameObject InstantiateBlock(Sprite _Sprite,Block block,bool Changeobj=false,GameObject ObjtoChange=null)
    {
       
        GameObject gTemp;
         SpriteRenderer spriteRenderer ;
        if (!Changeobj)
        {
            gTemp = new GameObject();

            spriteRenderer = gTemp.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _Sprite;

          
        }
        else
        {
            gTemp = ObjtoChange;
            spriteRenderer = gTemp.GetComponent<SpriteRenderer>();
           
         
        }
        if (block.Orientation == BlockOrientation.Orientation.Horizontal)
        {
            
            gTemp.transform.localRotation = Quaternion.Euler(Vector3.zero);
            gTemp.transform.localScale = new Vector3((BoxSize * block.Length) / spriteRenderer.sprite.texture.width, BoxSize / spriteRenderer.sprite.texture.height, 1f); 
        }
        else
        {
    
           gTemp.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, -90f));
           gTemp.transform.localScale = new Vector3((BoxSize * block.Length) / spriteRenderer.sprite.texture.width, (BoxSize) / spriteRenderer.sprite.texture.height, 1f);
        }
        gTemp.name = spriteRenderer.sprite.name;
        gTemp.transform.position= GetBlockPosition(block);
        return gTemp;
    }

    //return solution(hint) for current game
   public IEnumerator GetSolution()
   {
       object lockHandle = new System.Object();
       bool done = false;
       yield return null;
       var myThread = new System.Threading.Thread(() =>
       {
           sln = Puzzle.FindSolutionBFS(GameBoard);
           lock (lockHandle)
           {
               done = true;
           }
       });

       myThread.Start();

       while (true)
       {

           yield return null;
           lock (lockHandle)
           {
               if (done)
               {
                   break;
               }
           }
       }
    Solvingobj.SetActive(false);
    SetHint();
     HintMode = true;
     
       
   }

     public  void SetCurrMove()
    { 
        TotalMove++;
        CurrMovelbl.GetComponent<TextMesh>().text = TotalMove.ToString();
    }

    public void SetNewLevel(int LevelNo)
    {
      
        HintArrow.transform.parent = this.transform;
      
        HintMode = false;
        HintArrow.transform.position = new Vector3(500f, 500f, 1f);
        blockpositionList.Clear();
        GameBoard = GetPuzzle(currLevel - 1);
        SetPuzzle(GameBoard);
        TotalMove =0;
        currLevelText.GetComponent<TextMesh>().text = currLevel.ToString();
        CurrMovelbl.GetComponent<TextMesh>().text = TotalMove.ToString();

    }

    public void Undo()
    {
        if (blockpositionList.Count > 0  )
        {
          
           blockdata _blockdata= blockpositionList[blockpositionList.Count -1];
           gameObjs[_blockdata.index].transform.position = GetBlockPosition(_blockdata.block);
           GameBoard._blocks[_blockdata.index] = _blockdata.block;
           gameObjs[_blockdata.index].GetComponent<BlockMovement>().thisBlock = _blockdata.block;

           blockpositionList.RemoveAt(blockpositionList.Count - 1);

           if (HintMode)
           {
               TotalMove = _blockdata.SolutionBoardNo;
               SetHint();
           }
           else
           {
               if (TotalMove > 0)
               {
                   TotalMove--;
               }
           }

         

         
            
              
           
          


         
            CurrMovelbl.GetComponent<TextMesh>().text = TotalMove.ToString();
        }
    }


    public void LevelClear()
    {
       
        print("LEVEL CLEAR");
        LevelClearobj.SetActive(true);
        HintArrow.transform.parent = this.transform;
        StartCoroutine(AfterLevelClear());
    }

    IEnumerator AfterLevelClear()
    {
        yield return new WaitForSeconds(2f);
        LevelClearobj.SetActive(false);
        if (currLevel < MaxLevel)
        {
            currLevel++;
            SetNewLevel(currLevel - 1);
        }
        else
        {
            SetNewLevel(currLevel - 1);
        }
       
    }


    public void SetHint()
    {
     
                 Block newBlock,oldBlock;
              
               var blocks =sln.Moves.ToArray()[TotalMove]._blocks.Except(GameBoard._blocks);
               if (blocks.Count() > 0)
               {
                   newBlock = blocks.ElementAt(0); //Check next chnage in solution board
                   oldBlock = GameBoard._blocks.Except(sln.Moves.ToArray()[TotalMove]._blocks).ElementAt(0);
                   SetHintObjArrows(newBlock, oldBlock);
               }
        
              
    }

    void SetHintObjArrows(Block _newBlock, Block _oldBlock)
    {
          var objs= gameObjs.Where(g => g.transform.position == GetBlockPosition(_oldBlock));
         
               if (objs.Count() > 0)
                {

                    GameObject CurrHintObj = objs.ElementAt(0); //Get change in current board for show hint
                    foreach (GameObject gTemp in gameObjs)
                    {
                        gTemp.GetComponent<BlockMovement>().isthiscurrHintObj = false;
                    }
                    CurrHintObj.GetComponent<BlockMovement>().isthiscurrHintObj = true; //Set currObject as hint object 

                    InstantiateBlock(null, _newBlock, true, hintObj); //Adjust hint object according to next hint;
                    HintArrow.transform.position = CurrHintObj.GetComponent<Renderer>().bounds.center;
                    HintArrow.transform.parent = null;
                    if (_newBlock.Orientation == BlockOrientation.Orientation.Horizontal)
                    {
                        if (GetBlockPosition(_newBlock).x > GetBlockPosition(_oldBlock).x)
                        {
                            HintArrow.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        }
                        else
                        {
                            HintArrow.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                        }
                    }
                    else
                    {
                        if (GetBlockPosition(_newBlock).y > GetBlockPosition(_oldBlock).y)
                        {
                            HintArrow.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

                        }
                        else
                        {
                            HintArrow.transform.localRotation = Quaternion.Euler(0f, 180f, 270f);
                        }
                    }

                    HintArrow.transform.localScale = Vector3.one;
                    HintArrow.transform.parent = CurrHintObj.transform;
                }
                
            
           
          

        }

    Board GetPuzzle(int levelno)
    {
        Board b=new Board();
        if(currDifficulty==Difficulty.Easy)
         b= Puzzle.EasyPuzzles().ElementAt(levelno); 
        else if(currDifficulty==Difficulty.Medium)
               b= Puzzle.MediumPuzzles().ElementAt(levelno); 
        else
          b= Puzzle.HardPuzzles().ElementAt(levelno); 
       
        return b;

    }

    

    }

   



