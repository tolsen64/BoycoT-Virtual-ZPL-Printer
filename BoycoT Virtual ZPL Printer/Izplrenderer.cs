namespace BoycoT_Virtual_ZPL_Printer
{
    /// <summary>
    /// Common contract for ZPL rendering backends.
    /// </summary>
    public interface IZplRenderer
    {
        /// <summary>Name shown in the UI toggle.</summary>
        string DisplayName { get; }

        /// <summary>
        /// Renders ZPL to a GDI+ Image.
        /// </summary>
        /// <param name="zpl">Raw ZPL string.</param>
        /// <param name="labelWidthInches">Label width (e.g. 4.0).</param>
        /// <param name="labelHeightInches">Label height (e.g. 6.0).</param>
        /// <returns>Rendered Image, or null if rendering produced no output.</returns>
        Image? RenderZpl(string zpl, double labelWidthInches, double labelHeightInches);
    }
}