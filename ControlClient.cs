//this project is a retrofit, it should NOT be used as part of any example - kat
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using ConnectorLib.JSON;
using Newtonsoft.Json;
using Quantum;
using UnityEngine;
using Object = System.Object;

namespace BepinControl;

[SuppressMessage("ReSharper", "GrammarMistakeInComment")]
public class ControlClient
{

    static public GameManager gameManager
    {
        get
        {
            return GameManager.Instance;
        }
    }

    public DataManager dataManager => GameManager.Instance.DataManager;

    public static readonly string CV_HOST = "127.0.0.1";
    public static readonly int CV_PORT = 33379;

    private static readonly string[] CommonMetadata = new string[] { "health" };


    private static readonly Dictionary<string, EffectDelegate> Delegate = new()
    {
        //test
        { "test_effect", EffectDelegates.TestEffect},

        //health effect
        { "damage50", EffectDelegates.Damage50 },
        { "damage100", EffectDelegates.Damage100 },
        { "heal50", EffectDelegates.Heal50 },
        { "heal100", EffectDelegates.Heal100 },

        //spawn effects
        { "spawnitemrandom", EffectDelegates.SpawnItem },

        //player effects
        { "enhance_dash", EffectDelegates.EnhanceDash },
        { "killplayer", EffectDelegates.EffectKillPlayer },
        { "swapcharacterrandom", EffectDelegates.SwapCharacterRandom },

        //enemies effects
        { "killenemies", EffectDelegates.EffectKillEnemies},

        //match effects
        { "endmatch", EffectDelegates.EndMatch },
        { "cameratopview", EffectDelegates.CameraTopView},
        { "camerabottomview", EffectDelegates.CameraBottomView},
        { "firstpersonview", EffectDelegates.CameraFirstPerson},

        //Status Effect
        { "effect_sleep", EffectDelegates.EffectSleep },
        { "effect_slowdown", EffectDelegates.EffectSlowDown},
        { "effect_confused", EffectDelegates.EffectConfuse},
        { "effect_supersize", EffectDelegates.EffectSuperSize},
        { "effect_tinysize", EffectDelegates.EffectTinySize}

    };

    private IPEndPoint Endpoint { get; }
    private Queue<SimpleJSONRequest> Requests { get; }
    private bool Running { get; set; }

    private bool paused;
    public static Socket? Socket { get; set; }

    

    public bool inGame = true;

    public static readonly ControlClient Instance = new();

    public bool Connected => Socket?.Connected ?? false;

    private ControlClient()
    {
        Endpoint = new(IPAddress.Parse(CV_HOST), CV_PORT);
        Requests = new();
        Running = true;
        Socket = null;
    }

    public bool isReady()
    {
        try
        {
            //TestMod.mls.LogInfo($"landed: {StartOfRound.Instance.shipHasLanded}");
            //TestMod.mls.LogInfo($"planet: {RoundManager.Instance.currentLevel.PlanetName}");
            if (!(gameManager.CurrentGameContext == GameContext.Campaign)) return false;
            if(!Mod.hudDoneLoading) return false;

            //if (!StartOfRound.Instance.shipHasLanded) return false;

            //if (RoundManager.Instance.currentLevel.PlanetName.ToLower().Contains("gordion")) return false;
            //if (RoundManager.Instance.currentLevel.PlanetName.ToLower().Contains("company")) return false;

        }
        catch (Exception e)
        {
            Mod.mls.LogError(e.ToString());
            return false;
        }

        return true;
    }

    public bool HideEffect(string code)
        => Send(new EffectUpdate(code, EffectStatus.NotVisible));

    public bool ShowEffect(string code)
        => Send(new EffectUpdate(code, EffectStatus.Visible));

    public bool DisableEffect(string code)
        => Send(new EffectUpdate(code, EffectStatus.NotSelectable));

    public bool EnableEffect(string code)
        => Send(new EffectUpdate(code, EffectStatus.Selectable));

    private void ClientLoop()
    {

        Mod.mls.LogInfo("Connected to Crowd Control");
        Mod.barkMessage("Connected to Crowd Control", "CROWD CONTROL");

        var timer = new Timer(timeUpdate, null, 0, 200);

        try
        {
            while (Running)
            {
                SimpleJSONRequest? req = Recieve(this, Socket);
                if (req?.IsKeepAlive ?? true)
                {
                    Thread.Sleep(0); //prevent a meltdown if this ever tight loops
                    continue;
                }

                lock (Requests)
                    Requests.Enqueue(req);
            }
        }
        catch (Exception e)
        {
            Mod.mls.LogInfo($"Disconnected from Crowd Control: {e.Message}");
            Mod.barkMessage("Disconnected from Crowd Control", "CROWD CONTROL");
            Socket?.Close();
        }
    }

    public static readonly int RECV_BUF = 4096;
    public static readonly int RECV_TIME = 5000000;

    public SimpleJSONRequest? Recieve(ControlClient client, Socket socket)
    {
        Debug.Log("Recieve SIMPLEJSONREQUEST");
        byte[] buf = new byte[RECV_BUF];
        string content = "";
        int read = 0;

        do
        {
            if (!client.IsRunning()) return null;

            if (socket.Poll(RECV_TIME, SelectMode.SelectRead))
            {
                read = socket.Receive(buf);
                if (read < 0) return null;

                content += Encoding.ASCII.GetString(buf);
            }
            else
                KeepAlive();
        } while (read == 0 || (read == RECV_BUF && buf[RECV_BUF - 1] != 0));

        return SimpleJSONRequest.TryParse(content, out SimpleJSONRequest? result) ? result : null;
    }

    private static readonly EmptyResponse KEEPALIVE = new() { type = ResponseType.KeepAlive };

    public bool KeepAlive() => Send(KEEPALIVE);

    public bool Send(SimpleJSONResponse message)
    {
        Debug.Log("Send SIMPLEJSON");
        byte[] tmpData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));
        byte[] outData = new byte[tmpData.Length + 1];
        Buffer.BlockCopy(tmpData, 0, outData, 0, tmpData.Length);
        outData[tmpData.Length] = 0;
        int bytesSent = Socket?.Send(outData) ?? -1;
        return (bytesSent == outData.Length);
    }

    public void timeUpdate(Object state)
    {
        inGame = true;

        if (!isReady()) inGame = false;

        if (!inGame)
        {
            TimedThread.addTime(200);
            paused = true;
        }
        else if (paused)
        {
            paused = false;
            TimedThread.unPause();
            TimedThread.tickTime(200);
        }
        else
        {
            TimedThread.tickTime(200);
        }
    }

    public bool IsRunning() => Running;

    public void NetworkLoop()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        while (Running)
        {
            Mod.mls.LogInfo("Attempting to connect to Crowd Control");
            Mod.barkMessage("Attempting to connect to Crowd Control...", "CROWD CONTROL");
            try
            {
                Socket = new Socket(Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                if (Socket.BeginConnect(Endpoint, null, null).AsyncWaitHandle.WaitOne(10000, true) && Socket.Connected)
                    ClientLoop();
                else
                    Mod.mls.LogInfo("Failed to connect to Crowd Control");
                Socket.Close();
            }
            catch (Exception e)
            {
                Mod.mls.LogInfo(e.GetType().Name);
                Mod.mls.LogInfo("Failed to connect to Crowd Control");
            }

            Thread.Sleep(10000);
        }
    }

    public void FixedUpdate()
    {
        //Log.Message(_game_status_update_timer);
        _game_status_update_timer += Time.fixedDeltaTime;
        if (_game_status_update_timer >= GAME_STATUS_UPDATE_INTERVAL)
        {
            UpdateGameState();
            _game_status_update_timer = 0f;
        }
    }

    public void RequestLoop()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        while (Running)
        {
            try
            {
                SimpleJSONRequest req;
                lock (Requests)
                {
                    if (Requests.Count == 0)
                        continue;
                    req = Requests.Dequeue();
                }

                if (req is EffectRequest er)
                {
                    string? code = er.code;

                    try
                    {
                        EffectResponse? res;
                        if (code == null)
                            res = new EffectResponse(er.ID, EffectStatus.Unavailable, "No effect code was sent.");
                        else if (isReady())
                            if (Delegate.TryGetValue(code, out EffectDelegate? del))
                            {
                                res = del(this, er);

                                //we add the common metadata here, effects COULD return more meta
                                //on a per-effect basis but we're not doing that in this example
                                res.metadata = new();
                                //foreach (string key in CommonMetadata)
                                    //res.metadata.Add(key, Metadata[key].Invoke(this));
                            }
                            else
                                res = new EffectResponse(er.ID, EffectStatus.Unavailable,
                                    $"Unknown effect code: {code}");
                        else
                            res = new EffectResponse(er.ID, EffectStatus.Retry);

                        Send(res);
                    }
                    catch (KeyNotFoundException)
                    {
                        Send(new EffectResponse(er.ID, EffectStatus.Unavailable, $"Request error for '{code}'"));
                    }
                }
                else if (req.type == RequestType.GameUpdate) UpdateGameState(true);
            }
            catch (Exception)
            {
                Mod.mls.LogInfo("Disconnected from Crowd Control");
                Mod.barkMessage("Disconnected from Crowd Control", "CROWD CONTROL");
                Socket?.Close();
            }
        }
    }

    public void Stop() => Running = false;

    private GameState? _last_game_state;

    private const float GAME_STATUS_UPDATE_INTERVAL = 1f;
    private float _game_status_update_timer = 0f;

    [SuppressMessage("ReSharper", "RedundantIfElseBlock")]
    private bool UpdateGameState(bool force = false)
    {
        try
        {
            //if (!StartOfRound.Instance.shipHasLanded) return UpdateGameState(GameState.WrongMode, force);

            //if (RoundManager.Instance.currentLevel.PlanetName.ToLower().Contains("gordion")) UpdateGameState(GameState.SafeArea, force); ;
            //if (RoundManager.Instance.currentLevel.PlanetName.ToLower().Contains("company")) UpdateGameState(GameState.SafeArea, force);
        }
        catch (Exception e)
        {
            Mod.mls.LogError(e.ToString());
            return UpdateGameState(GameState.Error, force);
        }

        return UpdateGameState(GameState.Ready, force);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool UpdateGameState(GameState newState, bool force) => UpdateGameState(newState, null, force);
    private bool UpdateGameState(GameState newState, string? message = null, bool force = false)
    {
        if (force || (_last_game_state != newState))
        {
            _last_game_state = newState;
            return Send(new GameUpdate(newState, message));
        }
        return true;
    }
}