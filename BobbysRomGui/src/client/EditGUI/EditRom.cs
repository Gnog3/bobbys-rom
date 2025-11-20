using EccsGuiBuilder.Client.Layouts.Elements;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using LogicAPI.Data.BuildingRequests;
using LogicUI.MenuParts;
using LogicWorld.BuildingManagement;
using LogicWorld.UI;
using BobbysRom.Shared;
using TMPro;
using UnityEngine;
using System;

namespace BobbysRomGui.Client.EditGUI
{
    public class EditRom : EditComponentMenu<IRomData>, IAssignMyFields
    {
        public static void initialize()
        {
            WS.window("EditRomWindow")
                .configureContent(content =>
                    content.vertical(10f, new RectOffset(20, 20, 0, 20), expandHorizontal: true)
                        .addContainer("InputBox",
                            inputBox => inputBox.horizontal(20, anchor: TextAnchor.MiddleLeft)
                                .add(WS.textLine.setLocalizationKey("BobbysRomGui.EditRom.Inputs"))
                                .add(WS.button.setLocalizationKey("BobbysRomGui.EditRom.8bits")
                                    .injectionKey(nameof(input8BitsButton)).add<ButtonLayout>())
                                .add(WS.button.setLocalizationKey("BobbysRomGui.EditRom.16bits")
                                    .injectionKey(nameof(input16BitsButton)).add<ButtonLayout>())
                                .add(WS.button.setLocalizationKey("BobbysRomGui.EditRom.24bits")
                                    .injectionKey(nameof(input24BitsButton)).add<ButtonLayout>())
                        ).addContainer("OutputBox",
                            outputBox => outputBox.horizontal(20, anchor: TextAnchor.MiddleLeft)
                                .add(WS.textLine.setLocalizationKey("BobbysRomGui.EditRom.Outputs"))
                                .add(WS.button.setLocalizationKey("BobbysRomGui.EditRom.8bits")
                                    .injectionKey(nameof(output8bitsButton)).add<ButtonLayout>())
                                .add(WS.button.setLocalizationKey("BobbysRomGui.EditRom.16bits")
                                    .injectionKey(nameof(output16BitsButton)).add<ButtonLayout>())
                                .add(WS.button.setLocalizationKey("BobbysRomGui.EditRom.32bits")
                                    .injectionKey(nameof(output32BitsButton)).add<ButtonLayout>())
                        ).addContainer("DataBox",
                            dataBox => dataBox.horizontal(20, anchor: TextAnchor.MiddleLeft)
                                .add(WS.textLine.setLocalizationKey("BobbysRomGui.EditRom.Data"))
                                .add(WS.button.setLocalizationKey("BobbysRomGui.EditRom.Paste").injectionKey(nameof(pasteButton))
                                    .add<ButtonLayout>())
                                .add(WS.textLine.injectionKey(nameof(text)))
                        ))
                .add<EditRom>().build();
        }

        [AssignMe] public HoverButton input8BitsButton;
        [AssignMe] public HoverButton input16BitsButton;
        [AssignMe] public HoverButton input24BitsButton;
        [AssignMe] public HoverButton output8bitsButton;
        [AssignMe] public HoverButton output16BitsButton;
        [AssignMe] public HoverButton output32BitsButton;
        [AssignMe] public HoverButton pasteButton;
        [AssignMe] public TextMeshProUGUI text;

        private void SetInputCount(int count)
        {
            foreach (var entry in ComponentsBeingEdited)
            {
                BuildRequestManager.SendBuildRequest(
                    new BuildRequest_ChangeDynamicComponentPegCounts(entry.Address, count,
                        entry.ClientCode.OutputCount));
            }
        }

        private void SetOutputCount(int count)
        {
            foreach (var entry in ComponentsBeingEdited)
            {
                BuildRequestManager.SendBuildRequest(
                    new BuildRequest_ChangeDynamicComponentPegCounts(entry.Address, entry.ClientCode.InputCount,
                        count));
            }
        }

        private void SetData(byte[] data)
        {
            foreach (var entry in ComponentsBeingEdited)
            {
                entry.Data.Data = data;
            }
        }
        
        public override void Initialize()
        {
            base.Initialize();
            input8BitsButton.OnClickEnd += () => SetInputCount(8);
            input16BitsButton.OnClickEnd += () => SetInputCount(16);
            input24BitsButton.OnClickEnd += () => SetInputCount(24);
            output8bitsButton.OnClickEnd += () => SetOutputCount(8);
            output16BitsButton.OnClickEnd += () => SetOutputCount(16);
            output32BitsButton.OnClickEnd += () => SetOutputCount(32);
            pasteButton.OnClickEnd += () =>
            {
                string base64 = GUIUtility.systemCopyBuffer;
                byte[] data;
                try
                {
                    data = Convert.FromBase64String(base64);
                }
                catch (FormatException)
                {
                    data = Array.Empty<byte>();
                }
                SetData(data);
            };
        }
        protected override void OnStartEditing()
        {
            var data = FirstComponentBeingEdited.Data;
            text.text = data.Data.Length + " bytes";
        }

        protected override void OnRun()
        {
            var data = FirstComponentBeingEdited.Data;
            text.text = data.Data.Length + " bytes";
        }
    }
}
