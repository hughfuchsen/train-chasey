using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AreaType
{
    train,
    track,
    platform
}


public class PlatformTrackTrainScript : MonoBehaviour
{
    GameObject player;
    GameObject pig;
    GameObject person;
    GameObject pony;
    GameObject puppy;
    GameObject chicken;
    // ============================================================
    // AREA
    // ============================================================

    // public List<GameObject> characterListForPlatformArea = new List<GameObject>();
    // public List<GameObject> characterListForTrainArea = new List<GameObject>();
    
    [SerializeField] RoundScript roundScript;
    public List<GameObject> characterList = new List<GameObject>();

    // ============================================================
    // BARRIER
    // ============================================================

    public BoxCollider2D trigger;

    // ============================================================
    // MOVEMENT SETTINGS
    // ============================================================

    [Header("Movement")]

    [SerializeField]
    private float timeToReachTarget = 15f;

    public int platformDisplacementX = 3000;

    private float perspectiveAngle;

    public bool departureAwaiting = false;
    private bool wasOnThreshold = false;

    public Vector3 angularEquation;


    // ============================================================
    // TRAIN
    // ============================================================

    [Header("Train")]

    [SerializeField]
    private Vector3 trainsInitialPosition;

    [SerializeField] GameObject train1Colliders;
    [SerializeField] GameObject train2Colliders;
    [SerializeField] GameObject train3Colliders;

    public List<GameObject> train1ListForSettingAlphas =
        new List<GameObject>();
    public List<GameObject> train2ListForSettingAlphas =
        new List<GameObject>();
    public List<GameObject> train3ListForSettingAlphas =
        new List<GameObject>();

    public GameObject trains;

    private List<GameObject> currentTrainSpriteObjList = null;
    private List<GameObject> previousTrainSpriteObjList = null;

    // ============================================================
    // CURRENT TRAIN
    // ============================================================

    private enum TrainNumber
    {
        train1,
        train2,
        train3
    }

    private TrainNumber currentTrainNumber;

    // ============================================================
    // TRACKS AND PLATFORMS FOR MOVEMENT AND SPRITE ALPHA TO ZERO/1
    // ============================================================

    [Header("Track and Platform ")]

    [SerializeField] GameObject plaformsAndTrack;
    private Vector3 plaformsAndTrackInitialPos;

    [SerializeField] GameObject platform1Colliders;
    [SerializeField] GameObject platform2Colliders;
    [SerializeField] GameObject platform3Colliders;

    public List<GameObject> platform1ListForSettingAlphas =
        new List<GameObject>();
    public List<GameObject> platform2ListForSettingAlphas =
        new List<GameObject>();
    public List<GameObject> platform3ListForSettingAlphas =
        new List<GameObject>();
        
    public GameObject allPlatformSprites;

    private List<GameObject> currentPlatformSpriteObjList = null;
    private List<GameObject> previousPlatformSpriteObjList = null;


    // ============================================================
    // CURRENT / NEXT PLATFORM
    // ============================================================

    private enum PlatformNumber
    {
        platform1,
        platform2,
        platform3
    }

    private PlatformNumber currentPlatformNumber;



    // ============================================================
    // POSITIONS
    // ============================================================

    public Vector3 positionBottomRight =
        new Vector3(0, 0, 0);

    public Vector3 positionTopLeft =
        new Vector3(2320, 1160, 0);


    // ============================================================
    // COROUTINES
    // ============================================================

    [HideInInspector]
    public Coroutine currentPlatformTrackMotionCoroutine;

    [HideInInspector]
    public Coroutine currentTrainMotionCoroutine;


    // ============================================================
    // INITIALISATION
    // ============================================================

    private void Awake()
    {
        trigger.isTrigger = false;

        plaformsAndTrackInitialPos =
            plaformsAndTrack.transform.localPosition;
        trainsInitialPosition =
            trains.transform.localPosition;


        // choose current platform randomly
        currentPlatformNumber = (PlatformNumber)Random.Range(
            0,
            System.Enum.GetValues(typeof(PlatformNumber)).Length
        );

        player = GameObject.FindGameObjectWithTag("Player");

        // characterList.Add(pig);
        // characterList.Add(person);
        // characterList.Add(pony);
        // characterList.Add(puppy);
        // characterList.Add(chicken);
    }


    private void Start()
    {
        // characters start in train for now

        SetSpritesAlpha(
            allPlatformSprites,
            0f
        );
        SetSpritesAlpha(
            trains,
            0f
        );
        characterList = roundScript.npcs;

        PrepareStartTrainState();
        
        GridGenerator gridGenerator = GameObject.FindObjectOfType<GridGenerator>();

        gridGenerator.UpdateNodeViability();


    }

    private void Update()
    {        
        if (!departureAwaiting)
            return;


        if (player == null)
            return;

        CharacterMovement cm = player.GetComponent<CharacterMovement>();

        if (cm == null)
            return;

        // Threshold crossing has finished
        if (!cm.characterOnThresh)
        {
            departureAwaiting = false;

            StartDeparture(Random.value < 0.5f);
        }
    }


    // ============================================================
    // CHARACTER AREA MANAGEMENT
    // ============================================================

    // public void CharacterEnterArea(GameObject character)
    // {
    //     var ca = character.GetComponent<CharacterAnimation>();
    //     var cm = character.GetComponent<CharacterMovement>();
    //     var iss = character.GetComponent<IsoSpriteSorting>();

    //     // HandleInclineCollisionIgnoring(character);

    //     // if NPC was already in another room

    //         // cm.previousArea.characterListForArea.Remove(character);
    //     if(cm.currentArea == AreaType.platform)
    //     {
    //         characterListForPlatformArea.Add(character);
    //         characterListForTrainArea.Remove(character);
    //     }
    //     else if(cm.currentArea == AreaType.train)
    //     {
    //         characterListForPlatformArea.Remove(character);
    //         characterListForTrainArea.Add(character);
    //     }
        
    // }


    // ============================================================
    // DEPARTURE DECISION
    // ============================================================

    public void StartDeparture(bool moveUpLeft)
    {
        CharacterMovement cm = player.GetComponent<CharacterMovement>();
        GridGenerator gridGenerator = GameObject.FindObjectOfType<GridGenerator>();

        // --------------------------------------------------------
        // DON'T MAKE A DEPARTURE DECISION WHILE IN THRESHOLD
        // --------------------------------------------------------

        if (cm.characterOnThresh)
        {
            Debug.Log("Player is currently crossing a threshold. Departure awaiting.");

            departureAwaiting = true;
            return;
        }

        // --------------------------------------------------------
        // PLAYER WAS WAITING AND HAS NOW LEFT THRESHOLD
        // --------------------------------------------------------

        if (departureAwaiting)
        {
            departureAwaiting = false;

            Debug.Log("Threshold crossed. Departure proceeding.");
        }

        
        // --------------------------------------------------------
        // PLAYER IS ON TRAIN
        // --------------------------------------------------------

        if (cm.currentArea == AreaType.train)
        {
            PrepareStartPlatformState();
            // gridGenerator.UpdateNodeViability();
            MovePlatformAndTrack(moveUpLeft);
        }

        // --------------------------------------------------------
        // PLAYER IS ON PLATFORM
        // --------------------------------------------------------

        else if (cm.currentArea == AreaType.platform)
        {
            // PrepareStartTrainState();
            PrepareTrainState();
            // gridGenerator.UpdateNodeViability();
            MoveTrain(moveUpLeft);
        }



        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i] == null)
                continue;

            

            NPCPathFollower pf =
            characterList[i].GetComponent<NPCPathFollower>();

            pf.ResetForNewRound();
        }
        
        
        gridGenerator.UpdateNodeViability();


    }


    // ============================================================
    // MOVE PLATFORM + TRACK
    // ============================================================

    public void MovePlatformAndTrack(bool moveUpLeft)
    {
        if (currentPlatformTrackMotionCoroutine != null)
            return;

        currentPlatformTrackMotionCoroutine =
            StartCoroutine(
                LerpPlatformsAndTrack(moveUpLeft)
            );
    }


    // ============================================================
    // MOVE TRAIN ONLY
    // ============================================================

    public void MoveTrain(bool moveUpLeft)
    {
        if (currentTrainMotionCoroutine != null)
            return;

        currentTrainMotionCoroutine =
            StartCoroutine(LerpTrain(moveUpLeft));
    }


    // ============================================================
    // PLATFORM + TRACK COROUTINE
    // ============================================================

    private IEnumerator LerpPlatformsAndTrack(
        bool moveUpLeft
    )
    {
        if(previousPlatformSpriteObjList != null)
        {
            foreach (GameObject platform in previousPlatformSpriteObjList)
            {
                if (!platform.transform.parent.CompareTag("MiddlePlatformParent"))
                {
                    SetSpritesAlpha(platform, 0f);
                }
            }
        }

        if(currentPlatformSpriteObjList != null)
        {
            foreach (GameObject platform in currentPlatformSpriteObjList)
            {
                if (!platform.transform.parent.CompareTag("MiddlePlatformParent"))
                {
                    SetSpritesAlpha(platform, 1f);
                }
            }
        }


        Vector3 displacement =
            GetMovementDisplacement(moveUpLeft);

        CalculateAngularEquation(moveUpLeft);


        // --------------------------------------------------------
        // STORE START POSITIONS
        // --------------------------------------------------------

        // Vector3 platformStart =
        //     currentPlatformParent.transform.localPosition;

        Vector3 target =
            plaformsAndTrackInitialPos + displacement;

        Vector3[] initialCharacterPositions =
            new Vector3[characterList.Count];

        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i] == null)
                continue;

            if (characterList[i].CompareTag("Player"))
                continue;

            Transform xFlip =
                characterList[i].transform.Find("xFlip");

            if (xFlip == null)
                continue;

            initialCharacterPositions[i] =
                xFlip.localPosition;
        }

        bool charactersHaveJumped = false;

        float elapsedTime = 0f;


        // --------------------------------------------------------
        // MOVE
        // --------------------------------------------------------

        while (elapsedTime < timeToReachTarget)
        {
            elapsedTime += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsedTime / timeToReachTarget
                );


            // ----------------------------------------------------
            // PLATFORM
            // ----------------------------------------------------

            plaformsAndTrack.transform.localPosition =
                Vector3.Lerp(
                    plaformsAndTrackInitialPos,
                    target,
                    t
                );


            // ----------------------------------------------------
            // TRACK
            // ----------------------------------------------------

            // currentTrack.transform.localPosition =
            //     Vector3.Lerp(
            //         trackStart,
            //         trackTarget,
            //         t
            //     );


            // ----------------------------------------------------
            // CHARACTERS
            // ----------------------------------------------------
            

            MoveCharactersWithArea(
                t,
                characterList,
                AreaType.platform,
                displacement,
                initialCharacterPositions,
                moveUpLeft,
                ref charactersHaveJumped
             );

            yield return null;
        }


        // --------------------------------------------------------
        // FINAL POSITIONS
        // --------------------------------------------------------

        plaformsAndTrack.transform.localPosition =
            target;

        // currentTrack.transform.localPosition =
        //     trackTarget;


        // --------------------------------------------------------
        // RECYCLE PLATFORM + TRACK
        // --------------------------------------------------------

        if(previousPlatformSpriteObjList != null)
        {
            foreach (GameObject platform in previousPlatformSpriteObjList)
            {
                if (platform.transform.parent.CompareTag("MiddlePlatformParent"))
                {
                    SetSpritesAlpha(platform, 0f);
                }
            }
        }
        if(currentPlatformSpriteObjList != null)
        {
            foreach (GameObject platform in currentPlatformSpriteObjList)
            {
                if (platform.transform.parent.CompareTag("MiddlePlatformParent"))
                {
                    SetSpritesAlpha(platform, 1f);
                }
            }
        }

        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i] == null)
                continue;

            if (characterList[i].CompareTag("Player"))
                continue;

            Transform xFlip =
                characterList[i].transform.Find("xFlip");

            if (xFlip != null)
            {
                xFlip.localPosition =
                    initialCharacterPositions[i];
            }
        }

        trigger.isTrigger = true;

        plaformsAndTrack.transform.localPosition =
            plaformsAndTrackInitialPos;
        
        currentPlatformTrackMotionCoroutine = null;

    }


    // ============================================================
    // TRAIN ONLY COROUTINE
    // ============================================================

    private IEnumerator LerpTrain(
        bool moveUpLeft
    )
    {
        Vector3 displacement =
            GetMovementDisplacement(moveUpLeft);

        CalculateAngularEquation(moveUpLeft);


        Vector3 trainTarget =
            trainsInitialPosition + displacement;
        
        Vector3[] initialCharacterPositions =
            new Vector3[characterList.Count];

        for (int i = 0; i < initialCharacterPositions.Length; i++)
        {
            if (characterList[i] == null)
                continue;

            if (characterList[i].CompareTag("Player"))
                continue;

            Transform xFlip =
                characterList[i].transform.Find("xFlip");

            if (xFlip == null)
                continue;

            initialCharacterPositions[i] =
                xFlip.localPosition;
        }

        bool charactersHaveJumped = false;

        float elapsedTime = 0f;


        while (elapsedTime < timeToReachTarget)
        {
            elapsedTime += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsedTime / timeToReachTarget
                );


            trains.transform.localPosition =
                Vector3.Lerp(
                    trainsInitialPosition,
                    trainTarget,
                    t
                );

        // ----------------------------------------------------
        // CHARACTERS
        // ----------------------------------------------------

            MoveCharactersWithArea(
                t,
                characterList,
                AreaType.train,
                displacement,
                initialCharacterPositions,
                moveUpLeft,
                ref charactersHaveJumped
            );

            yield return null;
        }


        trains.transform.localPosition =
            trainTarget;


        // --------------------------------------------------------
        // RESET TRAIN FOR NEXT JOURNEY
        // --------------------------------------------------------
        
        if(previousTrainSpriteObjList != null)
        {
            foreach (GameObject train in previousTrainSpriteObjList)
            {
                // if (train.transform.parent.CompareTag("MiddleTrainParent"))
                // {
                    SetSpritesAlpha(train, 0f);
                // }
            }
        }

        if(currentTrainSpriteObjList != null)
        {
            foreach (GameObject train in currentTrainSpriteObjList)
            {
                // if (train.transform.parent.CompareTag("MiddleTrainParent"))
                // {
                    SetSpritesAlpha(train, 1f);
                // }
            }
        }
        
        for (int i = 0; i < initialCharacterPositions.Length; i++)
        {
            if (characterList[i] == null)
                continue;

            if (characterList[i].CompareTag("Player"))
                continue;

            Transform xFlip =
                characterList[i].transform.Find("xFlip");

            if (xFlip != null)
            {
                xFlip.localPosition =
                    initialCharacterPositions[i];
            }
        }
        
        trigger.isTrigger = true;
        
        trains.transform.localPosition =
            trainsInitialPosition;

        currentTrainMotionCoroutine = null;

    }


    // ============================================================
    // CHARACTER MOVEMENT
    // ============================================================

    private void MoveCharactersWithArea(
        float t,
        List<GameObject> characterListForArea,
        AreaType areaType,
        Vector3 displacement,
        Vector3[] initialCharacterPositions,
        bool moveUpLeft,
        ref bool charactersHaveJumped
        )
    {
        for (int i = 0; i < characterListForArea.Count; i++)
        {
            GameObject npc =
                characterListForArea[i];

            if (npc == null)
                continue;

            if (npc.CompareTag("Player"))
                continue;

            CharacterMovement cm =
                npc.GetComponent<CharacterMovement>();

            IsoSpriteSorting iso =
                npc.GetComponent<IsoSpriteSorting>();

            Transform visualRoot =
                npc.transform.Find("xFlip");

            if (cm == null ||
                iso == null ||
                visualRoot == null)
                continue;

            if (cm.currentArea != areaType)
                continue;

            // ----------------------------------------------------
            // NORMAL MOVEMENT
            // ----------------------------------------------------

            Vector3 startPos =
                initialCharacterPositions[i];

            Vector3 targetPos =
                startPos + displacement;

            Vector3 currentPosition =
                Vector3.Lerp(
                    startPos,
                    targetPos,
                    t
                );

            // ----------------------------------------------------
            // HALF-WAY TELEPORT
            // ----------------------------------------------------

            if (t >= 0.5f)
            {
                Vector3 jumpDisplacement =
                    -displacement;

                currentPosition += jumpDisplacement;
            }

            visualRoot.localPosition =
                currentPosition;

            cm.change = Vector3.zero;
        }

        if (t >= 0.5f)
        {
            charactersHaveJumped = true;
        }
    }

    // ============================================================
    // PLATFORM PREPARATION
    // ============================================================

    public void PrepareStartPlatformState()
    {
        trigger.isTrigger = false;

        previousPlatformSpriteObjList = currentPlatformSpriteObjList;
        PlatformNumber previousPlatformNumber = currentPlatformNumber;

        CharacterMovement cm = player.GetComponent<CharacterMovement>();

        do
        {
            currentPlatformNumber = (PlatformNumber)Random.Range(
                0,
                System.Enum.GetValues(typeof(PlatformNumber)).Length
            );
        }
        while (currentPlatformNumber == previousPlatformNumber);


        platform1Colliders.SetActive(false);
        platform2Colliders.SetActive(false);
        platform3Colliders.SetActive(false);

        switch (currentPlatformNumber)
        {
            case PlatformNumber.platform1:
                currentPlatformSpriteObjList = platform1ListForSettingAlphas;
                platform1Colliders.SetActive(true);
                break;

            case PlatformNumber.platform2:
                currentPlatformSpriteObjList = platform2ListForSettingAlphas;
                platform2Colliders.SetActive(true);
                // platform1Colliders.SetActive(true);

                break;

            case PlatformNumber.platform3:
                currentPlatformSpriteObjList = platform3ListForSettingAlphas;
                platform3Colliders.SetActive(true);
                // platform1Colliders.SetActive(true);

                break;
        }

        foreach (GameObject platform in currentPlatformSpriteObjList)
        {
            if (platform.transform.parent.CompareTag("MiddlePlatformParent"))
            {
                SetSpritesAlpha(platform, 0f);
            }
            else
            {
                SetSpritesAlpha(platform, 1f);
            }

        }

    }
   
    // ============================================================
    // TRAIN PREPARATION
    // ============================================================
    public void PrepareStartTrainState()
    {
        trigger.isTrigger = false;
        
        TrainNumber previousTrainNumber = currentTrainNumber;

        do
        {
            currentTrainNumber = (TrainNumber)Random.Range(
                0,
                System.Enum.GetValues(typeof(TrainNumber)).Length
            );
        }
        while (currentTrainNumber == previousTrainNumber);


        train1Colliders.SetActive(false);
        train2Colliders.SetActive(false);
        train3Colliders.SetActive(false);

        switch (currentTrainNumber)
        {
            case TrainNumber.train1:
                currentTrainSpriteObjList = train1ListForSettingAlphas;
                train1Colliders.SetActive(true);
                break;

            case TrainNumber.train2:
                currentTrainSpriteObjList = train2ListForSettingAlphas;
                train2Colliders.SetActive(true);
                break;

            case TrainNumber.train3:
                currentTrainSpriteObjList = train3ListForSettingAlphas;
                train3Colliders.SetActive(true);
                break;
        }

        foreach (GameObject trains in currentTrainSpriteObjList)
        {
            SetSpritesAlpha(trains, 1f);
        }
        
    }
    public void PrepareTrainState()
    {
        trigger.isTrigger = false;
        
        previousTrainSpriteObjList = currentTrainSpriteObjList;
        // currentPlatformSpriteObjList.Clear();
        TrainNumber previousTrainNumber = currentTrainNumber;

        CharacterMovement cm = player.GetComponent<CharacterMovement>();

        do
        {
            currentTrainNumber = (TrainNumber)Random.Range(
                0,
                System.Enum.GetValues(typeof(TrainNumber)).Length
            );
        }
        while (currentTrainNumber == previousTrainNumber);


        train1Colliders.SetActive(false);
        train2Colliders.SetActive(false);
        train3Colliders.SetActive(false);

        switch (currentTrainNumber)
        {
            case TrainNumber.train1:
                currentTrainSpriteObjList = train1ListForSettingAlphas;
                train1Colliders.SetActive(true);
                break;

            case TrainNumber.train2:
                currentTrainSpriteObjList = train2ListForSettingAlphas;
                train2Colliders.SetActive(true);
                break;

            case TrainNumber.train3:
                currentTrainSpriteObjList = train3ListForSettingAlphas;
                train3Colliders.SetActive(true);
                break;
        }

        if(previousTrainSpriteObjList != null)
        {
            foreach (GameObject train in previousTrainSpriteObjList)
            {
                if (train.transform.parent.CompareTag("MiddleTrainParent"))
                {
                    SetSpritesAlpha(train, 1f);
                }
                else
                {
                    SetSpritesAlpha(train, 0f);
                }
            }
        }

        if(currentTrainSpriteObjList != null)
        {
            foreach (GameObject train in currentTrainSpriteObjList)
            {
                if (train.transform.parent.CompareTag("MiddleTrainParent"))
                {
                    SetSpritesAlpha(train, 0f);
                }
                else
                {
                    SetSpritesAlpha(train, 1f);
                }
            }
        }
    }
    
    

    // ============================================================
    // PLATFORM VISUALS / COLLIDERS
    // ============================================================

    private void SetSpritesAlpha(
        GameObject parent,
        float alpha
    )
    {
        SpriteRenderer[] sprites =
            parent.GetComponentsInChildren<SpriteRenderer>(
                true
            );

        foreach (SpriteRenderer sprite in sprites)
        {
            Color colour = sprite.color;

            colour.a = alpha;

            sprite.color = colour;
        }
    }


    // ============================================================
    // MOVEMENT CALCULATIONS
    // ============================================================

    private Vector3 GetMovementDisplacement(
        bool moveUpLeft
    )
    {
        if (moveUpLeft)
        {
            return new Vector3(
                -2320,
                1160,
                0
            );
        }

        return new Vector3(
            2320,
            -1160,
            0
        );
    }


    private void CalculateAngularEquation(
        bool moveUpLeft
    )
    {
        perspectiveAngle =
            Mathf.Atan(-0.5f);

        platformDisplacementX =
            Mathf.Abs(platformDisplacementX) * -1;


        if (moveUpLeft)
        {
            angularEquation =
                new Vector3(
                    platformDisplacementX,

                    platformDisplacementX /
                    Mathf.Cos(perspectiveAngle) *
                    Mathf.Sin(perspectiveAngle)
                    + 1,

                    0
                );
        }
        else
        {
            angularEquation =
                -new Vector3(
                    platformDisplacementX,

                    platformDisplacementX /
                    Mathf.Cos(perspectiveAngle) *
                    Mathf.Sin(perspectiveAngle)
                    + 1,

                    0
                );
        }
    }


}


    