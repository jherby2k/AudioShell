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

namespace PowerShellAudio
{
    [ContractClassFor(typeof(IMetadataEncoder))]
    abstract class MetadataEncoderContract : IMetadataEncoder
    {
        public MetadataEncoderInfo EncoderInfo
        {
            get
            {
                Contract.Ensures(Contract.Result<MetadataEncoderInfo>() != null);

                return default(MetadataEncoderInfo);
            }
        }

        public void WriteMetadata(Stream stream, MetadataDictionary metadata, SettingsDictionary settings)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentException>(stream.CanWrite);
            Contract.Requires<ArgumentException>(stream.CanSeek);
            Contract.Requires<ArgumentException>(stream.Position == 0);
            Contract.Requires<ArgumentNullException>(metadata != null);
            Contract.Requires<ArgumentNullException>(settings != null);
            Contract.Ensures(stream.CanWrite);
            Contract.Ensures(stream.CanSeek);
            Contract.Ensures(stream.Length >= Contract.OldValue<Stream>(stream).Length);
        }
    }
}
