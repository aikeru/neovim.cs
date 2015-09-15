using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NeovimTerminal
{
    public class Cell : FrameworkElement
    {
        private string _text;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                if (RenderProps.Enabled == true)
                {
                    Background = RenderProps.Background;
                    Foreground = RenderProps.Foreground;
                    TypeFace = RenderProps.TypeFace;
                }
                InvalidateVisual();
            }
        }

        public RenderProperties RenderProps { get; set; }
        public Brush Background { get; set; }
        public Brush Foreground { get; set; }
        public Typeface TypeFace { get; set; }
        public double FontSize { get; set; }

        public Cell(RenderProperties renderProperties)
        {
            this.RenderProps = renderProperties;
            this._text = " ";
            this.TypeFace = new Typeface(new FontFamily("Courier"), FontStyles.Normal, FontWeights.Normal, FontStretches.Condensed);
            this.FontSize = 12;
            this.Width = this.FontSize;
            this.Height = Math.Ceiling(this.FontSize * this.TypeFace.FontFamily.LineSpacing);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(Background, null, new Rect(0, 0, this.Width, this.Height));
            var formattedText = new FormattedText(Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, TypeFace, FontSize, Foreground);
            drawingContext.DrawText(formattedText, new Point(0, 0));
        }
    }
}
