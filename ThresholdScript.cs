using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThresholdScript : MonoBehaviour
{
    public AreaType areaAbove;
    public AreaType areaBelow;
    public PlatformTrackTrainScript ptts;
    public RoundScript roundScript;
    private Dictionary<GameObject, bool> aboveColliderByCharacter = new Dictionary<GameObject, bool>();

    public bool characterCrossingLeft = false;

    public Coroutine thresholdSortingSequenceCoro;


    void Awake()
    {
        characterCrossingLeft = this.CompareTag("CrossLeft"); // CrossRight is default
        ptts = GameObject.FindGameObjectWithTag("PlatformTrackTrainScript").GetComponent<PlatformTrackTrainScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        areaAbove = AreaType.platform;
        areaBelow = AreaType.train;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    { 
            if(!other.GetComponent<BoxCollider2D>()) return;
            CharacterMovement cm = other.GetComponentInParent<CharacterMovement>();
            GameObject character = cm.gameObject;

            cm.currentArea = AreaType.train;

            bool isAbove = !IsCrossingUp(cm); // if crossing down, they’re above

            aboveColliderByCharacter[other.gameObject] = isAbove;

            cm.currentThreshold = null;
            
            cm.characterOnThresh = true;

            StopAllCoros();

 
            // cm.motionDirection = "normal";

            // if(!ptts.departureAwaiting)
            // {
            //     if(characterCrossingLeft) 
            //     {
            //         cm.fixedDirectionLeftDiagonal = true; // fix the player in \ left diag way while inside the collider
            //         cm.fixedDirectionRightDiagonal = false; // fix the player in \ left diag way while inside the collider

            //         cm.controlDirectionToPlayerDirection[Direction.Left] = Direction.UpLeft;
            //         cm.controlDirectionToPlayerDirection[Direction.UpLeft] = Direction.UpLeft;
            //         cm.controlDirectionToPlayerDirection[Direction.UpFacingLeft] = Direction.UpLeft;
            //         cm.controlDirectionToPlayerDirection[Direction.UpFacingRight] = Direction.UpLeft;
            //         cm.controlDirectionToPlayerDirection[Direction.UpRight] = Direction.UpLeft;
            //         cm.controlDirectionToPlayerDirection[Direction.Right] = Direction.RightDown;
            //         cm.controlDirectionToPlayerDirection[Direction.RightDown] = Direction.RightDown;
            //         cm.controlDirectionToPlayerDirection[Direction.DownFacingRight] = Direction.RightDown;
            //         cm.controlDirectionToPlayerDirection[Direction.DownFacingLeft] = Direction.RightDown;
            //         cm.controlDirectionToPlayerDirection[Direction.DownLeft] = Direction.RightDown;
            //     }
            //     else if (!characterCrossingLeft)
            //     {
            //         cm.fixedDirectionRightDiagonal = true; // fix the player in / right diag way while inside the collider
            //         cm.fixedDirectionLeftDiagonal = false; // fix the player in / right diag way while inside the collider

            //         cm.controlDirectionToPlayerDirection[Direction.Left] = Direction.DownLeft;
            //         cm.controlDirectionToPlayerDirection[Direction.UpLeft] = Direction.UpRight;
            //         cm.controlDirectionToPlayerDirection[Direction.UpFacingLeft] = Direction.UpRight;
            //         cm.controlDirectionToPlayerDirection[Direction.UpFacingRight] = Direction.UpRight;
            //         cm.controlDirectionToPlayerDirection[Direction.UpRight] = Direction.UpRight;
            //         cm.controlDirectionToPlayerDirection[Direction.Right] = Direction.UpRight;
            //         cm.controlDirectionToPlayerDirection[Direction.RightDown] = Direction.DownLeft;
            //         cm.controlDirectionToPlayerDirection[Direction.DownFacingRight] = Direction.DownLeft;
            //         cm.controlDirectionToPlayerDirection[Direction.DownFacingLeft] = Direction.DownLeft;
            //         cm.controlDirectionToPlayerDirection[Direction.DownLeft] = Direction.DownLeft;
                    
            //     }
            // }
            // else
            // {
            //     // if(characterCrossingLeft) 
            //     // {
            //     //     cm.fixedDirectionLeftDiagonal = true; // fix the player in \ left diag way while inside the collider
            //     //     cm.fixedDirectionRightDiagonal = false; // fix the player in \ left diag way while inside the collider

            //     //     cm.controlDirectionToPlayerDirection[Direction.Left] = Direction.RightDown;
            //     //     cm.controlDirectionToPlayerDirection[Direction.UpLeft] = Direction.RightDown;
            //     //     cm.controlDirectionToPlayerDirection[Direction.UpFacingLeft] = Direction.RightDown;
            //     //     cm.controlDirectionToPlayerDirection[Direction.UpFacingRight] = Direction.RightDown;
            //     //     cm.controlDirectionToPlayerDirection[Direction.UpRight] = Direction.RightDown;
            //     //     cm.controlDirectionToPlayerDirection[Direction.Right] = Direction.RightDown;
            //     //     cm.controlDirectionToPlayerDirection[Direction.RightDown] = Direction.RightDown;
            //     //     cm.controlDirectionToPlayerDirection[Direction.DownFacingRight] = Direction.RightDown;
            //     //     cm.controlDirectionToPlayerDirection[Direction.DownFacingLeft] = Direction.RightDown;
            //     //     cm.controlDirectionToPlayerDirection[Direction.DownLeft] = Direction.RightDown;
            //     // }
            //     // else if (!characterCrossingLeft)
            //     // {
            //     //     cm.fixedDirectionRightDiagonal = true; // fix the player in / right diag way while inside the collider
            //     //     cm.fixedDirectionLeftDiagonal = false; // fix the player in / right diag way while inside the collider
                    
            //     //     cm.controlDirectionToPlayerDirection[Direction.Left] = Direction.DownLeft;
            //     //     cm.controlDirectionToPlayerDirection[Direction.UpLeft] = Direction.DownLeft;
            //     //     cm.controlDirectionToPlayerDirection[Direction.UpFacingLeft] = Direction.DownLeft;
            //     //     cm.controlDirectionToPlayerDirection[Direction.UpFacingRight] = Direction.DownLeft;
            //     //     cm.controlDirectionToPlayerDirection[Direction.UpRight] = Direction.DownLeft;
            //     //     cm.controlDirectionToPlayerDirection[Direction.Right] = Direction.DownLeft;
            //     //     cm.controlDirectionToPlayerDirection[Direction.RightDown] = Direction.DownLeft;
            //     //     cm.controlDirectionToPlayerDirection[Direction.DownFacingRight] = Direction.DownLeft;
            //     //     cm.controlDirectionToPlayerDirection[Direction.DownFacingLeft] = Direction.DownLeft;
            //     //     cm.controlDirectionToPlayerDirection[Direction.DownLeft] = Direction.DownLeft;
            //     // }
            // }
    }
    // void OnTriggerStay2D(Collider2D other)
    // { 
    //         if(!other.GetComponent<BoxCollider2D>()) return;

    //         CharacterMovement cm = other.GetComponentInParent<CharacterMovement>();
    //         GameObject character = cm.gameObject;

    //         cm.currentArea = AreaType.train;
            
    //         cm.characterOnThresh = true;

    //         // cm.motionDirection = "normal";

    //         // if(!ptts.departureAwaiting)
    //         // {
    //         //     if(characterCrossingLeft) 
    //         //     {
    //         //         cm.fixedDirectionLeftDiagonal = true; // fix the player in \ left diag way while inside the collider
    //         //         cm.fixedDirectionRightDiagonal = false; // fix the player in \ left diag way while inside the collider

    //         //         cm.controlDirectionToPlayerDirection[Direction.Left] = Direction.UpLeft;
    //         //         cm.controlDirectionToPlayerDirection[Direction.UpLeft] = Direction.UpLeft;
    //         //         cm.controlDirectionToPlayerDirection[Direction.UpFacingLeft] = Direction.UpLeft;
    //         //         cm.controlDirectionToPlayerDirection[Direction.UpFacingRight] = Direction.UpLeft;
    //         //         cm.controlDirectionToPlayerDirection[Direction.UpRight] = Direction.UpLeft;
    //         //         cm.controlDirectionToPlayerDirection[Direction.Right] = Direction.RightDown;
    //         //         cm.controlDirectionToPlayerDirection[Direction.RightDown] = Direction.RightDown;
    //         //         cm.controlDirectionToPlayerDirection[Direction.DownFacingRight] = Direction.RightDown;
    //         //         cm.controlDirectionToPlayerDirection[Direction.DownFacingLeft] = Direction.RightDown;
    //         //         cm.controlDirectionToPlayerDirection[Direction.DownLeft] = Direction.RightDown;
    //         //     }
    //         //     else if (!characterCrossingLeft)
    //         //     {
    //                 cm.fixedDirectionRightDiagonal = true; // fix the player in / right diag way while inside the collider
    //         //         cm.fixedDirectionLeftDiagonal = false; // fix the player in / right diag way while inside the collider

    //         //         cm.controlDirectionToPlayerDirection[Direction.Left] = Direction.DownLeft;
    //         //         cm.controlDirectionToPlayerDirection[Direction.UpLeft] = Direction.UpRight;
    //         //         cm.controlDirectionToPlayerDirection[Direction.UpFacingLeft] = Direction.UpRight;
    //         //         cm.controlDirectionToPlayerDirection[Direction.UpFacingRight] = Direction.UpRight;
    //         //         cm.controlDirectionToPlayerDirection[Direction.UpRight] = Direction.UpRight;
    //         //         cm.controlDirectionToPlayerDirection[Direction.Right] = Direction.UpRight;
    //         //         cm.controlDirectionToPlayerDirection[Direction.RightDown] = Direction.DownLeft;
    //         //         cm.controlDirectionToPlayerDirection[Direction.DownFacingRight] = Direction.DownLeft;
    //         //         cm.controlDirectionToPlayerDirection[Direction.DownFacingLeft] = Direction.DownLeft;
    //         //         cm.controlDirectionToPlayerDirection[Direction.DownLeft] = Direction.DownLeft;
                    
    //         //     }
    //         // }
    //         // else
    //         // {
    //         //     // if(characterCrossingLeft) 
    //         //     // {
    //         //     //     cm.fixedDirectionLeftDiagonal = true; // fix the player in \ left diag way while inside the collider
    //         //     //     cm.fixedDirectionRightDiagonal = false; // fix the player in \ left diag way while inside the collider

    //         //     //     cm.controlDirectionToPlayerDirection[Direction.Left] = Direction.RightDown;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.UpLeft] = Direction.RightDown;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.UpFacingLeft] = Direction.RightDown;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.UpFacingRight] = Direction.RightDown;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.UpRight] = Direction.RightDown;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.Right] = Direction.RightDown;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.RightDown] = Direction.RightDown;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.DownFacingRight] = Direction.RightDown;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.DownFacingLeft] = Direction.RightDown;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.DownLeft] = Direction.RightDown;
    //         //     // }
    //         //     // else if (!characterCrossingLeft)
    //         //     // {
    //         //     //     cm.fixedDirectionRightDiagonal = true; // fix the player in / right diag way while inside the collider
    //         //     //     cm.fixedDirectionLeftDiagonal = false; // fix the player in / right diag way while inside the collider
                    
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.Left] = Direction.DownLeft;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.UpLeft] = Direction.DownLeft;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.UpFacingLeft] = Direction.DownLeft;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.UpFacingRight] = Direction.DownLeft;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.UpRight] = Direction.DownLeft;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.Right] = Direction.DownLeft;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.RightDown] = Direction.DownLeft;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.DownFacingRight] = Direction.DownLeft;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.DownFacingLeft] = Direction.DownLeft;
    //         //     //     cm.controlDirectionToPlayerDirection[Direction.DownLeft] = Direction.DownLeft;
    //         //     // }
    //         // }
    // }

    void OnTriggerExit2D(Collider2D other)
    {
        if(!other.GetComponent<BoxCollider2D>()) return;

        CharacterMovement cm = other.transform.root.GetComponent<CharacterMovement>();
        GameObject character = cm.gameObject;
        

        cm.currentThreshold = null;

        bool aboveCollider = aboveColliderByCharacter.ContainsKey(other.gameObject) && aboveColliderByCharacter[other.gameObject];
        
        cm.characterOnThresh = false;

        // cm.fixedDirectionLeftDiagonal = false;
        // cm.fixedDirectionRightDiagonal = false; // un-fix the player in \/ left/right diag way upon collider exit. 
        // cm.ResetPlayerMovement(); 
        cm.currentArea = AreaType.platform;

        if(character = roundScript.player)
        {
            cm.ResetPlayerMovement();
            cm.activeCollisions.Clear();
        }

        // if(character = roundScript.player)



        //     //ON EXIT CROSSING UP
        // if (IsCrossingUp(cm) || IsCrossingRight(cm))
        // {           
        //     cm.previousArea = AreaType.train;
        //     cm.currentArea = AreaType.platform;
        //     // ptts.CharacterEnterArea(character);
        // }  
        // else 
        // if (!IsCrossingUp(cm) || !IsCrossingRight(cm)) 
        // //ON EXIT CROSSING DOWN
        // {
           
        //     cm.previousArea = AreaType.platform;
        //     cm.currentArea = AreaType.train;
        //     // ptts.CharacterEnterArea(character);
        // }

        aboveColliderByCharacter.Remove(other.gameObject);
    }
    private bool IsCrossingUp(CharacterMovement cm)
    {
        return cm.change.y > 0;
    }
    private bool IsCrossingRight(CharacterMovement cm)
    {
        return cm.change.y > 0;
    }

    void StopAllCoros()
    {
        // if(this.closedDoorFadeCoroutine != null)
        // {
        //     StopCoroutine(this.closedDoorFadeCoroutine);
        // }

        // if(this.openDoorFadeCoroutine != null)
        // {
        //     StopCoroutine(this.openDoorFadeCoroutine);
        // }
        // if (thresholdSortingSequenceCoro != null)
        // {
        //     StopCoroutine(thresholdSortingSequenceCoro);
        //     SetTreeSortingLayer(player, initialSortingLayer);
        // }
        // if (moveToCldrCenterCoro != null)
        // {
        //     StopCoroutine(moveToCldrCenterCoro);
        // }
    }

    // IEnumerator ThresholdLayerSortingSequence(
    //     float waitTime,
    //     GameObject gameObject,
    //     string newSortingLayer) 
    // {  
    //     initialSortingLayer = gameObject.GetComponentInChildren<SpriteRenderer>().sortingLayerName; 
    //     // initialSortingLayer = LayerMask.LayerToName(this.gameObject.layer);

    //     SetTreeSortingLayer(gameObject, newSortingLayer);

    //     yield return new WaitForSeconds(waitTime);

    //     if (gameObject.GetComponentInChildren<SpriteRenderer>().sortingLayerName == newSortingLayer)
    //     {
    //         SetTreeSortingLayer(gameObject, initialSortingLayer);
    //     }

    //     yield return null;
    // }

    // static void SetTreeSortingLayer(GameObject gameObject, string sortingLayerName)
    // {
    //     if(gameObject.GetComponent<SpriteRenderer>() != null) 
    //     {
    //         gameObject.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
    //     }
    //     foreach (Transform child in gameObject.transform)
    //     {
    //         RoomThresholdColliderScript.SetTreeSortingLayer(child.gameObject, sortingLayerName);
    //     }
    // }  
}
