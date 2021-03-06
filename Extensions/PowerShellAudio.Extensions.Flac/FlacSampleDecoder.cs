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

using PowerShellAudio.Extensions.Flac.Properties;
using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;

namespace PowerShellAudio.Extensions.Flac
{
    [SampleDecoderExport(".flac")]
    public class FlacSampleDecoder : ISampleDecoder, IDisposable
    {
        NativeStreamSampleDecoder _decoder;

        public void Initialize(Stream stream)
        {
            Contract.Ensures(_decoder != null);

            _decoder = new NativeStreamSampleDecoder(stream);

            DecoderInitStatus initStatus = _decoder.Initialize();
            if (initStatus == DecoderInitStatus.Ok)
                return;

            if (initStatus == DecoderInitStatus.UnsupportedContainer)
                throw new UnsupportedAudioException(Resources.SampleDecoderUnsupportedContainerError);
            throw new IOException(string.Format(CultureInfo.CurrentCulture, Resources.SampleDecoderInitializationError,
                initStatus));
        }

        public SampleCollection DecodeSamples()
        {
            Contract.Ensures(Contract.Result<SampleCollection>() != null);

            while (_decoder.GetState() != DecoderState.EndOfStream)
            {
                if (!_decoder.ProcessSingle())
                    throw new IOException(string.Format(CultureInfo.CurrentCulture, Resources.SampleDecoderDecodingError,
                        _decoder.GetState()));

                if (_decoder.Error.HasValue)
                    throw new IOException(string.Format(CultureInfo.CurrentCulture, Resources.SampleDecoderDecodingError,
                        _decoder.Error.Value));

                if (_decoder.Samples == null)
                    continue;

                SampleCollection result = _decoder.Samples;
                _decoder.Samples = null;
                return result;
            }

            _decoder.Finish();
            return SampleCollectionFactory.Instance.Create(_decoder.AudioInfo.Channels, 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _decoder?.Dispose();
        }
    }
}
