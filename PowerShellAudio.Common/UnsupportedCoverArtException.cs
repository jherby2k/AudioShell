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
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace PowerShellAudio
{
    /// <summary>
    /// An exception that indicates a CoverArt object could not be created, because the data provided is in an
    /// unsupported format.
    /// </summary>
    /// <remarks>
    /// These exceptions should normally be caught and either ignored (skipping the cover art entirely) or the calling
    /// extension should convert the image to a support format to avoid the exception.
    /// </remarks>
    [Serializable]
    public class UnsupportedCoverArtException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedCoverArtException" /> class.
        /// </summary>
        public UnsupportedCoverArtException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedCoverArtException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnsupportedCoverArtException([CanBeNull] string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedCoverArtException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if
        /// no inner exception is specified.
        /// </param>
        public UnsupportedCoverArtException([CanBeNull] string message, [CanBeNull] Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedCoverArtException" /> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being
        /// thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext" /> that contains contextual information about the source or destination.
        /// </param>
        protected UnsupportedCoverArtException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
