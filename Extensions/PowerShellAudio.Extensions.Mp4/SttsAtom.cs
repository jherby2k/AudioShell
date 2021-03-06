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

namespace PowerShellAudio.Extensions.Mp4
{
    class SttsAtom
    {
        internal uint PacketCount { get; private set; }

        internal uint PacketSize { get; private set; }

        internal SttsAtom(byte[] data)
        {
            Contract.Requires(data != null);
            Contract.Requires(data.Length >= 24);

            Array.Reverse(data, 16, 4);
            PacketCount = BitConverter.ToUInt32(data, 16);

            Array.Reverse(data, 20, 4);
            PacketSize = BitConverter.ToUInt32(data, 20);
        }
    }
}
