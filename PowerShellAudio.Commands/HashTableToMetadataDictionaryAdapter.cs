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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PowerShellAudio.Commands
{
    class HashTableToMetadataDictionaryAdapter : MetadataDictionary
    {
        internal HashTableToMetadataDictionaryAdapter(IEnumerable hashtable)
        {
            if (hashtable == null)
                return;

            foreach (var entry in hashtable.Cast<DictionaryEntry>().Select(entry =>
                new KeyValuePair<string, string>(entry.Key.ToString(), entry.Value.ToString())))
                Add(entry);
        }
    }
}
