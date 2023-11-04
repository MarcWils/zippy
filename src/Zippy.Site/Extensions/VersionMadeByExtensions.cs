﻿namespace Zippy.Site.Extensions
{
    public static class VersionMadeByExtensions
    {
        public static string ToVersionMadeBy(this ushort versionMadeBy)
        {
            return versionMadeBy switch
            {
                0 => "MS-DOS and OS/2 (FAT/VFAT/FAT32 file systems)",
                1 => "Amiga",
                2 => "OpenVMS",
                3 => "UNIX",
                4 => "VM/CMS",
                5 => "Atari ST",
                6 => "OS/2 H.P.F.S.",
                7 => "Macintosh",
                8 => "Z-System",
                9 => "CP/M",
                10 => "Windows NTFS",
                11 => "MVS(OS/390-Z/OS)",
                12 => "VSE",
                13 => "Acorn Risc",
                14 => "VFAT",
                15 => "alternate MVS",
                16 => "BeOS",
                17 => "Tandem",
                18 => "OS/400",
                19 => "OS X(Darwin)",
                _ => string.Empty
            };
        }
    }
}
