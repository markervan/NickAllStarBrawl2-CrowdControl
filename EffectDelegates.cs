//this project is a retrofit, it should NOT be used as part of any example - kat

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using ConnectorLib.JSON;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using System.Linq;
using Cinemachine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace BepinControl;

public delegate EffectResponse EffectDelegate(ControlClient client, EffectRequest req);




public class EffectDelegates
{

    public static Dictionary<string, ItemInfo> itemDictionary = new Dictionary<string, ItemInfo>();
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
    public static CampaignManager campaignManager
    {
        get
        {
            return GameManager.Instance.MatchManager.CampaignManager;
        }
    }
    public static InputManager inputManager
    {
        get
        {
            return GameManager.Instance.InputManager;
        }
    }
    public static SceneLoadManager sceneLoadManager
    {
        get
        {
            return GameManager.Instance.SceneLoadManager;
        }
    }
    public static uint msgid = 0;

    public static uint givedelay = 0;

    #region effects 
    public static EffectResponse Damage50(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {

            if (Mod.hudDoneLoading)
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    

                    Mod.barkMessage("Added +50 Damage!", req.viewer);
                    Mod.ObjectiveMessage(req.viewer, "Added +50 Damage!");

                    CommandDealDamage command = new CommandDealDamage
                    {
                        CharacterIndex = 0,
                        Damage = 50,
                        Operation = MathOperationMethod.Add
                    };
                    dataManager.SendCommand(command);
                });
                
            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse Damage100(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {

            if (Mod.hudDoneLoading)
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    Mod.barkMessage("Added +100 Damage!" , req.viewer);

                    CommandDealDamage command = new CommandDealDamage
                    {
                        CharacterIndex = 0,
                        Damage = 100,
                        Operation = MathOperationMethod.Add
                    };
                    dataManager.SendCommand(command);
                });

            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse Heal50(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {

            if (Mod.hudDoneLoading)
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    Mod.barkMessage("Added +50 Healing!", req.viewer);

                    CommandDealDamage command = new CommandDealDamage
                    {
                        CharacterIndex = 0,
                        Damage = 50,
                        Operation = MathOperationMethod.Substract
                    };
                    dataManager.SendCommand(command);
                });

            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse Heal100(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {

            if (Mod.hudDoneLoading)
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    Mod.barkMessage("Added +100 Healing!", req.viewer);

                    CommandDealDamage command = new CommandDealDamage
                    {
                        CharacterIndex = 0,
                        Damage = 100,
                        Operation = MathOperationMethod.Substract
                    };
                    dataManager.SendCommand(command);
                });

            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse SpawnItem(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            if (Mod.hudDoneLoading &&
            dataManager.CurrentStageLayoutData.Layout != StageLayout.Extra1 &&
            dataManager.CurrentStageLayoutData.Layout != StageLayout.Extra2 &&
            dataManager.CurrentStageLayoutData.Layout != StageLayout.Extra3 &&
            dataManager.CurrentStageLayoutData.Layout != StageLayout.Extra4 &&
            dataManager.MatchData.StageID != SceneID.CampaignHub)
            {
                Mod.barkMessage("Spawning Random Item!", req.viewer);

                


                var randomKey = itemDictionary.Keys.ElementAt(UnityEngine.Random.Range(0, itemDictionary.Count));

                List<CharacterManager> currentCharacterManagers = matchManager.CurrentCharacterManagers;
                List<int> list = new List<int>();
                for (int i = 0; i < currentCharacterManagers.Count; i++)
                {
                    list.Add(currentCharacterManagers[i].Data.CharacterIndex);
                }
                CharacterManager characterManager = matchManager.GetCharacterManager(list[0]);

                Vector3 position = characterManager.CenterReferencePoint.gameObject.transform.position;

                // Remove the if statement and directly access the item
                ItemInfo itemByName = itemDictionary[randomKey];

                // If the item has complex data, create and send the command
                CommandOnComplexItemSpawn command = new CommandOnComplexItemSpawn
                {
                    ItemData = itemByName.ComplexItemData,
                    CharacterIndex = 0,
                    Spawnposition = new FPVector3(position.x.ToFP(), position.y.ToFP())
                };
                dataManager.SendCommand(command);
            }
            else
            {
                status = EffectStatus.Retry;
            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse SwapCharacterRandom(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
                Mod.barkMessage("Swapping to Random Character!", req.viewer);
                Mod.freecamera = null;
                Mod.headBone = null;
                Mod.fpsOn = false;

                matchManager.CameraManager.UpdateCamera(CameraManager.AvailableCameras.StageCamera);

                var randomCharacter = Mod.validCharacters[UnityEngine.Random.Range(0, Mod.validCharacters.Count)];

                CommandChangeCharacter command = new CommandChangeCharacter
                {
                    Character = (int)randomCharacter,
                    CharacterIndex = 0,
                    Skin = 0,
                    
                };

                dataManager.SendCommand(command);
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse EffectSleep(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {

            if (Mod.hudDoneLoading)
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    Mod.barkMessage("Added Effect: Sleep!", req.viewer);

                    CommandAddStatusEffect command = new CommandAddStatusEffect
                    {
                        CharacterIndex = 0,
                        StatusEffectIndex = (int)StatusEffects.Sleep,
                        AddTo = 1
                        

                    };
                    dataManager.SendCommand(command);
                });

            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse EffectSlowDown(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {

            if (Mod.hudDoneLoading)
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    Mod.barkMessage("Added Effect: Slow Down!", req.viewer);

                    CommandAddStatusEffect command = new CommandAddStatusEffect
                    {
                        CharacterIndex = 0,
                        StatusEffectIndex = (int)StatusEffects.SlowdownCampaign,
                        Pool = null,
                        AddTo = 1,

                    };
                    dataManager.SendCommand(command);
                });

            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    public static EffectResponse EffectConfuse(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {

            if (Mod.hudDoneLoading)
            {
                Mod.ActionQueue.Enqueue(() =>
                {
                    Mod.barkMessage("Added Effect: Confuse!", req.viewer);

                    CommandAddStatusEffect command = new CommandAddStatusEffect
                    {
                        CharacterIndex = 0,
                        StatusEffectIndex = (int)StatusEffects.Confuse,
                        Pool = null,
                        AddTo = 1,
                    };
                    dataManager.SendCommand(command);
                });

            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse EffectKillPlayer(ControlClient client, EffectRequest req)
    {
        Debug.Log("Test Effect");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            Mod.barkMessage("Kill Player!", req.viewer);

            teleportOffStage(1000, 0);
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse EffectKillEnemies(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        bool validRun = false;

        try
        {
            List<CharacterManager> currentCharacterManagers = matchManager.CurrentCharacterManagers;
            List<int> list = new List<int>();
            

            

            for (int i = 0; i < currentCharacterManagers.Count; i++)
            {
                if (currentCharacterManagers[i].Data.CharacterIndex != 0 && currentCharacterManagers[i].firstActivation && currentCharacterManagers[i].CharacterRenderer.IsVisible) 
                {
                    list.Add(currentCharacterManagers[i].Data.CharacterIndex);
                    
                }

                // Optional: modify the condition if you need a different number of enemies
                if (list.Count > 2)
                {
                    validRun = true;
                    break;
                }
            }

            // Only continue if the run is valid (3 or more enemies found)
            if (validRun)
            {
                Mod.barkMessage("Kill On-Screen Mobs!", req.viewer);
                foreach (int characterManagerIndex in list)
                {
                    teleportOffStage(1000, characterManagerIndex);
                }

                validRun = false;
                list.Clear();
                //currentCharacterManagers.Clear();

                status = EffectStatus.Success;
            }
            else
            {
                status = EffectStatus.Retry;
            }
        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static EffectResponse EndMatch(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            if(dataManager.MatchData.StageID != SceneID.CampaignHub)

            Mod.barkMessage("End Match!", req.viewer);
            CommandEndMatch command = new CommandEndMatch
            {
                EndState = EndMatchStates.EndAnimation,
            };

            dataManager.SendCommand(command);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    /*public static EffectResponse TestEffect(ControlClient client, EffectRequest req)
    {
        Debug.Log("Test Effect");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            Mod.barkMessage("Killing all!", req.viewer);

            CampaignHUD campaignHUD = campaignManager.CampaignHUD;

            campaignHUD.ToggleBlackscreen(true);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }*/

    #endregion
    
    public static EffectResponse TestEffect(ControlClient client, EffectRequest req)
    {
        Debug.Log("Test Effect");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            var randomCharacter = Mod.validCharacters[UnityEngine.Random.Range(0, Mod.validCharacters.Count)];

            sceneLoadManager.MatchPreloader.CleanMemoryCharacter();
            for (int j = 0; j < dataManager.PlayersData.LocalPlayers.Count; j++)
            {
                if (dataManager.PlayersData.LocalPlayers[j].PlayerIndex == dataManager.Player1Index)
                {
                    dataManager.PlayersData.LocalPlayers[j].CharacterMatchData.Character = randomCharacter;
                    dataManager.PlayersData.LocalPlayers[j].CharacterMatchData.Skin = 0;
                }
            }
            sceneLoadManager.LoadOfflineMatch(null, false);

            

            /*RuntimePlayer runtimePlayer = dataManager.PlayersData.LocalPlayers[0];
            RuntimePlayer defaultLocalCoopPlayer = matchManager.CampaignManager.DefaultLocalCoopPlayer;
            defaultLocalCoopPlayer.CharacterMatchData = runtimePlayer.CharacterMatchData;
            defaultLocalCoopPlayer.IsSpectator = runtimePlayer.IsSpectator;
            defaultLocalCoopPlayer.PlayerIndex = runtimePlayer.PlayerIndex;
            defaultLocalCoopPlayer.PlayerSettings = runtimePlayer.PlayerSettings;
            defaultLocalCoopPlayer.PlayerIndex = dataManager.PlayersData.GetAvailablePlayerIndex();

            defaultLocalCoopPlayer.CharacterMatchData.Nickname = "FREAKBOB";
            defaultLocalCoopPlayer.CharacterMatchData.Character = CharacterCodename.SpongeBob;

            dataManager.PlayersData.LocalPlayers.Add(defaultLocalCoopPlayer);
            dataManager.CurrentQuantumGame.SendPlayerData(defaultLocalCoopPlayer.PlayerIndex, defaultLocalCoopPlayer);



            CommandSpawnPlayerCharacter player = new CommandSpawnPlayerCharacter
            {
                PlayerRef = defaultLocalCoopPlayer.PlayerIndex,

                SpawnContext = CharacterSpawnContext.RuntimeCharacter,
            };
            dataManager.SendCommand(player);*/

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }

    #region timedeffects
    //Timed Effects
    public static EffectResponse EnhanceDash(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.ENHANCEDASH)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.ENHANCEDASH, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    public static EffectResponse CameraTopView(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 10000;

        if (TimedThread.isRunning(TimedType.CAMERABOTTOMVIEW)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.CAMERATOPVIEW)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.FIRSTPERSON)) return new EffectResponse(req.ID, EffectStatus.Retry, "");


        new Thread(new TimedThread(req.ID, TimedType.CAMERATOPVIEW, dur).Run).Start();

        Mod.barkMessage("Camera set to Top View!", req.viewer);
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    public static EffectResponse CameraBottomView(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 10000;
        if (TimedThread.isRunning(TimedType.CAMERABOTTOMVIEW)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.CAMERATOPVIEW)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.FIRSTPERSON)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.CAMERABOTTOMVIEW, dur).Run).Start();
        Mod.barkMessage("Camera set to Bottom View!", req.viewer);
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    public static EffectResponse CameraFirstPerson(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 10000;
        if (TimedThread.isRunning(TimedType.CAMERABOTTOMVIEW)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.CAMERATOPVIEW)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.FIRSTPERSON)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.FIRSTPERSON, dur).Run).Start();
        Mod.barkMessage("First Person Mode!", req.viewer);
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    public static EffectResponse EffectSuperSize(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 20000;

        if (TimedThread.isRunning(TimedType.STATUSTINY)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.STATUSGIANT)) return new EffectResponse(req.ID, EffectStatus.Retry, "");


        new Thread(new TimedThread(req.ID, TimedType.STATUSGIANT, dur).Run).Start();
        Mod.barkMessage("Super Size!", req.viewer);
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    public static EffectResponse EffectTinySize(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 20000;

        if (TimedThread.isRunning(TimedType.STATUSTINY)) return new EffectResponse(req.ID, EffectStatus.Retry, "");
        if (TimedThread.isRunning(TimedType.STATUSGIANT)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        new Thread(new TimedThread(req.ID, TimedType.STATUSTINY, dur).Run).Start();
        Mod.barkMessage("Super Tiny!", req.viewer);
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    #endregion
    //Custom Methods


    public static void teleportOffStage(float add, int playerIndex)
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
        commandTeleportCharacter.CharacterIndex = playerIndex;
        commandTeleportCharacter.FacingDirection = 1;
        commandTeleportCharacter.Position = zero;
        dataManager.SendCommand(commandTeleportCharacter);
    }
    public static void setProperty(System.Object a, string prop, System.Object val)
    {
        var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
        f.SetValue(a, val);
    }

    public static System.Object getProperty(System.Object a, string prop)
    {
        var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
        return f.GetValue(a);
    }

    public static void setSubProperty(System.Object a, string prop, string prop2, System.Object val)
    {
        var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
        var f2 = f.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
        f2.SetValue(f, val);
    }

    public static void callSubFunc(System.Object a, string prop, string func, System.Object val)
    {
        callSubFunc(a, prop, func, new object[] { val });
    }

    public static void callSubFunc(System.Object a, string prop, string func, System.Object[] vals)
    {
        var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);


        var p = f.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        p.Invoke(f, vals);

    }

    public static void callFunc(System.Object a, string func, System.Object val)
    {
        callFunc(a, func, new object[] { val });
    }

    public static void callFunc(System.Object a, string func, System.Object[] vals)
    {
        var p = a.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic);
        p.Invoke(a, vals);

    }

    public static System.Object callAndReturnFunc(System.Object a, string func, System.Object val)
    {
        return callAndReturnFunc(a, func, new object[] { val });
    }

    public static System.Object callAndReturnFunc(System.Object a, string func, System.Object[] vals)
    {
        var p = a.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic);
        return p.Invoke(a, vals);

    }

}