﻿using Xunit;

namespace ImGui.UnitTest
{
    public partial class GUIFacts
    {
        public class TheBeginEndMethods
        {
            [Fact]
            public void TheWindowShouldBeDrawn()
            {
                Application.Init();

                var form = new MainForm();
                bool open = true;
                form.OnGUIAction = () =>
                {
                    GUI.Begin("test window", ref open);
                    GUI.End();
                };

                Application.Run(form);
            }
        }
    }
}