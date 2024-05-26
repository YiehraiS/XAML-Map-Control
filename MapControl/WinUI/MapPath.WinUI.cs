﻿// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// Copyright © 2024 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

#if UWP
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
#endif

namespace MapControl
{
    public partial class MapPath : Path
    {
        public MapPath()
        {
            Stretch = Stretch.None;
            MapPanel.InitMapElement(this);
        }

        private void SetMapTransform(Matrix matrix)
        {
            if (Data.Transform is MatrixTransform transform)
            {
                transform.Matrix = matrix;
            }
            else
            {
                Data.Transform = new MatrixTransform { Matrix = matrix };
            }
        }
    }
}
