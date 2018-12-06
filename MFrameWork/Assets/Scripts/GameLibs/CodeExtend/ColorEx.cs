using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MFrameWork
{
    public class ColorEx
    {
        public static readonly Color EntityNormalColor = SetHex(0x808080FFu);  //RGBA          

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns>0xRGBA</returns>
        public static uint GetHex(Color color)
        {
            return (uint)(color.r * 255) << 24 |
                    (uint)(color.g * 255) << 16 |
                    (uint)(color.b * 255) << 8 |
                    (uint)(color.a * 255);
        }

        public static Color SetHex(uint rgba)
        {
            return new Color(
                (0xFF & (rgba >> 24)) / 255f,
                (0xFF & (rgba >> 16)) / 255f,
                (0xFF & (rgba >> 8)) / 255f,
                (0xFF & (rgba)) / 255f
                );
        }
    }
}
