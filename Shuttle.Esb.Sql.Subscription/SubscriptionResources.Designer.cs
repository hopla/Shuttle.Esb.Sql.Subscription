﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shuttle.Esb.Sql.Subscription {
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
    internal class SubscriptionResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SubscriptionResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Shuttle.Esb.Sql.Subscription.SubscriptionResources", typeof(SubscriptionResources).Assembly);
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
        ///   Looks up a localized string similar to The connection string used by &apos;{0}&apos; is empty..
        /// </summary>
        internal static string ConnectionStringEmpty {
            get {
                return ResourceManager.GetString("ConnectionStringEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a connection string with name &apos;{0}&apos;..
        /// </summary>
        internal static string ConnectionStringMissing {
            get {
                return ResourceManager.GetString("ConnectionStringMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Subscription to message type &apos;{0}&apos; is missing..
        /// </summary>
        internal static string MissingSubscription {
            get {
                return ResourceManager.GetString("MissingSubscription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The following missing message type subscriptions have been logged: {0}.
        /// </summary>
        internal static string MissingSubscriptionException {
            get {
                return ResourceManager.GetString("MissingSubscriptionException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provider name used by &apos;{0}&apos; is empty..
        /// </summary>
        internal static string ProviderNameEmpty {
            get {
                return ResourceManager.GetString("ProviderNameEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The database has not been configured for subscription storage and the creation script could not be executed against your database.  The inner exception should provide the reason.  Please run the SubscriptionManagerCreate.sql script file against your database..
        /// </summary>
        internal static string SubscriptionManagerCreateException {
            get {
                return ResourceManager.GetString("SubscriptionManagerCreateException", resourceCulture);
            }
        }
    }
}
