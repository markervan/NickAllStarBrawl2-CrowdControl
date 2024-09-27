//this project is a retrofit, it should NOT be used as part of any example - kat
using ConnectorLib.JSON;
using Quantum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;
using System.Threading;
using UnityEngine;
using Photon.Deterministic;
using HarmonyLib.Public.Patching;
namespace BepinControl;

public enum TimedType
{
    //porweups
    ENHANCEDASH,

    //camera
    CAMERATOPVIEW,
    CAMERABOTTOMVIEW,
    FIRSTPERSON,


    //status
    STATUSTINY,
    STATUSGIANT,

}

public class Timed
{

    private DataManager dataManager
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
    public TimedType type;

    public Timed(TimedType t) { 
        type = t;
    }

    public void addEffect(long duration)
    {
        switch (type)
        {
            case TimedType.ENHANCEDASH:
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        //Mod.barkMessage("Enhancing Dashes!", "Viewer");
                        Debug.Log("Enhancing Dash....");

                        CommandAddPowerUp command = new CommandAddPowerUp
                        {
                            CharacterIndex = 0,
                            PowerUpIndex = (int)PowerUps.DodgesRangeIncrease,
                            Level = 3,
                            Lifetime = 0, // Use duration here
                            AddToEveryone = false
                        };
                        dataManager.SendCommand(command);
                    });
                    break;
                }

            case TimedType.CAMERATOPVIEW:
                {
                    StageCameraSO stageCameraSO = dataManager.CurrentStageLayoutData.CameraData;
                    stageCameraSO.verticalOffset = 20;
                    break;
                }
            case TimedType.CAMERABOTTOMVIEW:
                {
                    StageCameraSO stageCameraSO = dataManager.CurrentStageLayoutData.CameraData;
                    stageCameraSO.verticalOffset = -20;
                    break;
                }
            case TimedType.STATUSGIANT:
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        //Mod.barkMessage("Added Effect: Super Size!", req.viewer);

                        CommandAddStatusEffect command = new CommandAddStatusEffect
                        {
                            CharacterIndex = 0,
                            StatusEffectIndex = (int)StatusEffects.SuperSize,
                            Pool = null,
                            AddTo = 1,
                        };
                        dataManager.SendCommand(command);
                    });
                    break;
                }
            case TimedType.STATUSTINY:
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        //Mod.barkMessage("Added Effect: Super Size!", req.viewer);

                        CommandAddStatusEffect command = new CommandAddStatusEffect
                        {
                            CharacterIndex = 0,
                            StatusEffectIndex = (int)StatusEffects.TinySize,
                            Pool = null,
                            AddTo = 1,
                        };
                        dataManager.SendCommand(command);


                    });
                    break;
                }
            case TimedType.FIRSTPERSON:
                {
                    if (Mod.headBone == null) // Only find the head bone if it hasn't been found yet
                    {
                        CameraManager cameraManager = matchManager.CameraManager;

                        cameraManager.UpdateCamera(CameraManager.AvailableCameras.FreeCamera);

                        List<CharacterManager> currentCharacterManagers = matchManager.CurrentCharacterManagers;

                        GameObject character = currentCharacterManagers[0].gameObject;


                        // Use the recursive function to find the head bone
                        Mod.headBone = Mod.FindHeadBone(character.transform, "head_jnt");
                        Mod.FindExpressions(character.transform, "expressions");

                        if (Mod.headBone != null)
                        {
                            Debug.Log("Head bone found");

                            Mod.freecamera = cameraManager.freeCameraSystem.GetComponent<FreeCameraSystem>();

                            Mod.fpsOn = true;

                            Mod.freecamera.virtualCamera.m_Lens.FieldOfView = 90f;

                        }
                    }
                    break;
                }

        }
    }

    public void removeEffect()
    {
        switch (type)
        {
            case TimedType.ENHANCEDASH:
                {
                    Mod.ActionQueue.Enqueue(() =>
                    {
                        Debug.Log("Removing Enhancing Dash....");
                        CommandRemovePowerUp command = new CommandRemovePowerUp
                        {
                            CharacterIndex = 0,
                            PowerUpIndex = (int)PowerUps.DodgesRangeIncrease,
                            RemoveFromEveryone = true
                        };
                        dataManager.SendCommand(command);
                    });
                    break;
                }

            case TimedType.CAMERABOTTOMVIEW:
            case TimedType.CAMERATOPVIEW:
                {
                    StageCameraSO stageCameraSO = dataManager.CurrentStageLayoutData.CameraData;
                    stageCameraSO.verticalOffset = 1;

                    break;
                }
            case TimedType.STATUSTINY:
                {
                    CommandRemoveStatusEffect command = new CommandRemoveStatusEffect
                    {
                        CharacterIndex = 0,
                        StatusEffectIndex = (int)StatusEffects.TinySize,
                        Pool = null,

                    };
                    dataManager.SendCommand(command);


                    break;
                }
            case TimedType.STATUSGIANT:
                {
                    CommandRemoveStatusEffect command = new CommandRemoveStatusEffect
                    {
                        CharacterIndex = 0,
                        StatusEffectIndex = (int)StatusEffects.SuperSize,
                        Pool = null,

                    };
                    dataManager.SendCommand(command);


                    break;
                }
            case TimedType.FIRSTPERSON:
                {
                    CameraManager cameraManager = matchManager.CameraManager;
                    
                    Mod.freecamera = null;
                    Mod.headBone = null;
                    Mod.fpsOn = false;
                    cameraManager.UpdateCamera(CameraManager.AvailableCameras.StageCamera);

                    break;
                }
        }
    }
    static int frames = 0;

    public void tick()
    {
        frames++;
        //var playerRef = StartOfRound.Instance.localPlayerController;

        switch (type)
        {
            
        }
    }
}

public class TimedThread
{
    public static List<TimedThread> threads = new();

    public readonly Timed effect;
    public long duration;
    public long remain;
    public uint id;
    public bool paused;

    public static bool isRunning(TimedType t)
    {
        foreach (var thread in threads)
        {
            if (thread.effect.type == t) return true;
        }
        return false;
    }


    public static void tick()
    {
        foreach (var thread in threads)
        {
            if (!thread.paused)
                thread.effect.tick();
        }
    }
    public static void addTime(int duration)
    {
        try
        {
            lock (threads)
            {
                foreach (var thread in threads)
                {
                    Interlocked.Add(ref thread.duration, duration + 5);
                    if (!thread.paused)
                    {
                        long time = Volatile.Read(ref thread.remain);
                        ControlClient.Instance.Send(new EffectResponse(thread.id, EffectStatus.Paused, time));
                        thread.paused = true;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Mod.mls.LogInfo(e.ToString());
        }
    }

    public static void tickTime(int duration)
    {
        try
        {
            lock (threads)
            {
                foreach (var thread in threads)
                {
                    long time = Volatile.Read(ref thread.remain);
                    time -= duration;
                    if (time < 0) time = 0;
                    Volatile.Write(ref thread.remain, time);
                }
            }
        }
        catch (Exception e)
        {
            Mod.mls.LogInfo(e.ToString());
        }
    }

    public static void unPause()
    {
        try
        {
            lock (threads)
            {
                foreach (var thread in threads)
                {
                    if (thread.paused)
                    {
                        long time = Volatile.Read(ref thread.remain);
                        ControlClient.Instance.Send(new EffectResponse(thread.id, EffectStatus.Resumed, time));
                        thread.paused = false;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Mod.mls.LogInfo(e.ToString());
        }
    }

    public TimedThread(uint id, TimedType type, long duration)
    {
        effect = new Timed(type);
        this.duration = duration;
        remain = duration;
        this.id = id;
        paused = false;

        try
        {
            lock (threads)
            {
                threads.Add(this);
            }
        }
        catch (Exception e)
        {
            Mod.mls.LogInfo(e.ToString());
        }
    }

    public void Run()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        effect.addEffect(duration);

        try
        {
            long time = Volatile.Read(ref duration);
            while (time > 0)
            {
                Interlocked.Add(ref duration, -time);
                Thread.Sleep((int)time);

                time = Volatile.Read(ref duration);
            }
            effect.removeEffect();
            lock (threads)
            {
                threads.Remove(this);
            }
            ControlClient.Instance.Send(new EffectResponse(id, EffectStatus.Finished, 0));
        }
        catch (Exception e)
        {
            Mod.mls.LogInfo(e.ToString());
        }
    }
}