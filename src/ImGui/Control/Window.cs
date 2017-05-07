﻿using ImGui.Layout;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    internal class Window
    {
        public int ID;
        public Point Position;
        public Size Size;
        public Size FullSize;
        public WindowFlags Flags;
        public DrawList DrawList;
        public Rect ClipRect;
        public Point PosFloat;

        public long LastActiveFrame;

        public StackLayout StackLayout { get; set; }

        public Stack<int> IDStack { get; set; } = new Stack<int>();

        public Window(string name, Point position, Size size, WindowFlags Flags)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;

            this.ID = name.GetHashCode();
            this.IDStack.Push(this.ID);
            this.Flags = Flags;
            this.PosFloat = position;
            this.Position = new Point((int)PosFloat.X, (int)PosFloat.Y);
            this.Size = this.FullSize = size;
            this.DC = new GUIDrawContext();
            this.DrawList = new DrawList();
            this.MoveID = GetID("#MOVE");
            this.Active = WasActive = false;


            {
                var style = new GUIStyle();
                var bgColor = Color.Rgb(34, 43, 46);
                style.Set(GUIStyleName.PaddingTop, 1.0);
                style.Set(GUIStyleName.PaddingRight, 1.0);
                style.Set(GUIStyleName.PaddingBottom, 2.0);
                style.Set(GUIStyleName.PaddingLeft, 1.0);
                style.Set(GUIStyleName.BackgroundColor, new Color(0.00f, 0.00f, 0.00f, 0.70f));
                style.Set(GUIStyleName.ResizeGripColor, new Color(1.00f, 1.00f, 1.00f, 0.30f));
                style.Set(GUIStyleName.ResizeGripColor, new Color(1.00f, 1.00f, 1.00f, 0.60f), GUIState.Hover);
                style.Set(GUIStyleName.ResizeGripColor, new Color(1.00f, 1.00f, 1.00f, 0.90f), GUIState.Active);
                this.Style = style;
            }
            {
                var style = new GUIStyle();
                var bgColor = Color.Rgb(86, 90, 160);
                style.Set(GUIStyleName.BackgroundColor, new Color(0.27f, 0.27f, 0.54f, 0.83f));
                style.Set(GUIStyleName.BackgroundColor, new Color(0.32f, 0.32f, 0.63f, 0.87f), GUIState.Active);
                style.Set(GUIStyleName.BackgroundColor, new Color(0.40f, 0.40f, 0.80f, 0.20f), GUIState.Disabled);
                style.Set(GUIStyleName.FontColor, Color.White);
                this.HeaderStyle = style;
            }

            this.StackLayout = new StackLayout(this.ID, this.Size);

            g.Windows.Add(this);
        }

        public int GetID(int int_id)
        {
            int seed = IDStack.Peek();
            var id = Hash(seed, int_id);
            return id;
        }

        public int GetID(string str_id)
        {
            int seed = IDStack.Peek();
            int int_id = str_id.GetHashCode();
            var id = Hash(seed, int_id);

            GUIContext g = Form.current.uiContext;
            g.KeepAliveID(id);
            return id;
        }

        public int GetID(ITexture texture)
        {
            int seed = IDStack.Peek();
            int int_id = texture.GetHashCode();
            var id = Hash(seed, int_id);

            GUIContext g = Form.current.uiContext;
            g.KeepAliveID(id);
            return id;
        }

        public void ApplySize(Size new_size)
        {
            GUIContext g = Form.current.uiContext;
            Window window = this;
            window.FullSize = new_size;
        }

        public Rect Rect => new Rect(Position, Size);

        public double TitleBarHeight => HeaderStyle.PaddingVertical + HeaderStyle.PaddingVertical + HeaderStyle.FontSize*96.0/72.0;

        public Rect TitleBarRect => new Rect(Position, Size.Width, TitleBarHeight);

        public Rect ClientRect { get; internal set; }

        public bool Collapsed { get; internal set; } = false;
        public bool Active { get; internal set; }
        public Window RootWindow { get; internal set; }

        /// <summary>
        /// == window->GetID("#MOVE")
        /// </summary>
        public int MoveID { get; internal set; }
        public Rect WindowClippedRect { get; internal set; }
        public bool WasActive { get; internal set; }
        public bool SkipItems { get; internal set; } = false;
        public GUIDrawContext DC { get; internal set; }
        public int HiddenFrames { get; internal set; } = 0;
        public object ParentWindow { get; internal set; }
        public int BeginCount { get; internal set; }
        public bool Accessed { get; internal set; }

        public GUIStyle Style;
        public GUIStyle HeaderStyle;

        internal void PopClipRect()
        {
            //this.DrawList.PopClipRect();
            //var clipRectStack = this.DrawList._ClipRectStack;
            //this.ClipRect = clipRectStack[clipRectStack.Count-1];
        }

        internal Rect GetRect(int id, Size size, GUIStyle style, LayoutOption[] options)
        {
            var rect = StackLayout.GetRect(id, size, style, options);
            rect.Offset(this.Position.X, this.TitleBarHeight + this.Position.Y);
            return rect;
        }

        internal void ProcessLayout()
        {
            if (this.StackLayout.Dirty)
            {
                this.StackLayout.Layout(this.ClientRect.Size);
            }
        }

        private int Hash(int seed, int int_id)
        {
            int hash = seed + 17;
            hash = hash * 23 + this.ID.GetHashCode();
            var result = hash * 23 + int_id;
            return result;
        }

    }

    [Flags]
    public enum WindowFlags
    {
        Default = 0,
        NoTitleBar = 1 << 0,   // Disable title-bar
        NoResize = 1 << 1,   // Disable user resizing with the lower-right grip
        NoMove = 1 << 2,   // Disable user moving the window
        NoScrollbar = 1 << 3,   // Disable scrollbars (window can still scroll with mouse or programatically)
        NoScrollWithMouse = 1 << 4,   // Disable user vertically scrolling with mouse wheel
        NoCollapse = 1 << 5,   // Disable user collapsing window by double-clicking on it
        AlwaysAutoResize = 1 << 6,   // Resize every window to its content every frame
        ShowBorders = 1 << 7,   // Show borders around windows and items
        NoSavedSettings = 1 << 8,   // Never load/save settings in .ini file
        NoInputs = 1 << 9,   // Disable catching mouse or keyboard inputs
        MenuBar = 1 << 10,  // Has a menu-bar
        HorizontalScrollbar = 1 << 11,  // Allow horizontal scrollbar to appear (off by default). You may use SetNextWindowContentSize(ImVec2(width,0.0f)); prior to calling Begin() to specify width. Read code in imgui_demo in the "Horizontal Scrolling" section.
        NoFocusOnAppearing = 1 << 12,  // Disable taking focus when transitioning from hidden to visible state
        NoBringToFrontOnFocus = 1 << 13,  // Disable bringing window to front when taking focus (e.g. clicking on it or programatically giving it focus)
        AlwaysVerticalScrollbar = 1 << 14,  // Always show vertical scrollbar (even if ContentSize.y < Size.y)
        AlwaysHorizontalScrollbar = 1 << 15,  // Always show horizontal scrollbar (even if ContentSize.x < Size.x)
        AlwaysUseWindowPadding = 1 << 16,  // Ensure child windows without border uses style.WindowPadding (ignored by default for non-bordered child windows, because more convenient)
        // [Internal]
        ChildWindow = 1 << 20,  // Don't use! For internal use by BeginChild()
        ChildWindowAutoFitX = 1 << 21,  // Don't use! For internal use by BeginChild()
        ChildWindowAutoFitY = 1 << 22,  // Don't use! For internal use by BeginChild()
        ComboBox = 1 << 23,  // Don't use! For internal use by ComboBox()
        Tooltip = 1 << 24,  // Don't use! For internal use by BeginTooltip()
        Popup = 1 << 25,  // Don't use! For internal use by BeginPopup()
        Modal = 1 << 26,  // Don't use! For internal use by BeginPopupModal()
        ChildMenu = 1 << 27   // Don't use! For internal use by BeginMenu()
    };
}