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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace PowerShellAudio
{
    /// <summary>
    /// A collection of 32-bit floating-point audio samples.
    /// </summary>
    /// <remarks>
    /// Samples should be normalized to between 1.0 and -1.0, otherwise clipping may occur.
    /// This class is a light-weight wrapper around a two-dimensional array. New instances should be created by using
    /// the <see cref="SampleCollectionFactory"/> helper class. To avoid frequent garbage collection cycles when
    /// allocating a large number of <see cref="SampleCollection"/> objects, always call the
    /// <see cref="SampleCollectionFactory"/>'s Free method once the instance is no longer needed, so that the arrays
    /// can be reused.
    /// </remarks>
    public class SampleCollection : IEnumerable<float[]>
    {
        readonly float[][] _samples;

        internal SampleCollection([NotNull] float[][] samples)
        {
            _samples = samples;
        }

        /// <summary>
        /// Gets the array of <see cref="Single"/>s containing samples for the specified channel.
        /// </summary>
        /// <remarks>
        /// This property does not return a copy of the array, but the original. It may be modified in-place.
        /// </remarks>
        /// <value>
        /// The array of <see cref="Single"/>s containing samples for the specified channel.
        /// </value>
        /// <param name="channel">The channel index.</param>
        /// <returns>The array of <see cref="Single"/>s containing samples for the specified channel.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throw if <paramref name="channel"/> is negative.</exception>
        [NotNull, CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "The array is intended to be modifiable, and cannot be wrapped in a collection for performance reasons.")]
        public float[] this[int channel]
        {
            get => _samples[channel];
            internal set => _samples[channel] = value;
        }

        /// <summary>
        /// Gets the number of channels.
        /// </summary>
        /// <value>
        /// The number of channels.
        /// </value>
        [CollectionAccess(CollectionAccessType.Read)]
        public int Channels => _samples.Length;

        /// <summary>
        /// Gets the sample count.
        /// </summary>
        /// <value>
        /// The sample count.
        /// </value>
        [CollectionAccess(CollectionAccessType.Read)]
        public int SampleCount => _samples[0].Length;

        /// <summary>
        /// Gets a value indicating whether this is the last <see cref="SampleCollection"/> in the stream. This is
        /// equivalent to checking if SampleCount equals 0.
        /// </summary>
        /// <value>True if this instance is last; otherwise, false.</value>
        [CollectionAccess(CollectionAccessType.Read)]
        public bool IsLast => _samples[0].Length == 0;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        /// </returns>
        [CollectionAccess(CollectionAccessType.Read)]
        public IEnumerator<float[]> GetEnumerator()
        {
            return ((IEnumerable<float[]>)_samples).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        [CollectionAccess(CollectionAccessType.Read)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _samples.GetEnumerator();
        }
    }
}
