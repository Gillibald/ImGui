﻿using System.Collections.Generic;
using System.Diagnostics;
using Cairo;
using ImGui;
using System;
using ImGui.Layout;

namespace Test
{
    public class Const
    {
        public static double ItemBorderTop = 2;
        public static double ItemBorderRight = 2;
        public static double ItemBorderBottom = 2;
        public static double ItemBorderLeft = 2;

        public static double ItemPaddingTop = 5;
        public static double ItemPaddingRight = 5;
        public static double ItemPaddingBottom = 5;
        public static double ItemPaddingLeft = 5;

        public static double GroupBorderTop = 10;
        public static double GroupBorderRight = 10;
        public static double GroupBorderBottom = 10;
        public static double GroupBorderLeft = 10;

        public static double GroupPaddingTop = 5;
        public static double GroupPaddingRight = 5;
        public static double GroupPaddingBottom = 5;
        public static double GroupPaddingLeft = 5;

        public static double CellingSpacingVertical = 10;
        public static double CellingSpacingHorizontal = 5;
    }

    public class Item
    {
        public Rect rect;//border-box
        public double contentWidth;//exact content width, pre-calculated from content and style
        public double contentHeight;//exact content height, pre-calculated from content and style
        public double minWidth = 1;//minimum width of content-box
        public double maxWidth = 9999;//maximum width of content-box
        public double minHeight = 1;//minimum height of content-box
        public double maxHeight = 9999;//maximum height of content-box
        public int horizontalStretchFactor;//horizontal stretch factor
        public int verticalStretchFactor;//vertical stretch factor

        internal GUIStyle style = new GUIStyle();

        public Item()
        {
            style.Set<double>(GUIStyleName.BorderTop, Const.ItemBorderTop);
            style.Set<double>(GUIStyleName.BorderRight, Const.ItemBorderRight);
            style.Set<double>(GUIStyleName.BorderBottom, Const.ItemBorderBottom);
            style.Set<double>(GUIStyleName.BorderLeft, Const.ItemBorderLeft);

            style.Set<double>(GUIStyleName.PaddingTop, Const.ItemPaddingTop);
            style.Set<double>(GUIStyleName.PaddingRight, Const.ItemPaddingRight);
            style.Set<double>(GUIStyleName.PaddingBottom, Const.ItemPaddingBottom);
            style.Set<double>(GUIStyleName.PaddingLeft, Const.ItemPaddingLeft);
        }

        public bool HorizontallyStretched { get { return !IsFixedWidth && horizontalStretchFactor > 0; } }
        public bool VerticallyStretched { get { return !IsFixedHeight && verticalStretchFactor > 0; } }

        public bool IsFixedWidth { get { return MathEx.AmostEqual(this.minWidth, this.maxWidth); } }
        public bool IsFixedHeight { get { return MathEx.AmostEqual(this.minHeight, this.maxHeight); } }

        public Item(params LayoutOption[] options) : this()
        {
            if (options != null)
            {
                this.ApplyOptions(options);
            }
        }

        protected void ApplyOptions(LayoutOption[] options)
        {
            if (options == null)
            {
                return;
            }
            //TODO handle min/max width/height
            for (var i = 0; i < options.Length; i++)
            {
                var option = options[i];
                switch (option.type)
                {
                    case LayoutOptionType.FixedWidth:
                        if ((double) option.Value < this.style.PaddingHorizontal + this.style.BorderHorizontal)
                        {
                            throw new InvalidOperationException(
                                string.Format("The specified width is too small. It must bigger than the horizontal padding and border size ({0}).", this.style.PaddingHorizontal + this.style.BorderHorizontal));
                        }
                        this.minWidth = this.maxWidth = (double)option.Value;
                        this.horizontalStretchFactor = 0;
                        break;
                    case LayoutOptionType.FixedHeight:
                        if ((double)option.Value < this.style.PaddingVertical + this.style.BorderVertical)
                        {
                            throw new InvalidOperationException(
                                string.Format("The specified height is too small. It must bigger than the vertical padding and border size ({0}).", this.style.PaddingVertical + this.style.BorderVertical));
                        }
                        this.minHeight = this.maxHeight = (double)option.Value;
                        this.verticalStretchFactor = 0;
                        break;
                    case LayoutOptionType.StretchWidth:
                        this.horizontalStretchFactor = (int)option.Value;
                        break;
                    case LayoutOptionType.StretchHeight:
                        this.verticalStretchFactor = (int)option.Value;
                        break;
                }
            }
        }

        public virtual void CalcWidth(double unitPartWidth = -1d)
        {
            if (this.HorizontallyStretched)
            {
                if (unitPartWidth > 0)
                {
                    this.rect.Width = unitPartWidth * this.horizontalStretchFactor;
                    this.contentWidth = this.rect.Width - this.style.PaddingHorizontal - this.style.BorderHorizontal;
                }
                else
                {
                    throw new ArgumentException("The unit part width is invalid", "unitPartWidth");
                }
            }
            else if (this.IsFixedWidth)
            {
                this.rect.Width = this.minWidth;
                this.contentWidth = this.rect.Width - this.style.PaddingHorizontal - this.style.BorderHorizontal;
            }
            else
            {
                this.rect.Width = this.contentWidth + this.style.PaddingHorizontal + this.style.BorderHorizontal;
            }
        }

        public virtual void CalcHeight(double unitPartHeight = -1d)
        {
            if (this.VerticallyStretched)
            {
                if (unitPartHeight > 0)
                {
                    this.rect.Height = unitPartHeight * this.verticalStretchFactor;
                    this.contentHeight = this.rect.Height - this.style.PaddingVertical - this.style.BorderVertical;
                }
                else
                {
                    throw new ArgumentException("The unit part height is invalid", "unitPartHeight");
                }
            }
            else if (this.IsFixedHeight)
            {
                this.rect.Height = this.minHeight;
                this.contentHeight = this.rect.Height - this.style.PaddingVertical - this.style.BorderVertical;
            }
            else
            {
                this.rect.Height = this.contentHeight + this.style.PaddingVertical + this.style.BorderVertical;
            }
        }

        public virtual void SetX(double x)
        {
            this.rect.X = x;
        }

        public virtual void SetY(double y)
        {
            this.rect.Y = y;
        }

        public Item Clone() { return (Item)this.MemberwiseClone(); }
    }

    public class Group : Item
    {
        public bool isVertical;
        public bool isClipped;
        public List<Item> entries = new List<Item>();

        internal new GUIStyle style = new GUIStyle();

        public Group()
        {
            style.Set<double>(GUIStyleName.BorderTop, Const.GroupBorderTop);
            style.Set<double>(GUIStyleName.BorderRight, Const.GroupBorderRight);
            style.Set<double>(GUIStyleName.BorderBottom, Const.GroupBorderBottom);
            style.Set<double>(GUIStyleName.BorderLeft, Const.GroupBorderLeft);

            style.Set<double>(GUIStyleName.PaddingTop, Const.GroupPaddingTop);
            style.Set<double>(GUIStyleName.PaddingRight, Const.GroupPaddingRight);
            style.Set<double>(GUIStyleName.PaddingBottom, Const.GroupPaddingBottom);
            style.Set<double>(GUIStyleName.PaddingLeft, Const.GroupPaddingLeft);

            style.Set<double>(GUIStyleName.CellingSpacingVertical, Const.CellingSpacingVertical);
            style.Set<double>(GUIStyleName.CellingSpacingHorizontal, Const.CellingSpacingHorizontal);
        }

        public Group(bool isVertical, params LayoutOption[] options) : this()
        {
            this.isVertical = isVertical;
            base.ApplyOptions(options);
        }

        public void Add(Item item)
        {
            if (this.IsFixedWidth)
            {
                Debug.Assert(!this.HorizontallyStretched);
                if (this.isVertical && item.horizontalStretchFactor > 1)
                {
                    item.horizontalStretchFactor = 1;
                }
            }
            else if (this.HorizontallyStretched)
            {
                if (this.isVertical && item.horizontalStretchFactor > 1)
                {
                    item.horizontalStretchFactor = 1;
                }
            }
            else
            {
                item.horizontalStretchFactor = 0;
            }

            if (this.IsFixedHeight)
            {
                Debug.Assert(!this.VerticallyStretched);
                if (!this.isVertical && item.verticalStretchFactor > 1)
                {
                    item.verticalStretchFactor = 1;
                }
            }
            else if (this.VerticallyStretched)
            {
                if (!this.isVertical && item.verticalStretchFactor > 1)
                {
                    item.verticalStretchFactor = 1;
                }
            }
            else
            {
                item.verticalStretchFactor = 0;
            }

            this.entries.Add(item);
        }

        public override void CalcWidth(double unitPartWidth = -1)
        {
            if (this.HorizontallyStretched)//stretched width
            {
                // calculate the width
                this.rect.Width = unitPartWidth * horizontalStretchFactor;
                this.contentWidth = this.rect.Width - this.style.PaddingHorizontal - this.style.BorderHorizontal;

                if (this.contentWidth <= 0) return;//container has no space to hold the children

                // calculate the width of children
                CalcChildrenWidth();
            }
            else if (this.IsFixedWidth)//fiexed width
            {
                // calculate the width
                this.rect.Width = this.minWidth;
                this.contentWidth = this.rect.Width - this.style.PaddingHorizontal - this.style.BorderHorizontal;

                if (this.contentWidth <= 0) return;//container has no space to hold the children

                // calculate the width of children
                CalcChildrenWidth();
            }
            else // default width
            {
                if (this.isVertical) //vertical group
                {
                    var temp = 0d;
                    // get the max width of children
                    foreach (var entry in entries)
                    {
                        entry.CalcWidth();
                        temp = Math.Max(temp, entry.rect.Width);
                    }
                    this.contentWidth = temp;
                }
                else
                {
                    var temp = 0d;
                    foreach (var entry in entries)
                    {
                        entry.CalcWidth();
                        temp += entry.rect.Width + this.style.CellingSpacingHorizontal;
                    }
                    temp -= this.style.CellingSpacingHorizontal;
                    this.contentWidth = temp < 0 ? 0 : temp;
                }
                this.rect.Width = this.contentWidth + this.style.PaddingHorizontal + this.style.BorderHorizontal;
            }
        }

        private void CalcChildrenWidth()
        {
            if (this.isVertical) //vertical group
            {
                foreach (var entry in entries)
                {
                    if (entry.HorizontallyStretched)
                    {
                        entry.CalcWidth(this.contentWidth); //the unitPartWidth for stretched children is the content-box width of the group
                    }
                    else
                    {
                        entry.CalcWidth();
                    }
                }
            }
            else //horizontal group
            {
                // calculate the unitPartWidth for stretched children
                // calculate the width of fixed-size children
                var childCount = this.entries.Count;
                var totalFactor = 0;
                var totalStretchedPartWidth = this.contentWidth -
                                              this.style.CellingSpacingHorizontal * (childCount - 1);
                foreach (var entry in entries)
                {
                    if (entry.HorizontallyStretched)
                    {
                        totalFactor += entry.horizontalStretchFactor;
                    }
                    else
                    {
                        entry.CalcWidth();
                        totalStretchedPartWidth -= entry.rect.Width;
                    }
                }
                var childUnitPartWidth = totalStretchedPartWidth / totalFactor;
                // calculate the width of stretched children
                foreach (var entry in entries)
                {
                    if (entry.HorizontallyStretched)
                    {
                        entry.CalcWidth(childUnitPartWidth);
                    }
                }
            }
        }

        public override void CalcHeight(double unitPartHeight = -1)
        {
            if (this.VerticallyStretched)
            {
                // calculate the height
                this.rect.Height = unitPartHeight * this.verticalStretchFactor;
                this.contentHeight = this.rect.Height - this.style.PaddingVertical - this.style.BorderVertical;

                if (this.contentHeight < 1) return;//container has no space to hold the children

                // calculate the height of children
                CalcChildrenHeight();
            }
            else if (this.IsFixedHeight)//fiexed height
            {
                // calculate the height
                this.rect.Height = this.minHeight;
                this.contentHeight = this.rect.Height - this.style.PaddingVertical - this.style.BorderVertical;

                if (this.contentHeight < 1) return;//container has no space to hold the children

                // calculate the height of children
                CalcChildrenHeight();
            }
            else // default height
            {
                if (this.isVertical) // vertical group
                {
                    var temp = 0d;
                    foreach (var entry in entries)
                    {
                        entry.CalcHeight();
                        temp += entry.rect.Height + this.style.CellingSpacingVertical;
                    }
                    temp -= this.style.CellingSpacingVertical;
                    this.contentHeight = temp < 0 ? 0 : temp;
                }
                else // horizontal group
                {
                    var temp = 0d;
                    // get the max height of children
                    foreach (var entry in entries)
                    {
                        entry.CalcHeight();
                        temp = Math.Max(temp, entry.rect.Height);
                    }
                    this.contentHeight = temp;
                }
                this.rect.Height = this.contentHeight + this.style.PaddingVertical + this.style.BorderVertical;
            }
        }

        private void CalcChildrenHeight()
        {
            if (this.isVertical) // vertical group
            {
                // calculate the unitPartHeight for stretched children
                // calculate the height of fixed-size children
                var childCount = this.entries.Count;
                var totalStretchedPartHeight = this.contentHeight - (childCount - 1) * this.style.CellingSpacingVertical;
                var totalFactor = 0;
                foreach (var entry in entries)
                {
                    if (entry.VerticallyStretched)
                    {
                        totalFactor += entry.verticalStretchFactor;
                    }
                    else
                    {
                        entry.CalcHeight();
                        totalStretchedPartHeight -= entry.rect.Height;
                    }
                }
                var childUnitPartHeight = totalStretchedPartHeight / totalFactor;
                // calculate the height of stretched children
                foreach (var entry in entries)
                {
                    if (entry.VerticallyStretched)
                    {
                        entry.CalcHeight(childUnitPartHeight);
                    }
                }
            }
            else // horizontal group
            {
                foreach (var entry in entries)
                {
                    if (entry.VerticallyStretched)
                    {
                        entry.CalcHeight(this.contentHeight);
                        //the unitPartHeight for stretched children is the content-box height of the group
                    }
                    else
                    {
                        entry.CalcHeight();
                    }
                }
            }
        }

        public override void SetX(double x)
        {
            this.rect.X = x;
            if (this.isVertical)
            {
                var childX = 0d;
                foreach (var entry in entries)
                {
                    switch (this.style.AlignmentHorizontal)
                    {
                        case Alignment.Start:
                            childX = x + this.style.BorderLeft + this.style.PaddingLeft;
                            break;
                        case Alignment.Center:
                        case Alignment.SpaceAround:
                        case Alignment.SpaceBetween:
                            childX = x + this.style.BorderLeft + this.style.PaddingLeft + (this.contentWidth - entry.rect.Width) / 2;
                            break;
                        case Alignment.End:
                            childX = x + this.rect.Width - this.style.BorderRight - this.style.PaddingRight - entry.rect.Width;
                            break;
                    }
                    entry.SetX(childX);
                }
            }
            else
            {
                var nextX = 0d;

                var childWidthWithCellSpcaing = 0d;
                var childWidthWithoutCellSpcaing = 0d;
                foreach (var entry in entries)
                {
                    childWidthWithCellSpcaing += entry.rect.Width + this.style.CellingSpacingHorizontal;
                    childWidthWithoutCellSpcaing += entry.rect.Width;
                }
                childWidthWithCellSpcaing -= this.style.CellingSpacingVertical;

                switch (this.style.AlignmentHorizontal)
                {
                    case Alignment.Start:
                        nextX = x + this.style.BorderLeft + this.style.PaddingLeft;
                        break;
                    case Alignment.Center:
                        nextX = x + this.style.BorderLeft + this.style.PaddingLeft + (this.contentWidth - childWidthWithCellSpcaing) / 2;
                        break;
                    case Alignment.End:
                        nextX = x + this.rect.Width - this.style.BorderRight - this.style.PaddingRight - childWidthWithCellSpcaing;
                        break;
                    case Alignment.SpaceAround:
                        nextX = x + this.style.BorderLeft + this.style.PaddingLeft +
                                (this.contentWidth - childWidthWithoutCellSpcaing) / (this.entries.Count + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextX = x + this.style.BorderLeft + this.style.PaddingLeft;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in entries)
                {
                    entry.SetX(nextX);
                    switch (this.style.AlignmentHorizontal)
                    {
                        case Alignment.Start:
                        case Alignment.Center:
                        case Alignment.End:
                            nextX += entry.rect.Width + this.style.CellingSpacingHorizontal;
                            break;
                        case Alignment.SpaceAround:
                            nextX += entry.rect.Width + (this.contentWidth - childWidthWithoutCellSpcaing) / (this.entries.Count + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextX += entry.rect.Width + (this.contentWidth - childWidthWithoutCellSpcaing) / (this.entries.Count - 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public override void SetY(double y)
        {
            this.rect.Y = y;
            if (this.isVertical)
            {
                var nextY = 0d;

                var childHeightWithCellSpcaing = 0d;
                var childHeightWithoutCellSpcaing = 0d;
                foreach (var entry in this.entries)
                {
                    childHeightWithCellSpcaing += entry.rect.Height + this.style.CellingSpacingVertical;
                    childHeightWithoutCellSpcaing += entry.rect.Height;
                }
                childHeightWithCellSpcaing -= this.style.CellingSpacingVertical;

                switch (this.style.AlignmentVertical)
                {
                    case Alignment.Start:
                        nextY = y + this.style.BorderTop + this.style.PaddingTop;
                        break;
                    case Alignment.Center:
                        nextY = y + this.style.BorderTop + this.style.PaddingTop + (this.contentHeight - childHeightWithCellSpcaing) / 2;
                        break;
                    case Alignment.End:
                        nextY = y + this.rect.Height - this.style.BorderBottom - this.style.PaddingBottom - childHeightWithCellSpcaing;
                        break;
                    case Alignment.SpaceAround:
                        nextY = y + this.style.BorderTop + this.style.PaddingTop +
                                (this.contentHeight - childHeightWithoutCellSpcaing) / (this.entries.Count + 1);
                        break;
                    case Alignment.SpaceBetween:
                        nextY = y + this.style.BorderTop + this.style.PaddingTop;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var entry in entries)
                {
                    entry.SetY(nextY);
                    switch (this.style.AlignmentVertical)
                    {
                        case Alignment.Start:
                        case Alignment.Center:
                        case Alignment.End:
                            nextY += entry.rect.Height + this.style.CellingSpacingVertical;
                            break;
                        case Alignment.SpaceAround:
                            nextY += entry.rect.Height + (this.contentHeight - childHeightWithoutCellSpcaing) / (this.entries.Count + 1);
                            break;
                        case Alignment.SpaceBetween:
                            nextY += entry.rect.Height + (this.contentHeight - childHeightWithoutCellSpcaing) / (this.entries.Count - 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                var childY = 0d;
                foreach (var entry in entries)
                {
                    switch (this.style.AlignmentVertical)
                    {
                        case Alignment.Start:
                            childY = y + this.style.BorderTop + this.style.PaddingTop;
                            break;
                        case Alignment.Center:
                        case Alignment.SpaceAround:
                        case Alignment.SpaceBetween:
                            childY = y + this.style.BorderTop + this.style.PaddingTop + (this.contentHeight - entry.rect.Height) / 2;
                            break;
                        case Alignment.End:
                            childY += y + this.rect.Height - this.style.BorderBottom - this.style.PaddingBottom - entry.rect.Height;
                            break;
                    }
                    entry.SetY(childY);
                }
            }
        }

        private void Draw(Cairo.Context context, bool needClip)
        {
            if (needClip)
            {
                context.Rectangle(rect.X + style.PaddingLeft + style.BorderLeft, rect.Y + style.PaddingTop + style.BorderTop,
                    rect.Width - style.PaddingHorizontal - style.BorderHorizontal, rect.Height - style.PaddingVertical - style.BorderVertical);
                //context.StrokePreserve();
                context.Clip();
            }
            foreach (var entry in this.entries)
            {
                if (entry.HorizontallyStretched || entry.VerticallyStretched)
                {
                    context.FillRectangle(entry.rect, CairoEx.ColorLightBlue);
                }
                else if (entry.IsFixedWidth || entry.IsFixedHeight)
                {
                    context.FillRectangle(entry.rect, CairoEx.ColorOrange);
                }
                else
                {
                    context.FillRectangle(entry.rect, CairoEx.ColorPink);
                }
                context.StrokeRectangle(entry.rect, CairoEx.ColorBlack);
                var innerGroup = entry as Group;
                if (innerGroup != null)
                {
                    context.Save();
                    innerGroup.Draw(context, needClip);
                    context.Restore();
                }
            }
            if (needClip)
            {
                context.ResetClip();
            }
        }

        public void ShowResult([System.Runtime.CompilerServices.CallerMemberName] string memberName = null)
        {
            var surface = CairoEx.BuildSurface((int)rect.Width, (int)rect.Height, CairoEx.ColorMetal, Format.Rgb24);
            var context = new Cairo.Context(surface);

            Draw(context, needClip: true);

            string outputPath = "D:\\LayoutTest";
            if (!System.IO.Directory.Exists(outputPath))
            {
                System.IO.Directory.CreateDirectory(outputPath);
            }

            string filePath = outputPath + "\\" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fff_") + surface.GetHashCode() + memberName + ".png";
            surface.WriteToPng(filePath);
            surface.Dispose();
            context.Dispose();

            Process.Start("rundll32.exe", @"""C:\Program Files\Windows Photo Viewer\PhotoViewer.dll"",ImageView_Fullscreen " + filePath);
        }
    }

    public static class Styles
    {
        public static readonly GUIStyle DefaultEntryStyle;
        public static readonly GUIStyle DefaultGroupStyle;

        static Styles()
        {
            {
                var style = new GUIStyle();
                style.Set<double>(GUIStyleName.BorderTop, Const.ItemBorderTop);
                style.Set<double>(GUIStyleName.BorderRight, Const.ItemBorderRight);
                style.Set<double>(GUIStyleName.BorderBottom, Const.ItemBorderBottom);
                style.Set<double>(GUIStyleName.BorderLeft, Const.ItemBorderLeft);

                style.Set<double>(GUIStyleName.PaddingTop, Const.ItemPaddingTop);
                style.Set<double>(GUIStyleName.PaddingRight, Const.ItemPaddingRight);
                style.Set<double>(GUIStyleName.PaddingBottom, Const.ItemPaddingBottom);
                style.Set<double>(GUIStyleName.PaddingLeft, Const.ItemPaddingLeft);
                DefaultEntryStyle = style;
            }

            {
                var style = new GUIStyle();
                style.Set<double>(GUIStyleName.BorderTop, Const.GroupBorderTop);
                style.Set<double>(GUIStyleName.BorderRight, Const.GroupBorderRight);
                style.Set<double>(GUIStyleName.BorderBottom, Const.GroupBorderBottom);
                style.Set<double>(GUIStyleName.BorderLeft, Const.GroupBorderLeft);

                style.Set<double>(GUIStyleName.PaddingTop, Const.GroupPaddingTop);
                style.Set<double>(GUIStyleName.PaddingRight, Const.GroupPaddingRight);
                style.Set<double>(GUIStyleName.PaddingBottom, Const.GroupPaddingBottom);
                style.Set<double>(GUIStyleName.PaddingLeft, Const.GroupPaddingLeft);

                style.Set<double>(GUIStyleName.CellingSpacingVertical, Const.CellingSpacingVertical);
                style.Set<double>(GUIStyleName.CellingSpacingHorizontal, Const.CellingSpacingHorizontal);
                DefaultGroupStyle = style;
            }
        }
    }

    internal static class TestExtensions
    {
        public static void ShowResult(this LayoutGroup group, [System.Runtime.CompilerServices.CallerMemberName] string memberName = null)
        {
            var rect = group.Rect;
            var surface = CairoEx.BuildSurface((int)rect.Width, (int)rect.Height, CairoEx.ColorMetal, Format.Rgb24);
            var context = new Cairo.Context(surface);

            Draw(group, context, needClip: true);

            string outputPath = "D:\\LayoutTest";
            if (!System.IO.Directory.Exists(outputPath))
            {
                System.IO.Directory.CreateDirectory(outputPath);
            }

            string filePath = outputPath + "\\" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fff_") + surface.GetHashCode() + memberName + ".png";
            surface.WriteToPng(filePath);
            surface.Dispose();
            context.Dispose();

            Process.Start("rundll32.exe", @"""C:\Program Files\Windows Photo Viewer\PhotoViewer.dll"",ImageView_Fullscreen " + filePath);
        }

        private static void Draw(LayoutGroup group, Cairo.Context context, bool needClip)
        {
            if (needClip)
            {
                var rect = group.Rect;
                var style = group.Style;
                context.Rectangle(rect.X + style.PaddingLeft + style.BorderLeft, rect.Y + style.PaddingTop + style.BorderTop,
                    rect.Width - style.PaddingHorizontal - style.BorderHorizontal, rect.Height - style.PaddingVertical - style.BorderVertical);
                //context.StrokePreserve();
                context.Clip();
            }
            foreach (var entry in @group.Entries)
            {
                if (entry.HorizontallyStretched || entry.VerticallyStretched)
                {
                    context.FillRectangle(entry.Rect, CairoEx.ColorLightBlue);
                }
                else if (entry.IsFixedWidth || entry.IsFixedHeight)
                {
                    context.FillRectangle(entry.Rect, CairoEx.ColorOrange);
                }
                else
                {
                    context.FillRectangle(entry.Rect, CairoEx.ColorPink);
                }
                context.StrokeRectangle(entry.Rect, CairoEx.ColorBlack);
                var innerGroup = entry as LayoutGroup;
                if (innerGroup != null)
                {
                    context.Save();
                    Draw(innerGroup, context, needClip);
                    context.Restore();
                }
            }
            if (needClip)
            {
                context.ResetClip();
            }
        }

    }
}
