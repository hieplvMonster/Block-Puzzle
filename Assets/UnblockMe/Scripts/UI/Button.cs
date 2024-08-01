using UnityEngine;
using System.Collections;
[RequireComponent(typeof(BoxCollider2D))]
public class Button : MonoBehaviour {
    float doubleClickStart = 0;
    public delegate void ButtonClick();

    public ButtonClick OnButtonClick,OnDoubleClick;
    
    public Sprite Hoverimg;

    [HideInInspector]
    public Sprite Normalimg;
   
	
	void Start () {
        if(this.transform.GetComponent<SpriteRenderer>()!=null)
        Normalimg = this.transform.GetComponent<SpriteRenderer>().sprite;
	}
	
	void Update ()
    {
    #if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            if (Input.touchCount ==0 && this.transform.GetComponent<SpriteRenderer>().sprite==Hoverimg )
            {
                this.transform.GetComponent<SpriteRenderer>().sprite = Normalimg;
            }
#endif
    }

    void OnMouseUpAsButton()
    {
        setNormal();

        if (OnButtonClick != null)
        {

            OnButtonClick();
        }
    }
    
    void OnMouseExit()
    {
        setNormal();
    }

    void OnMouseEnter()
    {
      //  if (Input.touchCount == 1)
            setHover();

    //#if UNITY_EDITOR
    //    setHover();
    //#endif
        
    }

    void OnMouseUp()
    {
        setNormal();
        if ((Time.time - doubleClickStart) < 0.3f)
        {
            this.OnMouseDoubleClick();
            doubleClickStart = -1;
        }
        else
        {
            doubleClickStart = Time.time;
        }

    }

    void OnMouseDown()
    {
     //   if (Input.touchCount == 1)
            setHover();

    //#if UNITY_EDITOR
    //    setHover();
    //#endif
    }

    void setHover()
    {
        if (Hoverimg)
            this.transform.GetComponent<SpriteRenderer>().sprite = Hoverimg;
    }

    void setNormal()
    {
        if (Normalimg && this.transform.GetComponent<SpriteRenderer>().sprite == Hoverimg)
            this.transform.GetComponent<SpriteRenderer>().sprite = Normalimg;
    }

    void OnMouseDoubleClick()
    {
        if (OnDoubleClick != null)
        {
            OnDoubleClick();
        }
    }
}
