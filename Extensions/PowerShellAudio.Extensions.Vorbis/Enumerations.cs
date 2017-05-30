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

namespace PowerShellAudio.Extensions.Vorbis
{
    enum Result
    {
        OkMoreAvailable = 1,
        Ok = 0,
        FaultError = -129,
        NotImplementedError = -130,
        InValueError = -131,
        NotVorbisError = -132,
        BadHeaderError = -133
    }

    enum PictureType : uint
    {
        Other,
        PngIcon,
        OtherIcon,
        CoverFront,
        CoverBack,
        Leaflet,
        Media,
        LeadArtist,
        Artist,
        Conductor,
        Band,
        Composer,
        Lyricist,
        Location,
        DuringRecording,
        DuringPerformance,
        ScreenCapture,
        BrightFish,
        Illustration,
        ArtistLogo,
        PublisherLogo
    }
}