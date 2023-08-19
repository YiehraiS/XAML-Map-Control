﻿// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// Copyright © 2023 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace MapControl
{
    public partial class GeoImage
    {
        private static async Task<GeoBitmap> ReadGeoTiffAsync(string sourcePath)
        {
            var file = await StorageFile.GetFileFromPathAsync(FilePath.GetFullPath(sourcePath));

            using (var stream = await file.OpenReadAsync())
            {
                Matrix transform;
                MapProjection projection = null;

                var decoder = await BitmapDecoder.CreateAsync(stream);
                var bitmap = await ImageLoader.LoadImageAsync(decoder);

                var geoKeyDirectoryQuery = QueryString(GeoKeyDirectoryTag);
                var pixelScaleQuery = QueryString(ModelPixelScaleTag);
                var tiePointQuery = QueryString(ModelTiePointTag);
                var transformationQuery = QueryString(ModelTransformationTag);
                var metadata = await decoder.BitmapProperties.GetPropertiesAsync(
                    new string[]
                    {
                        pixelScaleQuery,
                        tiePointQuery,
                        transformationQuery,
                        geoKeyDirectoryQuery
                    });

                if (metadata.TryGetValue(pixelScaleQuery, out BitmapTypedValue pixelScaleValue) &&
                    pixelScaleValue.Value is double[] pixelScale &&
                    pixelScale.Length == 3 &&
                    metadata.TryGetValue(tiePointQuery, out BitmapTypedValue tiePointValue) &&
                    tiePointValue.Value is double[] tiePoint &&
                    tiePoint.Length >= 6)
                {
                    transform = new Matrix(pixelScale[0], 0d, 0d, -pixelScale[1], tiePoint[3], tiePoint[4]);
                }
                else if (metadata.TryGetValue(transformationQuery, out BitmapTypedValue transformValue) &&
                         transformValue.Value is double[] transformValues &&
                         transformValues.Length == 16)
                {
                    transform = new Matrix(transformValues[0], transformValues[1],
                                           transformValues[4], transformValues[5],
                                           transformValues[3], transformValues[7]);
                }
                else
                {
                    throw new ArgumentException($"No coordinate transformation found in {sourcePath}.");
                }

                if (metadata.TryGetValue(geoKeyDirectoryQuery, out BitmapTypedValue geoKeyDirValue) &&
                    geoKeyDirValue.Value is short[] geoKeyDirectory)
                {
                    projection = GetProjection(sourcePath, geoKeyDirectory);
                }

                return new GeoBitmap(bitmap, transform, projection);
            }
        }
    }
}
