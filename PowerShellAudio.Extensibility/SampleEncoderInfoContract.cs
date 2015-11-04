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

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace PowerShellAudio
{
    [ContractClassFor(typeof(SampleEncoderInfo))]
    abstract class SampleEncoderInfoContract : SampleEncoderInfo
    {
        public override string Name
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

                return default(string);
            }
        }

        public override string FileExtension
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
                Contract.Ensures(Contract.Result<string>().StartsWith(".", StringComparison.OrdinalIgnoreCase));
                Contract.Ensures(!Contract.Result<string>().Any(char.IsWhiteSpace));
                Contract.Ensures(!Contract.Result<string>().Any(character => Path.GetInvalidFileNameChars().Contains(character)));
                Contract.Ensures(!Contract.Result<string>().Any(char.IsUpper));

                return default(string);
            }
        }

        public override bool IsLossless => default(bool);
    }
}
