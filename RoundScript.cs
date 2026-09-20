using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CharacterType
{
    pig,
    person,
    pony,
    puppy,
    chicken
}


public class RoundScript : MonoBehaviour
{
    [Header("Characters")]
    // [SerializeField] GameObject pony;
    // [SerializeField] GameObject pig;
    // [SerializeField] GameObject person;
    // [SerializeField] GameObject puppy;
    // [SerializeField] GameObject chicken;
    // [SerializeField] GameObject character4;
    // [SerializeField] GameObject character5;

    [SerializeField] PlatformTrackTrainScript platformTrackTrainScript;

    public CameraMovement cam;
    public GameObject mainCam;

    [SerializeField] GameObject startButton;
    [SerializeField] GameObject okButton;
    GameObject previousCharacter = null;

    [HideInInspector] public bool gameHasStarted = false;
    [HideInInspector] public bool noControls = true;
    
    [HideInInspector] public bool inTransit = false;
    [HideInInspector] public bool playerSettled = false;
    [HideInInspector] public bool aboutToDepart = false;
    [HideInInspector] public bool updateMute = true;
    // [HideInInspector] public bool updateZoom = true;
    [HideInInspector] public bool updateWon = true;
    [HideInInspector] public bool updateRouletteStart = false;
    // [HideInInspector] public bool updateRoundStart = true;
    [HideInInspector] public bool dynamicMovement = false;
    [HideInInspector] public bool playersDontMove = true;

    public GameObject pig;
    public GameObject pony;
    public GameObject person;
    public GameObject puppy;
    public GameObject chicken;


    public GameObject player;
    public GameObject roulettePlayer;


    public CharacterType playerType;
    public CharacterType npc1Type;
    public CharacterType npc2Type;

    public List<GameObject> allCharacters = new List<GameObject>();
    public List<GameObject> npcs = new List<GameObject>();

    public Vector3 initialPositionPlayer;
    public Vector3 initialPosition2;
    public Vector3 initialPosition3;
    public Vector3 initialPosition4;
    public Vector3 initialPosition5;

    [Header("UI")]
    public TMP_Text initialText1;
    public TMP_Text initialText2;
    public TMP_Text instructionText1;
    public TMP_Text instructionText2;
    public TMP_Text scoreText;
    public TMP_Text initialTimeText;
    public TMP_Text timeText;


    [Header("Soundtrack")]
    int currentTrack= 0;
    public AudioSource[] musicTracks;

    public Coroutine waitCoro;

    
    

    void Awake()
    {
        allCharacters.Add(pony);
        allCharacters.Add(pig);
        allCharacters.Add(person);
        allCharacters.Add(puppy);
        allCharacters.Add(chicken);

        initialPositionPlayer = new Vector3(0,-32,0);
        initialPosition2 = new Vector3(315,-186,0);
        initialPosition3 = new Vector3(-340,145,0);
        initialPosition4 = new Vector3(315,-186,0);
        initialPosition5 = new Vector3(-340,145,0);
        
        // rouletteCharacters.Add(pony);
        // rouletteCharacters.Add(pig);
        // rouletteCharacters.Add(person);
        // rouletteCharacters.Add(puppy);
        // rouletteCharacters.Add(chicken);


        // foreach (GameObject character in allCharacters)
        // {
        //     CharacterAnimation cAnim = character.GetComponent<CharacterAnimation>();
            
        //     // chance the character will have a runing anim
        //     cAnim.runningAnim = Random.value < 0.5f;

        //     CharacterMovement cMov = character.GetComponent<CharacterMovement>();
           
        //     // chance the character will have a wheelchair
        //     cMov.playerOnWheelchair = Random.value < 0.1f;
            
        //     // random speed
        //     cMov.movementSpeed = cMov.CompareTag("Player") ? 80 : 65;
            
        //     // Make everyone an NPC first
        //     character.tag = "NPC";
        //     character.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        //     character.GetComponentInChildren<BoxCollider2D>().gameObject.tag = "NPCCollider";

        //     npcs.Add(character);
        // }


        // // Pick one of the characters to be the Player
        // player =
        //     allCharacters[Random.Range(0, allCharacters.Count)];

        // player.tag = "Player";

        // player.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        // player.GetComponentInChildren<BoxCollider2D>().gameObject.tag = "PlayerCollider";

        // npcs.Remove(player);
    }

    void Start()
    {       
        initialText1.text = instructionText1.text;
        initialText2.text = instructionText2.text;

        initialTimeText.text = "train chasey";
        timeText.text = "train chasey";
       
        // allCharacters[Random.Range(0, 3)];

        // melody.loop = true;
        // melody.Play();

        // double startTime = AudioSettings.dspTime + 0.2;

        // foreach (AudioSource track in musicTracks)
        // {
        //     track.volume = 0;
        //     track.PlayScheduled(startTime);
        // }

        // musicTracks[0].volume = 1;


        foreach (AudioSource track in musicTracks)
        {
            track.time = 0f;
            track.volume = 0f;
            track.Play();
        }

        musicTracks[currentTrack].volume = 0.8f;

        updateWon = true;
        updateMute = !updateMute;

        // updateRoundStart = false;

        // foreach (GameObject character in allCharacters)
        // {
        //     if(character.CompareTag("NPC"))
        //     {
        //         platformTrackTrainScript.characterList.Add(character);
        //     }

        // }
　
        // RemixAndStartRound();

        // startButton.SetActive(true);
        // okButton.SetActive(false);


        
        
    }

    bool isRemixInitiated = false;
    bool remixBusy = false;

    public float timeLeft = 15f;

    void Update()
    {
        if(updateWon == true)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                gameHasStarted = false;
                InitiateRoulette();
                // RemixAndStartRound();
                updateWon = false;
            }   
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            updateMute = !updateMute;

            foreach (AudioSource track in musicTracks)
            {
                track.mute = updateMute;
            }

            instructionText1.text = updateMute ? 
                "@hughfuchsen\n[z] zoom  [m] unmute" : 
                "@hughfuchsen\n[z] zoom  [m] mute";
        }
        

        // if(updateRoundStart == true)
        // {
        //     if (Input.anyKeyDown)
        //     {
        //         InitiateRoulette();
        //         // RemixAndStartRound();
        //         updateRoundStart = false;
        //     }   
        // }


        if (timeLeft == 0f) // when the round has ended
        {
           
            if (waitCoro == null)
            {
                dynamicMovement = false;
                waitCoro = StartCoroutine(WaitForCharactersAndDepart());
            }
            
            
        }

        if(gameHasStarted)
        {
            timeLeft -= Time.deltaTime;
            timeLeft = Mathf.Max(timeLeft, 0f);

            int seconds = Mathf.FloorToInt(timeLeft);
            int milliseconds = Mathf.FloorToInt((timeLeft * 1000f) % 1000f);

            timeText.text = string.Format(
                "{0:00}:{1:000}",
                seconds,
                milliseconds
            );
        }
        
    }   


    bool CharactersSettled()
    {
        foreach (GameObject character in allCharacters)
        {
            CharacterMovement cm =
                character.GetComponent<CharacterMovement>();

            if (cm.change.magnitude > 0.01f)
            {
                return false;
            }
        }

        return true;
    }

    // Start button
    // character roulette function begins
    // roulette finishes
    // assigned character
    // ok button
    // 3,2,1 go!
    // round start
    // 

    // public IEnumerator RouletteCharacters()
    // {
    //     // set the camera to each character up close so (zoom param) it alternates the player
    //     // each shot will change the hue of the character with

    //             // ApplySharedHueShift(GetHueRemixGroup());
    //             // except with the person, it will call characterCustomization.UpdateRandom();

    //     // the camera position will rapidly change between 1-3 seconds in, then between 3-7 seconds, will slow down
    //     // the character the camera stops on will be the player's character for the round
    //     //  
    // }
    Coroutine rouletteCoro;
    public void InitiateRoulette()
    {
        if(rouletteCoro == null)
        {
            rouletteCoro = StartCoroutine(RouletteCharacters());
        }
        
    }

    public IEnumerator RouletteCharacters()
    {
        noControls = true;
        playersDontMove = true;
        gameHasStarted = false;
        cam.zoomSize = 160f;
        timeText.text = !gameHasStarted ? timeText.text = "train chasey" : timeText.text;

        // Disable Start button
        // startButton.SetActive(false);

        // - get the the characters available for this round
        List<GameObject> rouletteCharacters =
            new List<GameObject>(allCharacters);

        foreach(GameObject characterStart in rouletteCharacters)
        {
             if(characterStart != null)
            {
                NPCPathFollower cpf = characterStart.GetComponent<NPCPathFollower>();
                CharacterMovement cm = characterStart.GetComponent<CharacterMovement>();
                
                // if (cpf.pathFollowCoro != null)
                // {
                //     StopCoroutine(cpf.pathFollowCoro);
                //     cpf.pathFollowCoro = null;
                // }

                // if(cpf.goCoroutine != null)
                // {
                //     StopCoroutine(cpf.goCoroutine);
                //     cpf.goCoroutine = null;
                // }
                

                cm.change = Vector3.zero;
                // characterStart.transform.position = new Vector3(0,-32,0);
            }
        }
        mainCam.transform.position = new Vector3(0,-32,0);
        // cam.tar- get the = null;
        // Pick a random character
        
        GameObject character = pony; 
        // Make sure the camera starts somewhere sensible
        // Camera cam = Camera.main;

        // float rouletteTime = 7f;
        // float elapsedTime = 0f;

        // while (elapsedTime < rouletteTime)
        // {
            // elapsedTime += Time.deltaTime;

            // Fast at the beginning, slower towards the end
            // float t = Mathf.Clamp01(elapsedTime / rouletteTime);

            // This controls how long we stay on each character
            // float interval = 0.1f;
        
            // character.transform.position = new Vector3(600,0,0);
            
        if(previousCharacter != null)
        {
            do
            {
                character =
                rouletteCharacters[
                    Random.Range(0, rouletteCharacters.Count)
                ];
            }
            while(character == previousCharacter);
            
        }
        else
        {
            character =
                rouletteCharacters[
                    Random.Range(0, rouletteCharacters.Count)
                ];
        }

            // foreach(GameObject characterzero in Characters)
            // {
            //     SetSpritesAlpha(
            //         characterzero.GetComponent<CharacterAnimation>().characterSpriteList,
            //         characterzero.GetComponent<CharacterAnimation>().initialChrctrColorList,
            //         0f);
            // }
           
            
            

           
            
            // -------------------------
            // CHANGE CHARACTER APPEARANCE
            // -------------------------
            CharacterAnimation cAnima = character.GetComponent<CharacterAnimation>();
            if (cAnima.characterType == CharacterType.person)
            {
                character
                    .GetComponent<CharacterCustomization>()
                    .UpdateRandom();
            }
            // else
            // {
            //     // ApplySharedHueShift(GetHueRemixGroup()); 
            //     // SetSpritesAlpha(character, 0f);
               
            // }

            // SetSpritesAlpha(
            //     character.GetComponent<CharacterAnimation>().characterSpriteList,
            //     character.GetComponent<CharacterAnimation>().initialChrctrColorList,
            //     1f, true);

            // Vector3 origPos = character.transform.localPosition;
            // Vector3 newPos = character.transform.localPosition + new Vector3(0,-5,0);
            // character.transform.localPosition = newPos;
            
            character.transform.position = new Vector3(0,-32,0);


            // -------------------------
            // MOVE CAMERA TO CHARACTER
            // -------------------------
            // cam.tar- get the = character.transform;
            // cam.tar- getPosition = character.transform.localPosition;
            

            // Vector3 cameraPosition =
            //     character.transform.position;

            // cameraPosition.z =
            //     cam.transform.position.z;

            // cam.transform.position =
            //     cameraPosition;

            // Wait before next character
            // yield return new WaitForSeconds(interval);
            
            // elapsedTime += interval;
            roulettePlayer = character;
        // }

        // -------------------------
        // FINAL CHARACTER
        // -------------------------


        npcs.Clear();
        foreach (GameObject characterPick in allCharacters)
        {
            CharacterAnimation cAnim = characterPick.GetComponent<CharacterAnimation>();
            
            // chance the character will have a runing anim
            cAnim.runningAnim = Random.value < 0.5f;

            CharacterMovement cMov = characterPick.GetComponent<CharacterMovement>();
           
            // chance the character will have a wheelchair
            cMov.playerOnWheelchair = Random.value < 0.1f;
            
            // random speed
            // cMov.movementSpeed = cMov.CompareTag("Player") ? 80 : 65;
            
            // Make everyone an NPC first
            characterPick.tag = "NPC";
            characterPick.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            characterPick.GetComponentInChildren<BoxCollider2D>().gameObject.tag = "NPCCollider";
            characterPick.GetComponentInChildren<BoxCollider2D>().gameObject.layer = LayerMask.NameToLayer("NPC");
            characterPick.layer = LayerMask.NameToLayer("NPC");
            npcs.Add(characterPick);
        }  

        previousCharacter = character;
        player = character;
        player.tag = "Player";
        player.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        player.GetComponentInChildren<BoxCollider2D>().gameObject.tag = "PlayerCollider";
        player.GetComponentInChildren<BoxCollider2D>().gameObject.layer = LayerMask.NameToLayer("Player");
        player.layer = LayerMask.NameToLayer("Player");
        player.GetComponent<CharacterMovement>().activeCollisions.Clear();

        npcs.Remove(player);

        // -------------------------
        // SHOW RESULT / OK BUTTON
        // -------------------------

        // okButton.SetActive(true);
        // updateRoundStart = true;

        yield return null;

        okButton.SetActive(false);
        // GoToAnchorPoint(true);


        
        ApplyBackgroundHueShift();

        SwitchMusicTrack();

        instructionText1.text = "@hughfuchsen\n[z] zoom  [m] mute";
        CharacterAnimation playerCA = player.GetComponent<CharacterAnimation>();
        
        // back to initial pred and prey
        foreach(GameObject character1 in allCharacters)
        {
            NPCPathFollower pf = character1.GetComponent<NPCPathFollower>();
            pf.predator = pf.initialPredator;
            pf.prey = pf.initialPrey;

            CharacterMovement cm = character1.GetComponent<CharacterMovement>();
            if(playerCA.characterType == CharacterType.chicken || playerCA.characterType == CharacterType.puppy)
            {
                cm.movementSpeed = character1 == player ? Random.Range(115,125) : Random.Range(45,105);
            }
            else if (playerCA.characterType == CharacterType.person)
            {
                if (character1 == person)
                    cm.movementSpeed = Random.Range(115,125);

                if (character1 == pony)
                    cm.movementSpeed = Random.Range(115,125);

                if (character1 == pig)
                    cm.movementSpeed = Random.Range(55,85);

                if (character1 == chicken)
                    cm.movementSpeed = Random.Range(55,95);
                if (character1 == puppy)
                    cm.movementSpeed = Random.Range(55,95);
            }
            else if (playerCA.characterType == CharacterType.pony)
            {
                if (character1 == person)
                    cm.movementSpeed = Random.Range(55,85);

                if (character1 == pony)
                    cm.movementSpeed = Random.Range(115,125);

                if (character1 == pig)
                    cm.movementSpeed = Random.Range(115, 125);

                if (character1 == chicken)
                    cm.movementSpeed = Random.Range(55,95);
                if (character1 == puppy)
                    cm.movementSpeed = Random.Range(55,95);    
            }
            else if (playerCA.characterType == CharacterType.pig)
            {
                if (character1 == person)
                    cm.movementSpeed = Random.Range(115, 125);

                if (character1 == pony)
                    cm.movementSpeed = Random.Range(55, 85);

                if (character1 == pig)
                    cm.movementSpeed = Random.Range(115, 125);

                if (character1 == chicken)
                    cm.movementSpeed = Random.Range(55,95);
                if (character1 == puppy)
                    cm.movementSpeed = Random.Range(55,95);    
            }

            
            cm.initialmovementSpeed = cm.movementSpeed;
        }


        switch (playerCA.characterType)
        {
            case CharacterType.pig:
                instructionText2.text =
                    "  get the person\n  on the train" + "\n\n" +"  ditch the pony";
                    pony.GetComponent<NPCPathFollower>().isChasing = true;
                    person.GetComponent<NPCPathFollower>().isChasing = false;
                    puppy.GetComponent<NPCPathFollower>().isChasing = Random.value < 0.5f;                    
                    chicken.GetComponent<NPCPathFollower>().isChasing = Random.value < 0.5f;
                break;

            case CharacterType.person:
                instructionText2.text =
                    "  get the pony\n  on the train" + "\n\n" +"  ditch the pig";
                    pig.GetComponent<NPCPathFollower>().isChasing = true;
                    pony.GetComponent<NPCPathFollower>().isChasing = false;
                    puppy.GetComponent<NPCPathFollower>().isChasing = Random.value < 0.5f;                    
                    chicken.GetComponent<NPCPathFollower>().isChasing = Random.value < 0.5f;
                break;

            case CharacterType.pony:
                instructionText2.text =
                    "  get the pig\n  on the train" + "\n\n" +"  ditch the person";
                    person.GetComponent<NPCPathFollower>().isChasing = true;
                    pig.GetComponent<NPCPathFollower>().isChasing = false;
                    puppy.GetComponent<NPCPathFollower>().isChasing = Random.value < 0.5f;                    
                    chicken.GetComponent<NPCPathFollower>().isChasing = Random.value < 0.5f;
                break;

            case CharacterType.puppy:
                instructionText2.text =
                    "  round up everyone\n  on the train";
                    pony.GetComponent<NPCPathFollower>().isChasing = false;
                    person.GetComponent<NPCPathFollower>().isChasing = false;                    
                    pig.GetComponent<NPCPathFollower>().isChasing = false;                    
                    chicken.GetComponent<NPCPathFollower>().isChasing = false;        
                    pony.GetComponent<NPCPathFollower>().predator = puppy.transform;
                    person.GetComponent<NPCPathFollower>().predator = puppy.transform;
                    pig.GetComponent<NPCPathFollower>().predator = puppy.transform;
                    chicken.GetComponent<NPCPathFollower>().predator = puppy.transform;
                break;

            case CharacterType.chicken:
                instructionText2.text =
                    "  ditch everyone";
                    pony.GetComponent<NPCPathFollower>().isChasing = true;
                    person.GetComponent<NPCPathFollower>().isChasing = true;
                    pig.GetComponent<NPCPathFollower>().isChasing = true;
                    puppy.GetComponent<NPCPathFollower>().isChasing = true;
                    pony.GetComponent<NPCPathFollower>().prey = chicken.transform;
                    person.GetComponent<NPCPathFollower>().prey = chicken.transform;
                    pig.GetComponent<NPCPathFollower>().prey = chicken.transform;
                    puppy.GetComponent<NPCPathFollower>().prey = chicken.transform;
                break;
        }   

        // ApplySharedHueShift(GetHueRemixGroup());
        // foreach(GameObject npc in npcs)
        // {
        //     SetSpritesAlpha(
        //             npc.GetComponent<CharacterAnimation>().characterSpriteList,
        //             npc.GetComponent<CharacterAnimation>().initialChrctrColorList,
        //             1f, true);
        // }


        if (cam != null)
        {
            cam.target = player.transform;
        }

        StartCoroutine(WaitForCharactersAndDepart());
        rouletteCoro = null;

        
    }

    // public void RemixAndStartRound()
    // {
    //     okButton.SetActive(false);
    //     // GoToAnchorPoint(true);


        
    //     ApplyBackgroundHueShift();

    //     // SwitchMusicTrack();

    //     instructionText1.text = "mute sound [m]";

    //     CharacterAnimation playerCA = player.GetComponent<CharacterAnimation>();
        
    //     // back to initial pred and prey
    //     // foreach(GameObject character in allCharacters)
    //     // {
    //     //     NPCPathFollower pf = character.GetComponent<NPCPathFollower>();
    //     //     pf.predator = pf.initialPredator;
    //     //     pf.prey = pf.initialPrey;

    //     //     CharacterMovement cm = character.GetComponent<CharacterMovement>();
    //     //     cm.movementSpeed = character == player ? 135 : Random.Range(45,95);
    //     // }


    //     switch (playerCA.characterType)
    //     {
    //         case CharacterType.pig:
    //             instructionText2.text =
    //                 "o b j e c t i v e:" + "\n" + "person on the train" + "\n" +"ditch the pony";
    //                 pony.GetComponent<NPCPathFollower>().isChasing = true;
    //                 person.GetComponent<NPCPathFollower>().isChasing = false;
    //             break;

    //         case CharacterType.person:
    //             instructionText2.text =
    //                 "o b j e c t i v e:" + "\n" + "pony on the train" + "\n" +"ditch the pig";
    //                 pig.GetComponent<NPCPathFollower>().isChasing = true;
    //                 pony.GetComponent<NPCPathFollower>().isChasing = false;

    //             break;

    //         case CharacterType.pony:
    //             instructionText2.text =
    //                 "o b j e c t i v e:" + "\n" + "pig on the train" + "\n" +"ditch the person";
    //                 person.GetComponent<NPCPathFollower>().isChasing = true;
    //                 pig.GetComponent<NPCPathFollower>().isChasing = false;
    //             break;

    //         case CharacterType.puppy:
    //             instructionText2.text =
    //                 "o b j e c t i v e:" + "\n" + "everyone on the train";
    //                 pony.GetComponent<NPCPathFollower>().isChasing = false;
    //                 person.GetComponent<NPCPathFollower>().isChasing = false;                    
    //                 pig.GetComponent<NPCPathFollower>().isChasing = false;                    
    //                 chicken.GetComponent<NPCPathFollower>().isChasing = false;        
    //                 pony.GetComponent<NPCPathFollower>().predator = puppy.transform;
    //                 person.GetComponent<NPCPathFollower>().predator = puppy.transform;
    //                 pig.GetComponent<NPCPathFollower>().predator = puppy.transform;
    //                 chicken.GetComponent<NPCPathFollower>().predator = puppy.transform;
    //             break;

    //         case CharacterType.chicken:
    //             instructionText2.text =
    //                 "o b j e c t i v e:" + "\n" + "ditch everyone";
    //                 pony.GetComponent<NPCPathFollower>().isChasing = true;
    //                 person.GetComponent<NPCPathFollower>().isChasing = true;
    //                 pig.GetComponent<NPCPathFollower>().isChasing = true;
    //                 puppy.GetComponent<NPCPathFollower>().isChasing = true;
    //                 pony.GetComponent<NPCPathFollower>().prey = chicken.transform;
    //                 person.GetComponent<NPCPathFollower>().prey = chicken.transform;
    //                 pig.GetComponent<NPCPathFollower>().prey = chicken.transform;
    //                 puppy.GetComponent<NPCPathFollower>().prey = chicken.transform;
    //             break;
    //     }   

    //     // ApplySharedHueShift(GetHueRemixGroup());
    //     // foreach(GameObject npc in npcs)
    //     // {
    //     //     SetSpritesAlpha(
    //     //             npc.GetComponent<CharacterAnimation>().characterSpriteList,
    //     //             npc.GetComponent<CharacterAnimation>().initialChrctrColorList,
    //     //             1f, true);
    //     // }


    //     if (cam != null)
    //     {
    //         cam.target = player.transform;
    //     }

    //     StartCoroutine(WaitForCharactersAndDepart());

    // }
    public IEnumerator WaitForCharactersAndDepart()
    {
        CharacterMovement pcm =  player.GetComponent<CharacterMovement>();

        playersDontMove = false;
        noControls = false;
        GoToAnchorPoint();

        while (!CharactersSettled())
        {
            yield return null;
        }
        cam.target = player.transform;
        platformTrackTrainScript.doorCol.SetActive(true);
        // pcm.ResetPlayerMovement();
        // pcm.HandleQuadrantContact(pcm.controlDirection, pcm.currentContactQuadrant);


        if(CheckForWin())
        {
            updateWon = true;
            gameHasStarted = false;
            yield break;
        }

        player.tag = "Player";
        player.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        player.GetComponentInChildren<BoxCollider2D>().gameObject.tag = "PlayerCollider";
        player.GetComponentInChildren<BoxCollider2D>().gameObject.layer = LayerMask.NameToLayer("Player");
        player.layer = LayerMask.NameToLayer("Player");
        player.GetComponent<CharacterMovement>().activeCollisions.Clear();

        
        // pcm.activeCollisions.Clear();
        pcm.ResetPlayerMovement();
        dynamicMovement = true;
        
        platformTrackTrainScript.StartDeparture(Random.value < 0.5f);
        aboutToDepart = false;
        
        waitCoro = null;
        gameHasStarted = true;           
        yield return null;
    }

    public bool CheckForWin()
    {
        if (!gameHasStarted)
            return false;

        CharacterMovement playerCM = player.GetComponent<CharacterMovement>();
        CharacterAnimation playerAnim = player.GetComponent<CharacterAnimation>();
        NPCPathFollower playerPF = player.GetComponent<NPCPathFollower>();

        bool win = false;

        switch (playerAnim.characterType)
        {
            case CharacterType.pig:
            case CharacterType.person:
            case CharacterType.pony:

                bool preyOnTrain =
                    playerPF.prey != null &&
                    playerPF.prey.GetComponent<CharacterMovement>().currentArea
                    == AreaType.train;

                bool predatorOpposite =
                    playerPF.predator != null &&
                    playerPF.predator.GetComponent<CharacterMovement>().currentArea
                    != playerCM.currentArea;

                win = preyOnTrain && predatorOpposite;

                break;


            case CharacterType.puppy:

                win = true;

                foreach (GameObject character in allCharacters)
                {
                    if (character == player)
                        continue;

                    CharacterMovement cm =
                        character.GetComponent<CharacterMovement>();

                    if (cm.currentArea != AreaType.train)
                    {
                        win = false;
                        break;
                    }
                }

                break;


            case CharacterType.chicken:

                win = true;

                foreach (GameObject character in allCharacters)
                {
                    if (character == player)
                        continue;

                    CharacterMovement cm =
                        character.GetComponent<CharacterMovement>();

                    if (cm.currentArea == playerCM.currentArea)
                    {
                        win = false;
                        break;
                    }
                }

                break;
        }

        if (win)
        {
            // startButton.SetActive(true);
            okButton.SetActive(false);
            timeText.text = "success!";
            instructionText2.text = "  press [s]" + "\n" + "  to start again";
            // instructionText1.text = "[press [s]]";
        }
        else
        {
            return false;
        }

        return win;
    }

    public void GoToAnchorPoint()
    {
        foreach (GameObject character in allCharacters)
        {
            CharacterAnimation ca = character.GetComponent<CharacterAnimation>();

            if(!gameHasStarted)
            {
                // Debug.Log("tryyyy");
                Vector3 homePos;
                List<Vector3> otherPositions = new List<Vector3>
                {
                    initialPosition2,
                    initialPosition3,
                    initialPosition4,
                    initialPosition5
                };

                // shuffle positions
                for (int i = 0; i < otherPositions.Count; i++)
                {
                    Vector3 temp = otherPositions[i];
                    int rand = Random.Range(i, otherPositions.Count);
                    otherPositions[i] = otherPositions[rand];
                    otherPositions[rand] = temp;
                }

                int index = 0;

                homePos = otherPositions[index];
                character.transform.position = character.CompareTag("Player") ? initialPositionPlayer : homePos;
            
                ca.visualRoot.transform.localPosition = ca.initialState;
                index++;
            }
            
            NPCPathFollower pf = character.GetComponent<NPCPathFollower>();

            if(character.CompareTag("Player"))
            {pf.goCoroutine = null;}
            
            // pf.StopThinking();

            if(gameHasStarted)
            {
            player.GetComponent<CharacterMovement>().activeCollisions.Clear();
            player.tag = "NPC";
            player.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            player.GetComponentInChildren<BoxCollider2D>().gameObject.tag = "NPCCollider";
            player.GetComponentInChildren<BoxCollider2D>().gameObject.layer = LayerMask.NameToLayer("NPC");
            player.layer = LayerMask.NameToLayer("NPC");
            }
            cam.target = null;
            aboutToDepart = true;
            pf.StartThinking();

            CharacterMovement cm = character.GetComponent<CharacterMovement>();
            ca.visualRoot.transform.localPosition = ca.initialState;



            // if (cm.onDangerZoneBit == true &&
            //     cm.characterOnThresh == true) 
            // {
                GridNodeData homeNode = cm.currentArea == AreaType.platform 
                ? pf.GetClosestPlatformNode()
                : pf.GetClosestTrainNode();
                
                pf.currentGoal = homeNode;


            // }
            // else
            // {
            //     GridNodeData homeNode = pf.myNode;
            //     pf.currentGoal = homeNode;
            // }
            


            // CharacterMovement cm = character.GetComponent<CharacterMovement>();
            // GridNodeData homeNode = null;
            // if (cm.onDangerZoneBit == true &&
            //     cm.characterOnThresh == true) 
            // {
            //         homeNode = character.GetComponent<CharacterMovement>().currentArea == AreaType.platform
            //         ? pf.GetClosestPlatformNode()
            //         : pf.GetClosestTrainNode();

                    
            // }
            // if(homeNode != null)
            // {
            //     pf.currentGoal = homeNode;
            // }

            
            
            // aboutToDepart = true;
            // if(gameHasStarted)
            // pf.currentGoal = homeNode;

            
            // character.transform.position = homePos;
            // ai.GoToInitialPosition(homePos);
            

            

            
            // else
            // {
            //     ai.GoToInitialPosition();
            // }
            
        }
    }
    
        // if(cm.currentArea.areaType == AreaType.train)
        // switch (cm.currentArea.areaType)
        // {
        //     case AreaType.train:
        //         foreach (GameObject character in allCharacters)
        //         {
        //             NPCPathFollower ai = character.GetComponent<NPCPathFollower>();
        //             ai.GoToInitialPosition();
        //         }    
        //     break;

        //     case AreaType.platform:
        //         areaType = AreaType.platform;
        //     break;
        // }
    // }

    // void SetLayerRecursively(GameObject obj, int newLayer)
    // {
    //     obj.layer = newLayer;

    //     foreach (Transform child in obj.transform)
    //     {
    //         SetLayerRecursively(child.gameObject, newLayer);
    //     }
    // }
    public void SwitchMusicTrack()
    {
        int randomTrack = currentTrack;

        while (randomTrack == currentTrack)
        {
            randomTrack = Random.Range(0, musicTracks.Length);
        }

        StartCoroutine(FadeMusicTrack(randomTrack));
    }

    IEnumerator FadeMusicTrack(int newTrack)
    {
        AudioSource oldTrack = musicTracks[currentTrack];
        AudioSource newTrackSource = musicTracks[newTrack];

        float elapsed = 0f;

        while (elapsed < 5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 5f;

            oldTrack.volume = Mathf.Lerp(0.8f, 0f, t);
            newTrackSource.volume = Mathf.Lerp(0f, 0.8f, t);

            yield return null;
        }

        oldTrack.volume = 0f;
        newTrackSource.volume = 0.8f;

        currentTrack = newTrack;
    }

    // void ApplyTeleportPositions()
    // {
        
    //     List<Vector3> otherPositions = new List<Vector3>
    //     {
    //         initialPosition2,
    //         initialPosition3
    //         // initialPosition4,
    //         // initialPosition5
    //     };

    //      // shuffle
    //     for (int i = 0; i < otherPositions.Count; i++)
    //     {
    //         int rand = Random.Range(i, otherPositions.Count);

    //         Vector3 temp = otherPositions[i];
    //         otherPositions[i] = otherPositions[rand];
    //         otherPositions[rand] = temp;
    //     }

    //     int index = 0;

    //     foreach (GameObject character in allCharacters)
    //     {
    //         CharacterMovement cm = character.GetComponent<CharacterMovement>();

    //         if (character == player)
    //         {
    //             character.transform.position =
    //                 ApplyAreaOffset(initialPositionPlayer, cm);
    //         }
    //         else
    //         {
    //             character.transform.position =
    //                 ApplyAreaOffset(otherPositions[index], cm);

    //             index++;
    //         }

    //         cm.change = Vector3.zero;
    //     }
    // }

    // Vector3 ApplyAreaOffset(Vector3 pos, CharacterMovement cm)
    // {
    //     return cm.currentArea == AreaType.platform
    //         ? pos
    //         : pos + new Vector3(0, -60, 0);
    // }

    List<GameObject> GetHueRemixGroup()
    {
        List<GameObject> group = new List<GameObject>();

        foreach (GameObject character in allCharacters)
        {
            CharacterAnimation ca = character.GetComponent<CharacterAnimation>();

            if (ca.characterType == CharacterType.pig ||
                ca.characterType == CharacterType.pony 
                ||
                ca.characterType == CharacterType.chicken ||
                ca.characterType == CharacterType.puppy
                )
            {
                group.Add(character);
            }
        }

        return group;
    }

    void ApplySharedHueShift(List<GameObject> characters)
    {
        float hueShift = Random.Range(0f, 1f); // full hue wheel offset

        foreach (GameObject character in characters)
        {
            SpriteRenderer[] sprites =
                character.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sr in sprites)
            {
                Color col = sr.color;
                // float a = 0f;
                // col.a = a;
                float h, s, v;
                Color.RGBToHSV(col, out h, out s, out v);

                h = Mathf.Repeat(h + hueShift, 1f);

                sr.color = Color.HSVToRGB(h, s, v);
            }
        }
    }

    void ApplyBackgroundHueShift()
    {
        float hueShift = Random.Range(0f, 1f);

        Camera camera = cam.GetComponent<Camera>();

        Color col = camera.backgroundColor;

        float h, s, v;
        Color.RGBToHSV(col, out h, out s, out v);

        h = Mathf.Repeat(h + hueShift, 1f);
        s = 0.5f;

        camera.backgroundColor =
            Color.HSVToRGB(h, s, v);
    }
    private void SetSpritesAlpha(List<SpriteRenderer> list, List<Color> colorList, float alpha, bool restoreOriginal = false)
    {
        for (int i = 0; i < list.Count; i++)
        {
            SpriteRenderer sr = list[i];

            if (sr == null)
                continue;

            Color colour = sr.color;

            colour.a = restoreOriginal ? colorList[i].a : alpha;

            sr.color = colour;
        }
    }

    // public IEnumerator SortCharacters()
    // {

    // }

}