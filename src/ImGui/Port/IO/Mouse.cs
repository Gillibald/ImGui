﻿namespace ImGui
{
    /// <summary>
    /// input
    /// </summary>
    public class Mouse
    {
        #region Settings
        /// <summary>
        /// Double click interval time span
        /// </summary>
        /// <remarks>
        /// if the interval between two mouse click is longer than this value,
        /// the two clicking action is not considered as a double-click action.
        /// </remarks>
        internal const float DoubleClickIntervalTimeSpan = 0.2f;
        #endregion

        #region Left button

        /// <summary>
        /// Left button state
        /// </summary>
        private InputState leftButtonState = InputState.Up;

        /// <summary>
        /// Last recorded left mouse button state
        /// </summary>
        /// <remarks>for detecting left mouse button state' changes</remarks>
        public InputState LastLeftButtonState { get; internal set; } = InputState.Up;

        /// <summary>
        /// Button state of left mouse button(readonly)
        /// </summary>
        public InputState LeftButtonState
        {
            get { return leftButtonState; }
            set { leftButtonState = value; }
        }

        public long LeftButtonDownDuration { get; internal set; } = -1;

        /// <summary>
        /// Is left mouse button released?(readonly)
        /// </summary>
        public bool LeftButtonReleased { get; internal set; } = false;

        /// <summary>
        /// Is left mouse button clicked?(readonly)
        /// </summary>
        public bool LeftButtonPressed { get; internal set; } = false;

        #endregion

        #region Right button
        /// <summary>
        /// Last recorded right mouse button state
        /// </summary>
        public InputState LastRightButtonState { get; internal set; } = InputState.Up;

        /// <summary>
        /// Right button state
        /// </summary>
        InputState rightButtonState = InputState.Up;

        /// <summary>
        /// Button state of the right mouse button(readonly)
        /// </summary>
        public InputState RightButtonState
        {
            get { return rightButtonState; }
            set { rightButtonState = value; }
        }

        public long RightButtonDownDuration { get; internal set; } = -1;

        /// <summary>
        /// Is right mouse button released?(readonly)
        /// </summary>
        public bool RightButtonReleased { get; internal set; } = false;

        /// <summary>
        /// Check if the right mouse button is pressed(readonly)
        /// </summary>
        public bool RightButtonPressed { get; internal set; } = false;
        #endregion

        #region Position
        /// <summary>
        /// Mouse position
        /// </summary>
        static Point lastMousePos;

        /// <summary>
        /// Mouse position
        /// </summary>
        static Point mousePos;

        /// <summary>
        /// Last mouse position in screen (readonly)
        /// </summary>
        public Point LastMousePos
        {
            get { return lastMousePos; }
        }

        /// <summary>
        /// Mouse position in screen (readonly)
        /// </summary>
        public Point MousePos
        {
            get { return mousePos; }
            set
            {
                lastMousePos = mousePos;
                mousePos = value;
            }
        }

        /// <summary>
        /// Is mouse moving?
        /// </summary>
        public bool MouseMoving
        {
            get { return mousePos != lastMousePos; }
        }

        public float MouseWheel { get; set; }

        #endregion

        #region Cursor
        public Cursor Cursor
        {
            get { return Application.inputContext.MouseCursor; }
            set { Application.inputContext.MouseCursor = value; }
        }
        #endregion
    }

}