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
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using ConnectorLib.JSON;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.HID;

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

    public static List<CharacterCodename> randomChar = new List<CharacterCodename>();
    public static bool lockedCharacter = false;


    public static bool fpsOn = false;
    public static Transform headBone; // Class-level variable to store the head bone
    public static FreeCameraSystem freecamera; // Class-level variable to store the camera system


    public static bool hudDoneLoading = false;
    public static bool initDone = false;
    public static bool barkLoaded = false;
    public static List<CharacterCodename> validCharacters;
    public static List<CharacterCodename> validBrawlers = new List<CharacterCodename>
    {
        CharacterCodename.SpongeBob,
        CharacterCodename.Patrick,
        CharacterCodename.Squidward,
        CharacterCodename.MechaPlankton,
        CharacterCodename.ElTigre,
        CharacterCodename.Rocko,
        CharacterCodename.Jimmy,
        CharacterCodename.Lucy,
        CharacterCodename.Dagget,
        CharacterCodename.Garfield,
        CharacterCodename.Aang,
        CharacterCodename.Korra,
        CharacterCodename.Azula,
        CharacterCodename.Raphael,
        CharacterCodename.Donatello,
        CharacterCodename.April,
        CharacterCodename.Danny,
        CharacterCodename.Ember,
        CharacterCodename.GrandmaGertie,
        CharacterCodename.Gerald,
        CharacterCodename.Nigel,
        CharacterCodename.Lucy,
        CharacterCodename.Zim,
        CharacterCodename.Jenny,
        CharacterCodename.Reptar,
        CharacterCodename.RenStimpy,
        CharacterCodename.Sushi,
        CharacterCodename.Stevia,
        CharacterCodename.Teacher,
        CharacterCodename.Headbanger,
    };

    //pos and rot for head fov
    public static int posX;
    public static int posY;
    public static int posZ;
    public static int rotX;
    public static int rotY;
    public static int rotZ;
    public static int rotYinvert;

    //configuration bools
    public static bool instaShieldBreak;
    public static bool noSlimeUltimate;



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
    public static CampaignManager campaignManager
    {
        get
        {
            return GameManager.Instance.MatchManager.CampaignManager;
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
    static public GameManager gameManager
    {
        get
        {
            return GameManager.Instance;
        }
    }
    public static SceneLoadManager sceneLoadManager
    {
        get
        {
            return GameManager.Instance.SceneLoadManager;
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
        if (gameManager.CurrentGameContext == GameContext.Campaign)
        {
            if (!hudDoneLoading)
            {
                return;
            }

            if (matchManager.CampaignManager.InMatch)
            {
                //Debug.Log("in match");
            }
            if (matchManager.CampaignManager.InLevelSelector)
            {
                //Debug.Log("in levelselector");
            }

        }
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            Quantum.Frame frame = dataManager.VerifiedFrame;
            CombatData combatData = frame.SimulationConfig.GetCombatData();
            Debug.Log("BlockHPDecreaseRate " + combatData.BlockHPDecreaseRate);
            Debug.Log("BlockHPRecoveryRate " + combatData.BlockHPRecoveryRate);

            foreach (var entry in randomChar)
            {
                Debug.Log(entry);
            }
            
        }

        // If the head bone has been found, update the camera offset every frame
        if (headBone != null && freecamera != null)
        {
            // Position
            float headX = headBone.transform.position.x;
            float headY = headBone.transform.position.y;
            float headZ = headBone.transform.position.z;

            // Rotation 
            Vector3 headEuler = headBone.transform.rotation.eulerAngles; //Bone Rotation

            // Subtract 99 degrees from the Y rotation
            float adjustedRotX = headEuler.x;
            float adjustedRotY = headEuler.y - 79;
            float adjustedRotZ = headEuler.z;

            //Invert Y Rotation when Character is looking to the opposite Side
            if(matchManager.CurrentCharacterManagers[0].lastFacingDirection == -1)
            {
                adjustedRotY = -(headEuler.y - 82); //Substracts float from Rotation
            }
            //CombatData
            
            Quaternion newRotation = Quaternion.Euler(headEuler.x, adjustedRotY, headEuler.z);
            freecamera.gameObject.transform.position = new Vector3(headX, headY, headZ);
            freecamera.gameObject.transform.rotation = newRotation;

        }
    }
    public float AdjustRotation(float currentRotation, float adjustment)
    {
        return currentRotation + adjustment;
    }
    public static void HandleFirstPersonView(CharacterManager characterObject)
    {
        Transform characterTransform = characterObject.gameObject.transform;
        string boneName = "";
        CharacterCodename codename = characterObject.Codename;

        switch (codename)
        {
            case CharacterCodename.SpongeBob:

                boneName = "head_C0_head_03_jnt";
                break;
            case CharacterCodename.Patrick:

                boneName = "neck_C0_neck_jnt";
                break;
            case CharacterCodename.Squidward:

                boneName = "neck_C0_head_jnt";
                break;
            case CharacterCodename.MechaPlankton:

                boneName = "spine_C0_4_jnt";
                break;
            default:

                boneName = "head_jnt";
                break;
        }


        headBone = FindHeadBone(characterTransform, boneName);

    }

    public static Transform FindHeadBone(Transform parent, string boneName)
    {
        foreach (Transform child in parent)
        {
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


    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CommandSpawnEntity), "Execute")]
    static unsafe bool Execute(CommandSpawnEntity __instance, Quantum.Frame frame)
    {
        if(__instance.SpawnPointID == "custom1")
        {
            AssetRefEntityPrototype interactableEntityRef = frame.SimulationConfig.GetInteractableEntityRef(frame, __instance.InteractableID);
            EntityRef entity = frame.Create(interactableEntityRef);
            EntityData entityData = Utils.GetEntityData(frame, entity);
            //CustomSpawnPoint* customSpawnPoint = Utils.GetCustomSpawnPoint(frame, __instance.SpawnPointID);

            //Quantum.Frame frame = dataManager.VerifiedFrame;

            CharacterManager characterManager = matchManager.CurrentCharacterManagers[0];

            EntityData characterEntity = Utils.GetEntityData(frame, characterManager.CharacterEntity.EntityRef);

            FPVector3 basePosition = characterEntity.transform->Position;


            entityData.transform->Position = basePosition;
        }
        else
        {
            return true;
        }

        return false;
    }

    [HarmonyPatch(typeof(CampaignManager), "StartNextLevel")]
    [HarmonyPrefix]
    static void StartNextLevel(CampaignManager __instance)
    {
        if (randomChar == null || randomChar.Count == 0)
        {
            UnityEngine.Debug.LogError("randomChar list is empty or null.");
            return;
        }
        if (lockedCharacter)
        {
            return;
        }

        if (
            dataManager.CurrentStageLayoutData.Layout == StageLayout.Extra1 ||
            dataManager.CurrentStageLayoutData.Layout == StageLayout.Extra2 ||
            dataManager.CurrentStageLayoutData.Layout == StageLayout.Extra3 ||
            dataManager.CurrentStageLayoutData.Layout == StageLayout.Extra4 ||
            dataManager.MatchData.StageID == SceneID.CampaignHub)
        {
            return;
        }

        UnityEngine.Debug.Log($"Changing to character: {randomChar[0]}");

        ChangeCharacter(randomChar[0]);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ComplexItemData), "Initialize")]
    
    static unsafe bool Initialize(ComplexItemData __instance, EntityData __result, Quantum.Frame frame, EntityRef itemEntity, int characterOwnerIndex = -1)
    {

        if(gameManager.CurrentGameContext != GameContext.Campaign)
        {
            return true;
        }

        Debug.Log("INIT COMPLEX");
        __instance.InitStates(frame);
        CharacterTeam team = CharacterTeam.None;
        EntityData character = Utils.GetCharacter(frame, characterOwnerIndex);
        if (character.character != null)
        {
            //team = character.character->team;
        }
        ComplexItem* ptr;
        if (frame.Unsafe.TryGetPointer<ComplexItem>(itemEntity, out ptr))
        {
            if (character.character != null)
            {
                ptr->owner = character.entity;
            }
            ptr->active = true;
            ptr->visible = true;
            ptr->itemData = __instance;
            ptr->itemType = __instance.ItemType;
            ptr->lifeTime = __instance.Lifetime;
            ptr->droppedOnHurt = __instance.DroppedOnHurt;
            ptr->originalOwnerIndex = characterOwnerIndex;
            ptr->characterOwnerIndex = characterOwnerIndex;
            ptr->grabbedOnTouch = __instance.GrabbedOnTouch;
            ptr->grabbableOnlyByOwner = __instance.GrabbableOnlyByOwner;
            ptr->KeepOwnerAfterStopGrab = __instance.KeepOwnerAfterStopGrab;
        }
        else
        {
            ComplexItem value = default(ComplexItem);
            if (character.character != null)
            {
                value.owner = character.entity;
            }
            value.active = true;
            value.visible = true;
            value.itemData = __instance;
            value.itemType = __instance.ItemType;
            value.lifeTime = __instance.Lifetime;
            value.droppedOnHurt = __instance.DroppedOnHurt;
            value.originalOwnerIndex = characterOwnerIndex;
            value.characterOwnerIndex = characterOwnerIndex;
            value.grabbedOnTouch = __instance.GrabbedOnTouch;
            value.grabbableOnlyByOwner = __instance.GrabbableOnlyByOwner;
            value.KeepOwnerAfterStopGrab = __instance.KeepOwnerAfterStopGrab;
            frame.Set<ComplexItem>(itemEntity, value);
        }
        Interactable value2 = default(Interactable);
        value2.active = true;
        value2.interactableDataRef = __instance.InteractableData;
        InteractableData interactableData;
        if (frame.TryFindAsset<AssetRefInteractableData, InteractableData>(__instance.InteractableData, out interactableData))
        {
            value2.canBeGrabbed = interactableData.CanBeGrabbed;
        }
        frame.Set<Interactable>(itemEntity, value2);
        frame.Set<ItemStateMachine>(itemEntity, new ItemStateMachine
        {
            currentStateIndex = -1,
            currentStateIndexOverride = -1
        });
        AABBCollider value3 = new AABBCollider
        {
            type = AABBColliderType.Item,
            state = AABBColliderState.Dynamic
        };
        frame.Set<AABBCollider>(itemEntity, value3);
        int facingDirection = 1;
        if (character.physicsBody != null)
        {
            facingDirection = character.physicsBody->facingDirection;
        }
        frame.Set<PhysicsBody>(itemEntity, new PhysicsBody
        {
            active = true,
            facingDirection = facingDirection
        });
        frame.Set<Combat>(itemEntity, new Combat
        {
            active = true,
            entityHitFilter = __instance.EntityHitFilter,
            characterTeamIndex = -1,
            team = team,
            currentHitLagFrames = -1,
            healthMode = __instance.HealthMode,
            damageTaken = __instance.InitialDamage
        });
        EntityData entityData = Utils.GetEntityData(frame, itemEntity);
        entityData.physicsBody->UpdateRotationPivot(frame, entityData, __instance.PivotPoint);
        GertieSpawnedEntity.TryToInit(frame, entityData);
        __instance.ExecuteOnSpawnBehaviours(frame, entityData);

        __result = entityData;

        return false;

    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SimulationConfig), "GetCombatData")]
    static bool GetCombatData(SimulationConfig __instance, ref CombatData __result)
    {
        CombatData combatData = __instance.combatData;
        if (instaShieldBreak)
        {
            combatData.BlockHPDecreaseRate = 10;
            combatData.BlockHPRecoveryRate = 0;
        }
        if (noSlimeUltimate)
        {
            combatData.UltimatePointsCost = 0;
        }
        __result = combatData;

        return false;
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CampaignHub), "Initialize")]
    static void Initialize(CampaignHub __instance)
    {
        UnityEngine.Debug.Log("INIT CAMPAIGN");
        GameObject foundObject = GameObjectFinder.FindGameObjectByName("StatuePedestals");
        if (foundObject != null)
        {
            Debug.Log("Found GameObject: " + foundObject.name);
            foundObject.SetActive(true);
            int index = 0;
            // Loop through each direct child of the parent GameObject
            for (int i = 0; i < foundObject.transform.childCount; i++)
            {
                //index++;
                //PedestalAnchor child = parent.transform.GetChild(i).gameObject.GetComponent<PedestalAnchor>();
                var interactableEventHandler = foundObject.transform.GetChild(i).gameObject.GetComponent<InteractableEventHandler>();
                if (interactableEventHandler != null && interactableEventHandler.ExecuteEvents[0].Actions[0] is EventActionOpenShop eventActionOpenShop)
                {
                    eventActionOpenShop.statuePedestal = foundObject.transform.GetChild(i).gameObject.GetComponent<PedestalAnchor>();
                }
                //Debug.Log("Child name: " + child.name);}
                break;
            }
            
        }
        else
        {
            Debug.Log("GameObject not found.");
        }
    }

    public static class GameObjectFinder
    {
        public static GameObject FindGameObjectByName(string name)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                // Load all root objects in the scene, including inactive ones
                foreach (GameObject rootObj in scene.GetRootGameObjects())
                {
                    GameObject result = FindInHierarchy(rootObj, name);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        private static GameObject FindInHierarchy(GameObject parent, string name)
        {
            // Check if the current object matches
            if (parent.name == name)
                return parent;

            // Search through the object's children recursively
            foreach (Transform child in parent.transform)
            {
                GameObject result = FindInHierarchy(child.gameObject, name);
                if (result != null)
                    return result;
            }

            return null;
        }
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(EventActionOpenShop), "Execute")]
    static bool Execute(EventActionOpenShop __instance)
    {
        //BaseShow(__instance);
        UnityEngine.Debug.Log("PREFIX");
        int currentCharacterIndex = matchManager.StageEventsManager.CurrentCharacterIndex;
        switch (__instance.Shop)
        {
            case CampaignShops.PerksShop:
                campaignManager.CampaignHub.PerksShopScreen.ShowScreen(currentCharacterIndex);
                //campaignManager.CampaignHub.BrawlerShopScreen.ShowScreen(currentCharacterIndex);
                //campaignManager.CampaignHub.StatuesShop.ShowScreen(currentCharacterIndex, __instance.statuePedestal, true);
                return false;
            case CampaignShops.CharacterShop:

            case CampaignShops.StatuePedestal:

                campaignManager.CampaignHub.StatuesShop.ShowScreen(currentCharacterIndex, __instance.statuePedestal, true);
                break;
            case CampaignShops.BrawlerShop:

                campaignManager.CampaignHub.BrawlerShopScreen.ShowScreen(currentCharacterIndex);
                return false;
            case CampaignShops.DecorationShop:

                campaignManager.CampaignHub.DecorationsShop.ShowScreen(currentCharacterIndex, null, false);
                return false;
            case CampaignShops.PowerUpsShop:
                {
                    InteractableEventHandler[] array = UnityEngine.Object.FindObjectsOfType<InteractableEventHandler>();
                    bool flag = false;
                    foreach (InteractableEventHandler interactableEventHandler in array)
                    {
                        if (interactableEventHandler.InteractableId == __instance.InteractableID)
                        {
                            campaignManager.CampaignHUD.PowerUpsShop.ShowScreen(currentCharacterIndex, __instance.Vendor, __instance.ExclusiveVendor, interactableEventHandler, __instance.BannedPowerUps, __instance.BannedConsumables);
                            return false;
                        }
                    }
                    if (flag)
                    {
                        campaignManager.CampaignHUD.PowerUpsShop.ShowScreen(currentCharacterIndex, __instance.Vendor, __instance.ExclusiveVendor, null, __instance.BannedPowerUps, __instance.BannedConsumables);
                        return false;
                    }
                    break;
                }
            case CampaignShops.DifficultiesShop:
                campaignManager.CampaignHub.DifficultyPresets.ShowScreen(currentCharacterIndex);
                return false;
            case CampaignShops.TradingShop:
                {
                    InteractableEventHandler[] array3 = UnityEngine.Object.FindObjectsOfType<InteractableEventHandler>();
                    CampaignHub campaignHub = campaignManager.CampaignHub;
                    bool flag2 = false;
                    foreach (InteractableEventHandler interactableEventHandler2 in array3)
                    {
                        if (interactableEventHandler2.InteractableId == __instance.InteractableID)
                        {
                            campaignHub.TradingShopScreen.ShowScreen(currentCharacterIndex, __instance.Vendor, __instance.ExclusiveVendor, interactableEventHandler2, null, null);
                            return false;
                        }
                    }
                    if (flag2)
                    {
                        campaignHub.TradingShopScreen.ShowScreen(currentCharacterIndex, __instance.Vendor, __instance.ExclusiveVendor, null, null, null);
                    }
                    break;
                }
            default:
                return false;
        }
        return false;



        return false;
    }

    [HarmonyPatch(typeof(MatchHUD), "OnMatchReady")]
    [HarmonyPostfix]
    static void OnMatchReady()
    {
        hudDoneLoading = true;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BrawlerShop), "Init")]
    static bool Init(BrawlerShop __instance)
    {
        int index = 5;
        __instance.characterShopInfo.ShopCharacters.Clear();
        for (int i = 0; i < Mod.validBrawlers.Count; i++)
        {
            index++;

            GameObject newSlot = GameObject.Instantiate(__instance.Slots[5].gameObject);

            newSlot.name = "BrawlerSlot_"+index;

            newSlot.transform.parent = __instance.Slots[5].gameObject.transform.parent.gameObject.transform;
            newSlot.transform.localScale = new Vector3(1, 1, 1);

            BrawlerSlot brawlerSlot = newSlot.GetComponent<BrawlerSlot>();
            brawlerSlot.NavigationIndex = index;

            UINavigator.CustomNavigationData customNavigationData = new UINavigator.CustomNavigationData();

            UINavigator.ElementInfo elementInfo = new UINavigator.ElementInfo
            {
                CustomNavigation = customNavigationData,
                Element = brawlerSlot,
            };

            brawlerSlot.UINavigator.Elements.Add(elementInfo);

            __instance.Slots.Add(brawlerSlot);

            __instance.characterShopInfo.ShopCharacters.Add(Mod.validBrawlers[i]);
        }

        return true;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(StatueVisibilityManager), "SetStatueType")]
    static bool SetStatueType(StatueVisibilityManager __instance, string statueType)
    {
        UnityEngine.Debug.Log("SET");

        __instance.Bronze.SetActive(true);
        __instance.Silver.SetActive(true);
        __instance.Gold.SetActive(true);
        __instance.Slimed.SetActive(true);
        __instance.Cosmic.SetActive(true);
        if (statueType == "Bronze")
        {
            __instance.Bronze.SetActive(true);
            __instance.StatueSize = __instance.Bronze.GetComponentInChildren<MeshRenderer>().bounds.size;
            return false;
        }
        if (statueType == "Silver")
        {
            __instance.Silver.SetActive(true);
            __instance.StatueSize = __instance.Silver.GetComponentInChildren<MeshRenderer>().bounds.size;
            return false;
        }
        if (statueType == "Gold")
        {
            __instance.Gold.SetActive(true);
            __instance.StatueSize = __instance.Gold.GetComponentInChildren<MeshRenderer>().bounds.size;
            return false;
        }
        if (statueType == "Slimed")
        {
            __instance.Slimed.SetActive(true);
            __instance.StatueSize = __instance.Slimed.GetComponentInChildren<MeshRenderer>().bounds.size;
            return false;
        }
        if (!(statueType == "Cosmic"))
        {
            return false;
        }
        __instance.Cosmic.SetActive(true);
        __instance.StatueSize = __instance.Cosmic.GetComponentInChildren<MeshRenderer>().bounds.size;
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(BrawlerShop), "RefreshGrid")]
    static bool RefreshGridpre(BrawlerShop __instance)
    {

        // Null check for ShopCharacters and UnlockedCharacters lists
        if (__instance.characterShopInfo.ShopCharacters == null || __instance.characterShopInfo.UnlockedCharacters == null)
        {
            Debug.LogWarning("ShopCharacters or UnlockedCharacters list is null in characterShopInfo.");
            return false;
        }

        // Loop through ShopCharacters with null checks
        for (int i = 0; i < __instance.characterShopInfo.ShopCharacters.Count; i++)
        {
            // Null check for Slots list and individual Slot elements
            if (__instance.Slots == null || __instance.Slots[i] == null)
            {
                Debug.LogWarning($"Slot {i} is null in BrawlerShop.");
                continue;
            }

            bool unlocked = __instance.characterShopInfo.UnlockedCharacters.Contains(__instance.characterShopInfo.ShopCharacters[i]);
            CharacterCodename character = __instance.characterShopInfo.ShopCharacters[i];

            // Additional null check for character before initializing the slot
            if (character != null)
            {
                __instance.Slots[i].Init(character, __instance.price, unlocked, __instance);
                __instance.Slots[i].ToggleElement(true);
            }
            else
            {
                Debug.LogWarning($"Character at index {i} in ShopCharacters is null.");
            }
        }

        // Handle any remaining slots after ShopCharacters count
        for (int j = __instance.characterShopInfo.ShopCharacters.Count; j < __instance.Slots.Count; j++)
        {
            if (__instance.Slots[j] != null)
            {
                __instance.Slots[j].ToggleElement(false);
            }
            else
            {
                Debug.LogWarning($"1Slot {j} is null in BrawlerShop.");
            }
        }

        // Null check for UINavigator before calling HighlightCurrentElement
        if (__instance.UINavigator != null)
        {
            __instance.UINavigator.HighlightCurrentElement(false);
        }
        else
        {
            Debug.LogWarning("UINavigator is null in BrawlerShop.");
        }

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(InputCheckUtils), "CheckGrabInput")]
    static unsafe bool CheckGrabInput(InputCheckUtils __instance, Quantum.Frame frame, ref EntityData entityData, CombatData combatData, bool upHold, bool downHold, bool leftHold, bool rightHold, ref bool __result)
    {
        if (!entityData.character->detectGrab)
        {
            __result =  false;
            return false;
        }

        frame.Filter<FrameByFrame>().NextUnsafe(out var _, out var component);
        bool flag = false;

        if (component == null && InputBufferSystem.IsButtonJustPressed(frame, entityData.inputBuffer, InputFlag.Grab, combatData.InputSettings.ButtonActionsSettings.GrabBufferFrames))
        {
            Debug.LogWarning("first check passed");
            if (!entityData.physicsBody->grounded && entityData.itemGrab->interactableGrabbedEntity == EntityRef.None)
            {
                Debug.LogWarning("second check passed");
                flag = entityData.character->CheckInteractablesNearby(frame, entityData);
            }

            if (!flag)
            {
                Debug.LogWarning("third check passed");
                if (entityData.itemGrab->interactableGrabbedEntity != EntityRef.None)
                {
                    Debug.LogWarning("fourth check passed");
                    EntityData entityData2 = Utils.GetEntityData(frame, entityData.itemGrab->interactableGrabbedEntity);
                    if (entityData2.interactable->TryThrow(frame, entityData, entityData2, upHold, downHold, leftHold, rightHold, 43))
                    {
                        Debug.LogWarning("five true - check passed");
                        __result = true;
                        return false;
                    }
                }
                else if ((bool)entityData.physicsBody->grounded)
                {
                    Debug.LogWarning("sixth true - check passed");
                    CustomAnimator.SetTrigger(frame, entityData.animator, 43);
                    __result = true;
                    return false;
                }
                else if (!entityData.physicsBody->grounded)
                {
                    Debug.LogWarning("sixth true - check passed");
                    CustomAnimator.SetTrigger(frame, entityData.animator, 43);
                    __result = true;
                    return false;
                    
                }
            }
        }

        if ((bool)entityData.physicsBody->grounded && InputBufferSystem.IsButtonHeld(frame, entityData.inputBuffer, InputFlag.Block, 1) && InputBufferSystem.IsButtonJustPressed(frame, entityData.inputBuffer, InputFlag.LightAttack, combatData.InputSettings.ButtonActionsSettings.LightAttackBufferFrames))
        {
            if (!entityData.physicsBody->grounded && entityData.itemGrab->interactableGrabbedEntity == EntityRef.None)
            {
                flag = entityData.character->CheckInteractablesNearby(frame, entityData);
            }

            if (!flag)
            {
                if (entityData.itemGrab->interactableGrabbedEntity != EntityRef.None)
                {
                    EntityData entityData3 = Utils.GetEntityData(frame, entityData.itemGrab->interactableGrabbedEntity);
                    if (entityData3.interactable->TryThrow(frame, entityData, entityData3, upHold, downHold, leftHold, rightHold, 43))
                    {
                        __result = true;
                        return false;
                    }
                }
                else if ((bool)entityData.physicsBody->grounded)
                {
                    CustomAnimator.SetTrigger(frame, entityData.animator, 43);
                    __result = true;
                    return false;
                }
            }
        }

        __result = false;
        return false;
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
        if(gameManager.CurrentGameContext == GameContext.Campaign)
        {
            return false;
        }
        return true;
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

        if(validBrawlers.Contains<CharacterCodename>(player.CharacterMatchData.Character))
        {
            return true;
        }

        player.CharacterMatchData.Character = CharacterCodename.SpongeBob;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CutscenesLoader), "OnCutsceneComplete")]
    public static bool OnCutsceneComplete(CutscenesLoader __instance)
    {
        RuntimePlayer player = dataManager.PlayersData.LocalPlayers[0];

        if (validBrawlers.Contains<CharacterCodename>(player.CharacterMatchData.Character))
        {
            return true;
        }

        player.CharacterMatchData.Character = CharacterCodename.SpongeBob;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ScaleChange_PowerUp), "OnCreate")]
    public unsafe static bool OnCreate(ScaleChange_PowerUp __instance, Quantum.Frame frame, Quantum.EntityData characterEntityData)
    {
        foreach (EquippedPowerUp ptr in frame.ResolveList<EquippedPowerUp>(characterEntityData.character->currentPowerUps))
        {
            __instance.GetData(frame, characterEntityData);
            ScaleChangePowerUp value = default(ScaleChangePowerUp);
            value.originalScale = characterEntityData.physicsBody->bodyScale;
            FP level = ptr.level;
            value.targetScale = level;
            frame.Set<ScaleChangePowerUp>(characterEntityData.entity, value);
        }
        return false;
    }

    public static void ChangeCharacter(CharacterCodename character)
    {
        sceneLoadManager.MatchPreloader.CleanMemoryCharacter();
        dataManager.PlayersData.LocalPlayers[0].CharacterMatchData.Character = character;
        dataManager.PlayersData.LocalPlayers[0].CharacterMatchData.Skin = 0;

        
        randomChar.RemoveAt(0);
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
            CharacterCodename.Computer,
            CharacterCodename.Norbert


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