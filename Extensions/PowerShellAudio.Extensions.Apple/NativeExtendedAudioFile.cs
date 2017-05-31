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

using PowerShellAudio.Extensions.Apple.Properties;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace PowerShellAudio.Extensions.Apple
{
    class NativeExtendedAudioFile : NativeAudioFile
    {
        readonly NativeExtendedAudioFileHandle _handle;

        internal NativeExtendedAudioFile(
            AudioStreamBasicDescription description, 
            AudioFileType fileType,
            [NotNull] Stream stream)
            : base(description, fileType, stream)
        {
            ExtendedAudioFileStatus status = SafeNativeMethods.ExtAudioFileWrapAudioFile(Handle, true, out _handle);
            if (status != ExtendedAudioFileStatus.Ok)
                throw new IOException(string.Format(CultureInfo.CurrentCulture,
                    Resources.NativeExtendedAudioFileInitializationError, status));
        }

        internal T GetProperty<T>(ExtendedAudioFilePropertyId id) where T : struct
        {
            var sizeOfResult = (uint)Marshal.SizeOf(typeof(T));
            IntPtr unmanagedValue = Marshal.AllocHGlobal((int)sizeOfResult);
            try
            {
                SafeNativeMethods.ExtAudioFileGetProperty(_handle, id, ref sizeOfResult, unmanagedValue);
                return Marshal.PtrToStructure<T>(unmanagedValue);
            }
            finally
            {
                Marshal.FreeHGlobal(unmanagedValue);
            }
        }

        internal ExtendedAudioFileStatus SetProperty<T>(ExtendedAudioFilePropertyId id, T value) where T : struct
        {
            IntPtr unmanagedValue = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
            try
            {
                Marshal.StructureToPtr(value, unmanagedValue, false);
                return SafeNativeMethods.ExtAudioFileSetProperty(_handle, id, (uint)Marshal.SizeOf(typeof(T)),
                    unmanagedValue);
            }
            finally
            {
                Marshal.FreeHGlobal(unmanagedValue);
            }
        }

        internal ExtendedAudioFileStatus Write(AudioBufferList data, uint frames)
        {
            return SafeNativeMethods.ExtAudioFileWrite(_handle, frames, ref data);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _handle.Dispose();

            base.Dispose(disposing);
        }
    }
}
