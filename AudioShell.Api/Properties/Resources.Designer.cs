﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PowerShellAudio.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PowerShellAudio.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to There is no analyzer named &apos;{0}&apos; available.
        /// </summary>
        internal static string AnalyzableAudioFileFactoryError {
            get {
                return ResourceManager.GetString("AnalyzableAudioFileFactoryError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No decoders were able to read this file.
        /// </summary>
        internal static string AudioFileDecodeError {
            get {
                return ResourceManager.GetString("AudioFileDecodeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no encoder named &apos;{0}&apos; available.
        /// </summary>
        internal static string ExportableAudioFileFactoryError {
            get {
                return ResourceManager.GetString("ExportableAudioFileFactoryError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file &apos;{0}&apos; already exists.
        /// </summary>
        internal static string ExportableAudioFileFileExistsError {
            get {
                return ResourceManager.GetString("ExportableAudioFileFileExistsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The setting &apos;{0}&apos; is not recognized by this encoder.
        /// </summary>
        internal static string ExportableAudioFileSettingsError {
            get {
                return ResourceManager.GetString("ExportableAudioFileSettingsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The setting &apos;{0}&apos; is not recognized by this metadata format.
        /// </summary>
        internal static string TaggedAudioFileSettingsError {
            get {
                return ResourceManager.GetString("TaggedAudioFileSettingsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No metadata encoders are able to save metadata to this type of file.
        /// </summary>
        internal static string TaggedAudioFileUnsupportedError {
            get {
                return ResourceManager.GetString("TaggedAudioFileUnsupportedError", resourceCulture);
            }
        }
    }
}
