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
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using System.Threading.Tasks;
using static Quantum.AirDodgeDropBomb_PowerUp;

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
    public static UIManager uiManager
    {
        get
        {
            return GameManager.Instance.UIManager;
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
                    //Mod.ObjectiveMessage(req.viewer, "Added +50 Damage!");

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
        //status = EffectStatus.Retry;

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
    public static unsafe EffectResponse SpawnItem(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {   
            if (!Mod.hudDoneLoading)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);

            }

            if (
            dataManager.CurrentStageLayoutData.Layout == StageLayout.Extra1 ||
            dataManager.CurrentStageLayoutData.Layout == StageLayout.Extra2 ||
            dataManager.CurrentStageLayoutData.Layout == StageLayout.Extra3 ||
            dataManager.CurrentStageLayoutData.Layout == StageLayout.Extra4)
            {
                status = EffectStatus.NotReady;
                return new EffectResponse(req.ID, status, message);
            }



            Mod.barkMessage("Spawning Random Item!", req.viewer);


            Quantum.Frame frame = dataManager.VerifiedFrame;

            CharacterManager characterManager = matchManager.CurrentCharacterManagers[0];

            EntityData characterEntity = Utils.GetEntityData(frame, characterManager.CharacterEntity.EntityRef);

            var randomKey = itemDictionary.Keys.ElementAt(UnityEngine.Random.Range(0, itemDictionary.Count));

            ItemInfo itemByName = itemDictionary[randomKey];

            FPVector3 position = characterEntity.transform->Position;

            if (frame.TryFindAsset<AssetRefComplexItemData, ComplexItemData>(itemByName.ComplexItemData, out var asset))
            {
                asset.Spawn(frame, position, characterEntity.CharacterIndex);
            }
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
    public static EffectResponse SwapCharacterRandom(ControlClient client, EffectRequest req)
    {
        Debug.Log("Test Effect");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            if (!Mod.hudDoneLoading)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }

            var randomCharacter = Mod.validCharacters[UnityEngine.Random.Range(0, Mod.validCharacters.Count)];

            Mod.barkMessage("Added Effect: Swap to Random Fighter!", req.viewer);
            Mod.randomChar.Add(randomCharacter);

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
    public static EffectResponse EffectKaiju(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        bool validRun = false;

        try
        {
            if (!matchManager.CampaignManager.InMatch)
            {

                status |= EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }


            List<CharacterManager> currentCharacterManagers = matchManager.CurrentCharacterManagers;
            List<int> list = new List<int>();
            for (int i = 0; i < currentCharacterManagers.Count; i++)
            {
                if (currentCharacterManagers[i].Data.CharacterIndex != 0 && currentCharacterManagers[i].firstActivation && currentCharacterManagers[i].CharacterRenderer.IsVisible)
                {
                    list.Add(currentCharacterManagers[i].Data.CharacterIndex);
                }

                if (list.Count > 2)
                {
                    validRun = true;
                    break;
                }
            }

            // Only continue if the run is valid (3 or more enemies found)
            if (validRun)
            {
                Mod.barkMessage("Kaiju Enemies!", req.viewer);
                foreach (int characterManagerIndex in list)
                {
                    CommandAddPowerUp command = new CommandAddPowerUp
                    {
                        CharacterIndex = characterManagerIndex,
                        Level = 7,
                        PowerUpIndex = (int)PowerUps.ScaleChange,
                        
                    };


                    dataManager.SendCommand(command);
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
    public static EffectResponse EffectUltraman(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        bool validRun = false;

        try
        {
            if (!matchManager.CampaignManager.InMatch)
            {

                status |= EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }

            Mod.barkMessage("Ultraman!", req.viewer);
            CommandAddPowerUp command = new CommandAddPowerUp
            {
                CharacterIndex = 0,
                Level = 7,
                PowerUpIndex = (int)PowerUps.ScaleChange,

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
    public static EffectResponse EffectFullMeter(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";
        bool validRun = false;

        try
        {
            if (!matchManager.CampaignManager.InMatch)
            {

                status |= EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }

            Mod.barkMessage("Full Slime Meter!", req.viewer);
            CommandChangeSlimePoints command = new CommandChangeSlimePoints
            {
                CharacterIndex = 0,
                SlimePoints = 3,
                Operation = MathOperationMethod.Add,
                
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
    public static EffectResponse EndMatch(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            if(dataManager.MatchData.StageID == SceneID.CampaignHub || dataManager.MatchData.StageLayout == StageLayout.BotsMinigame)
            {
                status = EffectStatus.Retry; 
                return new EffectResponse(req.ID, status, message);
            }
            if (dataManager.MatchData.StageLayout == StageLayout.TargetsMinigame ||
                dataManager.MatchData.StageLayout == StageLayout.TargetsMinigameAang ||
                dataManager.MatchData.StageLayout == StageLayout.TargetsMinigameAngryBeavers ||
                dataManager.MatchData.StageLayout == StageLayout.TargetsMinigameApril ||
                dataManager.MatchData.StageLayout == StageLayout.TargetsMinigameAzula ||
                dataManager.MatchData.StageLayout == StageLayout.TargetsMinigameDanny ||
                dataManager.MatchData.StageLayout == StageLayout.TargetsMinigameDonatello ||
                dataManager.MatchData.StageLayout == StageLayout.TargetsMinigameElTigre ||
                dataManager.MatchData.StageLayout == StageLayout.TargetsMinigameEmber ||
                dataManager.MatchData.StageLayout == StageLayout.TargetsMinigameGarfield ||
                dataManager.MatchData.StageLayout == StageLayout.TargetsMinigameGerald
                )
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }

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
    public static EffectResponse LeaveCampaign(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            try
            {
                /*if (!matchManager.CampaignManager.InMatch)
                {
                    status = EffectStatus.Retry;
                    return new EffectResponse(req.ID, status, message);
                }*/

                Mod.barkMessage("To the Main Menu!", req.viewer);

                campaignManager.CampaignHUD.RunPauseMenu.campaignManager.SaveCampaign();
                campaignManager.CampaignHUD.RunPauseMenu.GoToMainMenu();
            }
            catch (Exception e)
            {
                status = EffectStatus.Retry;
                Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new EffectResponse(req.ID, status, message);
        }
    public static EffectResponse ShieldBreak1(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            if (!Mod.hudDoneLoading)
            {
                status = EffectStatus.NotReady;
                return new EffectResponse(req.ID, status, message);
            }
            Mod.barkMessage("Insta-Shield Break", req.viewer);

            

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
            //Mod.barkMessage("Killing all!", req.viewer);

            CampaignHUD campaignHUD = campaignManager.CampaignHUD;

            //campaignHUD.ToggleBlackscreen(true);




        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }*/

    #endregion
    public static unsafe EffectResponse AddSlime(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            campaignManager.CurrencyManager.SetCurrency(CampaignCurrency.Slime, 100, MathOperationMethod.Add, false);
            campaignManager.CampaignHUD.UpdateCurrency();
            
            Mod.barkMessage("Slime Currency!", req.viewer);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse AddSplat(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            campaignManager.CurrencyManager.SetCurrency(CampaignCurrency.Splat, 100, MathOperationMethod.Add, false);
            campaignManager.CampaignHUD.UpdateCurrency();

            Mod.barkMessage("Splat Currency!", req.viewer);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse AddBlimp(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            campaignManager.CurrencyManager.SetCurrency(CampaignCurrency.Blimp, 100, MathOperationMethod.Add, false);
            campaignManager.CampaignHUD.UpdateCurrency();

            Mod.barkMessage("Splat Currency!", req.viewer);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse SubSlime(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            campaignManager.CurrencyManager.SetCurrency(CampaignCurrency.Slime, 100, MathOperationMethod.Substract, false);
            campaignManager.CampaignHUD.UpdateCurrency();

            Mod.barkMessage("Remove Slime Currency!", req.viewer);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse SubSplat(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            campaignManager.CurrencyManager.SetCurrency(CampaignCurrency.Splat, 100, MathOperationMethod.Substract, false);
            campaignManager.CampaignHUD.UpdateCurrency();

            Mod.barkMessage("Remove Splat Currency!", req.viewer);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse SubBlimp(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            campaignManager.CurrencyManager.SetCurrency(CampaignCurrency.Blimp, 100, MathOperationMethod.Substract, false);
            campaignManager.CampaignHUD.UpdateCurrency();

            Mod.barkMessage("Remove Splat Currency!", req.viewer);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse TriggerUlt(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            /*if (!matchManager.CampaignManager.InMatch)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }*/
            if (matchManager.InCutscene)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }

            Quantum.Frame frame = dataManager.VerifiedFrame;

            CharacterManager characterManager = matchManager.CurrentCharacterManagers[0];
            
            EntityData entityData = Utils.GetEntityData(frame, characterManager.CharacterEntity.EntityRef);

            CustomAnimator.SetTrigger(frame, entityData.animator, 74);
            Mod.barkMessage("Ultimate!", req.viewer);

            //DelayedMethodCall(10000, TypeCancel.Animation);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse TriggerSpinOut(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            /*if (!matchManager.CampaignManager.InMatch)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }*/
            if (matchManager.InCutscene)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }

            Quantum.Frame frame = dataManager.VerifiedFrame;

            CharacterManager characterManager = matchManager.CurrentCharacterManagers[0];

            EntityData entityData = Utils.GetEntityData(frame, characterManager.CharacterEntity.EntityRef);

            CustomAnimator.SetTrigger(frame, entityData.animator, 54);
            Mod.barkMessage("Spin Out!", req.viewer);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse TriggerTeeter(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            /*if (!matchManager.CampaignManager.InMatch)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }*/
            if (matchManager.InCutscene)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }

            Quantum.Frame frame = dataManager.VerifiedFrame;

            CharacterManager characterManager = matchManager.CurrentCharacterManagers[0];

            EntityData entityData = Utils.GetEntityData(frame, characterManager.CharacterEntity.EntityRef);

            CustomAnimator.SetTrigger(frame, entityData.animator, 76);
            Mod.barkMessage("Teeter!", req.viewer);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse TriggerEntrace(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            /*if (!matchManager.CampaignManager.InMatch)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }*/
            if (matchManager.InCutscene)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }

            Quantum.Frame frame = dataManager.VerifiedFrame;

            CharacterManager characterManager = matchManager.CurrentCharacterManagers[0];

            EntityData entityData = Utils.GetEntityData(frame, characterManager.CharacterEntity.EntityRef);

            CustomAnimator.SetTrigger(frame, entityData.animator, 68);
            Mod.barkMessage("Entrance!", req.viewer);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse TriggerSlimeBurst(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            /*if (!matchManager.CampaignManager.InMatch)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }*/
            if (matchManager.InCutscene)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }

            Quantum.Frame frame = dataManager.VerifiedFrame;

            CharacterManager characterManager = matchManager.CurrentCharacterManagers[0];

            EntityData entityData = Utils.GetEntityData(frame, characterManager.CharacterEntity.EntityRef);

            CustomAnimator.SetTrigger(frame, entityData.animator, 73);
            Mod.barkMessage("Slime Burst!", req.viewer);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse SpawnBombs(ControlClient client, EffectRequest req)
    {
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            /*if (!matchManager.CampaignManager.InMatch)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }*/
            if (matchManager.InCutscene)
            {
                status = EffectStatus.Retry;
                return new EffectResponse(req.ID, status, message);
            }

            Quantum.Frame frame = dataManager.VerifiedFrame;

            CharacterManager characterManager = matchManager.CurrentCharacterManagers[0];

            EntityData characterEntity = Utils.GetEntityData(frame, characterManager.CharacterEntity.EntityRef);

            AirDodgeDropBomb_PowerUpAsset puasset = UnityDB.FindAsset<AirDodgeDropBomb_PowerUpAsset>(1320259745604944520);

            AirDodgeDropBomb_PowerUp.AirDodgeDropBomb_PowerUpData airDodgeDropBomb_PowerUpData = puasset.Settings.LevelsData[0];

            FPVector3 basePosition = characterEntity.transform->Position;
            ComplexItemData complexItemData;

            if (frame.TryFindAsset<AssetRefComplexItemData, ComplexItemData>(airDodgeDropBomb_PowerUpData.ComplexItemDataAsset, out complexItemData))
            {
                // Spawn at the base position
                complexItemData.Spawn(frame, basePosition, characterEntity.CharacterIndex);

                // Spawn additional items around the base position
                int radius = 5; // Radius for spreading the items (must be an integer)
                int numItems = 10; // Number of items to spawn around the base position

                for (int i = 0; i < numItems; i++)
                {
                    // Calculate angle and offset for each item
                    float angle = i * (360f / numItems); // Distribute items evenly in a circle
                    float radians = angle * Mathf.Deg2Rad;
                    int offsetX = (int)(Mathf.Cos(radians) * radius); // Convert float to int
                    int offsetY = (int)(Mathf.Sin(radians) * radius); // Convert float to int

                    // Calculate the new position
                    FPVector3 offset = new FPVector3(offsetX, offsetY, 0); // Ensure Z is 0 for 2D
                    FPVector3 newPosition = basePosition + offset;

                    // Spawn the item
                    complexItemData.Spawn(frame, newPosition, characterEntity.CharacterIndex);
                }
            }


            Mod.barkMessage("Spawn Bombs!", req.viewer);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse SpawnHugh(ControlClient client, EffectRequest req)
    {
        Debug.Log("Hugh Neutron!");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            CommandSpawnEntity commandSpawnEntity = new CommandSpawnEntity
            {
                InteractableID = Interactables.HughNeutron,
                SpawnPointID = "custom1",

            };

            dataManager.SendCommand(commandSpawnEntity);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse SpawnFrida(ControlClient client, EffectRequest req)
    {
        Debug.Log("Frida!");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            CommandSpawnEntity commandSpawnEntity = new CommandSpawnEntity
            {
                InteractableID = Interactables.Frida,
                SpawnPointID = "custom1",

            };

            dataManager.SendCommand(commandSpawnEntity);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse SpawnMrsPuff(ControlClient client, EffectRequest req)
    {
        Debug.Log("MrsPuff!");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            CommandSpawnEntity commandSpawnEntity = new CommandSpawnEntity
            {
                InteractableID = Interactables.MrsPuff,
                SpawnPointID = "custom1",

            };

            dataManager.SendCommand(commandSpawnEntity);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse SpawnGir(ControlClient client, EffectRequest req)
    {
        Debug.Log("Gir!");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            CommandSpawnEntity commandSpawnEntity = new CommandSpawnEntity
            {
                InteractableID = Interactables.Gir,
                SpawnPointID = "custom1",

            };

            dataManager.SendCommand(commandSpawnEntity);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse SpawnCabbaggeMerchant(ControlClient client, EffectRequest req)
    {
        Debug.Log("CabbaggeMerchant!");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            CommandSpawnEntity commandSpawnEntity = new CommandSpawnEntity
            {
                InteractableID = Interactables.CabbaggeMerchant,
                SpawnPointID = "custom1",

            };

            dataManager.SendCommand(commandSpawnEntity);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse SpawnPowderedToastMan(ControlClient client, EffectRequest req)
    {
        Debug.Log("PowderedToastMan!");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            CommandSpawnEntity commandSpawnEntity = new CommandSpawnEntity
            {
                InteractableID = Interactables.PowderedToastMan,
                SpawnPointID = "custom1",

            };

            dataManager.SendCommand(commandSpawnEntity);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    public static unsafe EffectResponse SpawnFountain(ControlClient client, EffectRequest req)
    {
        Debug.Log("Fountain!");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            CommandSpawnEntity commandSpawnEntity = new CommandSpawnEntity
            {
                InteractableID = Interactables.Fountain,
                SpawnPointID = "custom1",

            };

            dataManager.SendCommand(commandSpawnEntity);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    
    public static unsafe EffectResponse TestEffect(ControlClient client, EffectRequest req)
    {
        Debug.Log("Fountain!");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            Quantum.Frame frame = dataManager.VerifiedFrame;

            CharacterManager characterManager = matchManager.CurrentCharacterManagers[1];

            EntityData characterEntity = Utils.GetEntityData(frame, characterManager.CharacterEntity.EntityRef);

            //characterEntity.character->Freezeinput = slowdownEffectData.StopInput;

            characterEntity.physicsBody->velocityMultiplier = 2;
            characterEntity.animator->animationSpeedMultiplier = 2;

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }
    /*public static unsafe EffectResponse TestEffect(ControlClient client, EffectRequest req)
    {
        Debug.Log("Test Effect");
        EffectStatus status = EffectStatus.Success;
        string message = "";

        try
        {
            CommandSpawnEntity commandSpawnEntity = new CommandSpawnEntity
            {
                InteractableID = Interactables.HughNeutron,
                SpawnPointID = "custom1",
                
            };

            dataManager.SendCommand(commandSpawnEntity);

        }
        catch (Exception e)
        {
            status = EffectStatus.Retry;
            Mod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
        }

        return new EffectResponse(req.ID, status, message);
    }*/
    #region timedeffects
    //Timed Effects
    public static EffectResponse SuperSpeed(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 10000;

        if (TimedThread.isRunning(TimedType.SUPERSPEED)) return new EffectResponse(req.ID, EffectStatus.NotReady, "");
        if (TimedThread.isRunning(TimedType.SUPERSLOW)) return new EffectResponse(req.ID, EffectStatus.NotReady, "");
        new Thread(new TimedThread(req.ID, TimedType.SUPERSPEED, dur).Run).Start();

        Mod.barkMessage("Super Speed!", req.viewer);
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    public static EffectResponse SuperSlowness(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 10000;

        if (TimedThread.isRunning(TimedType.SUPERSPEED)) return new EffectResponse(req.ID, EffectStatus.NotReady, "");
        if (TimedThread.isRunning(TimedType.SUPERSLOW)) return new EffectResponse(req.ID, EffectStatus.NotReady, "");
        new Thread(new TimedThread(req.ID, TimedType.SUPERSLOW, dur).Run).Start();

        Mod.barkMessage("Super Slowness!", req.viewer);
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    public static EffectResponse MobsSuperSpeed(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 10000;

        if (TimedThread.isRunning(TimedType.MOBSSPEED)) return new EffectResponse(req.ID, EffectStatus.NotReady, "");
        if (TimedThread.isRunning(TimedType.MOBSSLOW)) return new EffectResponse(req.ID, EffectStatus.NotReady, "");
        new Thread(new TimedThread(req.ID, TimedType.MOBSSPEED, dur).Run).Start();

        Mod.barkMessage("Super Speed!", req.viewer);
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    public static EffectResponse MobsSuperSlowness(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 10000;

        if (TimedThread.isRunning(TimedType.MOBSSPEED)) return new EffectResponse(req.ID, EffectStatus.NotReady, "");
        if (TimedThread.isRunning(TimedType.MOBSSLOW)) return new EffectResponse(req.ID, EffectStatus.NotReady, "");
        new Thread(new TimedThread(req.ID, TimedType.MOBSSLOW, dur).Run).Start();

        Mod.barkMessage("Super Slowness!", req.viewer);
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    public static EffectResponse ShieldBreak(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 10000;

        if (TimedThread.isRunning(TimedType.INSTASHIELDBREAK)) return new EffectResponse(req.ID, EffectStatus.NotReady, "");

        new Thread(new TimedThread(req.ID, TimedType.INSTASHIELDBREAK, dur).Run).Start();

        Mod.barkMessage("Insta-Shield Break!", req.viewer);
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    public static EffectResponse NoSlimeUlt(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 10000;

        if (TimedThread.isRunning(TimedType.NOSLIMEULTIMATE)) return new EffectResponse(req.ID, EffectStatus.NotReady, "");
        new Thread(new TimedThread(req.ID, TimedType.NOSLIMEULTIMATE, dur).Run).Start();

        Mod.barkMessage("Ultimate always available!", req.viewer);
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    public static EffectResponse EnhanceDash(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.ENHANCEDASH)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        Mod.barkMessage("Enhance Dash!", req.viewer);

        new Thread(new TimedThread(req.ID, TimedType.ENHANCEDASH, dur).Run).Start();
        return new EffectResponse(req.ID, EffectStatus.Success, dur);
    }
    public static EffectResponse ExplosiveProjectiles(ControlClient client, EffectRequest req)
    {
        long dur = req.duration ?? 30000;

        if (TimedThread.isRunning(TimedType.EXPLOSIVEPROJ)) return new EffectResponse(req.ID, EffectStatus.Retry, "");

        Mod.barkMessage("Explosive Projectiles!", req.viewer);

        new Thread(new TimedThread(req.ID, TimedType.EXPLOSIVEPROJ, dur).Run).Start();
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
    private static async void DelayedMethodCall(int time, TypeCancel typeCancel)
    {
        Debug.Log("DELAYED");
        await Task.Delay(time); // 10 seconds delay
        switch (typeCancel)
        {
            case TypeCancel.Animation:
                Debug.Log("ANIMATION");
                CancelAnimation();

                break;
        }
        
    }


    private static unsafe void CancelAnimation()
    {
        Debug.Log("Cancel Anim");
        Quantum.Frame frame = dataManager.VerifiedFrame;

        CharacterManager characterManager = matchManager.CurrentCharacterManagers[0];

        EntityData entityData = Utils.GetEntityData(frame, characterManager.CharacterEntity.EntityRef);


        CustomAnimator.SetTrigger(frame, entityData.animator, 49);
        CameraManager cameraManager = matchManager.CameraManager;

        cameraManager.UpdateCamera(CameraManager.AvailableCameras.StageCamera);
    }
    public enum TypeCancel
    {
        Animation
    }


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