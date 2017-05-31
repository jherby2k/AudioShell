﻿/*
 * Copyright © 2014-2017 Jeremy Herbison
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

using PowerShellAudio.Extensions.ReplayGain.Properties;
using System;
using System.Globalization;

namespace PowerShellAudio.Extensions.ReplayGain
{
    class ReplayGain2SampleAnalyzerInfo : SampleAnalyzerInfo
    {
        public override string Name => "ReplayGain 2.0";

        public override string ExternalLibrary
        {
            get
            {
                try
                {
                    SafeNativeMethods.GetVersion(out int majorVersion, out int minorVersion, out int patchVersion);

                    return string.Format(CultureInfo.CurrentCulture, Resources.SampleAnalyzerDescription, majorVersion,
                        minorVersion, patchVersion);
                }
                catch (TypeInitializationException e)
                {
                    return e.InnerException?.Message ?? e.Message;
                }
            }
        }
    }
}
