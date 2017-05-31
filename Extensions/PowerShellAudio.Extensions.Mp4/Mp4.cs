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

using PowerShellAudio.Extensions.Mp4.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace PowerShellAudio.Extensions.Mp4
{
    class Mp4
    {
        readonly Stream _stream;
        readonly Stack<AtomInfo> _atomInfoStack = new Stack<AtomInfo>();

        [NotNull]
        public AtomInfo CurrentAtom => _atomInfoStack.Peek();

        internal Mp4([NotNull] Stream stream)
        {
            _stream = stream;
        }

        internal void DescendToAtom([NotNull] params string[] hierarchy)
        {
            _stream.Position = 0;
            _atomInfoStack.Clear();

            using (var reader = new BinaryReader(_stream, Encoding.GetEncoding(1252), true))
            {
                foreach (string fourCC in hierarchy)
                {
                    do
                    {
                        var subAtom = new AtomInfo((uint)_stream.Position, reader.ReadUInt32BigEndian(),
                            reader.ReadFourCC());
                        if (subAtom.Size == 0)
                            throw new IOException(Resources.Mp4AtomNotFoundError);

                        if (subAtom.FourCC == fourCC)
                        {
                            _atomInfoStack.Push(subAtom);

                            // Some containers also contain data, which needs to be skipped:
                            switch (fourCC)
                            {
                                case "meta":
                                    _stream.Seek(4, SeekOrigin.Current);
                                    break;
                                case "stsd":
                                    _stream.Seek(8, SeekOrigin.Current);
                                    break;
                                case "mp4a":
                                    _stream.Seek(28, SeekOrigin.Current);
                                    break;
                            }

                            break;
                        }

                        _stream.Position = subAtom.End;

                    } while (_stream.Position < (_atomInfoStack.Count == 0 ? _stream.Length : _atomInfoStack.Peek().End));
                }
            }
        }

        [NotNull, ItemNotNull]
        internal AtomInfo[] GetChildAtomInfo()
        {
            var result = new List<AtomInfo>();

            using (var reader = new BinaryReader(_stream, Encoding.GetEncoding(1252), true))
            {
                _stream.Position = _atomInfoStack.Count == 0 ? 0 : _atomInfoStack.Peek().Start + 8;

                while (_stream.Position < (_atomInfoStack.Count == 0 ? _stream.Length : _atomInfoStack.Peek().End))
                {
                    var childAtom = new AtomInfo((uint)_stream.Position, reader.ReadUInt32BigEndian(), reader.ReadFourCC());
                    result.Add(childAtom);
                    _stream.Position = childAtom.End;
                }
            }

            return result.ToArray();
        }

        [NotNull]
        internal byte[] ReadAtom([NotNull] AtomInfo atom)
        {
            _stream.Position = atom.Start;

            using (var reader = new BinaryReader(_stream, Encoding.Default, true))
                return reader.ReadBytes((int)atom.Size);
        }

        internal void CopyAtom([NotNull] AtomInfo atom, [NotNull] Stream output)
        {
            _stream.Position = atom.Start;
            _stream.CopyRangeTo(output, atom.Size);
        }

        internal void UpdateAtomSizes(uint increase)
        {
            if (_atomInfoStack.Count <= 0) return;

            using (var writer = new BinaryWriter(_stream, Encoding.Default, true))
            {
                do
                {
                    AtomInfo currentAtom = _atomInfoStack.Pop();
                    _stream.Position = currentAtom.Start;
                    writer.WriteBigEndian(currentAtom.Size + increase);
                } while (_atomInfoStack.Count > 0);
            }
        }

        internal void UpdateMvhd(DateTime creation, DateTime modification)
        {
            DescendToAtom("moov", "mvhd");
            int version = _stream.ReadByte();
            _stream.Seek(3, SeekOrigin.Current);

            var epoch = new DateTime(1904, 1, 1);
            double creationSeconds = creation.Subtract(epoch).TotalSeconds;
            double modificationSeconds = modification.Subtract(epoch).TotalSeconds;

            using (var writer = new BinaryWriter(_stream, Encoding.Default, true))
            {
                if (version == 0)
                {
                    writer.WriteBigEndian((uint)creationSeconds);
                    writer.WriteBigEndian((uint)modificationSeconds);
                }
                else
                {
                    writer.WriteBigEndian((ulong)creationSeconds);
                    writer.WriteBigEndian((ulong)modificationSeconds);
                }
            }
        }

        internal void UpdateTkhd(DateTime creation, DateTime modification)
        {
            DescendToAtom("moov", "trak", "tkhd");
            int version = _stream.ReadByte();
            _stream.Seek(3, SeekOrigin.Current);

            var epoch = new DateTime(1904, 1, 1);
            double creationSeconds = creation.Subtract(epoch).TotalSeconds;
            double modificationSeconds = modification.Subtract(epoch).TotalSeconds;

            using (var writer = new BinaryWriter(_stream, Encoding.Default, true))
            {
                if (version == 0)
                {
                    writer.WriteBigEndian((uint)creationSeconds);
                    writer.WriteBigEndian((uint)modificationSeconds);
                }
                else
                {
                    writer.WriteBigEndian((ulong)creationSeconds);
                    writer.WriteBigEndian((ulong)modificationSeconds);
                }
            }
        }

        internal void UpdateMdhd(DateTime creation, DateTime modification)
        {
            DescendToAtom("moov", "trak", "mdia", "mdhd");
            int version = _stream.ReadByte();
            _stream.Seek(3, SeekOrigin.Current);

            var epoch = new DateTime(1904, 1, 1);
            double creationSeconds = creation.Subtract(epoch).TotalSeconds;
            double modificationSeconds = modification.Subtract(epoch).TotalSeconds;

            using (var writer = new BinaryWriter(_stream, Encoding.Default, true))
            {
                if (version == 0)
                {
                    writer.WriteBigEndian((uint)creationSeconds);
                    writer.WriteBigEndian((uint)modificationSeconds);
                }
                else
                {
                    writer.WriteBigEndian((ulong)creationSeconds);
                    writer.WriteBigEndian((ulong)modificationSeconds);
                }
            }
        }

        internal void UpdateStco(int offset)
        {
            if (offset != 0)
            {
                DescendToAtom("moov", "trak", "mdia", "minf", "stbl", "stco");
                _stream.Seek(4, SeekOrigin.Current);

                using (var reader = new BinaryReader(_stream, Encoding.Default, true))
                using (var writer = new BinaryWriter(_stream, Encoding.Default, true))
                {
                    uint count = reader.ReadUInt32BigEndian();
                    long dataStart = _stream.Position;

                    for (var i = 0; i < count; i++)
                    {
                        _stream.Position = dataStart + i * 4;
                        var value = (int)reader.ReadUInt32BigEndian();
                        _stream.Seek(-4, SeekOrigin.Current);
                        writer.WriteBigEndian((uint)(value + offset));
                    }
                }
            }
        }
    }
}
