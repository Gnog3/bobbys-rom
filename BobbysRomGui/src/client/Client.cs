using System;
using BobbysRomGui.Client.EditGUI;
using EccsLogicWorldAPI.Client.Hooks;
using LogicAPI.Client;
using LogicWorld;

namespace BobbysRomGui.Client
{
    public class Client : ClientMod
    {
        protected override void Initialize()
        {
            WorldHook.worldLoading += () => {
                //This action is in Unity execution scope, errors must be caught manually:
                try
                {
                    EditRom.initialize();
                }
                catch(Exception e)
                {
                    Logger.Error("Failed to initialize ROM Edit GUI:");
                    SceneAndNetworkManager.TriggerErrorScreen(e);
                }
            };
        }
    }
}