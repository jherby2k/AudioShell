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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;

namespace PowerShellAudio.Extensions.Vorbis
{
    class VorbisSampleEncoderInfo : SampleEncoderInfo
    {
        public override string Name
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

                return "Ogg Vorbis";
            }
        }

        public override string FileExtension
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

                return ".ogg";
            }
        }

        public override bool IsLossless => false;

        public override string ExternalLibrary
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                try
                {
                    return Marshal.PtrToStringAnsi(SafeNativeMethods.VorbisVersion());
                }
                catch (TypeInitializationException e)
                {
                    return e.InnerException?.Message ?? e.Message;
                }
            }
        }

        public override SettingsDictionary DefaultSettings
        {
            get
            {
                Contract.Ensures(Contract.Result<SettingsDictionary>() != null);

                var result = new SettingsDictionary { { "ControlMode", "Variable" }, { "VBRQuality", "5" } };

                // Call the external ReplayGain filter for scaling the input:
                ExportFactory<ISampleFilter> replayGainFilterFactory =
                    ExtensionProvider.GetFactories<ISampleFilter>("Name", "ReplayGain").SingleOrDefault();
                if (replayGainFilterFactory != null)
                    using (ExportLifetimeContext<ISampleFilter> replayGainFilterLifetime = replayGainFilterFactory.CreateExport())
                        replayGainFilterLifetime.Value.DefaultSettings.CopyTo(result);

                return result;
            }
        }

        public override IReadOnlyCollection<string> AvailableSettings
        {
            get
            {
                Contract.Ensures(Contract.Result<IReadOnlyCollection<string>>() != null);

                var partialResult = new List<string> { "BitRate", "ControlMode", "SerialNumber", "VBRQuality" };

                // Call the external ReplayGain filter for scaling the input:
                ExportFactory<ISampleFilter> replayGainFilterFactory =
                    ExtensionProvider.GetFactories<ISampleFilter>("Name", "ReplayGain").SingleOrDefault();
                if (replayGainFilterFactory != null)
                    using (ExportLifetimeContext<ISampleFilter> replayGainFilterLifetime = replayGainFilterFactory.CreateExport())
                        partialResult = partialResult.Concat(replayGainFilterLifetime.Value.AvailableSettings).ToList();

                return partialResult;
            }
        }
    }
}
