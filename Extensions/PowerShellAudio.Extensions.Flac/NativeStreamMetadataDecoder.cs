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

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;

namespace PowerShellAudio.Extensions.Flac
{
    class NativeStreamMetadataDecoder : NativeStreamDecoder
    {
        [NotNull]
        internal MetadataDictionary Metadata { get; }

        internal NativeStreamMetadataDecoder([NotNull] Stream input)
            : base(input)
        {
            Metadata = new MetadataDictionary();
        }

        protected override void MetadataCallback(IntPtr handle, IntPtr metadata, IntPtr userData)
        {
            switch ((MetadataType)Marshal.ReadInt32(metadata))
            {
                case MetadataType.VorbisComment:
                    var commentAdapter = new VorbisCommentToMetadataAdapter();
                    VorbisComment vorbisComment = Marshal.PtrToStructure<VorbisCommentMetadataBlock>(metadata).VorbisComment;
                    for (var commentIndex = 0; commentIndex < vorbisComment.Count; commentIndex++)
                    {
                        var entry = Marshal.PtrToStructure<VorbisCommentEntry>(IntPtr.Add(vorbisComment.Comments,
                                commentIndex * Marshal.SizeOf<VorbisCommentEntry>()));
                        var commentBytes = new byte[entry.Length];
                        Marshal.Copy(entry.Entry, commentBytes, 0, commentBytes.Length);
                        string[] comment = Encoding.UTF8.GetString(commentBytes).Split(new[] { '=' }, 2);
                        commentAdapter[comment[0]] = comment[1];
                    }
                    commentAdapter.CopyTo(Metadata);
                    break;

                case MetadataType.Picture:
                    Picture picture = Marshal.PtrToStructure<PictureMetadataBlock>(metadata).Picture;
                    if (picture.Type == PictureType.CoverFront || picture.Type == PictureType.Other)
                    {
                        var coverBytes = new byte[picture.DataLength];
                        Marshal.Copy(picture.Data, coverBytes, 0, coverBytes.Length);
                        try
                        {
                            Metadata.CoverArt = new CoverArt(coverBytes);
                        }
                        catch (UnsupportedCoverArtException)
                        { }
                    }
                    break;
            }
        }
    }
}
