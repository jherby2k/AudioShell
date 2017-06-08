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

using PowerShellAudio.Extensions.Mp3.Properties;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace PowerShellAudio.Extensions.Mp3
{
    [AudioInfoDecoderExport(".mp3")]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Loaded via reflection")]
    class Mp3AudioInfoDecoder : IAudioInfoDecoder
    {
        [NotNull]
        public AudioInfo ReadAudioInfo([NotNull] Stream stream)
        {
            using (var reader = new FrameReader(stream))
            {
                // Seek to the first valid frame header:
                FrameHeader frameHeader;
                do
                {
                    reader.SeekToNextFrame();
                    frameHeader = new FrameHeader(reader.ReadBytes(4));
                } while (!reader.VerifyFrameSync(frameHeader));

                if (frameHeader.Layer != "III")
                    throw new UnsupportedAudioException(string.Format(CultureInfo.InvariantCulture,
                        Resources.AudioInfoDecoderLayerError, frameHeader.Layer));

                // Seek past the CRC, if present:
                if (frameHeader.HasCrc)
                    reader.BaseStream.Seek(2, SeekOrigin.Current);

                // Seek past the side information:
                reader.BaseStream.Seek(GetSideInfoLength(frameHeader.ChannelMode, frameHeader.MpegVersion),
                    SeekOrigin.Current);

                // Read the XING header (if present):
                XingHeader xingHeader = reader.ReadXingHeader();

                // If the byte count isn't present in the Xing header, use the file length:
                if (xingHeader.ByteCount == 0)
                    xingHeader.ByteCount = (uint)reader.BaseStream.Length;

                // Calculate the (approximate) sample count:
                uint sampleCount = frameHeader.MpegVersion == "1"
                    ? xingHeader.FrameCount * 1152
                    : xingHeader.FrameCount * 576;

                // Calculate the bitrate from the VBR header, or use the information from the first frame:
                int bitRate = sampleCount > 0
                    ? CalculateBitRate(xingHeader.ByteCount, sampleCount, frameHeader.SampleRate)
                    : frameHeader.BitRate;

                var format = new StringBuilder(bitRate.ToString(CultureInfo.CurrentCulture));
                format.Append("kbps MPEG ");
                format.Append(frameHeader.MpegVersion);
                format.Append(" Layer ");
                format.Append(frameHeader.Layer);

                return new AudioInfo(format.ToString(), frameHeader.ChannelMode == "Mono" ? 1 : 2, 0,
                    frameHeader.SampleRate, sampleCount);
            }
        }

        static int GetSideInfoLength([NotNull] string channelMode, [NotNull] string mpegVersion)
        {
            if (channelMode == "Mono")
                return mpegVersion == "1" ? 17 : 9;
            return mpegVersion == "1" ? 32 : 17;
        }

        static int CalculateBitRate(uint byteCount, uint sampleCount, int sampleRate)
        {
            return (int)Math.Round(byteCount * 8 / (sampleCount / (double)sampleRate) / 1000);
        }
    }
}
