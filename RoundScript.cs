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

    [SerializeField] GameObject startButton;
    [SerializeField] GameObject okButton;

    [HideInInspector] public bool gameHasStarted = false;

    public GameObject pig;
    public GameObject pony;
    public GameObject person;
    public GameObject puppy;
    public GameObject chicken;


    public GameObject player;


    public CharacterType playerType;
    public CharacterType npc1Type;
    public CharacterType npc2Type;

    public List<GameObject> allCharacters = new List<GameObject>();
    public List<GameObject> npcs = new List<GameObject>();

    public Vector3 initialPositionPlayer = new Vector3(-40,22,0);
    public Vector3 initialPosition2 = new Vector3(-143,73,0);
    public Vector3 initialPosition3 = new Vector3(65,-31,0);
    // public Vector3 initialPosition4 = new Vector3(65,-31,0);
    // public Vector3 initialPosition5 = new Vector3(65,-31,0);

    [Header("UI")]
    public TMP_Text instructionText1;
    public TMP_Text instructionText2;
    public TMP_Text scoreText;
    public TMP_Text timeText;


    [Header("Soundtrack")]
    int currentTrack= 0;
    public AudioSource[] musicTracks;

    
    

    void Awake()
    {
        allCharacters.Add(pony);
        allCharacters.Add(pig);
        allCharacters.Add(person);
        allCharacters.Add(puppy);
        allCharacters.Add(chicken);


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
        instructionText1.text = "Hello";

        timeText.text = "time left 15:00";
       
        // allCharacters[Random.Range(0, 3)];

        // melody.loop = true;
        // melody.Play();

        double startTime = AudioSettings.dspTime + 0.2;

        foreach (AudioSource track in musicTracks)
        {
            track.volume = 0;
            track.PlayScheduled(startTime);
        }

        musicTracks[0].volume = 1;


        // foreach (GameObject character in allCharacters)
        // {
        //     if(character.CompareTag("NPC"))
        //     {
        //         platformTrackTrainScript.characterList.Add(character);
        //     }

        // }
　
        // RemixAndStartRound();

        startButton.SetActive(true);
        okButton.SetActive(false);


        
        
    }

    bool isRemixInitiated = false;
    bool remixBusy = false;

    float timeLeft = 25f;

    void Update()
    {
        if (timeLeft == 0f) // when the round has ended
        {
            if (platformTrackTrainScript.departureAwaiting)
            {
                instructionText1.text = "BingBing";
                instructionText2.text = ""; 
                return;  
            }
            else if(!platformTrackTrainScript.departureAwaiting)
            {
                // if(Input.GetKey(KeyCode.W))
                platformTrackTrainScript.StartDeparture(Random.value < 0.5f);
                timeLeft = 25f;              
            }
        }


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
    public void InitiateRoulette()
    {
        StartCoroutine(RouletteCharacters());
    }

    public IEnumerator RouletteCharacters()
    {
        // Disable Start button
        startButton.SetActive(false);

        // Get the characters available for this round
        List<GameObject> rouletteCharacters =
            new List<GameObject>(allCharacters);

        GameObject character = null; 
        // Make sure the camera starts somewhere sensible
        // Camera cam = Camera.main;

        float rouletteTime = 7f;
        float elapsedTime = 0f;

        while (elapsedTime < rouletteTime)
        {
            elapsedTime += Time.deltaTime;

            // Fast at the beginning, slower towards the end
            float t = Mathf.Clamp01(elapsedTime / rouletteTime);

            // This controls how long we stay on each character
            float interval = Mathf.Lerp(0.05f, 0.5f, t);

            // foreach(GameObject characterzero in rouletteCharacters)
            // {
            //     SetSpritesAlpha(
            //         characterzero.GetComponent<CharacterAnimation>().characterSpriteList,
            //         characterzero.GetComponent<CharacterAnimation>().initialChrctrColorList,
            //         0f);
            // }
            if(character != null)
            character.transform.position = new Vector3(600,0,0);

            // Pick a random character
            character =
                rouletteCharacters[
                    Random.Range(0, rouletteCharacters.Count)
                ];
            
            // -------------------------
            // CHANGE CHARACTER APPEARANCE
            // -------------------------

            if (character.GetComponent<CharacterAnimation>()
                .characterType == CharacterType.person)
            {
                character
                    .GetComponent<CharacterCustomization>()
                    .UpdateRandom();
                
                // character
                //     .GetComponent<CharacterAnimation>()
                //     .ResetColorList(
                //         character,
                //         character.GetComponent<CharacterAnimation>().characterSpriteList,
                //         character.GetComponent<CharacterAnimation>().initialChrctrColorList
                //     );

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
            
            character.transform.position = new Vector3(0,0,0);


            // -------------------------
            // MOVE CAMERA TO CHARACTER
            // -------------------------
            // cam.target = character.transform;
            // cam.targetPosition = character.transform.localPosition;
            

            // Vector3 cameraPosition =
            //     character.transform.position;

            // cameraPosition.z =
            //     cam.transform.position.z;

            // cam.transform.position =
            //     cameraPosition;

            // Wait before next character
            yield return new WaitForSeconds(interval);
            
            elapsedTime += interval;
            player = character;
        }

        // -------------------------
        // FINAL CHARACTER
        // -------------------------



        foreach (GameObject characterPick in allCharacters)
        {
            CharacterAnimation cAnim = characterPick.GetComponent<CharacterAnimation>();
            
            // chance the character will have a runing anim
            cAnim.runningAnim = Random.value < 0.5f;

            CharacterMovement cMov = characterPick.GetComponent<CharacterMovement>();
           
            // chance the character will have a wheelchair
            cMov.playerOnWheelchair = Random.value < 0.1f;
            
            // random speed
            cMov.movementSpeed = cMov.CompareTag("Player") ? 80 : 65;
            
            // Make everyone an NPC first
            characterPick.tag = "NPC";
            characterPick.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            characterPick.GetComponentInChildren<BoxCollider2D>().gameObject.tag = "NPCCollider";
            characterPick.layer = LayerMask.NameToLayer("NPC");
            npcs.Add(characterPick);
        }  

        player.tag = "Player";

        player.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        player.GetComponentInChildren<BoxCollider2D>().gameObject.tag = "PlayerCollider";
        player.layer = LayerMask.NameToLayer("Player");

        npcs.Remove(player);

        // -------------------------
        // SHOW RESULT / OK BUTTON
        // -------------------------

        okButton.SetActive(true);
        yield return null;
    }

    public void RemixAndStartRound()
    {
        okButton.SetActive(false);

        gameHasStarted = true;

        
        ApplyBackgroundHueShift();

        SwitchMusicTrack();

        instructionText1.text = "Hello";
        CharacterAnimation playerCA = player.GetComponent<CharacterAnimation>();

        switch (playerCA.characterType)
        {
            case CharacterType.pig:
                instructionText2.text =
                    "stick with the person" + "\n" +"ditch the pony";
                    // pony.GetComponent<NPCPathFollower>().isChasing = true;
                    // person.GetComponent<NPCPathFollower>().isChasing = false;
                break;

            case CharacterType.person:
                instructionText2.text =
                    "stick with the pony" + "\n" +"ditch the pig";
                    // pig.GetComponent<NPCPathFollower>().isChasing = true;
                    // pony.GetComponent<NPCPathFollower>().isChasing = false;

                break;

            case CharacterType.pony:
                instructionText2.text =
                    "stick with the pig" + "\n" +"ditch the person";
                    // person.GetComponent<NPCPathFollower>().isChasing = true;
                    // pig.GetComponent<NPCPathFollower>().isChasing = false;
                break;

            case CharacterType.puppy:
                instructionText2.text =
                    "stick with everyone";
                    // pony.GetComponent<NPCPathFollower>().isChasing = false;
                    // person.GetComponent<NPCPathFollower>().isChasing = false;
                    // pig.GetComponent<NPCPathFollower>().isChasing = false;
                    // chicken.GetComponent<NPCPathFollower>().isChasing = false;
                break;

            case CharacterType.chicken:
                instructionText2.text =
                    "ditch everyone";
                    // pony.GetComponent<NPCPathFollower>().isChasing = true;
                    // person.GetComponent<NPCPathFollower>().isChasing = true;
                    // pig.GetComponent<NPCPathFollower>().isChasing = true;
                    // chicken.GetComponent<NPCPathFollower>().isChasing = true;
                break;
        }   

        // ApplySharedHueShift(GetHueRemixGroup());
        foreach(GameObject npc in npcs)
        {
            SetSpritesAlpha(
                    npc.GetComponent<CharacterAnimation>().characterSpriteList,
                    npc.GetComponent<CharacterAnimation>().initialChrctrColorList,
                    1f, true);
        }


        if (cam != null)
        {
            cam.target = player.transform;
        }


        // reset think loop
       
        // if(cm.currentArea == AreaType.train)
        // platformTrackTrainScript.PrepareStartPlatformState();

        platformTrackTrainScript.StartDeparture(Random.value < 0.5f);

    }


    public void GoToAnchorPoint()
    {
        List<Vector3> otherPositions = new List<Vector3>
        {
            initialPosition2,
            initialPosition3
            // initialPosition4,
            // initialPosition5
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

        foreach (GameObject character in allCharacters)
        {
            Vector3 homePos;
            if (character.CompareTag("Player"))
            {
                GameObject player = character;

                if(player.GetComponent<CharacterMovement>().characterOnThresh)
                player.GetComponent<CharacterMovement>().currentArea = AreaType.train;

                homePos = player.GetComponent<CharacterMovement>().currentArea == AreaType.platform 
                ? initialPositionPlayer
                : initialPositionPlayer + new Vector3(0, -60, 0);
            }
            else
            {
                homePos = otherPositions[index];
                index++;
            }

            NPCPathFollower ai = character.GetComponent<NPCPathFollower>();
            ai.GoToInitialPosition(homePos);
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
            randomTrack =
                Random.Range(0, musicTracks.Length);
        } 


        musicTracks[currentTrack].volume = 0;

        musicTracks[randomTrack].volume = 1;

        currentTrack = randomTrack;
    }


    void ApplyTeleportPositions()
    {
        
        List<Vector3> otherPositions = new List<Vector3>
        {
            initialPosition2,
            initialPosition3
            // initialPosition4,
            // initialPosition5
        };

         // shuffle
        for (int i = 0; i < otherPositions.Count; i++)
        {
            int rand = Random.Range(i, otherPositions.Count);

            Vector3 temp = otherPositions[i];
            otherPositions[i] = otherPositions[rand];
            otherPositions[rand] = temp;
        }

        int index = 0;

        foreach (GameObject character in allCharacters)
        {
            CharacterMovement cm = character.GetComponent<CharacterMovement>();

            if (character == player)
            {
                character.transform.position =
                    ApplyAreaOffset(initialPositionPlayer, cm);
            }
            else
            {
                character.transform.position =
                    ApplyAreaOffset(otherPositions[index], cm);

                index++;
            }

            cm.change = Vector3.zero;
        }
    }

    Vector3 ApplyAreaOffset(Vector3 pos, CharacterMovement cm)
    {
        return cm.currentArea == AreaType.platform
            ? pos
            : pos + new Vector3(0, -60, 0);
    }

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