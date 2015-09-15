using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NeovimTerminal
{
    public class RenderProperties
    {
        public Brush Background { get; set; }
        public Brush Foreground { get; set; }
        public Typeface TypeFace { get; set; }
        public bool Enabled { get; set; }

        public RenderProperties()
        {
            this.Enabled = true;
            this.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            this.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            this.TypeFace = new Typeface(new FontFamily("Courier"), FontStyles.Normal, FontWeights.Normal, FontStretches.Condensed);
        }
    }
}
