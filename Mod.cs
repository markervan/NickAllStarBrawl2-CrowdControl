//this project is a retrofit, it should NOT be used as part of any example - kat
using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Collections;
using System.Threading;
using Quantum;
using TMPro;
using System.Linq;
using Photon.Deterministic;
using UnityEngine.InputSystem;
using Cinemachine;

namespace BepinControl;

[BepInPlugin(modGUID, modName, modVersion)]
public class Mod : BaseUnityPlugin
{
    // Mod Details
    private const string modGUID = "WarpWorld.CrowdControl";
    private const string modName = "Crowd Control";
    private const string modVersion = "1.1.11";

    public static TextMeshProUGUI barkTMP;
    public static BarkHUD barkHudData;
    public static List<BarkData> barkDataList = new List<BarkData>();
    public static ObjectiveCard objectiveCard;


    public static bool fpsOn = false;
    public static Transform headBone; // Class-level variable to store the head bone
    public static FreeCameraSystem freecamera; // Class-level variable to store the camera system


    public static bool hudDoneLoading = false;
    public static bool initDone = false;
    public static bool barkLoaded = false;
    public static List<CharacterCodename> validCharacters;
    public static string tsVersion = "1.1.11";
    public static Dictionary<string, (string name, string conn)> version = new Dictionary<string, (string name, string conn)>();

    private readonly Harmony harmony = new Harmony(modGUID);


    
    public static ManualLogSource mls;
    internal static bool isHost = false;

    internal static Mod Instance = null;
    private static ControlClient client = null;

    public static bool test = false;
    public static uint msgid = 0;
    public static uint msgid2 = 0;
    public static uint msgid3 = 0;
    public static uint verwait = 0;

    public static uint floodtime = 0;

    private SettingsManager settingsManager
    {
        get
        {
            return GameManager.Instance.SettingsManager;
        }
    }
    static public OnlineManager onlineManager
    {
        get
        {
            return GameManager.Instance.OnlineManager;
        }
    }
    private InputManager inputManager
    {
        get
        {
            return GameManager.Instance.InputManager;
        }
    }
    private GameResourcesManager gameResourcesManager
    {
        get
        {
            return GameManager.Instance.GameResourcesManager;
        }
    }
    static public UIManager uiManager
    {
        get
        {
            return GameManager.Instance.UIManager;
        }
    }
    static public NetworkClient client1
    {
        get
        {
            return GameManager.Instance.OnlineManager.Client;
        }
    }
    static public DataManager dataManager
    {
        get
        {
            return GameManager.Instance.DataManager;
        }
    }
    static public MatchManager matchManager
    {
        get
        {
            return GameManager.Instance.MatchManager;
        }
    }



    void Awake()
    {
        Instance = this;
        mls = BepInEx.Logging.Logger.CreateLogSource("Crowd Control");
        // Plugin startup logic
        mls.LogInfo($"Loaded {modGUID}. Patching.");
        harmony.PatchAll(typeof(Mod));
        mls.LogInfo($"Initializing Crowd Control");
        mls = Logger;

    }


    private static string OnCCVersion()
    {
        string res = "Checking Crowd Control Versions...\n\n";


        foreach (var versionNum in version)
        {
            res += $"{versionNum.Key}: version {versionNum.Value.name} Live: {versionNum.Value.conn}\n";
        }
        return res;
    }

    public void Update()
    {
        // If the head bone has been found, update the camera offset every frame
        if (headBone != null && freecamera != null)
        {
            float headX = headBone.transform.position.x;
            float headY = headBone.transform.position.y;

            Vector3 headEuler = headBone.transform.rotation.eulerAngles;

            // Subtract 99 degrees from the Y rotation
            
            float adjustedRotX = headEuler.x;
            float adjustedRotY = headEuler.y - 79;
            float adjustedRotZ = headEuler.z;

            if(matchManager.CurrentCharacterManagers[0].lastFacingDirection == -1)
            {
                adjustedRotY = -(headEuler.y - 82);
            }


            Quaternion newRotation = Quaternion.Euler(headEuler.x, adjustedRotY, headEuler.z);


            //CinemachineVirtualCamera cinemachineVirtualCamera = freecamera.virtualCamera;
            //cinemachineVirtualCamera.m_Lens.FieldOfView = 100f;
            //freecamera.virtualCamera.m_Lens.FieldOfView = 100f;
            freecamera.gameObject.transform.position = new Vector3(headX, headY, 0);
            freecamera.gameObject.transform.rotation = newRotation;

        }
    }

    public static Transform FindHeadBone(Transform parent, string boneName)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains("expressions"))
            {
                child.gameObject.SetActive(false);
            }
            if (child.name.Contains(boneName))
            {
                return child;
            }
            


            Transform found = FindHeadBone(child, boneName);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }
    public static void FindExpressions(Transform parent, string boneName)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(boneName))
            {
                child.gameObject.SetActive(false);
            }
        }
    }
    private string GetFullPath(Transform child)
    {
        string path = child.name;
        while (child.parent != null)
        {
            child = child.parent;
            path = child.name + "/" + path;
        }
        return path;
    }

    public static Queue<Action> ActionQueue = new Queue<Action>();

    [HarmonyPatch(typeof(QuantumRunner), "Update")]
    [HarmonyPrefix]
    static void roundUpdate(QuantumRunner __instance)
    {
        if (EffectDelegates.givedelay > 0) EffectDelegates.givedelay--;
        if (verwait > 0) verwait--;

        if (ActionQueue.Count > 0)
        {
            Action action = ActionQueue.Dequeue();
            action.Invoke();
        }

        lock (TimedThread.threads)
        {
            foreach (var thread in TimedThread.threads)
            {
                if (!thread.paused)
                    thread.effect.tick();
            }
        }

    }

    [HarmonyPatch(typeof(MatchHUD), "OnMatchReady")]
    [HarmonyPostfix]
    static void OnMatchReady()
    {
        hudDoneLoading = true;
    }
    [HarmonyPatch(typeof(MatchManager), "EndMatch")]
    [HarmonyPostfix]
    static void EndMatch()
    {
        hudDoneLoading = false;
    }
    [HarmonyPatch(typeof(MatchManager), "SimulationEnded")]
    [HarmonyPostfix]
    static void SimulationEnded()
    {
        hudDoneLoading = false;
        freecamera = null;
        headBone = null;
        fpsOn = false;
    }

    [HarmonyPatch(typeof(FreeCameraSystem), "OnEnable")]
    [HarmonyPrefix]
    static bool OnEnable()
    {
        //hudDoneLoading = false;
        //freecamera = null;
        //headBone = null;

        
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenu), "LoadingFinished")]
    public static void LoadingFinished(MainMenu __instance)
    {
        if (initDone)
        {
            return;
        }
        InitializeSettings();
        initDone = true;
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CampaignHUD), "Init")]
    public static void SetObjective(CampaignHUD __instance)
    {
        objectiveCard = __instance.ObjectiveCard;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CampaignResultScreen), "OnEnable")]
    public static bool OnEnable_CampaignResultScreen(CampaignResultScreen __instance)
    {
        RuntimePlayer player = dataManager.PlayersData.LocalPlayers[0];

        player.CharacterMatchData.Character = CharacterCodename.SpongeBob;

        return true;
    }

    public static void ObjectiveMessage(string message, string viewer)
    {
        objectiveCard.SetObjective(message, viewer, "");

    }


    public static void InitializeSettings()
    {
        List<ItemInfo> itemInfoList = uiManager.ItemsInfo.ItemInfoList;
        for (int i = 0; i < itemInfoList.Count; i++)
        {
            EffectDelegates.itemDictionary.Add(itemInfoList[i].ItemName.ToLower(), itemInfoList[i]);

        }
        foreach (KeyValuePair<string, ItemInfo> entry in EffectDelegates.itemDictionary)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }

        validCharacters = Enum.GetValues(typeof(CharacterCodename))
    .Cast<CharacterCodename>()
    .Where(codename => !new[]
    {
            CharacterCodename.Undefined,
            CharacterCodename.CharacterBase,
            CharacterCodename.MobBase,
            CharacterCodename.BossBase,
            CharacterCodename.Plankton,
            CharacterCodename.Goddard,
            CharacterCodename.Tommy,
            CharacterCodename.Spunky,
            CharacterCodename.VladClone,
            CharacterCodename.Norah,
            CharacterCodename.Splinter,
            CharacterCodename.Gary,
            CharacterCodename.Gaz,
            CharacterCodename.CabbageMerchant,
            CharacterCodename.HughNeutron,
            CharacterCodename.PowderedToastMan,
            CharacterCodename.Frida,
            CharacterCodename.MrsPuff,
            CharacterCodename.Computer


    }.Contains(codename))
    .ToList();

        List<BarkData> barks = new List<BarkData>();

        // Initialize a LocalizedString with the table name and entry reference
        UnityEngine.Localization.LocalizedString localizedString = new UnityEngine.Localization.LocalizedString
        {
            TableReference = "c948a787-94a1-c214-f80d-4e3e6ceb2322",
            TableEntryReference = "10704560128"
        };



        BarkData barkData = new BarkData
        {
            Duration = 3,
            LocalizationID = localizedString,
            UseAudioDuration = false,
        };

        barks.Add(barkData);


        barkDataList = barks;

        // Find all BarkHUD components in all scenes, including inactive objects
        var barkHUDs = UnityEngine.Object.FindObjectsOfType<BarkHUD>(true);

        if (barkHUDs.Length > 0)
        {


            BarkHUD barkHUD = barkHUDs[0];
            barkHudData = barkHUDs[0]; //Grab the BarkHUD
            barkHUD.PlayBarkSequence(barks, false, false);

            var textMeshProUGUIComponents = barkHUD.gameObject.GetComponentsInChildren<TextMeshProUGUI>(true);

            foreach (TextMeshProUGUI textComponent in textMeshProUGUIComponents)
            {
                Debug.LogWarning("Found TextComponent: " + textComponent.gameObject.transform.parent.name);
                // Example action: Set the text of each TextMeshProUGUI component
                if (textComponent.gameObject.transform.parent.name == "BarkBackground")
                {
                    textComponent.text = "Initializing...";
                    barkTMP = textComponent;
                }


                // You can also set up or modify other properties as needed
            }
        }
        else
        {
            Debug.LogError("No BarkHUD components found in any scene.");
        }

        barkLoaded = true;

        try
        {
            client = ControlClient.Instance;
            new Thread(new ThreadStart(client.NetworkLoop)).Start();
            new Thread(new ThreadStart(client.RequestLoop)).Start();
        }
        catch (Exception e)
        {
            mls.LogInfo($"CC Init Error: {e.ToString()}");
        }

        mls.LogInfo($"Crowd Control Initialized");

    }

    void teleportOffStage(float add, int playerIndex)
    {
        CharacterManager characterManager = matchManager.GetCharacterManager(playerIndex);
        FPVector2 zero = FPVector2.Zero;
        Vector3 currentPositon = characterManager.CenterReferencePoint.gameObject.transform.position;
        float newX;
        float newY;
        if (characterManager != null)
        {
            //Vector3 position = 
            //zero = new FPVector2(position.x.ToFP(), position.y.ToFP());
        }
        if (currentPositon.x > 0)
        {
            newX = currentPositon.x + add;


            zero = new FPVector2(newX.ToFP(), currentPositon.y.ToFP());
        }
        if (currentPositon.x < 0)
        {
            newX = currentPositon.x - add;


            zero = new FPVector2(newX.ToFP(), currentPositon.y.ToFP());

        }

        CommandTeleportCharacter commandTeleportCharacter = new CommandTeleportCharacter();
        commandTeleportCharacter.CharacterIndex = 0;
        commandTeleportCharacter.FacingDirection = 1;
        commandTeleportCharacter.Position = zero;
        dataManager.SendCommand(commandTeleportCharacter);
    }
    public static void barkMessage(string message, string viewer)
    {
        if (!barkLoaded)
        {
            return;
        }

        string viewerFixed = null;
        
        if (viewer != "CROWD CONTROL")
        {
            viewerFixed = $"<color=#2dfa78>{viewer}: </color>Sent ";
        }
        else
        {
            viewerFixed = $"<color=#F5CC36>{viewer}: </color> ";
        }



        barkHudData.PlayBarkSequence(barkDataList, false, false);
        barkTMP.text = viewerFixed + message;
    }


    static IEnumerator getVersions()
    {
        version.Clear();
        //HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_vercheck</size>");

        yield return new WaitForSeconds(0.5f);

        foreach (var versionNum in version)
        {
            mls.LogError($"{versionNum.Key} is running LC Crowd Control version {versionNum.Value}");
        }

    }

    static IEnumerator getTermVersions()
    {
        version.Clear();
        //HUDManager.Instance.AddTextToChatOnServer($"<size=0>/cc_vercheck</size>");

        yield return new WaitForSeconds(0.5f);
    }

}