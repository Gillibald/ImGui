using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Layout;
using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal class RenderTree
    {
        public Node Root { get; }

        private Node currentContainer;
        public Node CurrentContainer
        {
            get
            {
                return currentContainer ?? Root;
            }
            set
            {
                currentContainer = value;
            }
        }

        public RenderTree(int rootId, Point position, Size size)
        {
            Root = new Node(rootId);
            Root.Rect = new Rect(position, size);
            Root.AttachLayoutGroup(true, GUILayout.Width((int)size.Width).Height((int)size.Height));

            Debug.Assert(Root.IsFixedWidth);
            Debug.Assert(Root.IsFixedHeight);
            Debug.Assert(Root.IsVertical);
        }

        public Node GetNodeById(int id)
        {
            return Root.GetNodeById(id);
        }

        public void Foreach(Func<Node, bool> func)
        {
            Root.Foreach(func);
        }
    }
}
