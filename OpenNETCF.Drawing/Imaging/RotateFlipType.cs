using System.Drawing;

namespace OpenNETCF.Drawing
{     
    /// <summary>
    /// Specifies the direction of an image's rotation and the axis used to flip the image.
    /// </summary>
    public enum RotateFlipType
    {
        /// <summary>
        /// Specifies a 180-degree rotation followed by a horizontal and vertical flip.
        /// </summary>
        Rotate180FlipXY = 0,    
        /// <summary>
        /// Specifies no rotation and no flipping.
        /// </summary>
        RotateNoneFlipNone = 0,    
        /// <summary>
        /// Specifies a 270-degree rotation followed by a horizontal and vertical flip.
        /// </summary>
        Rotate270FlipXY = 1,  
        /// <summary>
        /// Specifies a 90-degree rotation without flipping.
        /// </summary>
        Rotate90FlipNone = 1,   
        /// <summary>
        /// Specifies a 180-degree rotation without flipping.
        /// </summary>
        Rotate180FlipNone = 2, 
        /// <summary>
        /// Specifies no rotation followed by a horizontal and vertical flip.
        /// </summary>
        RotateNoneFlipXY = 2,    
        /// <summary>
        /// Specifies a 270-degree rotation without flipping.
        /// </summary>
        Rotate270FlipNone = 3,
        /// <summary>
        /// Specifies a 90-degree rotation followed by a horizontal and vertical flip.
        /// </summary>
        Rotate90FlipXY = 3,   
        /// <summary>
        /// Specifies a 180-degree rotation followed by a vertical flip.
        /// </summary>
        Rotate180FlipY = 4,
        /// <summary>
        /// Specifies no rotation followed by a horizontal flip.
        /// </summary>
        RotateNoneFlipX = 4,   
        /// <summary>
        /// Specifies a 90-degree rotation followed by a horizontal flip.
        /// </summary>
        Rotate90FlipX = 5,    
        /// <summary>
        /// Specifies a 270-degree rotation followed by a vertical flip.
        /// </summary>
        Rotate270FlipY = 5,    
        /// <summary>
        /// Specifies no rotation followed by a vertical flip.
        /// </summary>
        RotateNoneFlipY = 6,  
        /// <summary>
        /// Specifies a 180-degree rotation followed by a horizontal flip.
        /// </summary>
        Rotate180FlipX = 6,    
        /// <summary>
        /// Specifies a 90-degree rotation followed by a vertical flip.
        /// </summary>
        Rotate90FlipY = 7,    
        /// <summary>
        /// Specifies a 270-degree rotation followed by a horizontal flip.
        /// </summary>
        Rotate270FlipX = 7,
    }
}