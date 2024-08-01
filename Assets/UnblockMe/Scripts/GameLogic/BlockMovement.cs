using UnityEngine;
using System.Collections;
//Apply this Script On Each Block To Move LEFT,RIGHT,UP,DOWN



[RequireComponent(typeof(BoxCollider2D))]
public class BlockMovement : MonoBehaviour {

    public bool isthiscurrHintObj;
    
    [HideInInspector]
    public Block thisBlock,previousBlock;
    [HideInInspector]
    public int thisblockId;
    private float Left,Right,Up,Down;
    bool SwipeStart, LerpStart;
    public Vector2 startPos,startMousePos;
    public Vector3 swipeStartPos, swipeEndPos;
    public float startMouseTime,speed,startTime,EndTime,LerpStartTime;
    Vector3 currPos;
    Block[] currblocks;
	// Use this for initialization
	void OnEnable ()
    {
        
        SwipeStart = false;
        EasyTouch.On_SwipeStart += On_SwipeStart;
        EasyTouch.On_Swipe += On_Swipe;
        EasyTouch.On_SwipeEnd += On_SwipeEnd;
	}

    void OnDisable()
    {
        EasyTouch.On_SwipeStart -= On_SwipeStart;
        EasyTouch.On_Swipe -= On_Swipe;
        EasyTouch.On_SwipeEnd -= On_SwipeEnd;
    }

   
    void On_SwipeStart(Gesture g)
    {
    
        Ray ray=Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x,Input.mousePosition.y,0f));
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject == this.transform.gameObject )
            {
                if (!GameController.HintMode || (GameController.HintMode && isthiscurrHintObj))
                {
                    SwipeStart = true;
                    currPos = transform.position;
                    startMouseTime = Time.time;
                    startMousePos = GetTouchPoint();
                    //Count Left,Right,Up,down for selected block
                    Left = currPos.x - GameController.GameBoard.CalculateMovement(thisblockId).Left * GameController.BoxSize / 100f;
                    Right = currPos.x + GameController.GameBoard.CalculateMovement(thisblockId).Right * GameController.BoxSize / 100f;
                    Up = currPos.y + GameController.GameBoard.CalculateMovement(thisblockId).Up * GameController.BoxSize / 100f;
                    Down = currPos.y - GameController.GameBoard.CalculateMovement(thisblockId).Down * GameController.BoxSize / 100f;

                }

            }
        }
    }

    void On_Swipe(Gesture g)
    {
        //Change clamped position according to mouse position or touch position of selected object
        if (SwipeStart && !LerpStart)
        {
            if (thisBlock.Orientation == BlockOrientation.Orientation.Horizontal)
            {

                if (g.swipe == EasyTouch.SwipeType.Left || g.swipe == EasyTouch.SwipeType.Right)
                {
                   
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x+  ((GetTouchPoint().x -startMousePos.x)/30f) ,Left,Right), transform.position.y, 0f);
               
                   
                 
                }

            }
            else
            {
                if (g.swipe == EasyTouch.SwipeType.Up || g.swipe == EasyTouch.SwipeType.Down)
                {
                    transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y + ((GetTouchPoint().y- startMousePos.y)/30f), Down, Up), 0f);
                }
            }
        }
    }

    void On_SwipeEnd(Gesture g)
    {

        //Changed value after drag..
        if (SwipeStart && !LerpStart)
        {

        
            int Value;
             if (thisBlock.Orientation==BlockOrientation.Orientation.Horizontal)
             {
                 speed = (GetTouchPoint().x - startMousePos.x) / (Time.time - startMouseTime>0?Time.time - startMouseTime:0.1f ) / 10f;

                 Value = Mathf.RoundToInt(Mathf.Abs(startPos.x - Mathf.Clamp(transform.position.x + speed, Left, Right)) / (GameController.BoxSize / 100f));
               
                swipeStartPos = transform.position;
                LerpStart = true;
                LerpStartTime = Time.time;
              
                previousBlock = thisBlock;
               thisBlock= GameController.GameBoard.AddNewValue(thisblockId, Value > 5 ? 5 : Value);
             }
             else if (thisBlock.Orientation == BlockOrientation.Orientation.Vertical)
             {
                 speed = (GetTouchPoint().y - startMousePos.y) / (Time.time - startMouseTime > 0 ? Time.time - startMouseTime : 0.1f) / 10f;
              
                 LerpStart = true;
                 LerpStartTime = Time.time;
                 Value = Mathf.RoundToInt(Mathf.Abs(startPos.y - Mathf.Clamp(transform.position.y + speed, Down, Up)) / (GameController.BoxSize / 100f));
                 swipeStartPos = transform.position;
              
                 previousBlock = thisBlock;
               thisBlock= GameController.GameBoard.AddNewValue(thisblockId, Value > 5 ? 5 : Value);
             }
             swipeEndPos = GameController.GetBlockPosition(thisBlock);
           

            //change in Position
          
           }
           

    }


    //Return 3d position of current touch position
    Vector3 GetTouchPoint()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)); 
    }

    void Update()
    {
        
        if (LerpStart)
        {
            transform.position = Vector3.Lerp(swipeStartPos, swipeEndPos, (Time.time - LerpStartTime) / 0.2f);
        }
        if(LerpStart && (Time.time - LerpStartTime) > 0.2f)
        {
            LerpStart = false;
            if (transform.position != currPos)
            {
                if(!GameController.HintMode)
                    GameController.GameControllerObj.SetCurrMove();
                int index = thisblockId;
                Block prev = previousBlock;

                GameController.blockdata _blockdata = new GameController.blockdata();
              
              
                _blockdata.index = index;

                _blockdata.block = prev;

             
                
                _blockdata.SolutionBoardNo = GameController.TotalMove;
             
                GameController.blockpositionList.Add(_blockdata);
                if (GameController.GameBoard._blocks[0]._col > 4)
                {
                     GameController.GameControllerObj.LevelClear();
                    
                }
                else if (GameController.HintMode && isthiscurrHintObj)
                {
                    print("HERE ");
                    if (transform.position == GameController.hintObj.transform.position)
                    {

                        isthiscurrHintObj = false;
                        
                          
                            GameController.GameControllerObj.SetCurrMove();
                       
                        

                     

                    }
                    GameController.GameControllerObj.SetHint();

                }
               
          



                 
                   
                
                
               
            }
            SwipeStart = false;
            
        }
    }

   
    
}
