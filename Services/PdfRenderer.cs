using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class PdfRenderer
{
    public static void RenderPdfToImage(string outputImagePath)
    {
        // Create the document content (common between PDF generation and image rendering)
        // var content = new Action<IDrawContext>((context) =>
        // {
        //     // Draw text and elements similar to QuestPDF generation
        //     context
        //         .Canvas
        //         .DrawText("Rendering this content to an image!",
        //             new TextStyle
        //             {
        //                 FontSize = 36,
        //                 Color = Colors.Black,
        //                 Bold = true
        //             });
        // });

        // Define the image size (example: A4 at 300 DPI)
        int width = (int)(8.27 * 300); // A4 width in inches * 300 DPI
        int height = (int)(11.69 * 300); // A4 height in inches * 300 DPI

        // Create a SkiaSharp surface to draw the content onto
        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;

        // Render the content onto Skia canvas
        // content(new SkiaDrawContext(canvas));
        //
        // // Save the rendered image to PNG file
        // using var image = surface.Snapshot();
        // using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        // using var fileStream = File.OpenWrite(outputImagePath);
        // data.SaveTo(fileStream);
    }
}