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

using PowerShellAudio.Commands.Properties;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace PowerShellAudio.Commands
{
    [Cmdlet(VerbsData.Export, "AudioFile", DefaultParameterSetName = "ByPath", SupportsShouldProcess = true), OutputType(typeof(ExportableAudioFile))]
    public class ExportAudioFileCommand : PSCmdlet, IDisposable
    {
        readonly IList<ExportableAudioFile> _audioFiles = new List<ExportableAudioFile>();
        readonly CancellationTokenSource _cancelSource = new CancellationTokenSource();

        [Parameter(Mandatory = true, Position = 0)]
        public string Encoder { get; set; }

        [Parameter(ParameterSetName = "ByPath", Mandatory = true, Position = 1)]
        public string Path { get; set; }

        [Parameter(ParameterSetName = "ByLiteralPath", Mandatory = true, Position = 1), Alias("PSPath")]
        public string LiteralPath { get; set; }

        [Parameter(Mandatory = true, Position = 2, ValueFromPipeline = true)]
        public AudioFile AudioFile { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public Hashtable Setting { get; set; }

        [Parameter]
        public SwitchParameter Replace { get; set; }

        protected override void ProcessRecord()
        {
            if (ShouldProcess(AudioFile.FileInfo.FullName))
                _audioFiles.Add(new ExportableAudioFile(AudioFile));
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Non-terminating Cmdlet exceptions should be written to an ErrorRecord")]
        protected override void EndProcessing()
        {
            if (_audioFiles.Count > 0)
            {
                var completed = 0;

                using (var outputQueue = new BlockingCollection<object>())
                {
                    outputQueue.Add(new ProgressRecord(0,
                        string.Format(CultureInfo.CurrentCulture, Resources.ExportAudioFileCommandActivityMessage,
                            Encoder),
                        string.Format(CultureInfo.CurrentCulture, Resources.ExportAudioFileCommandStatusMessage,
                            0, _audioFiles.Count)) {PercentComplete = 0});

                    Task.Run(() => Parallel.ForEach(_audioFiles, new ParallelOptions { CancellationToken = _cancelSource.Token }, audioFile =>
                    {
                        try
                        {
                            var substituter = new MetadataSubstituter(audioFile.Metadata);

                            DirectoryInfo outputDirectory;
                            try
                            {
                                outputDirectory = new DirectoryInfo(this.GetFileSystemPaths(substituter.Substitute(Path), substituter.Substitute(LiteralPath)).First());
                            }
                            catch (ItemNotFoundException e)
                            {
                                outputDirectory = new DirectoryInfo(e.ItemName);
                            }

                            ExportableAudioFile result = audioFile.Export(Encoder, _cancelSource.Token,
                                new HashTableToSettingsDictionaryAdapter(Setting), outputDirectory,
                                string.IsNullOrEmpty(Name) ? null : substituter.Substitute(Name), Replace);

                            Interlocked.Increment(ref completed);

                            outputQueue.Add(result);

                            outputQueue.Add(new ProgressRecord(0,
                                string.Format(CultureInfo.CurrentCulture, Resources.ExportAudioFileCommandActivityMessage,
                                    Encoder),
                                string.Format(CultureInfo.CurrentCulture, Resources.ExportAudioFileCommandStatusMessage,
                                    completed, _audioFiles.Count))
                            {
                                PercentComplete = completed.GetPercent(_audioFiles.Count)
                            });
                        }
                        catch (Exception e)
                        {
                            Interlocked.Increment(ref completed);
                            outputQueue.Add(new ErrorRecord(e, e.HResult.ToString(CultureInfo.CurrentCulture), ErrorCategory.WriteError, audioFile));
                        }
                    })).ContinueWith(task => outputQueue.CompleteAdding());

                    // Process output on the main thread:
                    this.ProcessOutput(outputQueue, _cancelSource.Token);
                }
            }
        }

        protected override void StopProcessing()
        {
            _cancelSource.Cancel();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _cancelSource.Dispose();
        }
    }
}
