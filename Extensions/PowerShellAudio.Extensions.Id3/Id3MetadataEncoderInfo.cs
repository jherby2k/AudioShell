﻿/*
 * Copyright © 2014, 2015 Jeremy Herbison
 * 
 * This file is part of PowerShell Audio.
 * 
 * PowerShell Audio is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser
 * General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 * 
 * PowerShell Audio is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the
 * implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License
 * for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License along with PowerShell Audio.  If not, see
 * <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace PowerShellAudio.Extensions.Id3
{
    class Id3MetadataEncoderInfo : MetadataEncoderInfo
    {
        public override string Format
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

                return "ID3v2";
            }
        }

        public override string FileExtension
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

                return ".mp3";
            }
        }

        public override SettingsDictionary DefaultSettings
        {
            get
            {
                Contract.Ensures(Contract.Result<SettingsDictionary>() != null);

                return new SettingsDictionary
                {
                    { "AddSoundCheck", bool.FalseString },
                    { "ID3Version", "2.3" },
                    { "PaddingSize", "0" },
                    { "UsePadding", bool.FalseString }
                };
            }
        }

        public override IReadOnlyCollection<string> AvailableSettings
        {
            get
            {
                Contract.Ensures(Contract.Result<IReadOnlyCollection<string>>() != null);

                return new List<string> { "AddSoundCheck", "ID3Version", "PaddingSize", "UsePadding" };
            }
        }
    }
}
