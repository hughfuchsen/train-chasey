using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;  // This is necessary for using TMP_InputField


public class CharacterAnimation : MonoBehaviour
{
    public float xFlipOffset = 15f; //bounds width
    [HideInInspector] public Bounds bounds; //bounds
    [SerializeField] Transform visualRoot;
    [SerializeField] GameObject bodyPartToAnimate;
    [HideInInspector] public Transform bodyPartInitialState;
    [HideInInspector] public Transform initialState;
    public CharacterType characterType;
    [HideInInspector] public float initialAnimationSpeed;
    public float animationSpeed = 0.08f; // Time between frames

    public bool runningAnim;

    public List<GameObject> characterSpriteList = new List<GameObject>();
    public List<Vector3> initialChrctrSpriteTransformList = new List<Vector3>();
    [HideInInspector] public List<GameObject> zeroInitialAlphaSpriteList = new List<GameObject>();
    [HideInInspector] public List<Color> initialChrctrColorList = new List<Color>();

    public bool isFacingLeft;


    IsoSpriteSorting IsoSpriteSorting;

    [HideInInspector]
    public int animDirectionAdjustorInt = 26, rightDownAnim, leftDownAnim, rightAnim, leftAnim,
                            upRightAnim, upLeftAnim, ladderAnimDirectionIndex;


    [HideInInspector] public int idle, walk, run, sit, climb, rideBike, rideWheelchair;




    [HideInInspector]
    public Sprite[]
                        //person
                        allHeadSprites, allEyeSprites, allThroatSprites, allCollarSprites, allTorsoSprites,
                        allWaistSprites, allWaistShortsSprites, allKneesShinsSprites, allAnklesSprites, allFeetSprites,
                        allDressSprites, allJakettoSprites, allLongSleeveSprites, allHandSprites, allShortSleeveSprites,
                        allHat1TopSprites, allMohawk5TopSprites, allMohawk5BottomSprites, allHair0TopSprites, allHair0BottomSprites,
                        allHair1TopSprites, allHair7TopSprites, allHair8TopSprites, allHair1BottomSprites,
                        allHair2BottomSprites, allHair3BottomSprites, allHair4BottomSprites, allHair6BottomSprites,
                        allHair7BottomSprites, allHair8BottomSprites, allHairFringe1Sprites, allHairFringe2Sprites,
                        allBikeSprites, allWheelchairSprites,
                        //chicken
                        chickenFeetSprites, 
                        //pony
                        ponyEyesSprites, ponyFaceSprites, ponyShadowSprites, ponyHairSprites, ponyFeetSprites,
                        //pig
                        pigEyesSprites, pigFaceSprites, pigNoseSprites, pigBodySprites, pigFeetSprites,
                        //puppy
                        puppyEyesSprites, puppyNoseSprites, puppyMain1Sprites, puppyMain2Sprites, puppyFeetSprites;

    [HideInInspector]
    public SpriteRenderer
                          //person
                          headSprite, eyeSprite, throatSprite, collarSprite, torsoSprite, waistSprite,
                          waistShortsSprite, kneesShinsSprite, anklesSprite, feetSprite, jakettoSprite, dressSprite,
                          longSleeveSprite, handSprite, shortSleeveSprite, hat1TopSprite, mohawk5TopSprite, mohawk5BottomSprite,
                          hair0TopSprite, hair0BottomSprite, hair1TopSprite, hair7TopSprite, hair8TopSprite,
                          hair1BottomSprite, hair2BottomSprite, hair3BottomSprite, hair4BottomSprite, hair6BottomSprite,
                          hair7BottomSprite, hair8BottomSprite, hairFringe1Sprite, hairFringe2Sprite, wheelchairWheelSprite, 
                          wheelchairBackSprite, wheelchairFrontSprite, bikeSprite,

                          //chicken
                          chickenBodySprite, chickenFeetSprite, chickenWingSprite, chickenSmallCombSprite, chickenLargeCombSprite,
                          chickenHeadSprite, chickenBeakSprite, chickenEyeSprite,
                          //pony
                          ponyEyesSprite, ponyFaceSprite, ponyShadowSprite, ponyHairSprite, ponyFeetSprite,
                          //pig
                          pigEyesSprite, pigFaceSprite, pigNoseSprite, pigBodySprite, pigFeetSprite,
                          //puppy
                          puppyEyesSprite, puppyNoseSprite, puppyMain1Sprite, puppyMain2Sprite, puppyFeetSprite;



    WheelVehicleTransformAdjustment wheelVehicleTransformAdjustment;
    BikeScript bikeScript;

    [SerializeField] GameObject bikeStatic;


    [HideInInspector] public Vector3 wheelchairWheelSpriteInitialPos;
    [HideInInspector] public Vector3 wheelchairBackSpriteInitialPos;
    [HideInInspector] public Vector3 wheelchairFrontSpriteInitialPos;
    [HideInInspector] private Vector3 wheelchairFlipOffset = new Vector3(12,0,0);
    [HideInInspector] private Vector3 wheelchairWheelFlipOffset = new Vector3(12,0,0);


    [HideInInspector] public Color currentWheelVehicleColor, wheelVehicleInitialColor;


    [HideInInspector] public Coroutine allowTimeForSpaceBarCoro;
    [HideInInspector] public Coroutine lateStartUpdateCharacterDataCoro;
    [HideInInspector] public Coroutine bodyPartMoveCoro;




    [HideInInspector] public bool playerOnFurniture = false;


    [HideInInspector] public float timer;
    [HideInInspector]
    public int currentFrame, movementStartIndex, movementFrameCount, currentAnimationDirection = 0,
                        // bodyTypeNumber, 
                        bodyTypeIndexMultiplier = 156;

    public int bodyTypeNumber;

    [HideInInspector] public int[] movementIndices;

    private float idleHopTimer = 0f;
    private float bodyPartMoveTimer = 0f;



    CharacterCustomization characterCustomization;

    CharacterMovement characterMovement;

    [HideInInspector]
    public Color currentSkinColor, currentHairColor, currentHatColor, currentShirtColor, currentPantsColor,
                            currentShoeColor, currentJakettoColor;



    [HideInInspector] public Coroutine characterCustomizationIdleCoro;


    [HideInInspector] public TMP_InputField[] inputFields;


    [HideInInspector] public FurnitureScript currentFurnitureScript;
    [HideInInspector] public Vector3 initialWaistTransformPos;




    void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        IsoSpriteSorting = GetComponent<IsoSpriteSorting>();
        characterCustomization = GetComponent<CharacterCustomization>();

        if (visualRoot != null)
            initialState = visualRoot.transform;

        if (bodyPartToAnimate != null)
            bodyPartInitialState = bodyPartToAnimate.transform;

        initialAnimationSpeed = animationSpeed;

        movementFrameCount = 4;

        wheelVehicleInitialColor = Color.black;
        wheelVehicleInitialColor.a = 0f;
    }

    void Start()
    {
        lateStartUpdateCharacterDataCoro = StartCoroutine(LateStartUpdateCharacterData());

    }

    IEnumerator LateStartUpdateCharacterData()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        GetSpritesAndAddToLists(this.gameObject, characterSpriteList, new List<GameObject>(), initialChrctrColorList, initialChrctrSpriteTransformList);
        LoadCharacter();
    }



    public void LoadCharacter()
    {
        switch (characterType)
        {
            case CharacterType.pig:
                LoadPig();
                break;

            case CharacterType.person:
                LoadPerson();
                break;

            case CharacterType.pony:
                LoadPony();
                break;

            case CharacterType.puppy:
                LoadPuppy();
                break;

            case CharacterType.chicken:
                LoadChicken();
                break;
        }

        SwitchOutAlpha(characterType.ToString().ToLower());

        currentFrame = 0;
    }

    void SwitchOutAlpha(string characterType)
    {        
        // // {
        //     foreach (GameObject obj in characterSpriteList)
        //     {
        //         SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        //         SetAlpha(sr, 0f);
        //     }

        //     Transform parent = transform.Find("xFlip").Find(characterType);
        //     SpriteRenderer [] renderers = parent.GetComponentsInChildren<SpriteRenderer>(true);
        //     foreach (SpriteRenderer sr in renderers)
        //     {
        //         // if(!sr.CompareTag("Wheelchair") 
        //         // && !characterMovement.playerOnWheelchair)
        //         // {
        //             SetAlpha(sr, 1f);
        //         // }
        //     }
            if(characterType == "person")
            {
                characterCustomization.UpdateRandom();
                
            }   
            
        // }
    }

    void LoadPerson()
    {
        currentFrame = 0;
        animDirectionAdjustorInt = 26;
        bodyTypeIndexMultiplier = 156;

        rightDownAnim = 0 * animDirectionAdjustorInt;
        leftDownAnim = 1 * animDirectionAdjustorInt;
        rightAnim = 2 * animDirectionAdjustorInt;
        leftAnim = 3 * animDirectionAdjustorInt;
        upRightAnim = 4 * animDirectionAdjustorInt;
        upLeftAnim = 5 * animDirectionAdjustorInt;

        idle = 0; walk = 1; run = 5; sit = 10; climb = 11; rideBike = 15; rideWheelchair = 9;
        // person
        allHeadSprites = Resources.LoadAll<Sprite>("head");
        allEyeSprites = Resources.LoadAll<Sprite>("eyes");
        allThroatSprites = Resources.LoadAll<Sprite>("throat");
        allCollarSprites = Resources.LoadAll<Sprite>("collar");
        allTorsoSprites = Resources.LoadAll<Sprite>("torso");
        allWaistSprites = Resources.LoadAll<Sprite>("waist");
        allWaistShortsSprites = Resources.LoadAll<Sprite>("waistShorts");
        allKneesShinsSprites = Resources.LoadAll<Sprite>("kneesShins");
        allAnklesSprites = Resources.LoadAll<Sprite>("ankles");
        allFeetSprites = Resources.LoadAll<Sprite>("feet");
        allJakettoSprites = Resources.LoadAll<Sprite>("jaketto");
        allDressSprites = Resources.LoadAll<Sprite>("dress");
        allLongSleeveSprites = Resources.LoadAll<Sprite>("longSleeve");
        allHandSprites = Resources.LoadAll<Sprite>("hands");
        allShortSleeveSprites = Resources.LoadAll<Sprite>("shortSleeve");
        allHat1TopSprites = Resources.LoadAll<Sprite>("waterOnHead");
        allMohawk5TopSprites = Resources.LoadAll<Sprite>("mohawk5Top");
        allMohawk5BottomSprites = Resources.LoadAll<Sprite>("mohawk5Bottom");
        allHair0TopSprites = Resources.LoadAll<Sprite>("hair0Top");
        allHair0BottomSprites = Resources.LoadAll<Sprite>("hair0Bottom");
        allHair1TopSprites = Resources.LoadAll<Sprite>("hair1Top");
        allHair7TopSprites = Resources.LoadAll<Sprite>("hair7Top");
        allHair8TopSprites = Resources.LoadAll<Sprite>("hair8Top");
        allHair1BottomSprites = Resources.LoadAll<Sprite>("hair1Bottom");
        allHair2BottomSprites = Resources.LoadAll<Sprite>("hair2Bottom");
        allHair3BottomSprites = Resources.LoadAll<Sprite>("hair3Bottom");
        allHair4BottomSprites = Resources.LoadAll<Sprite>("hair4Bottom");
        allHair6BottomSprites = Resources.LoadAll<Sprite>("hair6Bottom");
        allHair7BottomSprites = Resources.LoadAll<Sprite>("hair7Bottom");
        allHair8BottomSprites = Resources.LoadAll<Sprite>("hair8Bottom");
        allHairFringe1Sprites = Resources.LoadAll<Sprite>("hairFringe1");
        allHairFringe2Sprites = Resources.LoadAll<Sprite>("hairFringe2");
        allBikeSprites = Resources.LoadAll<Sprite>("bike");
        allWheelchairSprites = Resources.LoadAll<Sprite>("wheelChair");

        headSprite = transform.Find("xFlip/headParent/head").GetComponent<SpriteRenderer>();
        eyeSprite = transform.Find("xFlip/headParent/eyes").GetComponent<SpriteRenderer>();
        throatSprite = transform.Find("xFlip/throat").GetComponent<SpriteRenderer>();
        collarSprite = transform.Find("xFlip/collar").GetComponent<SpriteRenderer>();
        torsoSprite = transform.Find("xFlip/torso").GetComponent<SpriteRenderer>();
        waistSprite = transform.Find("xFlip/waist").GetComponent<SpriteRenderer>();
        waistShortsSprite = transform.Find("xFlip/waistShorts").GetComponent<SpriteRenderer>();
        kneesShinsSprite = transform.Find("xFlip/kneesShins").GetComponent<SpriteRenderer>();
        anklesSprite = transform.Find("xFlip/ankles").GetComponent<SpriteRenderer>();
        feetSprite = transform.Find("xFlip/feet").GetComponent<SpriteRenderer>();
        jakettoSprite = transform.Find("xFlip/jaketto").GetComponent<SpriteRenderer>();
        dressSprite = transform.Find("xFlip/dress").GetComponent<SpriteRenderer>();
        longSleeveSprite = transform.Find("xFlip/longSleeve").GetComponent<SpriteRenderer>();
        handSprite = transform.Find("xFlip/hands").GetComponent<SpriteRenderer>();
        shortSleeveSprite = transform.Find("xFlip/shortSleeve").GetComponent<SpriteRenderer>();
        hat1TopSprite = transform.Find("xFlip/headParent/hair").transform.Find("hat1Top").GetComponent<SpriteRenderer>();
        mohawk5TopSprite = transform.Find("xFlip/headParent/hair").transform.Find("mohawk5Top").GetComponent<SpriteRenderer>();
        mohawk5BottomSprite = transform.Find("xFlip/headParent/hair").transform.Find("mohawk5Bottom").GetComponent<SpriteRenderer>();
        hair0TopSprite = transform.Find("xFlip/headParent/hair").transform.Find("hair0Top").GetComponent<SpriteRenderer>();
        hair0BottomSprite = transform.Find("xFlip/headParent/hair").transform.Find("hair0Bottom").GetComponent<SpriteRenderer>();
        hair1TopSprite = transform.Find("xFlip/headParent/hair").transform.Find("hair1Top").GetComponent<SpriteRenderer>();
        hair7TopSprite = transform.Find("xFlip/headParent/hair").transform.Find("hair7Top").GetComponent<SpriteRenderer>();
        hair8TopSprite = transform.Find("xFlip/headParent/hair").transform.Find("hair8Top").GetComponent<SpriteRenderer>();
        hair1BottomSprite = transform.Find("xFlip/headParent/hair").transform.Find("hair1Bottom").GetComponent<SpriteRenderer>();
        hair2BottomSprite = transform.Find("xFlip/headParent/hair").transform.Find("hair2Bottom").GetComponent<SpriteRenderer>();
        hair3BottomSprite = transform.Find("xFlip/headParent/hair").transform.Find("hair3Bottom").GetComponent<SpriteRenderer>();
        hair4BottomSprite = transform.Find("xFlip/headParent/hair").transform.Find("hair4Bottom").GetComponent<SpriteRenderer>();
        hair6BottomSprite = transform.Find("xFlip/headParent/hair").transform.Find("hair6Bottom").GetComponent<SpriteRenderer>();
        hair7BottomSprite = transform.Find("xFlip/headParent/hair").transform.Find("hair7Bottom").GetComponent<SpriteRenderer>();
        hair8BottomSprite = transform.Find("xFlip/headParent/hair").transform.Find("hair8Bottom").GetComponent<SpriteRenderer>();
        hairFringe1Sprite = transform.Find("xFlip/headParent/hair").transform.Find("hairFringe1").GetComponent<SpriteRenderer>();
        hairFringe2Sprite = transform.Find("xFlip/headParent/hair").transform.Find("hairFringe2").GetComponent<SpriteRenderer>();
        bikeSprite = transform.Find("xFlip/bike").GetComponent<SpriteRenderer>();
        wheelchairBackSprite = transform.Find("xFlip/wheelchairBack").GetComponent<SpriteRenderer>();
        wheelchairFrontSprite = transform.Find("xFlip/wheelchairFront").GetComponent<SpriteRenderer>();
        wheelchairWheelSprite = transform.Find("xFlip/wheelchairWheel").GetComponent<SpriteRenderer>();


        wheelVehicleTransformAdjustment = GetComponent<WheelVehicleTransformAdjustment>();

        // Find all input fields in the scene
        inputFields = FindObjectsOfType<TMP_InputField>();

        bodyTypeNumber = 5;

        bikeStatic = GameObject.FindGameObjectWithTag("Bike");
        bikeScript = bikeStatic.transform.Find("bikeTrig").GetComponent<BikeScript>();
        
        wheelchairBackSpriteInitialPos = wheelchairBackSprite.transform.localPosition;
        wheelchairFrontSpriteInitialPos = wheelchairFrontSprite.transform.localPosition;
        wheelchairWheelSpriteInitialPos = wheelchairWheelSprite.transform.localPosition;



        // Set the initial state to idle left using the idleLeftIndex
        headSprite.sprite = allHeadSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        eyeSprite.sprite = allEyeSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        throatSprite.sprite = allThroatSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        collarSprite.sprite = allCollarSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        torsoSprite.sprite = allTorsoSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        waistSprite.sprite = allWaistSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        waistShortsSprite.sprite = allWaistShortsSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        kneesShinsSprite.sprite = allKneesShinsSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        anklesSprite.sprite = allAnklesSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        feetSprite.sprite = allFeetSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        jakettoSprite.sprite = allJakettoSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        dressSprite.sprite = allDressSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        longSleeveSprite.sprite = allLongSleeveSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        handSprite.sprite = allHandSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        shortSleeveSprite.sprite = allShortSleeveSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hat1TopSprite.sprite = allHat1TopSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        mohawk5TopSprite.sprite = allMohawk5TopSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        mohawk5BottomSprite.sprite = allMohawk5BottomSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hair0TopSprite.sprite = allHair0TopSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hair0BottomSprite.sprite = allHair0BottomSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hair1TopSprite.sprite = allHair1TopSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hair7TopSprite.sprite = allHair7TopSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hair8TopSprite.sprite = allHair8TopSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hair1BottomSprite.sprite = allHair1BottomSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hair2BottomSprite.sprite = allHair2BottomSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hair3BottomSprite.sprite = allHair3BottomSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hair4BottomSprite.sprite = allHair4BottomSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hair6BottomSprite.sprite = allHair6BottomSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hair7BottomSprite.sprite = allHair7BottomSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hair8BottomSprite.sprite = allHair8BottomSprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hairFringe1Sprite.sprite = allHairFringe1Sprites[bodyTypeNumber * bodyTypeIndexMultiplier];
        hairFringe2Sprite.sprite = allHairFringe2Sprites[bodyTypeNumber * bodyTypeIndexMultiplier];


        currentSkinColor = (HexToColor("#000000"));
        headSprite.color = currentSkinColor;
        eyeSprite.color = (HexToColor("#000000"));
        currentHairColor = HexToColor("#4E342E");
        currentHatColor = (HexToColor("#000000"));
        currentShirtColor = (HexToColor("#000000"));
        currentPantsColor = (HexToColor("#000000"));
        waistSprite.color = currentPantsColor;
        currentShoeColor = (HexToColor("#000000"));
        currentJakettoColor = (HexToColor("#000000"));

        // characterCustomization.UpdateRandom();

        // StopBikeFunction
        characterMovement.StartDeactivateSpaceBar();
        characterMovement.characterOnBike = false;
        currentWheelVehicleColor.a = 0f;
        // bikeSprite.color = currentWheelVehicleColor;
        // wheelchairBackSprite.color.a = currentWheelVehicleColor;
        // wheelchairFrontSprite.color.a = currentWheelVehicleColor;
        // wheelchairWheelSprite.color.a = currentWheelVehicleColor;

        foreach(GameObject obj in characterSpriteList)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            if(((sr.CompareTag("Wheelchair")) && !characterMovement.playerOnWheelchair) || sr.name.Contains("bike"))
            {
                sr.color = new Color(
                            sr.color.r,
                            sr.color.g,
                            sr.color.b,
                            0f
                        );

            // continue;
            }
        }
            
        initialWaistTransformPos = waistSprite.transform.localPosition;
    }
    void LoadChicken()
    {

        currentFrame = 0;
        idle = 0; walk = 1;
        run = walk;

        movementFrameCount = 5;
        movementStartIndex = 0;

        bodyTypeNumber = 1;
        animDirectionAdjustorInt = 0;
        currentAnimationDirection = 0;
        rightDownAnim = 0;
        leftDownAnim = 0;
        rightAnim = 0;
        leftAnim = 0;
        upRightAnim = 0;
        upLeftAnim = 0;
        bodyTypeIndexMultiplier = 0;
    
        //chicken
        chickenFeetSprites = Resources.LoadAll<Sprite>("chickenFeet");


        chickenBodySprite = transform.Find("xFlip").transform.Find("chickenBody").GetComponent<SpriteRenderer>();
        chickenFeetSprite = transform.Find("xFlip").transform.Find("chickenFeet").GetComponent<SpriteRenderer>();
        chickenWingSprite = transform.Find("xFlip").transform.Find("chickenWing").GetComponent<SpriteRenderer>();
        chickenSmallCombSprite = transform.Find("xFlip").transform.Find("chickenHead").transform.Find("smallComb").GetComponent<SpriteRenderer>();
        chickenLargeCombSprite = transform.Find("xFlip").transform.Find("chickenHead").transform.Find("largeComb").GetComponent<SpriteRenderer>();
        chickenHeadSprite = transform.Find("xFlip").transform.Find("chickenHead").transform.Find("head").GetComponent<SpriteRenderer>();
        chickenBeakSprite = transform.Find("xFlip").transform.Find("chickenHead").transform.Find("beak").GetComponent<SpriteRenderer>();
        chickenEyeSprite = transform.Find("xFlip").transform.Find("chickenHead").transform.Find("eye").GetComponent<SpriteRenderer>();

        //for animations
        chickenFeetSprite.sprite = chickenFeetSprites[0];


        int choice = Random.Range(0, 3); // 0, 1, or 2 (equal chance)

        if(chickenWingSprite != null)
        {
            SetAlpha(chickenWingSprite, 0f);
        }

        switch (choice)
        {
            case 0: // small comb only
                SetAlpha(chickenLargeCombSprite, 0f);
                SetAlpha(chickenSmallCombSprite, 1f);
                break;

            case 1: // medium (both visible)
                SetAlpha(chickenLargeCombSprite, 1f);
                SetAlpha(chickenSmallCombSprite, 1f);
                break;

            case 2: // large comb (big + stretched small)
                SetAlpha(chickenLargeCombSprite, 1f);
                SetAlpha(chickenSmallCombSprite, 1f);

                Vector3 scale = chickenSmallCombSprite.transform.localScale;
                scale.y = 2f;
                chickenSmallCombSprite.transform.localScale = scale;
                break;
        }

    }
    void LoadPony()
    {

        

        // //pony
        ponyEyesSprites = Resources.LoadAll<Sprite>("ponyEyes");
        ponyFaceSprites = Resources.LoadAll<Sprite>("ponyFace");
        ponyShadowSprites = Resources.LoadAll<Sprite>("ponyShadow");
        ponyHairSprites = Resources.LoadAll<Sprite>("ponyHair");
        ponyFeetSprites = Resources.LoadAll<Sprite>("ponyFeet");


        ponyEyesSprite = transform.Find("xFlip").transform.Find("ponyEyes").GetComponent<SpriteRenderer>();
        ponyFaceSprite = transform.Find("xFlip").transform.Find("ponyFace").GetComponent<SpriteRenderer>();
        ponyShadowSprite = transform.Find("xFlip").transform.Find("ponyShadow").GetComponent<SpriteRenderer>();
        ponyHairSprite = transform.Find("xFlip").transform.Find("ponyHair").GetComponent<SpriteRenderer>();
        ponyFeetSprite = transform.Find("xFlip").transform.Find("ponyFeet").GetComponent<SpriteRenderer>();

    
        ponyEyesSprite.sprite = ponyEyesSprites[0];
        ponyFaceSprite.sprite = ponyFaceSprites[0];
        ponyShadowSprite.sprite = ponyShadowSprites[0];
        ponyHairSprite.sprite = ponyHairSprites[0];
        ponyFeetSprite.sprite = ponyFeetSprites[0];

        currentFrame = 0;
        idle = characterMovement.facingUp ? 1 : 0; walk = 4; run = 26;

        bodyTypeNumber = 1;
        animDirectionAdjustorInt = 0;
        currentAnimationDirection = 0;
        rightDownAnim = 0;
        leftDownAnim = 0;
        rightAnim = 0;
        leftAnim = 0;
        upRightAnim = 0;
        upLeftAnim = 0;
        bodyTypeIndexMultiplier = 0;
    }
    void LoadPig()
    {
        currentFrame = 0;
        idle = characterMovement.facingUp ? 1 : 0; walk = characterMovement.facingUp ? 12 : 8;
        run = characterMovement.facingUp ? 12 : 8;

        bodyTypeNumber = 1;
        animDirectionAdjustorInt = 0;
        currentAnimationDirection = 0;
        rightDownAnim = 0;
        leftDownAnim = 0;
        rightAnim = 0;
        leftAnim = 0;
        upRightAnim = 0;
        upLeftAnim = 0;
        bodyTypeIndexMultiplier = 0;

        // //pig
        pigEyesSprites = Resources.LoadAll<Sprite>("pigEyes");
        pigFaceSprites = Resources.LoadAll<Sprite>("pigFace");
        pigNoseSprites = Resources.LoadAll<Sprite>("pigNose");
        pigBodySprites = Resources.LoadAll<Sprite>("pigBody");
        pigFeetSprites = Resources.LoadAll<Sprite>("pigFeet");

        pigEyesSprite = transform.Find("xFlip").transform.Find("pigEyes").GetComponent<SpriteRenderer>();
        pigFaceSprite = transform.Find("xFlip").transform.Find("pigFace").GetComponent<SpriteRenderer>();
        pigNoseSprite = transform.Find("xFlip").transform.Find("pigNose").GetComponent<SpriteRenderer>();
        pigBodySprite = transform.Find("xFlip").transform.Find("pigBody").GetComponent<SpriteRenderer>();
        pigFeetSprite = transform.Find("xFlip").transform.Find("pigFeet").GetComponent<SpriteRenderer>();
        
        pigEyesSprite.sprite = pigEyesSprites[0];
        pigFaceSprite.sprite = pigFaceSprites[0];
        pigNoseSprite.sprite = pigNoseSprites[0];
        pigBodySprite.sprite = pigBodySprites[0];
        pigFeetSprite.sprite = pigFeetSprites[0];
    }
    void LoadPuppy()
    {
        currentFrame = 0;
        idle = characterMovement.facingUp ? 1 : 0; walk = characterMovement.facingUp ? 12 : 8;
        run = characterMovement.facingUp ? 12 : 8;

        bodyTypeNumber = 1;
        animDirectionAdjustorInt = 0;
        currentAnimationDirection = 0;
        rightDownAnim = 0;
        leftDownAnim = 0;
        rightAnim = 0;
        leftAnim = 0;
        upRightAnim = 0;
        upLeftAnim = 0;
        bodyTypeIndexMultiplier = 0;
        // //puppy
        puppyEyesSprites = Resources.LoadAll<Sprite>("dogEyes");
        puppyNoseSprites = Resources.LoadAll<Sprite>("dogNose");
        puppyMain1Sprites = Resources.LoadAll<Sprite>("dogMain1");
        puppyMain2Sprites = Resources.LoadAll<Sprite>("dogMain2");
        puppyFeetSprites = Resources.LoadAll<Sprite>("dogFeet");


        puppyEyesSprite = transform.Find("xFlip").transform.Find("dogEyes").GetComponent<SpriteRenderer>();
        puppyNoseSprite = transform.Find("xFlip").transform.Find("dogNose").GetComponent<SpriteRenderer>();
        puppyMain1Sprite = transform.Find("xFlip").transform.Find("dogMain1").GetComponent<SpriteRenderer>();
        puppyMain2Sprite = transform.Find("xFlip").transform.Find("dogMain2").GetComponent<SpriteRenderer>();
        puppyFeetSprite = transform.Find("xFlip").transform.Find("dogFeet").GetComponent<SpriteRenderer>();

        puppyEyesSprite.sprite = puppyEyesSprites[0];
        puppyNoseSprite.sprite = puppyNoseSprites[0];
        puppyMain1Sprite.sprite = puppyMain1Sprites[0];
        puppyMain2Sprite.sprite = puppyMain2Sprites[0];
        puppyFeetSprite.sprite = puppyFeetSprites[0];
      
        
    }


    public void Animate(int movementStartIndex, int movementFrameCount, int animationDirection, int bodyTypeNumber)
    {
    //     Debug.Log(
    //     $"ANIMATE | character={characterType} | " +
    //     $"start={movementStartIndex} | " +
    //     $"count={movementFrameCount} | " +
    //     $"direction={animationDirection} | " +
    //     $"body={bodyTypeNumber}"
    // );

        if(characterMovement.characterOnBike)
        {
            if(bikeSprite != null)
            bikeSprite.color = currentWheelVehicleColor;
        }
        if(characterMovement.playerOnWheelchair)
        {
            if(wheelchairBackSprite != null)
            wheelchairBackSprite.color = currentWheelVehicleColor;
            if(wheelchairFrontSprite != null)
            wheelchairFrontSprite.color = currentWheelVehicleColor;
            if(wheelchairWheelSprite != null)
            wheelchairWheelSprite.color = currentWheelVehicleColor;
        }
        
        if (this.gameObject.tag == "Player")
        {
            if (characterType == CharacterType.person && characterMovement.characterOnBike == true)
            {
                RideBike();
            }
            else if (characterType == CharacterType.person && characterMovement.playerOnWheelchair == true)
            {
                RideWheelchair();
            }
            else if (characterType == CharacterType.person && (characterMovement.motionDirection == "upDownLadder" || characterMovement.motionDirection == "upLadder" || characterMovement.motionDirection == "downLadder"))
            {
                ClimbLadder();
            }
            else if(runningAnim)
            {
                Run();
            }
            else
            {
                Walk();
            }

            HandleGettingOffBikeOrSeat();

        }
        else if (this.gameObject.tag == "NPC")
        {
            if (characterType == CharacterType.person && characterMovement.characterOnBike == true)
            {
                RideBike();
            }
            else if (characterType == CharacterType.person && characterMovement.playerOnWheelchair == true)
            {
                RideWheelchair();
            }
            else if (characterType == CharacterType.person && (characterMovement.motionDirection == "upDownLadder" || characterMovement.motionDirection == "upLadder" || characterMovement.motionDirection == "downLadder"))
            {
                ClimbLadder();
            }
            else if (runningAnim && characterMovement.change != Vector3.zero)
            {
                Run();
            }
            else
            {
                Walk();
            }
        }

        // Only idle-hop when not moving, or climbing
        if (characterMovement.change == Vector3.zero)
        {
            // Idle(movementStartIndex, animationDirection);
            switch (characterType)
            {
                case CharacterType.person:
                    bool isIdleLeft = animationDirection == leftAnim || animationDirection == leftDownAnim;
                    bool isIdleRight = animationDirection == rightAnim || animationDirection == rightDownAnim;

                    // if (isIdleLeft || isIdleRight)
                    // {
                    //     idleHopTimer -= Time.deltaTime;

                    //     if (idleHopTimer <= 0f)
                    //     {
                    //         if (isIdleLeft)
                    //             animationDirection = (animationDirection == leftAnim) ? leftDownAnim : leftAnim;
                    //         else if (isIdleRight)
                    //             animationDirection = (animationDirection == rightAnim) ? rightDownAnim : rightAnim;

                    //         currentAnimationDirection = animationDirection;

                    //         idleHopTimer = Random.Range(Random.Range(0.5f, 6f), 15f);
                    //     }
                    // }
                    // else
                    // {
                    //     idleHopTimer = 0f;
                    // }
                    break;

                case CharacterType.chicken:
                    idleHopTimer -= Time.deltaTime;
                    bodyPartMoveTimer -= Time.deltaTime;

                    if (idleHopTimer <= 0f)
                    {
                        isFacingLeft = !isFacingLeft;
                        UpdateFlip();
                        idleHopTimer = Random.Range(Random.Range(0.5f, 6f), 15f);
                    }

                    if (bodyPartToAnimate != null)
                    {
                        if (bodyPartMoveTimer <= 0f)
                        {
                            if (bodyPartMoveCoro == null)
                                bodyPartMoveCoro = StartCoroutine(MoveBodyPartAnimation());

                            bodyPartMoveTimer = Random.Range(Random.Range(0.5f, 5f), 10f);
                            bodyPartMoveCoro = null;
                        }
                    }
                    break;

                case CharacterType.pony:

                    idleHopTimer -= Time.deltaTime;

                    if (idleHopTimer <= 0f)
                    {
                        isFacingLeft = !isFacingLeft;
                        UpdateFlip();
                        idleHopTimer = Random.Range(Random.Range(0.5f, 6f), 15f);
                    }
                    break;


                case CharacterType.pig:

                    movementStartIndex = characterMovement.facingUp ? 1 : 0;

                    idleHopTimer -= Time.deltaTime;

                    if (idleHopTimer <= 0f)
                    {
                        isFacingLeft = !isFacingLeft;
                        UpdateFlip();
                        idleHopTimer = Random.Range(Random.Range(0.5f, 6f), 15f);
                    }
                    break;

                case CharacterType.puppy:

                    movementStartIndex = characterMovement.facingUp ? 1 : 0;

                    idleHopTimer -= Time.deltaTime;

                    if (idleHopTimer <= 0f)
                    {
                        isFacingLeft = !isFacingLeft;
                        UpdateFlip();
                        idleHopTimer = Random.Range(Random.Range(0.5f, 6f), 15f);
                    }

                    break;
            }
        }

        bodyTypeIndexMultiplier = characterType != CharacterType.person ? 0 : bodyTypeIndexMultiplier;

        movementIndices = Enumerable.Range(
            movementStartIndex + animationDirection +
            ((bodyTypeNumber - 1) * bodyTypeIndexMultiplier),
            movementFrameCount
        ).ToArray();


        // Timer to control the animation frame rate
        timer += Time.deltaTime;

        // If enough time has passed, move to the next frame
        if (timer >= animationSpeed)
        {
            timer = 0f; // Reset timer

            // Update the current frame
            if (characterMovement.change != Vector3.zero)
            {
                currentFrame++;
            }

            // If we've reached the end of the movementIndices array, loop back to the first sprite
            if (currentFrame >= movementIndices.Length)
            {
                currentFrame = 0;
            }
            switch (characterType)
            {
                case CharacterType.person:
                    // Set the sprite to the current frame in the movementIndices array
                    headSprite.sprite = allHeadSprites[movementIndices[currentFrame]];
                    eyeSprite.sprite = allEyeSprites[movementIndices[currentFrame]];
                    throatSprite.sprite = allThroatSprites[movementIndices[currentFrame]];
                    collarSprite.sprite = allCollarSprites[movementIndices[currentFrame]];
                    torsoSprite.sprite = allTorsoSprites[movementIndices[currentFrame]];
                    waistSprite.sprite = allWaistSprites[movementIndices[currentFrame]];
                    waistShortsSprite.sprite = allWaistShortsSprites[movementIndices[currentFrame]];
                    kneesShinsSprite.sprite = allKneesShinsSprites[movementIndices[currentFrame]];
                    anklesSprite.sprite = allAnklesSprites[movementIndices[currentFrame]];
                    feetSprite.sprite = allFeetSprites[movementIndices[currentFrame]];
                    jakettoSprite.sprite = allJakettoSprites[movementIndices[currentFrame]];
                    dressSprite.sprite = allDressSprites[movementIndices[currentFrame]];
                    longSleeveSprite.sprite = allLongSleeveSprites[movementIndices[currentFrame]];
                    handSprite.sprite = allHandSprites[movementIndices[currentFrame]];
                    shortSleeveSprite.sprite = allShortSleeveSprites[movementIndices[currentFrame]];
                    hat1TopSprite.sprite = allHat1TopSprites[movementIndices[currentFrame]];
                    mohawk5TopSprite.sprite = allMohawk5TopSprites[movementIndices[currentFrame]];
                    mohawk5BottomSprite.sprite = allMohawk5BottomSprites[movementIndices[currentFrame]];
                    hair0TopSprite.sprite = allHair0TopSprites[movementIndices[currentFrame]];
                    hair0BottomSprite.sprite = allHair0BottomSprites[movementIndices[currentFrame]];
                    hair1TopSprite.sprite = allHair1TopSprites[movementIndices[currentFrame]];
                    hair7TopSprite.sprite = allHair7TopSprites[movementIndices[currentFrame]];
                    hair8TopSprite.sprite = allHair8TopSprites[movementIndices[currentFrame]];
                    hair1BottomSprite.sprite = allHair1BottomSprites[movementIndices[currentFrame]];
                    hair2BottomSprite.sprite = allHair2BottomSprites[movementIndices[currentFrame]];
                    hair3BottomSprite.sprite = allHair3BottomSprites[movementIndices[currentFrame]];
                    hair4BottomSprite.sprite = allHair4BottomSprites[movementIndices[currentFrame]];
                    hair6BottomSprite.sprite = allHair6BottomSprites[movementIndices[currentFrame]];
                    hair7BottomSprite.sprite = allHair7BottomSprites[movementIndices[currentFrame]];
                    hair8BottomSprite.sprite = allHair8BottomSprites[movementIndices[currentFrame]];
                    hairFringe1Sprite.sprite = allHairFringe1Sprites[movementIndices[currentFrame]];
                    hairFringe2Sprite.sprite = allHairFringe2Sprites[movementIndices[currentFrame]];
                    break;

                case CharacterType.chicken:
                    chickenFeetSprite.sprite = chickenFeetSprites[movementIndices[currentFrame]];
                    break;

                case CharacterType.pony:
                    ponyEyesSprite.sprite = ponyEyesSprites[movementIndices[currentFrame]];
                    ponyFaceSprite.sprite = ponyFaceSprites[movementIndices[currentFrame]];
                    ponyShadowSprite.sprite = ponyShadowSprites[movementIndices[currentFrame]];
                    ponyHairSprite.sprite = ponyHairSprites[movementIndices[currentFrame]];
                    ponyFeetSprite.sprite = ponyFeetSprites[movementIndices[currentFrame]];
                    break;

                case CharacterType.pig:
                    pigEyesSprite.sprite = pigEyesSprites[movementIndices[currentFrame]];
                    pigFaceSprite.sprite = pigFaceSprites[movementIndices[currentFrame]];
                    pigNoseSprite.sprite = pigNoseSprites[movementIndices[currentFrame]];
                    pigBodySprite.sprite = pigBodySprites[movementIndices[currentFrame]];
                    pigFeetSprite.sprite = pigFeetSprites[movementIndices[currentFrame]];
                    break;

                case CharacterType.puppy:
                    puppyEyesSprite.sprite = puppyEyesSprites[movementIndices[currentFrame]];
                    puppyNoseSprite.sprite = puppyNoseSprites[movementIndices[currentFrame]];
                    puppyMain1Sprite.sprite = puppyMain1Sprites[movementIndices[currentFrame]];
                    puppyMain2Sprite.sprite = puppyMain2Sprites[movementIndices[currentFrame]];
                    puppyFeetSprite.sprite = puppyFeetSprites[movementIndices[currentFrame]];
                    break;

                    // case CharacterType.Cat:
                    //     sprites = Resources.LoadAll<Sprite>("head_cat");
                    //     break;
            }

        }
    }

        public void RideBike()
    {
        wheelVehicleTransformAdjustment.SetBikeTransformPosition();

        currentWheelVehicleColor.a = 1f;
        movementStartIndex = rideBike;
        movementFrameCount = 4;
        //characterMovement.movementSpeed = 80;
        animationSpeed = 0.08f;
        if (characterMovement.change.x > 0)
        {
            bikeSprite.flipX = true;
        }
        else if (characterMovement.change.x < 0)
        {
            bikeSprite.flipX = false;
        }

        if (characterMovement.change.y > 0)
        {
            bikeSprite.sprite = allBikeSprites[0];
        }
        else if (characterMovement.change.y < 0)
        {
            bikeSprite.sprite = allBikeSprites[1];
        }
    }

    bool wasMovingUp;
    public void RideWheelchair()
    {
        // transform.SetAsFirstSibling(); // moves to top in hierarchy
        // transform.SetAsLastSibling();  // moves to bottom in hierarchy
        currentWheelVehicleColor.a = 1f;
        movementStartIndex = rideWheelchair;
        movementFrameCount = 1;
        //characterMovement.movementSpeed = 80;
        animationSpeed = 0.08f;
        if (
            (bodyTypeNumber >= 1 && bodyTypeNumber <= 4) ||
            (bodyTypeNumber >= 13 && bodyTypeNumber <= 16) ||
            (bodyTypeNumber >= 25 && bodyTypeNumber <= 28)
        )
        {
            wheelchairWheelSprite.sprite = allWheelchairSprites[2];
            wheelchairBackSprite.sprite = allWheelchairSprites[3];
            wheelchairFrontSprite.sprite = allWheelchairSprites[3];
            // wheelchairWheelFlipOffset = new Vector3(10,0,0);
        }
        else
        {
            wheelchairWheelSprite.sprite = allWheelchairSprites[0];
            wheelchairBackSprite.sprite = allWheelchairSprites[1];
            wheelchairFrontSprite.sprite = allWheelchairSprites[1];
            // wheelchairWheelFlipOffset = new Vector3(10,0,0);
        }

        if (characterMovement.change.x > 0)
        {
            wheelchairBackSprite.flipX = false;
            wheelchairFrontSprite.flipX = false;
            wheelchairWheelSprite.flipX = false;

            wheelchairBackSprite.transform.localPosition = wheelchairBackSpriteInitialPos;
            wheelchairFrontSprite.transform.localPosition = wheelchairFrontSpriteInitialPos;
            wheelchairWheelSprite.transform.localPosition = wheelchairWheelSpriteInitialPos;      
        }
        else if (characterMovement.change.x < 0)
        {
            wheelchairBackSprite.flipX = true;
            wheelchairFrontSprite.flipX = true;
            wheelchairWheelSprite.flipX = true;
            
            wheelchairBackSprite.transform.localPosition = wheelchairBackSpriteInitialPos + wheelchairFlipOffset;
            wheelchairFrontSprite.transform.localPosition = wheelchairFrontSpriteInitialPos + wheelchairFlipOffset;        
            wheelchairWheelSprite.transform.localPosition = wheelchairWheelSpriteInitialPos + wheelchairWheelFlipOffset;        
        }

            SetAlpha(wheelchairBackSprite, characterMovement.facingUp ? 0f : 1f);
            SetAlpha(wheelchairFrontSprite, characterMovement.facingUp ? 1f : 0f);
            SetAlpha(wheelchairWheelSprite, characterMovement.facingUp ? 0f : 1f);
    }

    // public void Idle(int movementStartIndex, int animationDirection)
    // {
        

    // }
    public void Sit()
    {
        movementStartIndex = sit;
        movementFrameCount = 1;
    }

    public void ClimbLadder()
    {
        movementStartIndex = climb;
        movementFrameCount = 4;
        animationSpeed = initialAnimationSpeed;
        currentWheelVehicleColor.a = 0f;
    }

    public void Run()
    {
        switch (characterType)
        {
            case CharacterType.person:
                movementStartIndex = run;
                movementFrameCount = 4;
                //characterMovement.movementSpeed = 60;
                animationSpeed = 0.10f;
                currentWheelVehicleColor.a = 0f;
                break;

            case CharacterType.chicken:
                if (characterMovement.facingLeft != isFacingLeft)
                {
                    isFacingLeft = characterMovement.facingLeft;
                    UpdateFlip();
                }
                movementFrameCount = 5;
                movementStartIndex = 0;

                animationSpeed = 0.10f;
                break;

            case CharacterType.pony:
                if (characterMovement.facingLeft != isFacingLeft)
                {
                    isFacingLeft = characterMovement.facingLeft;
                    UpdateFlip();
                }
                movementFrameCount = 4;
                movementStartIndex = characterMovement.facingUp ? 30 : 26;
                //characterMovement.movementSpeed = 60;
                animationSpeed = 0.10f;
                break;

            case CharacterType.pig:
                if (characterMovement.facingLeft != isFacingLeft)
                {
                    isFacingLeft = characterMovement.facingLeft;
                    UpdateFlip();
                }
                movementFrameCount = 4;
                movementStartIndex = characterMovement.facingUp ? 12 : 8;
                //characterMovement.movementSpeed = 60;
                animationSpeed = 0.10f;
                break;

            case CharacterType.puppy:
                if (characterMovement.facingLeft != isFacingLeft)
                {
                    isFacingLeft = characterMovement.facingLeft;
                    UpdateFlip();
                }
                movementFrameCount = 4;
                movementStartIndex = characterMovement.facingUp ? 12 : 8;
                //characterMovement.movementSpeed = 60;
                animationSpeed = 0.10f;
                break;
        }
    }

    public void Walk()
    {
        switch (characterType)
        {
            case CharacterType.person:
                movementFrameCount = 4;
                movementStartIndex = walk;
                break;

            case CharacterType.chicken:
                if (characterMovement.facingLeft != isFacingLeft)
                {
                    isFacingLeft = characterMovement.facingLeft;
                    UpdateFlip();
                }
                movementFrameCount = 5;
                movementStartIndex = 0;

                break;

            case CharacterType.pony:
                if (characterMovement.facingLeft != isFacingLeft)
                {
                    isFacingLeft = characterMovement.facingLeft;
                    UpdateFlip();
                }
                movementFrameCount = 11;
                movementStartIndex = characterMovement.facingUp ? 15 : 4;
                break;

            case CharacterType.pig:
                if (characterMovement.facingLeft != isFacingLeft)
                {
                    isFacingLeft = characterMovement.facingLeft;
                    UpdateFlip();
                }
                movementFrameCount = 4;
                movementStartIndex = characterMovement.facingUp ? 12 : 8;
                break;

            case CharacterType.puppy:
                if (characterMovement.facingLeft != isFacingLeft)
                {
                    isFacingLeft = characterMovement.facingLeft;
                    UpdateFlip();
                }
                movementFrameCount = 4;
                movementStartIndex = characterMovement.facingUp ? 12 : 8;
                break;
        }
    }

    public void HandleGettingOffBikeOrSeat()
    {
        // Check if space was released before this code
        if (characterMovement.spaceBarDeactivated == false)
        {
            if (Input.GetKey(KeyCode.Space) ||
        Input.GetKey(KeyCode.JoystickButton0) ||  // A button
        Input.GetKey(KeyCode.JoystickButton1) ||  // B button
        Input.GetKey(KeyCode.JoystickButton2)  // X button
                                               // Input.GetKey(KeyCode.JoystickButton3)
        )
            {
                if (characterMovement.characterOnBike)
                {
                    // StopBikeFunction
                    characterMovement.StartDeactivateSpaceBar();
                    characterMovement.characterOnBike = false;
                    bikeScript.GetOffBoike();
                    bikeStatic.transform.position = transform.position + new Vector3(8, -22, 0);
                    currentWheelVehicleColor.a = 0f;
                }
                else if (characterMovement.playerOnFurniture)
                {
                    if (currentFurnitureScript != null)
                    {
                        CheckFurnitureType();
                        characterMovement.StartDeactivateSpaceBar();

                        currentFurnitureScript.StopEngaging();
                    }

                }
            }
        }

        //StopBikeFunctionWhile Entering Building 
        if (characterMovement.characterOnThresh && characterMovement.characterOnBike == true)
        {
            if (characterMovement.change.x > 0 && characterMovement.change.y > 0)//up right
            {
                bikeStatic.transform.position = transform.position + new Vector3(8, -22, 0) + new Vector3(-16, -8, 0);
            }
            if (characterMovement.change.x < 0 && characterMovement.change.y < 0)//down left
            {
                bikeStatic.transform.position = transform.position + new Vector3(8, -22, 0) + new Vector3(16, 8, 0);
            }
            if (characterMovement.change.x < 0 && characterMovement.change.y > 0)//up left
            {
                bikeStatic.transform.position = transform.position + new Vector3(8, -22, 0) + new Vector3(16, -8, 0);
            }
            if (characterMovement.change.x > 0 && characterMovement.change.y < 0)// down right
            {
                bikeStatic.transform.position = transform.position + new Vector3(8, -22, 0) + new Vector3(-16, 8, 0);
            }

            characterMovement.characterOnBike = false;
            bikeScript.GetOffBoike();
            currentWheelVehicleColor.a = 0f;

            characterMovement.activeCollisions.Clear();
        }

    }


    Color HexToColor(string hex)
    {
        Color newCol;
        if (ColorUtility.TryParseHtmlString(hex, out newCol))
        {
            return newCol;
        }
        else
        {
            Debug.LogError("Invalid hex color string: " + hex);
            return Color.black; // Return black if conversion fails
        }
    }


    public static void SetTreeSortingLayer(GameObject gameObject, string sortingLayerName)
    {
        if (gameObject.GetComponent<SpriteRenderer>() != null)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
        }
        foreach (Transform child in gameObject.transform)
        {
            CharacterAnimation.SetTreeSortingLayer(child.gameObject, sortingLayerName);
        }
    }

    public void GetSpritesAndAddToLists(GameObject obj, List<GameObject> spriteList, List<GameObject> excludeList, List<Color> colorList, List<Vector3> transformList)
    {
        // Clear the lists before repopulating
        spriteList.Clear();
        colorList.Clear();

        Stack<GameObject> stack = new Stack<GameObject>();
        stack.Push(obj);

        while (stack.Count > 0)
        {
            GameObject currentNode = stack.Pop();
            SpriteRenderer sr = currentNode.GetComponent<SpriteRenderer>();
            Transform tr = currentNode.GetComponent<Transform>();

            if (sr != null)
            {
                Color col = sr.color;
                spriteList.Add(currentNode);
                colorList.Add(col);
                transformList.Add(tr.localPosition);
            }

            foreach (Transform child in currentNode.transform)
            {
                if (!excludeList.Contains(child.gameObject))
                {
                    stack.Push(child.gameObject);
                }
            }
        }
    }

    public void initializeCharacterSprites()
    {
        for (int i = 0; i < characterSpriteList.Count; i++)
        {
            characterSpriteList[i].GetComponent<SpriteRenderer>().color = initialChrctrColorList[i];
        }
    }


    void SetAlpha(SpriteRenderer sr, float alpha)
    {
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }


    public void SetTreeAlpha(GameObject treeNode, float alpha)
    {
        if (treeNode == null)
        {
            return; // TODO: remove this
        }
        else if (treeNode != transform.Find("bike"))
        {
            SpriteRenderer sr = treeNode.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            }
            foreach (Transform child in treeNode.transform)
            {
                SetTreeAlpha(child.gameObject, alpha);
            }
        }
    }

    void CheckFurnitureType()
    {
        switch (currentFurnitureScript.currentFurnitureType)
        {
            case FurnitureScript.FurnitureType.chair:
                characterCustomization.UpdatePants();
                characterCustomization.UpdateWaist();
                break;

            case FurnitureScript.FurnitureType.toilet:
                characterCustomization.UpdatePants();
                characterCustomization.UpdateWaist();
                break;

            case FurnitureScript.FurnitureType.bed:
                currentFurnitureScript.HandlePlayerAlphaEngagement(false);
                currentFurnitureScript.SetBedCoverAlpha(false);
                // Add your bed-specific logic here
                break;

            default:
                Debug.Log("Unknown furniture type.");
                break;
        }
    }


    

    public void UpdateFlip()
    {
        float x = isFacingLeft ? -1 : 1;
        float xOffset = isFacingLeft ? initialState.position.x + xFlipOffset : initialState.position.x - xFlipOffset;

        visualRoot.localScale = new Vector3(
          x,
          visualRoot.localScale.y,
          visualRoot.localScale.z
        );
        visualRoot.position = new Vector3(
          xOffset,
          visualRoot.position.y,
          visualRoot.position.z
        );
    }

    IEnumerator MoveBodyPartAnimation()
    {
        float t = 0.1f;
        Transform part = bodyPartToAnimate.transform;

        Vector3 startPos = part.localPosition;

        Vector3[][] patterns = new Vector3[][]
        {
        new [] { Vector3.right, Vector3.left },
        new [] { Vector3.left, Vector3.right },
        new [] { Vector3.down, Vector3.up },
        new [] { Vector3.up, Vector3.down },
        new [] { Vector3.right, Vector3.down, Vector3.left, Vector3.up },
        new [] { Vector3.down, Vector3.right, Vector3.up, Vector3.left }
        };

        var pattern = patterns[Random.Range(0, patterns.Length)];

        try
        {
            foreach (var dir in pattern)
            {
                yield return new WaitForSeconds(t);
                part.localPosition += dir;
            }
        }
        finally
        {
            // always snap back to original
            part.localPosition = startPos;
        }
    }
}



























