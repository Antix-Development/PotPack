/// <summary>
/// PotPack is  a C# conversion of potpack, a tiny JavaScript  library for packing 
/// 2D rectangles into a near-square container (https://github.com/mapbox/potpack)
/// 
/// ISC License
/// 
/// Copyright(c) 2022, Mapbox
/// 
/// Permission to use, copy, modify, and/or  distribute this software for any purpose
/// with or without  fee is hereby granted, provided that  the above copyright notice
/// and this permission notice appear in all copies.
/// 
/// THE  SOFTWARE IS PROVIDED  "AS IS" AND THE  AUTHOR DISCLAIMS ALL  WARRANTIES WITH
/// REGARD TO THIS  SOFTWARE INCLUDING ALL IMPLIED  WARRANTIES OF MERCHANTABILITY AND
/// FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, 
/// OR CONSEQUENTIAL DAMAGES  OR ANY DAMAGES  WHATSOEVER RESULTING FROM  LOSS OF USE, 
/// DATA OR PROFITS, WHETHER IN AN  ACTION OF CONTRACT, NEGLIGENCE  OR OTHER TORTIOUS 
/// ACTION,  ARISING OUT OF  OR IN CONNECTION  WITH THE USE  OR  PERFORMANCE OF  THIS 
/// SOFTWARE.
/// 
/// </summary>
public class PotPack
{
    // Dimensions of the packed rectangles
    private int Width;
    private int Height;

    /// <summary>
    /// Pack the given list of rectangles
    /// </summary>
    /// <param name="boxes"></param>
    public void Pack(List<Rectangle> boxes)
    {
        int area = 0;
        int maxWidth = 0;

        foreach (var box in boxes)
        {
            area += box.Width * box.Height; // Add to area
            maxWidth = Math.Max(maxWidth, box.Width); // Find widest
        }

        boxes.Sort((a, b) => (b.Height.CompareTo(a.Height))); // Sort highest > shortest

        float startWidth = (float)Math.Max(Math.Ceiling(Math.Sqrt(area / 0.95)), maxWidth); // Start with a single empty space, unbounded at the bottom

        int height = 0;
        int width = 0;

        List<Rectangle> spaces = new List<Rectangle>();

        spaces.Add(new Rectangle(0, 0, (int)startWidth, int.MaxValue));

        for (var i = 0; i < boxes.Count; i++)
        {
            var box = boxes[i];

            for (var j = spaces.Count - 1; j >= 0; j--) // Look through spaces backwards so that we check smaller spaces first
            {
                var space = spaces[j];

                // Look for empty spaces that can accommodate the current box
                if (box.Width > space.Width || box.Height > space.Height) continue;

                // Found the space; add the box to its top-left corner
                // |-------|-------|
                // |  box  |       |
                // |_______|       |
                // |         space |
                // |_______________|
                box.X = space.X;
                box.Y = space.Y;

                height = Math.Max(height, box.Y + box.Height);
                width = Math.Max(width, box.X + box.Width);

                if (box.Width == space.Width && box.Height == space.Height)
                {
                    // Space matches the box exactly; remove it
                    var last = spaces[spaces.Count - 1];
                    spaces.RemoveAt(spaces.Count - 1);

                    if (j < spaces.Count) spaces[j] = last;

                }
                else if (box.Height == space.Height)
                {
                    // Space matches the box height; update it accordingly
                    // |-------|---------------|
                    // |  box  | updated space |
                    // |_______|_______________|
                    space.X += box.Width;
                    space.Width -= box.Width;

                }
                else if (box.Width == space.Width)
                {
                    // Space matches the box width; update it accordingly
                    // |---------------|
                    // |      box      |
                    // |_______________|
                    // | updated space |
                    // |_______________|
                    space.Y += box.Height;
                    space.Height -= box.Height;
                }
                else
                {
                    // Otherwise the box splits the space into two spaces
                    // |-------|-----------|
                    // |  box  | new space |
                    // |_______|___________|
                    // | updated space     |
                    // |___________________|
                    spaces.Add(new Rectangle(space.X + box.Width, space.Y, space.Width - box.Width, box.Height));

                    space.Y += box.Height;
                    space.Height -= box.Height;
                }
                break;
            }
        }

        Width = width;
        Height = height;
    }
}