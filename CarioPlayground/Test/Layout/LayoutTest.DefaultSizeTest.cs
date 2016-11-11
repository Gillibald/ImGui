﻿using System.Collections.Generic;
using ImGui;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test
{
    [TestClass]
    public class DefaultSizeTest
    {
        [TestMethod, TestCategory("rect & content size"), Description("The size of an entry is correctly calculated")]
        public void TheSizeOfAnEntryIsCorrectlyCalculated()
        {
            Item item = new Item {contentWidth = 50, contentHeight = 50};

            item.CalcWidth();
            item.CalcHeight();

            Assert.AreEqual(item.contentWidth + Const.ItemPaddingLeft + Const.ItemPaddingRight + Const.ItemBorderLeft + Const.ItemBorderRight, item.rect.Width);
            Assert.AreEqual(item.contentHeight + Const.ItemPaddingTop + Const.ItemPaddingBottom + Const.ItemBorderTop + Const.ItemBorderBottom, item.rect.Height);
        }

        [TestMethod, TestCategory("rect & content size"), Description("The size of an empty vertical group is correctly calculated")]
        public void TheSizeOfAEmptyVerticalGroupIsCorrectlyCalculated()
        {
            Group group = new Group(true);

            group.CalcWidth();
            group.CalcHeight();

            Assert.AreEqual(group.style.PaddingHorizontal + group.style.BorderHorizontal, group.rect.Width);
            Assert.AreEqual(group.style.PaddingVertical + group.style.BorderVertical, group.rect.Height);
        }

        [TestMethod, TestCategory("rect & content size"), Description("The size of a vertical group that contains a single entry is correctly calculated")]
        public void TheSizeOfAVerticalGroupThatContainsASingleEntryIsCorrectlyCalculated()
        {
            Group group = new Group(true);
            Item item = new Item { contentWidth = 50, contentHeight = 50 };
            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();

            Assert.AreEqual(item.rect.Width + group.style.PaddingHorizontal + group.style.BorderHorizontal, group.rect.Width);
            Assert.AreEqual(item.rect.Height + group.style.PaddingVertical + group.style.BorderVertical, group.rect.Height);
        }

        [TestMethod, TestCategory("rect & content size"), Description("The size of a vertical group that contains multiple entries is correctly calculated")]
        public void TheSizeOfAVerticalGroupThatContainsMultipleEntriesIsCorrectlyCalculated()
        {
            Group group = new Group(true);
            Item[] items =
            {
                new Item{contentWidth = 10, contentHeight = 20},
                new Item{contentWidth = 20, contentHeight = 30},
                new Item{contentWidth = 30, contentHeight = 40},
                new Item{contentWidth = 40, contentHeight = 50},
                new Item{contentWidth = 50, contentHeight = 60},
            };
            foreach (var item in items)
            {
                group.Add(item);
            }

            group.CalcWidth();
            group.CalcHeight();

            var expectedWidth = 0d;
            var expectedHeight = 0d;
            foreach (var item in items)
            {
                expectedWidth = Math.Max(expectedWidth, item.rect.Width);
                expectedHeight += item.rect.Height + group.style.CellingSpacingVertical;
            }
            expectedHeight -= group.style.CellingSpacingVertical;
            expectedWidth += group.style.PaddingHorizontal + group.style.BorderHorizontal;
            expectedHeight += group.style.PaddingVertical + group.style.BorderVertical;
            Assert.AreEqual(expectedWidth, group.rect.Width);
            Assert.AreEqual(expectedHeight, group.rect.Height);
        }

        [TestMethod, TestCategory("rect & content size"), Description("The size of a horizontal group that contains a single entry is correctly calculated")]
        public void TheSizeOfAHorizontalGroupThatContainsASingleEntryIsCorrectlyCalculated()
        {
            Group group = new Group(false);
            Item item = new Item { contentWidth = 50, contentHeight = 50 };
            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();

            Assert.AreEqual(item.rect.Width + group.style.PaddingHorizontal + group.style.BorderHorizontal, group.rect.Width);
            Assert.AreEqual(item.rect.Height + group.style.PaddingVertical + group.style.BorderVertical, group.rect.Height);
        }

        [TestMethod, TestCategory("rect & content size"), Description("The size of a horizontal group that contains multiple entries is correctly calculated")]
        public void TheSizeOfAHorizontalGroupThatContainsMultipleEntriesIsCorrectlyCalculated()
        {
            Group group = new Group(false);
            Item[] items =
            {
                new Item{contentWidth = 10, contentHeight = 20},
                new Item{contentWidth = 20, contentHeight = 30},
                new Item{contentWidth = 30, contentHeight = 40},
                new Item{contentWidth = 40, contentHeight = 50},
                new Item{contentWidth = 50, contentHeight = 60},
            };
            foreach (var item in items)
            {
                group.Add(item);
            }

            group.CalcWidth();
            group.CalcHeight();

            var expectedWidth = 0d;
            var expectedHeight = 0d;
            foreach (var item in items)
            {
                expectedWidth += item.rect.Width + group.style.CellingSpacingHorizontal;
                expectedHeight = Math.Max(expectedHeight, item.rect.Height);
            }
            expectedWidth -= group.style.CellingSpacingHorizontal;
            expectedWidth += group.style.PaddingHorizontal + group.style.BorderHorizontal;
            expectedHeight += group.style.PaddingVertical + group.style.BorderHorizontal;
            Assert.AreEqual(expectedWidth, group.rect.Width);
            Assert.AreEqual(expectedHeight, group.rect.Height);
        }


        [TestMethod, TestCategory("layout"), Description("Show an empty horizontal group")]
        public void ShowAnEmptyHorizontalGroup()
        {
            Group group = new Group(false);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [TestMethod, TestCategory("layout"), Description("Show a horizontal group of 1 item")]
        public void ShowAHorizontalGroupOf1Item()
        {
            Group group = new Group(false);
            Item item = new Item {contentWidth = 50, contentHeight = 50};
            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [TestMethod, TestCategory("layout"), Description("Show a horizontal group of 3 items")]
        public void ShowAHorizontalGroupOf3Items()
        {
            Group group = new Group(false);
            Item item = new Item { contentWidth = 50, contentHeight = 50 };
            group.Add(item.Clone());
            group.Add(item.Clone());
            group.Add(item.Clone());

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [TestMethod, TestCategory("layout"), Description("Show an empty vertical group")]
        public void ShowAnEmptyVerticalGroup()
        {
            Group group = new Group(true);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [TestMethod, TestCategory("layout"), Description("Show a vertical group of 1 items")]
        public void ShowAVerticalGroupOf1Items()
        {
            Group group = new Group(true);
            Item item = new Item { contentWidth = 50, contentHeight = 50 };
            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [TestMethod, TestCategory("layout"), Description("Show a vertical group of 3 items")]
        public void ShowAVerticalGroupOf3Items()
        {
            Group group = new Group(true);
            Item item = new Item { contentWidth = 50, contentHeight = 50 };
            group.Add(item.Clone());
            group.Add(item.Clone());
            group.Add(item.Clone());

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [TestMethod, TestCategory("layout"), Description("Show a group of 3x3 items, outter group is vertical")]
        public void ShowAGroupOf3x3Items_OutterGroupIsVertical()
        {
            Group outterGroup = new Group(true);
            Group innerGroup0 = new Group(false);
            Group innerGroup1 = new Group(false);
            Group innerGroup2 = new Group(false);
            Item item = new Item { contentWidth = 50, contentHeight = 50 };
            innerGroup0.Add(item.Clone());
            innerGroup0.Add(item.Clone());
            innerGroup0.Add(item.Clone());
            innerGroup1.Add(item.Clone());
            innerGroup1.Add(item.Clone());
            innerGroup1.Add(item.Clone());
            innerGroup2.Add(item.Clone());
            innerGroup2.Add(item.Clone());
            innerGroup2.Add(item.Clone());
            outterGroup.Add(innerGroup0);
            outterGroup.Add(innerGroup1);
            outterGroup.Add(innerGroup2);

            outterGroup.CalcWidth();
            outterGroup.CalcHeight();
            outterGroup.SetX(0);
            outterGroup.SetY(0);

            outterGroup.ShowResult();
        }

        [TestMethod, TestCategory("layout"), Description("Show a group of 3x3 items, outter group is horizontal")]
        public void ShowAGroupOf3x3Items_OutterGroupIsHorizontal()
        {
            Group outterGroup = new Group(false);
            Group innerGroup0 = new Group(true);
            Group innerGroup1 = new Group(true);
            Group innerGroup2 = new Group(true);
            Item item = new Item { contentWidth = 50, contentHeight = 50 };
            innerGroup0.Add(item.Clone());
            innerGroup0.Add(item.Clone());
            innerGroup0.Add(item.Clone());
            innerGroup1.Add(item.Clone());
            innerGroup1.Add(item.Clone());
            innerGroup1.Add(item.Clone());
            innerGroup2.Add(item.Clone());
            innerGroup2.Add(item.Clone());
            innerGroup2.Add(item.Clone());
            outterGroup.Add(innerGroup0);
            outterGroup.Add(innerGroup1);
            outterGroup.Add(innerGroup2);

            outterGroup.CalcWidth();
            outterGroup.CalcHeight();
            outterGroup.SetX(0);
            outterGroup.SetY(0);

            outterGroup.ShowResult();
        }

        [TestMethod, TestCategory("layout"), Description("Show a 3 layer group")]
        public void ShowA3LayerGroup()
        {
            Group group1 = new Group(true);
            Group group2 = new Group(false);
            Group group3 = new Group(false);
            Group group4 = new Group(false);
            Group group5 = new Group(true);
            Item item1 = new Item { contentWidth = 50, contentHeight = 50 };
            Item item2 = new Item { contentWidth = 50, contentHeight = 80 };
            Item item3 = new Item { contentWidth = 80, contentHeight = 50 };
            Item item4 = new Item { contentWidth = 400, contentHeight = 50 };

            group1.Add(group2);
            group1.Add(group3);
            group1.Add(group4);

            group2.Add(item1.Clone());
            group2.Add(item2.Clone());
            group2.Add(item3.Clone());

            group3.Add(item1.Clone());
            group3.Add(group5);
            group3.Add(item1.Clone());

            group4.Add(item4.Clone());

            group5.Add(item1.Clone());
            group5.Add(item2.Clone());
            group5.Add(item1.Clone());

            group1.CalcWidth();
            group1.CalcHeight();
            group1.SetX(0);
            group1.SetY(0);

            group1.ShowResult();
        }

    }
}