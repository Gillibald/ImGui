﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui
{
    class WindowManager
    {
        public readonly List<Window> Windows = new List<Window>();

        public readonly List<Window> WindowStack = new List<Window>();

        public Window CurrentWindow;
        public Window PopupWindow;
        public Window HoveredWindow { get; internal set; }
        public Window HoveredRootWindow { get; internal set; }

        public Window MovedWindow { get; internal set; }
        public int MovedWindowMoveId { get; internal set; }

        public Window FocusedWindow { get; private set; }

        public Window ActiveIdWindow { get; internal set; }

        public bool IsWindowContentHoverable(Window window)
        {
            // An active popup disable hovering on other windows (apart from its own children)
            Window focused_window = this.FocusedWindow;
            if (focused_window != null)
            {
                Window focused_root_window = focused_window.RootWindow;
                if (focused_root_window != null)
                {
                    if (focused_root_window.Flags.HaveFlag(WindowFlags.Popup) && focused_root_window.WasActive && focused_root_window != window.RootWindow)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Window FindWindowByName(string name)
        {
            for (int i = 0; i < this.Windows.Count; i++)
            {
                if (this.Windows[i].ID == name.GetHashCode())
                {
                    return this.Windows[i];
                }
            }
            return null;
        }

        public Window FindHoveredWindow(Point pos, bool excluding_childs)
        {
            for (int i = this.Windows.Count - 1; i >= 0; i--)
            {
                Window window = this.Windows[i];
                if (!window.Active)
                    continue;
                if (excluding_childs && window.Flags.HaveFlag(WindowFlags.ChildWindow))
                    continue;

                if (window.WindowClippedRect.Contains(pos))
                    return window;
            }
            return null;
        }

        /// <summary>
        /// Moving window to front of display (which happens to be back of our sorted list)
        /// </summary>
        /// <param name="window"></param>
        public void FocusWindow(Window window)
        {
            var g = Form.current.uiContext;

            // Always mark the window we passed as focused. This is used for keyboard interactions such as tabbing.
            this.FocusedWindow = window;

            // Passing null allow to disable keyboard focus
            if (window == null) return;

            // And move its root window to the top of the pile
            if (window.RootWindow != null)
            {
                window = window.RootWindow;
            }

            // Steal focus on active widgets
            if (window.Flags.HaveFlag(WindowFlags.Popup))
            {
                if (g.ActiveId != 0 && (this.ActiveIdWindow != null) && this.ActiveIdWindow.RootWindow != window)
                {
                    g.SetActiveID(0);
                }
            }

            // Bring to front
            if ((window.Flags.HaveFlag(WindowFlags.NoBringToFrontOnFocus) || this.Windows[this.Windows.Count - 1] == window))
            {
                return;
            }
            for (int i = 0; i < this.Windows.Count; i++)
            {
                if (this.Windows[i] == window)
                {
                    this.Windows.RemoveAt(i);
                    break;
                }
            }
            this.Windows.Add(window);
        }

        internal Window CreateWindow(string name, Point position, Size size, WindowFlags flags)
        {
            return new Window(name, position, size, flags);
        }

        public void NewFrame(GUIContext g)
        {
            // Handle user moving window (at the beginning of the frame to avoid input lag or sheering). Only valid for root windows.
            if (this.MovedWindowMoveId != 0 && this.MovedWindowMoveId == g.ActiveId)
            {
                g.KeepAliveID(this.MovedWindowMoveId);
                Debug.Assert(this.MovedWindow != null && this.MovedWindow.RootWindow != null);
                Debug.Assert(this.MovedWindow.RootWindow.MoveID == this.MovedWindowMoveId);
                if (Input.Mouse.LeftButtonState == InputState.Down)
                {
                    if (!this.MovedWindow.Flags.HaveFlag(WindowFlags.NoMove))
                    {
                        this.MovedWindow.PosFloat += Input.Mouse.MouseDelta;
                    }
                    this.FocusWindow(this.MovedWindow);
                }
                else
                {
                    g.SetActiveID(0);
                    this.MovedWindow = null;
                    this.MovedWindowMoveId = 0;
                }
            }
            else
            {
                this.MovedWindow = null;
                this.MovedWindowMoveId = 0;
            }

            // Find the window we are hovering. Child windows can extend beyond the limit of their parent so we need to derive HoveredRootWindow from HoveredWindow
            this.HoveredWindow = this.MovedWindow ?? this.FindHoveredWindow(Input.Mouse.MousePos, false);
            if (this.HoveredWindow != null && (this.HoveredWindow.Flags.HaveFlag(WindowFlags.ChildWindow)))
                this.HoveredRootWindow = this.HoveredWindow.RootWindow;
            else
                this.HoveredRootWindow = (this.MovedWindow != null) ? this.MovedWindow.RootWindow : this.FindHoveredWindow(Input.Mouse.MousePos, true);

            // Mark all windows as not visible
            for (int i = 0; i != this.Windows.Count; i++)
            {
                Window window = this.Windows[i];
                window.WasActive = window.Active;
                window.Active = false;
                window.Accessed = false;
            }

            // No window should be open at the beginning of the frame.
            // But in order to allow the user to call NewFrame() multiple times without calling Render(), we are doing an explicit clear.
            this.WindowStack.Clear();
        }

        public void EndFrame(GUIContext g)
        {
            // Click to focus window and start moving (after we're done with all our widgets)
            if (g.ActiveId == 0 && g.HoverId == 0 && Input.Mouse.LeftButtonPressed)
            {
                if (!(this.FocusedWindow != null && !this.FocusedWindow.WasActive && this.FocusedWindow.Active)) // Unless we just made a popup appear
                {
                    if (this.HoveredRootWindow != null)
                    {
                        this.FocusWindow(this.HoveredWindow);
                        if (!(this.HoveredWindow.Flags.HaveFlag(WindowFlags.NoMove)))
                        {
                            this.MovedWindow = this.HoveredWindow;
                            this.MovedWindowMoveId = this.HoveredRootWindow.MoveID;
                            g.SetActiveID(this.MovedWindowMoveId, this.HoveredRootWindow);
                        }
                    }
                    else if (this.FocusedWindow != null)
                    {
                        // Clicking on void disable focus
                        this.FocusWindow(null);
                    }
                }
            }
        }
    }
}
