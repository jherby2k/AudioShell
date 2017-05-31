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
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace PowerShellAudio.Extensions.ReplayGain
{
    abstract class IirFilter
    {
        readonly int _order;
        readonly float[] _a;
        readonly float[] _b;
        float[][] _inputBuffer;
        float[][] _outputBuffer;

        internal IirFilter([NotNull] float[] a, [NotNull] float[] b)
        {
            _order = a.Length - 1;
            _a = a;
            _b = b;
        }

        internal void Process([NotNull] SampleCollection input)
        {
            // Optimization - using SampleCollections here is too expensive:
            if (_inputBuffer == null)
                _inputBuffer = GetBuffer(input.Channels, _order + input.SampleCount);
            if (_outputBuffer == null)
                _outputBuffer = GetBuffer(input.Channels, _order + input.SampleCount);

            // Process each channel in parallel:
            Parallel.For(0, input.Channels, channel =>
            {
                input[channel].CopyTo(_inputBuffer[channel], _order);

                for (int sample = _order; sample < _inputBuffer[channel].Length; sample++)
                {
                    float adjustedSample = 0;
                    for (var i = 0; i < _order; i++)
                        adjustedSample += _inputBuffer[channel][sample - i] * _a[i] -
                                          _outputBuffer[channel][sample - i - 1] * _b[i];
                    adjustedSample += _inputBuffer[channel][sample - _order] * _a[_order];

                    _outputBuffer[channel][sample] = adjustedSample;
                }

                // Save order number of samples from the ends of both buffers:
                Array.Copy(_inputBuffer[channel], input[channel].Length, _inputBuffer[channel], 0, _order);
                Array.Copy(_outputBuffer[channel], input[channel].Length, _outputBuffer[channel], 0, _order);

                // Modify the input directly, rather than returning a new array:
                Array.Copy(_outputBuffer[channel], _order, input[channel], 0, input[channel].Length);
            });
        }

        static float[][] GetBuffer(int channels, int samples)
        {
            var result = new float[channels][];
            for (var channel = 0; channel < channels; channel++)
                result[channel] = new float[samples];

            return result;
        }
    }
}
