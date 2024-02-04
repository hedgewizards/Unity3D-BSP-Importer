﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scopa.Formats.Id
{
    public static class QuakePalette
    {
        // The Quake palette is in the public domain
        public static readonly byte[] Data =
        {
            0x00, 0x00, 0x00, 0x0F, 0x0F, 0x0F, 0x1F, 0x1F,
            0x1F, 0x2F, 0x2F, 0x2F, 0x3F, 0x3F, 0x3F, 0x4B,
            0x4B, 0x4B, 0x5B, 0x5B, 0x5B, 0x6B, 0x6B, 0x6B,
            0x7B, 0x7B, 0x7B, 0x8B, 0x8B, 0x8B, 0x9B, 0x9B,
            0x9B, 0xAB, 0xAB, 0xAB, 0xBB, 0xBB, 0xBB, 0xCB,
            0xCB, 0xCB, 0xDB, 0xDB, 0xDB, 0xEB, 0xEB, 0xEB,
            0x0F, 0x0B, 0x07, 0x17, 0x0F, 0x0B, 0x1F, 0x17,
            0x0B, 0x27, 0x1B, 0x0F, 0x2F, 0x23, 0x13, 0x37,
            0x2B, 0x17, 0x3F, 0x2F, 0x17, 0x4B, 0x37, 0x1B,
            0x53, 0x3B, 0x1B, 0x5B, 0x43, 0x1F, 0x63, 0x4B,
            0x1F, 0x6B, 0x53, 0x1F, 0x73, 0x57, 0x1F, 0x7B,
            0x5F, 0x23, 0x83, 0x67, 0x23, 0x8F, 0x6F, 0x23,
            0x0B, 0x0B, 0x0F, 0x13, 0x13, 0x1B, 0x1B, 0x1B,
            0x27, 0x27, 0x27, 0x33, 0x2F, 0x2F, 0x3F, 0x37,
            0x37, 0x4B, 0x3F, 0x3F, 0x57, 0x47, 0x47, 0x67,
            0x4F, 0x4F, 0x73, 0x5B, 0x5B, 0x7F, 0x63, 0x63,
            0x8B, 0x6B, 0x6B, 0x97, 0x73, 0x73, 0xA3, 0x7B,
            0x7B, 0xAF, 0x83, 0x83, 0xBB, 0x8B, 0x8B, 0xCB,
            0x00, 0x00, 0x00, 0x07, 0x07, 0x00, 0x0B, 0x0B,
            0x00, 0x13, 0x13, 0x00, 0x1B, 0x1B, 0x00, 0x23,
            0x23, 0x00, 0x2B, 0x2B, 0x07, 0x2F, 0x2F, 0x07,
            0x37, 0x37, 0x07, 0x3F, 0x3F, 0x07, 0x47, 0x47,
            0x07, 0x4B, 0x4B, 0x0B, 0x53, 0x53, 0x0B, 0x5B,
            0x5B, 0x0B, 0x63, 0x63, 0x0B, 0x6B, 0x6B, 0x0F,
            0x07, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x17, 0x00,
            0x00, 0x1F, 0x00, 0x00, 0x27, 0x00, 0x00, 0x2F,
            0x00, 0x00, 0x37, 0x00, 0x00, 0x3F, 0x00, 0x00,
            0x47, 0x00, 0x00, 0x4F, 0x00, 0x00, 0x57, 0x00,
            0x00, 0x5F, 0x00, 0x00, 0x67, 0x00, 0x00, 0x6F,
            0x00, 0x00, 0x77, 0x00, 0x00, 0x7F, 0x00, 0x00,
            0x13, 0x13, 0x00, 0x1B, 0x1B, 0x00, 0x23, 0x23,
            0x00, 0x2F, 0x2B, 0x00, 0x37, 0x2F, 0x00, 0x43,
            0x37, 0x00, 0x4B, 0x3B, 0x07, 0x57, 0x43, 0x07,
            0x5F, 0x47, 0x07, 0x6B, 0x4B, 0x0B, 0x77, 0x53,
            0x0F, 0x83, 0x57, 0x13, 0x8B, 0x5B, 0x13, 0x97,
            0x5F, 0x1B, 0xA3, 0x63, 0x1F, 0xAF, 0x67, 0x23,
            0x23, 0x13, 0x07, 0x2F, 0x17, 0x0B, 0x3B, 0x1F,
            0x0F, 0x4B, 0x23, 0x13, 0x57, 0x2B, 0x17, 0x63,
            0x2F, 0x1F, 0x73, 0x37, 0x23, 0x7F, 0x3B, 0x2B,
            0x8F, 0x43, 0x33, 0x9F, 0x4F, 0x33, 0xAF, 0x63,
            0x2F, 0xBF, 0x77, 0x2F, 0xCF, 0x8F, 0x2B, 0xDF,
            0xAB, 0x27, 0xEF, 0xCB, 0x1F, 0xFF, 0xF3, 0x1B,
            0x0B, 0x07, 0x00, 0x1B, 0x13, 0x00, 0x2B, 0x23,
            0x0F, 0x37, 0x2B, 0x13, 0x47, 0x33, 0x1B, 0x53,
            0x37, 0x23, 0x63, 0x3F, 0x2B, 0x6F, 0x47, 0x33,
            0x7F, 0x53, 0x3F, 0x8B, 0x5F, 0x47, 0x9B, 0x6B,
            0x53, 0xA7, 0x7B, 0x5F, 0xB7, 0x87, 0x6B, 0xC3,
            0x93, 0x7B, 0xD3, 0xA3, 0x8B, 0xE3, 0xB3, 0x97,
            0xAB, 0x8B, 0xA3, 0x9F, 0x7F, 0x97, 0x93, 0x73,
            0x87, 0x8B, 0x67, 0x7B, 0x7F, 0x5B, 0x6F, 0x77,
            0x53, 0x63, 0x6B, 0x4B, 0x57, 0x5F, 0x3F, 0x4B,
            0x57, 0x37, 0x43, 0x4B, 0x2F, 0x37, 0x43, 0x27,
            0x2F, 0x37, 0x1F, 0x23, 0x2B, 0x17, 0x1B, 0x23,
            0x13, 0x13, 0x17, 0x0B, 0x0B, 0x0F, 0x07, 0x07,
            0xBB, 0x73, 0x9F, 0xAF, 0x6B, 0x8F, 0xA3, 0x5F,
            0x83, 0x97, 0x57, 0x77, 0x8B, 0x4F, 0x6B, 0x7F,
            0x4B, 0x5F, 0x73, 0x43, 0x53, 0x6B, 0x3B, 0x4B,
            0x5F, 0x33, 0x3F, 0x53, 0x2B, 0x37, 0x47, 0x23,
            0x2B, 0x3B, 0x1F, 0x23, 0x2F, 0x17, 0x1B, 0x23,
            0x13, 0x13, 0x17, 0x0B, 0x0B, 0x0F, 0x07, 0x07,
            0xDB, 0xC3, 0xBB, 0xCB, 0xB3, 0xA7, 0xBF, 0xA3,
            0x9B, 0xAF, 0x97, 0x8B, 0xA3, 0x87, 0x7B, 0x97,
            0x7B, 0x6F, 0x87, 0x6F, 0x5F, 0x7B, 0x63, 0x53,
            0x6B, 0x57, 0x47, 0x5F, 0x4B, 0x3B, 0x53, 0x3F,
            0x33, 0x43, 0x33, 0x27, 0x37, 0x2B, 0x1F, 0x27,
            0x1F, 0x17, 0x1B, 0x13, 0x0F, 0x0F, 0x0B, 0x07,
            0x6F, 0x83, 0x7B, 0x67, 0x7B, 0x6F, 0x5F, 0x73,
            0x67, 0x57, 0x6B, 0x5F, 0x4F, 0x63, 0x57, 0x47,
            0x5B, 0x4F, 0x3F, 0x53, 0x47, 0x37, 0x4B, 0x3F,
            0x2F, 0x43, 0x37, 0x2B, 0x3B, 0x2F, 0x23, 0x33,
            0x27, 0x1F, 0x2B, 0x1F, 0x17, 0x23, 0x17, 0x0F,
            0x1B, 0x13, 0x0B, 0x13, 0x0B, 0x07, 0x0B, 0x07,
            0xFF, 0xF3, 0x1B, 0xEF, 0xDF, 0x17, 0xDB, 0xCB,
            0x13, 0xCB, 0xB7, 0x0F, 0xBB, 0xA7, 0x0F, 0xAB,
            0x97, 0x0B, 0x9B, 0x83, 0x07, 0x8B, 0x73, 0x07,
            0x7B, 0x63, 0x07, 0x6B, 0x53, 0x00, 0x5B, 0x47,
            0x00, 0x4B, 0x37, 0x00, 0x3B, 0x2B, 0x00, 0x2B,
            0x1F, 0x00, 0x1B, 0x0F, 0x00, 0x0B, 0x07, 0x00,
            0x00, 0x00, 0xFF, 0x0B, 0x0B, 0xEF, 0x13, 0x13,
            0xDF, 0x1B, 0x1B, 0xCF, 0x23, 0x23, 0xBF, 0x2B,
            0x2B, 0xAF, 0x2F, 0x2F, 0x9F, 0x2F, 0x2F, 0x8F,
            0x2F, 0x2F, 0x7F, 0x2F, 0x2F, 0x6F, 0x2F, 0x2F,
            0x5F, 0x2B, 0x2B, 0x4F, 0x23, 0x23, 0x3F, 0x1B,
            0x1B, 0x2F, 0x13, 0x13, 0x1F, 0x0B, 0x0B, 0x0F,
            0x2B, 0x00, 0x00, 0x3B, 0x00, 0x00, 0x4B, 0x07,
            0x00, 0x5F, 0x07, 0x00, 0x6F, 0x0F, 0x00, 0x7F,
            0x17, 0x07, 0x93, 0x1F, 0x07, 0xA3, 0x27, 0x0B,
            0xB7, 0x33, 0x0F, 0xC3, 0x4B, 0x1B, 0xCF, 0x63,
            0x2B, 0xDB, 0x7F, 0x3B, 0xE3, 0x97, 0x4F, 0xE7,
            0xAB, 0x5F, 0xEF, 0xBF, 0x77, 0xF7, 0xD3, 0x8B,
            0xA7, 0x7B, 0x3B, 0xB7, 0x9B, 0x37, 0xC7, 0xC3,
            0x37, 0xE7, 0xE3, 0x57, 0x7F, 0xBF, 0xFF, 0xAB,
            0xE7, 0xFF, 0xD7, 0xFF, 0xFF, 0x67, 0x00, 0x00,
            0x8B, 0x00, 0x00, 0xB3, 0x00, 0x00, 0xD7, 0x00,
            0x00, 0xFF, 0x00, 0x00, 0xFF, 0xF3, 0x93, 0xFF,
            0xF7, 0xC7, 0xFF, 0xFF, 0xFF, 0x9F, 0x5B, 0x53
        };
    }
}
