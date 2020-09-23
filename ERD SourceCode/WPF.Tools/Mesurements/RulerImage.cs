using IconSet;
using System;
using System.Windows.Controls;

namespace WPF.Tools.Mesurements
{
    public class RulerImage : Image
    {
        private System.Drawing.Bitmap bmp;

        public RulerImage()
        {
            this.Initialize();
        }

        public RulerOrientationEnum Orientation
        {
            get;

            set;
        }

        public void Refresh(double width, double height)
        {
            this.Width = width;

            this.Height = height;

            if (this.Orientation == RulerOrientationEnum.Horizontal)
            {
                this.DrawHorizontalRuler();
            }
            else
            {
                this.DrawVerticalRuler();
            }

            this.Source = ImageConverters.ConvertToImageSource(this.bmp);

            this.bmp = null;
        }

        private void Initialize()
        {
            this.Orientation = RulerOrientationEnum.Horizontal;
        }

        private void DrawHorizontalRuler()
        {
            if (this.bmp == null)
            {
                int width = this.ActualWidth < 0.1 ? 3 : (int)this.Width;

                int height = this.ActualHeight < 0.1 ? 3 : (int)this.Height;

                if (width <= 0)
                {
                    width = 1;
                }

                if (height <= 0)
                {
                    height = 1;
                }

                this.bmp = new System.Drawing.Bitmap(width, height);

                System.Drawing.Graphics grap = System.Drawing.Graphics.FromImage(bmp);

                try
                {
                    double cmVal = DistanceConverter.ConvertPixelToCm(this.Width);

                    double iSmall = Math.Abs(DistanceConverter.ConvertCmToPixel(0.1));

                    grap.FillRectangle(System.Drawing.Brushes.WhiteSmoke, 0, 0, this.bmp.Width, bmp.Height);

                    for (int x = 0; x < cmVal; x++)
                    {
                        int iX = Convert.ToInt32(Math.Abs(DistanceConverter.ConvertCmToPixel(x)));

                        double iStart = Math.Abs(DistanceConverter.ConvertCmToPixel(x)) + iSmall;

                        double iEnd = Math.Abs(DistanceConverter.ConvertCmToPixel(x + 1));

                        int midCount = 1;

                        this.DrawRulerLine(grap, iX, iX, 15, 0);

                        if (x > 0)
                        {
                            this.DrawRulerNumber(grap, x, iX);
                        }

                        while (Convert.ToInt32(iStart) < Convert.ToInt32(iEnd))
                        {
                            int iS = Convert.ToInt32(iStart);

                            this.DrawRulerLine(grap, iS, iS, midCount == 5 ? 10 : 6, 0);

                            iStart += iSmall;

                            midCount++;
                        }

                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    grap.Dispose();
                }
            }
        }

        private void DrawVerticalRuler()
        {
            if (this.bmp == null)
            {
                int width = this.ActualWidth < 0.1 ? 3 : (int)this.Width;

                int height = this.ActualHeight < 0.1 ? 3 : (int)this.Height;

                if (height <= 0)
                {
                    height = 1;
                }

                this.bmp = new System.Drawing.Bitmap(width, height);

                System.Drawing.Graphics grap = System.Drawing.Graphics.FromImage(bmp);

                try
                {
                    double cmVal = DistanceConverter.ConvertPixelToCm(this.Height);

                    double iSmall = Math.Abs(DistanceConverter.ConvertCmToPixel(0.1));

                    grap.FillRectangle(System.Drawing.Brushes.WhiteSmoke, 0, 0, this.bmp.Width, bmp.Height);

                    for (int x = 0; x < cmVal; x++)
                    {
                        int iX = Convert.ToInt32(Math.Abs(DistanceConverter.ConvertCmToPixel(x)));

                        double iStart = Math.Abs(DistanceConverter.ConvertCmToPixel(x)) + iSmall;

                        double iEnd = Math.Abs(DistanceConverter.ConvertCmToPixel(x + 1));

                        int midCount = 1;

                        this.DrawRulerLine(grap, 0, 15, iX, iX);

                        if (x > 0)
                        {
                            this.DrawRulerNumber(grap, x, iX);
                        }

                        while (Convert.ToInt32(iStart) < Convert.ToInt32(iEnd))
                        {
                            int iS = Convert.ToInt32(iStart);

                            this.DrawRulerLine(grap, 0, midCount == 5 ? 10 : 6, iS, iS);

                            iStart += iSmall;

                            midCount++;
                        }

                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    grap.Dispose();
                }
            }
        }

        private void DrawRulerNumber(System.Drawing.Graphics grap, int value, int posistion)
        {
            System.Drawing.StringFormat format = new System.Drawing.StringFormat(System.Drawing.StringFormatFlags.MeasureTrailingSpaces);

            System.Drawing.Font font = null;

            if (this.Orientation == RulerOrientationEnum.Vertical)
            {
                format.FormatFlags |= System.Drawing.StringFormatFlags.DirectionVertical;

                font = new System.Drawing.Font("Segoe UI", (float)(this.Width / 3));
            }
            else
            {
                font = new System.Drawing.Font("Segoe UI", (float)(this.Height / 3));
            }

            System.Drawing.SizeF size = grap.MeasureString((value).ToString(), font, 30, format);

            int x = 0;

            int y = 0;

            if (this.Orientation == RulerOrientationEnum.Horizontal)
            {
                x = posistion - (int)size.Width - 3;

                y = (int)this.Height - (int)size.Height;
            }
            else
            {
                x = (int)this.Width - (int)size.Width;

                y = posistion + 5 - (int)size.Height / 2;
            }

            System.Drawing.Point drawAt = new System.Drawing.Point(x, y);

            grap.DrawString(value.ToString(), font, System.Drawing.Brushes.DarkGray, drawAt, format);
        }

        private void DrawRulerLine(System.Drawing.Graphics grap, int x1, int x2, int y1, int y2)
        {
            using (System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkGray))
            {
                using (System.Drawing.Pen pen = new System.Drawing.Pen(brush, (float)0.3))
                {
                    grap.DrawLine(pen, x1, y1, x2, y2);

                    pen.Dispose();

                    brush.Dispose();
                }
            }
        }
    }
}
