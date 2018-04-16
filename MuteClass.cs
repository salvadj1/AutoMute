using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fougerite;
using Fougerite.Events;
using System.IO;

namespace AutoMute
{
    public class MuteClass : Fougerite.Module
    {
        public override string Name { get { return "AutoMute"; } }
        public override string Author { get { return "ice cold, salva/juli"; } }
        public override string Description { get { return "Automute players when they spam, advanced mute options, mute people for seconds, minutes, hours, days"; } }
        public override Version Version { get { return new Version("0.0.2"); } }

        //yep thats right salva i like doing this way of messages
        //ok
        public string AutoMuteMessage = "{Player} has been automuted for {MuteTime}";
        public string ManualMuteMessage = "{Player} has been muted for {MuteTime} by {Muter}";
        public string MutedMessage = "[color #b22222]You are muted";
        public bool AutoMute = true;
        public bool AntiAdvertise = true;
        public int ChatCooldown = 60;
        public int MaxChatMessages = 3;


        // ########################################################################################## Shhhhh coding!!
        public IniParser ini;
        public Dictionary<string, DateTime> LastChatUse = new Dictionary<string, DateTime>();
        public List<string> PlayersMuted = new List<string>();

        public override void Initialize()
        {
            Fougerite.Hooks.OnChat += Chat;
            ReloadConfig();
        }
        public override void DeInitialize()
        {
            Fougerite.Hooks.OnChat -= Chat;
        }

        public void Chat(Fougerite.Player pl, ref ChatString chatString)
        {
            DateTime current = DateTime.Now;

            //check if player is muted
            if (PlayersMuted.Contains(pl.SteamID))
            {
                chatString.NewText = "";
                pl.Message("YOU STILL MUTED! fuck you"); 
                return;
            }

            if (!LastChatUse.ContainsKey(pl.SteamID))
            {
                //add player to list
                LastChatUse.Add(pl.SteamID, DateTime.Now);
            }
            else
            {
                DateTime lastchat = LastChatUse[pl.SteamID];
                TimeSpan ts = current - lastchat;
                int difference = ts.Seconds;
                if (difference < 5)
                {
                    pl.Message("YOU ARE MUTED FOR 30seconds BITCH! only 1 message is allowed every 5 seconds");
                    MutePlayer(pl);
                }
                else
                {
                    //update last chat
                    LastChatUse[pl.SteamID] = DateTime.Now;
                }
            }
        }

        public void MutePlayer(Fougerite.Player pl)
        {
            PlayersMuted.Add(pl.SteamID);
            //create timer to autounmute player
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["player"] = pl;
            TimerMute(30 * 1000, dic).Start();
        }
        public void RemoveMute(Fougerite.Player pl)
        {
            PlayersMuted.RemoveAll(t => PlayersMuted.Contains(pl.SteamID));
            if (pl.IsAlive && !pl.IsDisconnecting)
            {
                pl.Message("Congratulations motherfucker!!! --  you are unmuted ");
            }
        }
        public TimedEvent TimerMute(int timeoutDelay, Dictionary<string, object> args)
        {
            TimedEvent timedEvent = new TimedEvent(timeoutDelay);
            timedEvent.Args = args;
            timedEvent.OnFire += CallBackMute;
            return timedEvent;
        }
        public void CallBackMute(TimedEvent e)
        {
            Dictionary<string, object> dic = e.Args;
            e.Kill();
            Fougerite.Player pl = (Fougerite.Player)dic["player"];
            RemoveMute(pl);
          
        }

        // ##########################################################################################



        public void ReloadConfig()
        {
            if (!File.Exists(Path.Combine(ModuleFolder, "Config.ini")))
            {
                Logger.Log("[AutoMute] Config file not found.... Created one");
                File.Create(Path.Combine(ModuleFolder, "Config.ini")).Dispose();
                ini = new IniParser(Path.Combine(ModuleFolder, "Config.ini"));
                ini.AddSetting("Options", "AutoMute", AutoMute.ToString());
                ini.AddSetting("Options", "AntiAdvertise", AntiAdvertise.ToString());
                ini.AddSetting("Options", "ChatCooldown", ChatCooldown.ToString());
                ini.AddSetting("Options", "AntiAdvertise", AntiAdvertise.ToString());
                ini.AddSetting("Options", "MaxChatMessages", MaxChatMessages.ToString());
                ini.AddSetting("Messages", "AutoMuteMessage", AutoMuteMessage.ToString());
                ini.AddSetting("Messages", "ManualMuteMessage", ManualMuteMessage.ToString());
                ini.AddSetting("Messages", "MutedMessage", MutedMessage.ToString());
                ini.Save();
            }
            else
            {
                //skrrrrraaaaa
                ini = new IniParser(Path.Combine(ModuleFolder, "Config.ini"));
                AutoMuteMessage = ini.GetSetting("Messages", "AutoMuteMessage");
                ManualMuteMessage = ini.GetSetting("Messages", "ManualMuteMessage");
                MutedMessage = ini.GetSetting("Messages", "MutedMessage");
                AutoMute = bool.Parse(ini.GetSetting("Options", "AutoMute"));
                AntiAdvertise = bool.Parse(ini.GetSetting("Options", "AntiAdvertise"));
                ChatCooldown = int.Parse(ini.GetSetting("Options", "ChatCooldown"));
                MaxChatMessages = int.Parse(ini.GetSetting("Options", "MaxChatMessages"));
                Logger.Log("[AutoMute] Config file founded and succesfully loaded");

            }
        }
        
    }
}
