using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum GuageType
{
    Evironment,
    Economy,
    Happiness,
    Security
}

public class GuageManager : MonoBehaviour
{
    public static GuageManager Instance;

    public float evironmentValue;
    public float economyValue;
    public float happinessValue;
    public float securityValue;

    public GameObject canvasGuageUI;

    public Slider evironmentSlider;
    public Slider economySlider;
    public Slider happinessSlider;
    public Slider securitySlider;

    public Transform player;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(canvasGuageUI);
        }
        else
        {
            Destroy(canvasGuageUI);
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateGuageUI();
    }

    public void UpdateGuageUI()
    {
        DOTween.To(() => evironmentSlider.value, x => evironmentSlider.value = x, evironmentValue / 100f, 0.5f);
        DOTween.To(() => economySlider.value, x => economySlider.value = x, economyValue / 100f, 0.5f);
        DOTween.To(() => happinessSlider.value, x => happinessSlider.value = x, happinessValue / 100f, 0.5f);
        DOTween.To(() => securitySlider.value, x => securitySlider.value = x, securityValue / 100f, 0.5f);

        Debug.Log("Gauge UI Updated");
    }

    void FixedUpdate()
    {
        if(SceneManager.GetActiveScene().name == "Title")
        {
            canvasGuageUI.SetActive(false);
        }
        else
        {
            canvasGuageUI.SetActive(true);
            UpdateGuageUI();
        }
    }

    public void AddGuageValue(GuageType type, float value)
    {
        switch (type)
        {
            case GuageType.Evironment:
                evironmentValue += value;
                FloatingPairSpawner.Instance.SpawnFloatingPair(0, (int)value, player);
                break;
            case GuageType.Economy:
                economyValue += value;
                FloatingPairSpawner.Instance.SpawnFloatingPair(1, (int)value, player);
                break;
            case GuageType.Happiness:
                happinessValue += value;
                FloatingPairSpawner.Instance.SpawnFloatingPair(2, (int)value, player);
                break;
            case GuageType.Security:
                securityValue += value;
                FloatingPairSpawner.Instance.SpawnFloatingPair(3, (int)value, player);
                break;
        }

        Debug.Log("" + type.ToString() + " Gauge Updated: " + value);
    }

    public void SetGuageValue(GuageType type, float value)
    {
        switch (type)
        {
            case GuageType.Evironment:
                evironmentValue = value;
                break;
            case GuageType.Economy:
                economyValue = value;
                break;
            case GuageType.Happiness:
                happinessValue = value;
                break;
            case GuageType.Security:
                securityValue = value;
                break;
        }
        Debug.Log("" + type.ToString() + " Gauge Set: " + value);
    }   

        // ------------------ 여기부터 엔딩 로직 ------------------

    private bool InRange(float value, float min, float max)
    {
        return value >= min && value <= max;
    }

        /// <summary>
    /// 런타임 엔딩 PropData 생성 (대사 리스트 버전)
    /// </summary>
    private PropData CreateEndingProp(string id, string title, string[] lines)
    {
        PropData prop = ScriptableObject.CreateInstance<PropData>();
        prop.propID = id;

        prop.dialogues = new DialogueData[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            DialogueData data = new DialogueData();
            data.dialogueType = DialogueType.Normal;
            data.characterName = title;
            data.dialogueText = lines[i];

            data.actionGaugeReductionRate = 0f;
            data.evironmentGuageChange = 0f;
            data.economyGuageChange = 0f;
            data.happinessGuageChange = 0f;
            data.securityGaugeChange = 0f;
            data.branches = null;

            prop.dialogues[i] = data;
        }

        return prop;
    }

    /// <summary>
    /// 현재 게이지 값으로 어떤 엔딩이 나오는지 계산하고, 해당 엔딩용 PropData를 반환
    /// (어떤 경우에도 null을 반환하지 않고 항상 하나의 엔딩을 리턴)
    /// </summary>
    public PropData GetEndingProp()
    {
        // 클램프
        evironmentValue = Mathf.Clamp(evironmentValue, 0f, 100f);
        economyValue    = Mathf.Clamp(economyValue,    0f, 100f);
        happinessValue  = Mathf.Clamp(happinessValue,  0f, 100f);
        securityValue   = Mathf.Clamp(securityValue,   0f, 100f);

        float env = evironmentValue;
        float eco = economyValue;
        float hap = happinessValue;
        float sec = securityValue;

        // =======================
        // [Priority 0] 8. Destruction (Game Over)
        // Any stat <= 0
        // =======================
        if (env <= 0f || eco <= 0f || hap <= 0f || sec <= 0f)
        {
            string[] lines =
            {
                "Where did it all go wrong... The balance has crumbled beyond repair.",
                "The streets are filled with screams, and burning buildings dye the night sky red.",
                "People trample each other just to survive, and the city stopped functioning long ago.",
                "There is nothing more I, or you—the leader—can do.",
                "I'm sorry. I wanted to protect everyone… but our world ends today."
            };

            return CreateEndingProp("ENDING_08_DESTRUCTION", "Destruction (Game Over)", lines);
        }

        // =======================
        // [Priority 1] Hidden Endings
        // 14. Mechanical Paradise (Hidden Ending)
        // All stats >= 70
        // =======================
        if (env >= 70f && eco >= 70f && sec >= 70f && hap >= 70f)
        {
            string[] lines =
            {
                "The AI has calculated the city to perfection; mistakes remain only as error codes.",
                "The morning sun rises at a fixed angle, and the crime rate has dropped below the decimal point.",
                "Every citizen enjoys standardized happiness, and anxiety has been removed like a disease with a vaccine.",
                "We finally built heaven—but where did we hide our freedom?",
                "In this perfectly optimized life, the word 'choice' has disappeared.",
                "Paradise, as they say, is quieter than hell."
            };

            return CreateEndingProp("ENDING_14_MECHANICAL_PARADISE", "Mechanical Paradise (Hidden Ending)", lines);
        }

        // =======================
        // 15. The Fading Lantern (Hidden Bad Ending)
        // All stats < 40 (non-zero는 위에서 이미 걸러짐)
        // =======================
        if (env < 40f && eco < 40f && hap < 40f && sec < 40f)
        {
            string[] lines =
            {
                "We survived. But can we really call this 'living'?",
                "The factories have halted, the forests have withered, and in the absence of security, people have forgotten how to smile.",
                "There was no massive explosion, no spectacular apocalypse—everything just slowly, very quietly, ceased to function.",
                "That faint light just before a candle goes out... that is what we are now.",
                "The sun will rise tomorrow, but no one will be there to welcome its light."
            };

            return CreateEndingProp("ENDING_15_FADING_LANTERN", "The Fading Lantern (Hidden Bad Ending)", lines);
        }

        // =======================
        // [Priority 2] Extreme Bad Endings (Opposing Stats)
        // 1. The Masked Tycoon (Bad Ending)
        // env <= 20, eco >= 80
        // =======================
        if (env <= 20f && eco >= 80f)
        {
            string[] lines =
            {
                "Look out the window. See the crumbling slums beyond that gray smog?",
                "We’ll be safe here forever, sipping premium wine inside this dome.",
                "A world has come where even the air we breathe must be bought with money—but so what? We are the winners.",
                "My lungs feel a bit heavy, but watching my bank account grow makes it feel like I can breathe again.",
                "Shall we make a toast? To the rotting Earth, and to our wealth that will last forever upon it."
            };

            return CreateEndingProp("ENDING_01_MASKED_TYCOON", "The Masked Tycoon (Bad Ending)", lines);
        }

        // =======================
        // 2. People Who Returned to the Forest (Bad Ending)
        // env >= 80, eco <= 20
        // =======================
        if (env >= 80f && eco <= 20f)
        {
            string[] lines =
            {
                "Look at the flower blooming on top of that rusty car.",
                "People used to dedicate their whole lives to buying that thing, but now it’s just a pile of scrap metal.",
                "The electricity is gone and the markets are filled with dust, but the Milky Way in the night sky has never been clearer.",
                "We fill our stomachs by hunting and endure the cold with each other's body heat.",
                "Civilization didn't perish—we simply returned to being part of true 'Nature'."
            };

            return CreateEndingProp("ENDING_02_RETURN_FOREST", "People Who Returned to the Forest (Bad Ending)", lines);
        }

        // =======================
        // 3. The City Where Laughter is Forbidden (Bad Ending)
        // sec >= 80, hap <= 20
        // =======================
        if (sec >= 80f && hap <= 20f)
        {
            string[] lines =
            {
                "Citizens, rest assured. The Central Control System is perfectly managing even your heart rates.",
                "Unnecessary laughter or tears are viruses that disturb social order.",
                "If your neighbor displays suspicious emotions, report them immediately. That is patriotism.",
                "We have completed a flawless society: precise like a machine, and hard as steel.",
                "Freedom only breeds chaos. Do not think. Just obey."
            };

            return CreateEndingProp("ENDING_03_FORBIDDEN_CITY", "The City Where Laughter is Forbidden (Bad Ending)", lines);
        }

        // =======================
        // 4. Pleasure in the Lawless Zone (Bad Ending)
        // sec <= 20, hap >= 80
        // =======================
        if (sec <= 20f && hap >= 80f)
        {
            string[] lines =
            {
                "Yeeeah!! Turn the music up louder!! Play like today is the last day of the world!!",
                "Police? Laws? We burned that boring junk yesterday!",
                "We are the law! Who cares if we starve to death or get stabbed tomorrow? I feel amazing right now!",
                "Bring more of that looted booze and meat over here! The frenzy is just getting started!",
                "Anyone who stops is a traitor! Drink, enjoy, and lose your mind!!"
            };

            return CreateEndingProp("ENDING_04_LAWLESS_PLEASURE", "Pleasure in the Lawless Zone (Bad Ending)", lines);
        }

        // =======================
        // [Priority 3] Complex Normal Endings (Dual High Stats)
        // 5. The Cold Steel Empire (Normal Ending)
        // eco >= 70, sec >= 70, env < 70, hap < 70
        // =======================
        if (eco >= 70f && sec >= 70f && env < 70f && hap < 70f)
        {
            string[] lines =
            {
                "This city is a giant factory, and you are merely parts that can be replaced at any time.",
                "There are no ration tickets for inefficient humans.",
                "Stop whining. Instead of getting sentimental and looking at the sky, tighten one more screw.",
                "That is the value of your existence. The profit of the corporation is the profit of the state.",
                "Break time is over. The siren has sounded—get back to work, until you die."
            };

            return CreateEndingProp("ENDING_05_COLD_STEEL", "The Cold Steel Empire (Normal Ending)", lines);
        }

        // =======================
        // 6. Utopia of the Slums (Normal Ending)
        // env >= 70, hap >= 70, eco < 70, sec < 70
        // =======================
        if (env >= 70f && hap >= 70f && eco < 70f && sec < 70f)
        {
            string[] lines =
            {
                "We’ve got absolutely nothing, so I guess we’ll be boiling grass porridge for dinner again.",
                "Still, isn’t it strange? We don’t have a penny, yet our hearts feel so at peace.",
                "Breathing clear air, playing the guitar, and singing songs... I don’t envy the rich at all.",
                "I don’t know what will happen tomorrow, but at least in this moment, we are free.",
                "Isn’t that enough? Come on, put your worries away and keep singing."
            };

            return CreateEndingProp("ENDING_06_SLUM_UTOPIA", "Utopia of the Slums (Normal Ending)", lines);
        }

        // =======================
        // 10. Glass Garden Citizen-State (Normal Ending)
        // env >= 60, eco >= 60, sec < 40, hap < 40
        // =======================
        if (env >= 60f && eco >= 60f && sec < 40f && hap < 40f)
        {
            string[] lines =
            {
                "In the blue greenhouse, the four seasons have vanished, and flowers grow in artificial soil.",
                "The air is clean and the production lines never stop; at least no one is starving.",
                "But people don't trust each other—smiles have been replaced by streams of data.",
                "This place has prospered, but hearts are barren.",
                "We gained abundance but lost warmth.",
                "The plants are thriving... so why is our humanity withering away?"
            };

            return CreateEndingProp("ENDING_10_GLASS_GARDEN", "Glass Garden Citizen-State (Normal Ending)", lines);
        }

        // =======================
        // [Priority 4] Special Conditions (Single High Stat / Fallback)
        // 9. The Golden Desert (Bad Ending)
        // eco >= 70, env <= 30
        // =======================
        if (eco >= 70f && env <= 30f)
        {
            string[] lines =
            {
                "Listen to the sound of the sandstorm scratching against the glass walls.",
                "There are no forests and no rivers left in this world, but the vaults are full.",
                "The land is dead, but the corporations live. Currency has increased, but people have decreased.",
                "We became rich, yet we lost the very soil in which to set our roots.",
                "Someday, the next generation will ask: Why was all we left behind nothing but gold and ruins?"
            };

            return CreateEndingProp("ENDING_09_GOLDEN_DESERT", "The Golden Desert (Bad Ending)", lines);
        }

        // =======================
        // 11. The Blue Prison (Bad Ending)
        // sec >= 70, 40 <= hap <= 60, env < 70, eco < 70
        // =======================
        if (sec >= 70f && InRange(hap, 40f, 60f) && env < 70f && eco < 70f)
        {
            string[] lines =
            {
                "The streets are quiet and the crime rate is zero.",
                "No one fights, no one resists.",
                "But every night, when the drone outside scans my window, my heart freezes.",
                "It is not peace that rules here, but silence.",
                "Riots have disappeared, and our voices were buried along with them.",
                "There are no visible bars... but this entire city is a prison."
            };

            return CreateEndingProp("ENDING_11_BLUE_PRISON", "The Blue Prison (Bad Ending)", lines);
        }

        // =======================
        // 12. Twilight of Smiles (Normal Ending)
        // hap >= 70, sec <= 40, env <= 70, eco <= 70
        // =======================
        if (hap >= 70f && sec <= 40f && env <= 70f && eco <= 70f)
        {
            string[] lines =
            {
                "Even in alleys without a single light, the sound of guitars never stops.",
                "Children, though poor, call each other's names with bright smiles.",
                "The glass towers have fallen, but our hearts have risen.",
                "The law is weak and the nights are dangerous, yet hope feels strangely large.",
                "We’re laughing sincerely in this fragile paradise.",
                "When the storm comes, it might all collapse... but at least, we were truly happy."
            };

            return CreateEndingProp("ENDING_12_TWILIGHT_SMILES", "Twilight of Smiles (Normal Ending)", lines);
        }

        // =======================
        // [Priority 5] Balanced Ending
        // 7. Precarious Peace (True Ending)
        // All stats 40 ~ 60
        // =======================
        if (InRange(env, 40f, 60f) &&
            InRange(eco, 40f, 60f) &&
            InRange(sec, 40f, 60f) &&
            InRange(hap, 40f, 60f))
        {
            string[] lines =
            {
                "It isn’t a perfect heaven, but we miraculously kept it from falling into hell.",
                "Some people complain they want more money; others complain they want more freedom.",
                "Yet we managed a precarious tightrope walk that doesn’t lean too far to any side.",
                "No riots, no mass starvation, no suffocating dictatorship—just this ordinary, boring day.",
                "Maybe we fought so fiercely all this time... just to protect this ordinariness."
            };

            return CreateEndingProp("ENDING_07_PRECARIOUS_PEACE", "Precarious Peace (True Ending)", lines);
        }

        // =======================
        // 13. The Colorless Middle Ground (Neutral Ending)
        // 최종 Fallback: 위 조건에 하나도 해당하지 않을 때 항상 발동
        // =======================
        {
            string[] lines =
            {
                "We avoided the extremes, but we never reached the ideal.",
                "This is a faint gray world—neither a dream nor a nightmare.",
                "No one starves to death in the streets, but no festivals are held either.",
                "Everything is slightly lacking, and slightly remaining.",
                "We survived, but it’s hard to say we are truly 'alive'.",
                "And yet... maybe that means there is still room to make a new decision."
            };

            return CreateEndingProp("ENDING_13_COLORLESS_MID", "The Colorless Middle Ground (Neutral Ending)", lines);
        }
    }
}
