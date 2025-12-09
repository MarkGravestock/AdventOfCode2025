using System.Text;
using NetTopologySuite.Geometries;

namespace MarkGravestock.AdventOfCode2025.Day09;

public class SvgGenerator(List<Corner> tiles, Polygon boundary, string outputPath)
{
    public string GenerateVisualization(List<Rectangle> rectangles)
    {
        var (minX, maxX, minY, maxY) = CalculateBounds();
        var (viewBoxMinX, viewBoxMinY, viewBoxWidth, viewBoxHeight) = CalculateViewBox(minX, maxX, minY, maxY);

        var svg = new StringBuilder();
        AppendSvgHeader(svg, viewBoxMinX, viewBoxMinY, viewBoxWidth, viewBoxHeight);

        var (minArea, maxArea, strokeWidth, boundaryStrokeWidth) = CalculateRenderingParameters(rectangles, viewBoxWidth, viewBoxHeight);

        RenderRectangles(svg, rectangles, minArea, maxArea, strokeWidth);
        RenderBoundary(svg, strokeWidth);

        AppendSvgFooter(svg);

        SaveToFile(svg.ToString());
        return Path.GetFullPath(outputPath);
    }

    private (double minX, double maxX, double minY, double maxY) CalculateBounds()
    {
        var minX = tiles.Min(t => t.X.Value);
        var maxX = tiles.Max(t => t.X.Value);
        var minY = tiles.Min(t => t.Y.Value);
        var maxY = tiles.Max(t => t.Y.Value);
        return (minX, maxX, minY, maxY);
    }

    private (double viewBoxMinX, double viewBoxMinY, double viewBoxWidth, double viewBoxHeight) CalculateViewBox(
        double minX, double maxX, double minY, double maxY)
    {
        var width = maxX - minX;
        var height = maxY - minY;
        var padding = Math.Max(width, height) * 0.05;

        var viewBoxMinX = minX - padding;
        var viewBoxMinY = minY - padding;
        var viewBoxWidth = width + (2 * padding);
        var viewBoxHeight = height + (2 * padding);

        return (viewBoxMinX, viewBoxMinY, viewBoxWidth, viewBoxHeight);
    }

    private void AppendSvgHeader(StringBuilder svg, double viewBoxMinX, double viewBoxMinY, double viewBoxWidth, double viewBoxHeight)
    {
        svg.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"{viewBoxMinX} {viewBoxMinY} {viewBoxWidth} {viewBoxHeight}\">");
    }

    private (long minArea, long maxArea, double strokeWidth, double boundaryStrokeWidth) CalculateRenderingParameters(
        List<Rectangle> rectangles, double viewBoxWidth, double viewBoxHeight)
    {
        var minArea = rectangles.Min(r => r.Area);
        var maxArea = rectangles.Max(r => r.Area);
        var strokeWidth = Math.Min(viewBoxWidth, viewBoxHeight) * 0.003;
        var boundaryStrokeWidth = Math.Min(viewBoxWidth, viewBoxHeight) * 0.015;

        return (minArea, maxArea, strokeWidth, boundaryStrokeWidth);
    }

    private void RenderRectangles(StringBuilder svg, List<Rectangle> rectangles, long minArea, long maxArea, double strokeWidth)
    {
        foreach (var rect in rectangles.OrderBy(r => r.Area))
        {
            var (rectMinX, rectMinY, rectWidth, rectHeight) = CalculateRectangleDimensions(rect);
            var color = CalculateColor(rect.Area, minArea, maxArea);

            svg.AppendLine($"  <rect x=\"{rectMinX}\" y=\"{rectMinY}\" width=\"{rectWidth}\" height=\"{rectHeight}\" " +
                          $"fill=\"{color}\" fill-opacity=\"0.7\" stroke=\"hsl(120, 70%, 20%)\" stroke-width=\"{strokeWidth}\"/>");
        }
    }

    private (int rectMinX, int rectMinY, int rectWidth, int rectHeight) CalculateRectangleDimensions(Rectangle rect)
    {
        var rectMinX = Math.Min(rect.Corner1.X.Value, rect.Corner2.X.Value);
        var rectMinY = Math.Min(rect.Corner1.Y.Value, rect.Corner2.Y.Value);
        var rectWidth = Math.Abs(rect.Corner1.X.Value - rect.Corner2.X.Value) + 1;
        var rectHeight = Math.Abs(rect.Corner1.Y.Value - rect.Corner2.Y.Value) + 1;

        return (rectMinX, rectMinY, rectWidth, rectHeight);
    }

    private string CalculateColor(long area, long minArea, long maxArea)
    {
        var normalized = (double)(area - minArea) / (maxArea - minArea);
        var lightness = 80 - (normalized * 50);
        return $"hsl(120, 70%, {lightness:F1}%)";
    }

    private void RenderBoundary(StringBuilder svg, double strokeWidth)
    {
        var coordinates = boundary.Coordinates;
        var points = string.Join(" ", coordinates.Select(c => $"{c.X},{c.Y}"));
        svg.AppendLine($"  <polygon points=\"{points}\" fill=\"none\" stroke=\"black\" stroke-width=\"{strokeWidth}\" stroke-linejoin=\"round\"/>");
    }

    private void AppendSvgFooter(StringBuilder svg)
    {
        svg.AppendLine("</svg>");
    }

    private void SaveToFile(string svgContent)
    {
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(outputPath, svgContent);
    }
}
