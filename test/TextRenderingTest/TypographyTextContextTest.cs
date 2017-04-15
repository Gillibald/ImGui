﻿using ImGui;
using System;
using System.Diagnostics;
using Xunit;
using System.Collections.Generic;
using System.Text;
using Typography.Rendering;
using System.IO;

namespace TextRenderingTest
{
    public class TypographyTextContextTest
    {
        private const string ModelViewerPath = @"E:\Program Files (green)\open3mod_1_1_standalone\open3mod.exe";

        public TypographyTextContextTest()
        {
            Application.InitSysDependencies();
        }
        
        const string PathImagePath = "D:\\typography_path_image.png";
        const string PathTextPath = "D:\\typography_path_text.txt";
        Cairo.ImageSurface surface;
        Cairo.Context g;

        [Fact]
        public void ShouldGetCorrectOutline()
        {
            var printer = new TextPrinter();
            printer.FontFilename = @"W:\VS2015\msjh.ttf";
            printer.FontSizeInPoints = 36;

            // prepare debug contexts
            {
                surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 2000, 2000);
                g = new Cairo.Context(surface);
                g.SetSourceRGBA(1, 1, 1, 1);
                g.Paint();
                g.SetSourceRGBA(0, 0, 0, 1);
                g.LineWidth = 1;
            }

            // build path
            CairoPathBuilder cairoPathBuilder;
            {
                cairoPathBuilder = new CairoPathBuilder(g, 0, 0, 1);
                printer.Draw(cairoPathBuilder, "8".ToCharArray(), 0, 0);
            }

            // show debug results
            // image
            {
                surface.WriteToPng(PathImagePath);
                g.Dispose();
            
                Assert.False(PathImagePath.Contains(" "));
                // open the image in Windows Photo Viewer
                Process.Start(@"C:\WINDOWS\System32\rundll32.exe", @"""C:\Program Files\Windows Photo Viewer\PhotoViewer.dll"", ImageView_Fullscreen " + PathImagePath);
            }
            //text
            {
                var text = cairoPathBuilder.Result;
                File.WriteAllText(PathTextPath, text);
                Process.Start(@"notepad", PathTextPath);
            }

            // Now, check if the output text and image shows a right outline of the text.
        }

        [Fact]
        public void ShouldGetCorrectOffsetOfAGlyph()
        {
            var style = GUIStyle.Default;
            style.Set<double>(GUIStyleName.FontSize, 36);

            var state = GUIState.Normal;
            var fontFamily = style.Get<string>(GUIStyleName.FontFamily, state);
            var fontSize = style.Get<double>(GUIStyleName.FontSize, state);
            var fontStretch = (FontStretch)style.Get<int>(GUIStyleName.FontStretch, state);
            var fontStyle = (FontStyle)style.Get<int>(GUIStyleName.FontStyle, state);
            var fontWeight = (FontWeight)style.Get<int>(GUIStyleName.FontWeight, state);
            var textAlignment = (TextAlignment)style.Get<int>(GUIStyleName.TextAlignment, state);
            
            var rect = new Rect(0, 0, 200, 200);
            var textContext = new TypographyTextContext(
                "A",
                fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                (int)Math.Ceiling(rect.Size.Width), (int)Math.Ceiling(rect.Size.Height),
                textAlignment);

            var textMesh = new TextMesh();
            textMesh.Build(new Point(0, fontSize), style, textContext);

            DrawList drawList = new DrawList();
            drawList.Append(textMesh);

            var objFilePath = "D:\\TextRenderingTest_ShouldGetARightMeshFromTypography.obj";
            Utility.SaveToObjFile(objFilePath, drawList.DrawBuffer.VertexBuffer, drawList.DrawBuffer.IndexBuffer);
            Process.Start(ModelViewerPath, objFilePath);
        }


        [Fact]
        public void ShouldGetARightMeshFromTypography()
        {
            var style = GUIStyle.Default;
            style.Set<double>(GUIStyleName.FontSize, 8);

            var state = GUIState.Normal;
            var fontFamily = style.Get<string>(GUIStyleName.FontFamily, state);
            var fontSize = style.Get<double>(GUIStyleName.FontSize, state);
            var fontStretch = (FontStretch)style.Get<int>(GUIStyleName.FontStretch, state);
            var fontStyle = (FontStyle)style.Get<int>(GUIStyleName.FontStyle, state);
            var fontWeight = (FontWeight)style.Get<int>(GUIStyleName.FontWeight, state);
            var textAlignment = (TextAlignment)style.Get<int>(GUIStyleName.TextAlignment, state);

            var rect = new Rect(0, 0, 200, 200);
            var textContext = new TypographyTextContext(
                "8",
                fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                (int)Math.Ceiling(rect.Size.Width), (int)Math.Ceiling(rect.Size.Height),
                textAlignment);

            var textMesh = new TextMesh();
            textMesh.Build(new Point(0, fontSize), style, textContext);

            //PathUtil.SaveToPng(paths, @"D:\TypographyTextPath.png");

            DrawList drawList = new DrawList();
            drawList.Append(textMesh);
            
            var objFilePath = "D:\\TextRenderingTest_ShouldGetARightMeshFromTypography.obj";
            Utility.SaveToObjFile(objFilePath, drawList.DrawBuffer.VertexBuffer, drawList.DrawBuffer.IndexBuffer);
            Process.Start(ModelViewerPath, objFilePath);
        }

        [Fact]
        public void ShouldGetARightMeshAfterAppendingATextMesh()
        {
            var style = GUIStyle.Default;
            style.Set<double>(GUIStyleName.FontSize, 36);

            var state = GUIState.Normal;
            var fontFamily = style.Get<string>(GUIStyleName.FontFamily, state);
            var fontSize = style.Get<double>(GUIStyleName.FontSize, state);
            var fontStretch = (FontStretch)style.Get<int>(GUIStyleName.FontStretch, state);
            var fontStyle = (FontStyle)style.Get<int>(GUIStyleName.FontStyle, state);
            var fontWeight = (FontWeight)style.Get<int>(GUIStyleName.FontWeight, state);
            var textAlignment = (TextAlignment)style.Get<int>(GUIStyleName.TextAlignment, state);

            var rect = new Rect(0, 0, 200, 200);
            var textContext = new TypographyTextContext(
                "A",
                fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                (int)Math.Ceiling(rect.Size.Width), (int)Math.Ceiling(rect.Size.Height),
                textAlignment);

            var textMesh = new TextMesh();
            textMesh.Build(Point.Zero, style, textContext);

            var anotherTextContext = new TypographyTextContext(
                "auto-sized",
                fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                (int)Math.Ceiling(rect.Size.Width), (int)Math.Ceiling(rect.Size.Height),
                textAlignment);

            var anotherTextMesh = new TextMesh();
            anotherTextMesh.Build(new Point(50, 100), style, anotherTextContext);

            DrawList drawList = new DrawList();
            var expectedVertexCount = 0;
            var expectedIndexCount = 0;

            drawList.AddRectFilled(Point.Zero, new Point(200, 100), Color.Metal);
            expectedVertexCount += 4;
            expectedIndexCount += 6;
            Assert.Equal(drawList.DrawBuffer.VertexBuffer.Count, expectedVertexCount);
            Assert.Equal(drawList.DrawBuffer.IndexBuffer.Count, expectedIndexCount);

            drawList.Append(textMesh);
            expectedVertexCount += textMesh.VertexBuffer.Count;
            expectedIndexCount += textMesh.IndexBuffer.Count;
            Assert.Equal(drawList.DrawBuffer.VertexBuffer.Count, expectedVertexCount);
            Assert.Equal(drawList.DrawBuffer.IndexBuffer.Count, expectedIndexCount);

            drawList.AddRectFilled(new Point(0, 110), new Point(200, 150), Color.Metal);
            expectedVertexCount += 4;
            expectedIndexCount += 6;
            Assert.Equal(drawList.DrawBuffer.VertexBuffer.Count, expectedVertexCount);
            Assert.Equal(drawList.DrawBuffer.IndexBuffer.Count, expectedIndexCount);

            drawList.AddRectFilled(new Point(0, 160), new Point(200, 200), Color.Metal);
            expectedVertexCount += 4;
            expectedIndexCount += 6;
            Assert.Equal(drawList.DrawBuffer.VertexBuffer.Count, expectedVertexCount);
            Assert.Equal(drawList.DrawBuffer.IndexBuffer.Count, expectedIndexCount);

            drawList.Append(anotherTextMesh);
            expectedVertexCount += anotherTextMesh.VertexBuffer.Count;
            expectedIndexCount += anotherTextMesh.IndexBuffer.Count;
            Assert.Equal(drawList.DrawBuffer.VertexBuffer.Count, expectedVertexCount);
            Assert.Equal(drawList.DrawBuffer.IndexBuffer.Count, expectedIndexCount);

            var objFilePath = "D:\\Typography_ShouldGetARightMeshAfterAppendingATextMesh.obj";
            Utility.SaveToObjFile(objFilePath, drawList.DrawBuffer.VertexBuffer, drawList.DrawBuffer.IndexBuffer);
            Process.Start(ModelViewerPath, objFilePath);
        }
    }
}