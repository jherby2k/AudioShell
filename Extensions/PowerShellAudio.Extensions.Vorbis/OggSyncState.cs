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

using System;

#pragma warning disable 169, 649

namespace PowerShellAudio.Extensions.Vorbis
{
    struct OggSyncState
    {
          internal IntPtr Data;

          internal int Storage;

          internal int Fill;

          internal int Returned;

          internal int Unsynced;

          internal int HeaderBytes;

          internal int BodyBytes;
    }
}

#pragma warning restore 169, 649