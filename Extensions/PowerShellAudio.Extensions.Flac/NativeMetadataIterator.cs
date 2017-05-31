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
using JetBrains.Annotations;

namespace PowerShellAudio.Extensions.Flac
{
    class NativeMetadataIterator : IDisposable
    {
        readonly NativeMetadataIteratorHandle _handle = SafeNativeMethods.MetadataIteratorNew();

        internal NativeMetadataIterator([NotNull] NativeMetadataChainHandle chainHandle)
        {
            SafeNativeMethods.MetadataIteratorInitialize(_handle, chainHandle);
        }

        internal IntPtr GetBlock()
        {
            return SafeNativeMethods.MetadataIteratorGetBlock(_handle);
        }

        internal bool DeleteBlock(bool replaceWithPadding)
        {
            return SafeNativeMethods.MetadataIteratorDeleteBlock(_handle, replaceWithPadding);
        }

        internal bool Next()
        {
            return SafeNativeMethods.MetadataIteratorNext(_handle);
        }

        internal bool InsertBlockAfter([NotNull] NativeMetadataBlockHandle metadataBlock)
        {
            return SafeNativeMethods.MetadataIteratorInsertBlockAfter(_handle, metadataBlock);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _handle.Dispose();
        }
    }
}
