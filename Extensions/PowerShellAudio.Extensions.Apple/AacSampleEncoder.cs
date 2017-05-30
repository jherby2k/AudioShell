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

using PowerShellAudio.Extensions.Apple.Properties;
using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace PowerShellAudio.Extensions.Apple
{
    [SampleEncoderExport("Apple AAC")]
    public class AacSampleEncoder : ISampleEncoder, IDisposable
    {
        static readonly SampleEncoderInfo _encoderInfo = new AacSampleEncoderInfo();
        static readonly uint[] _vbrQualities = { 0, 5, 14, 23, 32, 41, 50, 59, 69, 78, 87, 96, 105, 114, 123 };

        Stream _stream;
        ExportLifetimeContext<ISampleFilter> _replayGainFilterLifetime;
        MetadataDictionary _metadata;
        SettingsDictionary _settings;
        NativeExtendedAudioFile _audioFile;
        int[] _buffer;

        [NotNull]
        public SampleEncoderInfo EncoderInfo => _encoderInfo;

        public void Initialize(
            [NotNull] Stream stream, 
            [NotNull] AudioInfo audioInfo, 
            [NotNull] MetadataDictionary metadata,
            [NotNull] SettingsDictionary settings)
        {
            _stream = stream;
            _metadata = metadata;
            _settings = settings;

            // Load the external gain filter:
            ExportFactory<ISampleFilter> sampleFilterFactory =
                ExtensionProvider.GetFactories<ISampleFilter>("Name", "ReplayGain").SingleOrDefault();
            _replayGainFilterLifetime = sampleFilterFactory?.CreateExport()
                ?? throw new ExtensionInitializationException(Resources.AacSampleEncoderReplayGainFilterError);
            _replayGainFilterLifetime.Value.Initialize(metadata, settings);

            AudioStreamBasicDescription inputDescription = GetInputDescription(audioInfo);
            AudioStreamBasicDescription outputDescription = GetOutputDescription(inputDescription);

            try
            {
                _audioFile = new NativeExtendedAudioFile(outputDescription, AudioFileType.M4A, stream);

                ExtendedAudioFileStatus status = _audioFile.SetProperty(ExtendedAudioFilePropertyId.ClientDataFormat,
                    inputDescription);
                if (status != ExtendedAudioFileStatus.Ok)
                    throw new IOException(string.Format(CultureInfo.CurrentCulture,
                        Resources.SampleEncoderInitializationError, status));

                // Configure the audio converter:
                ConfigureConverter(settings, audioInfo.Channels,
                    _audioFile.GetProperty<IntPtr>(ExtendedAudioFilePropertyId.AudioConverter));

                // Setting the ConverterConfig property to null resynchronizes the converter settings:
                ExtendedAudioFileStatus fileStatus = _audioFile.SetProperty(
                    ExtendedAudioFilePropertyId.ConverterConfig, IntPtr.Zero);
                if (fileStatus != ExtendedAudioFileStatus.Ok)
                    throw new IOException(string.Format(CultureInfo.CurrentCulture,
                        Resources.SampleEncoderConverterError, status));
            }
            catch (TypeInitializationException e)
            {
                if (e.InnerException != null && e.InnerException.GetType() == typeof(ExtensionInitializationException))
                    throw e.InnerException;
                throw;
            }
        }

        public bool ManuallyFreesSamples => false;

        public void Submit([NotNull] SampleCollection samples)
        {
            if (_buffer == null)
                _buffer = new int[samples.SampleCount * samples.Channels];

            if (!samples.IsLast)
            {
                // Filter by ReplayGain, depending on settings:
                _replayGainFilterLifetime.Value.Submit(samples);

                var index = 0;
                for (var sample = 0; sample < samples.SampleCount; sample++)
                    for (var channel = 0; channel < samples.Channels; channel++)
                        _buffer[index++] = (int)Math.Round(samples[channel][sample] * 0x7fffffff);

                GCHandle handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);

                try
                {
                    var bufferList = new AudioBufferList
                    {
                        NumberBuffers = 1,
                        Buffers = new AudioBuffer[1]
                    };
                    bufferList.Buffers[0].NumberChannels = (uint)samples.Channels;
                    bufferList.Buffers[0].DataByteSize = (uint)(index * Marshal.SizeOf<int>());
                    bufferList.Buffers[0].Data = handle.AddrOfPinnedObject();

                    ExtendedAudioFileStatus status = _audioFile.Write(bufferList, (uint)samples.SampleCount);
                    if (status != ExtendedAudioFileStatus.Ok)
                        throw new IOException(string.Format(CultureInfo.CurrentCulture,
                            Resources.SampleEncoderWriteError, status));
                }
                finally
                {
                    handle.Free();
                }
            }
            else
            {
                _audioFile.Dispose();

                // Call an external MP4 encoder for writing iTunes-compatible atoms:
                _stream.Position = 0;

                ExportFactory<IMetadataEncoder> metadataEncoderFactory =
                    ExtensionProvider.GetFactories<IMetadataEncoder>("Extension", EncoderInfo.FileExtension)
                        .SingleOrDefault();
                if (metadataEncoderFactory == null)
                    throw new ExtensionInitializationException(string.Format(CultureInfo.CurrentCulture,
                        Resources.SampleEncoderMetadataEncoderError, EncoderInfo.FileExtension));

                using (ExportLifetimeContext<IMetadataEncoder> metadataEncoderLifetime = metadataEncoderFactory.CreateExport())
                    metadataEncoderLifetime.Value.WriteMetadata(_stream, _metadata, _settings);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _replayGainFilterLifetime?.Dispose();
            _audioFile?.Dispose();
        }

        static AudioStreamBasicDescription GetInputDescription([NotNull] AudioInfo audioInfo)
        {
            return new AudioStreamBasicDescription
            {
                SampleRate = audioInfo.SampleRate,
                AudioFormat = AudioFormat.LinearPcm,
                Flags = AudioFormatFlags.PcmIsSignedInteger | AudioFormatFlags.PcmIsPacked,
                BytesPerPacket = 4 * (uint)audioInfo.Channels,
                FramesPerPacket = 1,
                BytesPerFrame = 4 * (uint)audioInfo.Channels,
                ChannelsPerFrame = (uint)audioInfo.Channels,
                BitsPerChannel = 32
            };
        }

        static AudioStreamBasicDescription GetOutputDescription(AudioStreamBasicDescription inputDescription)
        {
            var result = new AudioStreamBasicDescription
            {
                FramesPerPacket = 1024,
                AudioFormat = AudioFormat.AacLowComplexity,
                ChannelsPerFrame = inputDescription.ChannelsPerFrame
            };

            // Some sample rates aren't supported on output, so a best match should be made:
            switch ((int)inputDescription.SampleRate)
            {
                case 192000:
                case 144000:
                case 128000: // conversion required
                case 96000:
                case 64000: // conversion required
                case 48000:
                    result.SampleRate = 48000;
                    break;

                case 176400:
                case 88200:
                case 44100:
                case 37800: // conversion required
                case 36000: // conversion required
                    result.SampleRate = 44100;
                    break;

                case 32000:
                case 28000: // conversion required
                    result.SampleRate = 32000;
                    break;

                case 22050:
                case 18900: // conversion required
                    result.SampleRate = 22050;
                    break;
            }

            return result;
        }

        static void ConfigureConverter([NotNull] SettingsDictionary settings, int channels, IntPtr converter)
        {
            // Set the quality if specified, otherwise select "High":
            Quality quality;
            if (string.IsNullOrEmpty(settings["Quality"]) ||
                string.Compare(settings["Quality"], "High", StringComparison.OrdinalIgnoreCase) == 0)
                quality = Quality.High;
            else if (string.Compare(settings["Quality"], "Medium", StringComparison.OrdinalIgnoreCase) == 0)
                quality = Quality.Medium;
            else if (string.Compare(settings["Quality"], "Low", StringComparison.OrdinalIgnoreCase) == 0)
                quality = Quality.Low;
            else
                throw new InvalidSettingException(string.Format(CultureInfo.CurrentCulture,
                    Resources.AacSampleEncoderBadQuality, settings["Quality"]));

            AudioConverterStatus status = SetConverterProperty(converter, AudioConverterPropertyId.CodecQuality, (uint)quality);
            if (status != AudioConverterStatus.Ok)
                throw new IOException(string.Format(CultureInfo.CurrentCulture,
                    Resources.SampleEncoderConverterQualityError, status));

            // Set a bitrate only if specified. Otherwise, default to a variable bitrate:
            if (!string.IsNullOrEmpty(settings["BitRate"]))
                ConfigureConverterForBitRate(settings, channels, converter);
            else
                ConfigureConverterForQuality(settings, converter);
        }

        static void ConfigureConverterForBitRate([NotNull] SettingsDictionary settings, int channels, IntPtr converter)
        {
            uint minBitRate = channels == 1 ? 32u : 64u;
            uint maxBitRate = channels == 1 ? 256u : 320u;

            if (!uint.TryParse(settings["BitRate"], out uint bitRate) || bitRate < minBitRate || bitRate > maxBitRate)
                throw new InvalidSettingException(string.Format(CultureInfo.CurrentCulture,
                    Resources.AacSampleEncoderBadBitRate, settings["BitRate"], minBitRate, maxBitRate));

            AudioConverterStatus status = SetConverterProperty(converter, AudioConverterPropertyId.BitRate, bitRate * 1000);
            if (status != AudioConverterStatus.Ok)
                throw new IOException(string.Format(CultureInfo.CurrentCulture,
                    Resources.SampleEncoderConverterBitRateError, status));

            BitrateControlMode controlMode;
            if (string.IsNullOrEmpty(settings["ControlMode"]) ||
                string.Compare(settings["ControlMode"], "Constrained", StringComparison.OrdinalIgnoreCase) == 0)
                controlMode = BitrateControlMode.VariableConstrained;
            else if (string.Compare(settings["ControlMode"], "Average", StringComparison.OrdinalIgnoreCase) == 0)
                controlMode = BitrateControlMode.LongTermAverage;
            else if (string.Compare(settings["ControlMode"], "Constant", StringComparison.OrdinalIgnoreCase) == 0)
                controlMode = BitrateControlMode.Constant;
            else
                throw new InvalidSettingException(string.Format(CultureInfo.CurrentCulture,
                    Resources.AacSampleEncoderBadBitRateControlMode, settings["ControlMode"]));

            status = SetConverterProperty(converter, AudioConverterPropertyId.BitRateControlMode, (uint)controlMode);
            if (status != AudioConverterStatus.Ok)
                throw new InvalidSettingException(string.Format(CultureInfo.CurrentCulture,
                    Resources.SampleEncoderConverterControlModeError, status));
        }

        static void ConfigureConverterForQuality([NotNull] SettingsDictionary settings, IntPtr converter)
        {
            if (!string.IsNullOrEmpty(settings["ControlMode"]) &&
                string.Compare(settings["ControlMode"], "Variable", StringComparison.OrdinalIgnoreCase) != 0)
                throw new InvalidSettingException(Resources.AacSampleEncoderBadQualityControlMode);

            AudioConverterStatus status = SetConverterProperty(converter, AudioConverterPropertyId.BitRateControlMode, (uint)BitrateControlMode.Variable);
            if (status != AudioConverterStatus.Ok)
                throw new IOException(string.Format(CultureInfo.CurrentCulture,
                    Resources.SampleEncoderConverterControlModeError, status));

            // There are only 15 distinct settings actually available:
            uint vbrQualityIndex;
            if (string.IsNullOrEmpty(settings["VBRQuality"]))
                vbrQualityIndex = 9;
            else if (!uint.TryParse(settings["VBRQuality"], out vbrQualityIndex) ||
                     vbrQualityIndex >= _vbrQualities.Length)
                throw new InvalidSettingException(string.Format(CultureInfo.CurrentCulture,
                    Resources.AacSampleEncoderBadVbrQuality, settings["VBRQuality"]));

            status = SetConverterProperty(converter, AudioConverterPropertyId.VbrQuality, _vbrQualities[vbrQualityIndex]);
            if (status != AudioConverterStatus.Ok)
                throw new IOException(string.Format(CultureInfo.CurrentCulture,
                    Resources.SampleEncoderConverterQualityError, status));
        }

        static AudioConverterStatus SetConverterProperty<T>(IntPtr converter, AudioConverterPropertyId propertyId, T value) where T : struct
        {
            IntPtr unmanagedValue = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
            try
            {
                Marshal.StructureToPtr(value, unmanagedValue, false);
                return SafeNativeMethods.AudioConverterSetProperty(converter, propertyId, (uint)Marshal.SizeOf(typeof(T)), unmanagedValue);
            }
            finally
            {
                Marshal.FreeHGlobal(unmanagedValue);
            }
        }
    }
}
