﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PowerShellAudio.Extensions.Wave.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PowerShellAudio.Extensions.Wave.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected wBitsPerSample to be a multiple of 8.
        /// </summary>
        internal static string AudioInfoDecoderBitsPerSampleError {
            get {
                return ResourceManager.GetString("AudioInfoDecoderBitsPerSampleError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected a 22 byte fmt chunk extension.
        /// </summary>
        internal static string AudioInfoDecoderFmtExtensionLengthError {
            get {
                return ResourceManager.GetString("AudioInfoDecoderFmtExtensionLengthError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The fmt chunk is too small.
        /// </summary>
        internal static string AudioInfoDecoderFmtLengthError {
            get {
                return ResourceManager.GetString("AudioInfoDecoderFmtLengthError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The stream is too short to be a valid Wave file.
        /// </summary>
        internal static string AudioInfoDecoderLengthError {
            get {
                return ResourceManager.GetString("AudioInfoDecoderLengthError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Wave file does not contain a fmt chunk.
        /// </summary>
        internal static string AudioInfoDecoderMissingFmtError {
            get {
                return ResourceManager.GetString("AudioInfoDecoderMissingFmtError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Wave file contains no samples.
        /// </summary>
        internal static string AudioInfoDecoderMissingSamplesError {
            get {
                return ResourceManager.GetString("AudioInfoDecoderMissingSamplesError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not a Wave file.
        /// </summary>
        internal static string AudioInfoDecoderNotWaveError {
            get {
                return ResourceManager.GetString("AudioInfoDecoderNotWaveError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only PCM Wave files are supported by this decoder.
        /// </summary>
        internal static string AudioInfoDecoderUnsupportedError {
            get {
                return ResourceManager.GetString("AudioInfoDecoderUnsupportedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected wBitsPerSample to be a multiple of 8.
        /// </summary>
        internal static string SampleDecoderBitsPerSampleError {
            get {
                return ResourceManager.GetString("SampleDecoderBitsPerSampleError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only 1 (mono) or 2 (stereo) channels are supported.
        /// </summary>
        internal static string SampleDecoderChannelsError {
            get {
                return ResourceManager.GetString("SampleDecoderChannelsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpectedly encountered the end of the stream.
        /// </summary>
        internal static string SampleDecoderEndOfStreamError {
            get {
                return ResourceManager.GetString("SampleDecoderEndOfStreamError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected a 22 byte fmt chunk extension.
        /// </summary>
        internal static string SampleDecoderFmtExtensionLengthError {
            get {
                return ResourceManager.GetString("SampleDecoderFmtExtensionLengthError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The fmt chunk is too small.
        /// </summary>
        internal static string SampleDecoderFmtLengthError {
            get {
                return ResourceManager.GetString("SampleDecoderFmtLengthError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Wave file does not contain a fmt chunk.
        /// </summary>
        internal static string SampleDecoderMissingFmtError {
            get {
                return ResourceManager.GetString("SampleDecoderMissingFmtError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not a Wave file.
        /// </summary>
        internal static string SampleDecoderNotWaveError {
            get {
                return ResourceManager.GetString("SampleDecoderNotWaveError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only PCM Wave files are supported by this decoder.
        /// </summary>
        internal static string SampleDecoderUnsupportedError {
            get {
                return ResourceManager.GetString("SampleDecoderUnsupportedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linear PCM Wave.
        /// </summary>
        internal static string SampleEncoderDescription {
            get {
                return ResourceManager.GetString("SampleEncoderDescription", resourceCulture);
            }
        }
    }
}
