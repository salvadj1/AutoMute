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
        private IniParser ini;
        public override string Name { get { return "AutoMute"; } }
        public override string Author { get { return "ice cold, salva/juli"; } }
        public override string Description { get { return "Automute players when they spam, advanced mute options, mute people for seconds, minutes, hours, days"; } }
        public override Version Version { get { return new Version("1.0"); } }
        //yep thats right salva i like doing this way of messages
        public string AutoMuteMessage = "{Player} has been automuted for {MuteTime}";
        public string ManualMuteMessage = "{Player} has been muted for {MuteTime} by {Muter}";
        public string MutedMessage = "[color #b22222]You are muted";
        private static bool AutoMute = true;
        private static bool AntiAdvertise = true;
        private static int ChatCooldown = 60;
        private static int MaxChatMessages = 3;

        private List<ulong> IsManualMute = new List<ulong>();
        private List<ulong> IsAutoMute = new List<ulong>();
        private List<ulong> UnderChatCD = new List<ulong>();

        public override void Initialize()
        {
            Fougerite.Hooks.OnChat += Chat;
            ReloadConfig();

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
        void Chat(Fougerite.Player Player, ref ChatString chatString)
        {
            //codes du maniakes
        }
    }
}
