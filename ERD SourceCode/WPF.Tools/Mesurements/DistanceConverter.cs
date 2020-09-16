namespace WPF.Tools.Mesurements
{
    public static class DistanceConverter
    {
        public static double ConvertCmToPixel(double cm)
        {
            return cm * PixelUnitFactor.Cm;
        }

        public static double ConvertInchToPixel(double inch)
        {
            return inch * PixelUnitFactor.Inch;
        }

        public static double ConvertPtToPixel(double pt)
        {
            return pt * PixelUnitFactor.Pt;
        }

        public static double ConvertPixelToCm(double px)
        {
            return px / PixelUnitFactor.Cm;
        }

        public static double ConvertPixelToInch(double px)
        {
            return px / PixelUnitFactor.Inch;
        }

        public static double ConvertPixelToPt(double px)
        {
            return px / PixelUnitFactor.Pt;
        }

        private struct PixelUnitFactor
        {
            /// <summary>
            ///  Return a pixel constant value
            /// </summary>
            public const double Px = 1.0;

            /// <summary>
            /// Return a Dpi Inch value
            /// </summary>
            public const double Inch = 96.0;

            /// <summary>
            ///     The cm.
            /// </summary>
            public const double Cm = 37.7952755905512;

            /// <summary>
            ///     The pt.
            /// </summary>
            public const double Pt = 1.33333333333333;
        }
    }
}
