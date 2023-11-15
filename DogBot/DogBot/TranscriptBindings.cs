using NuGet.Configuration;

namespace DogBot
{
    /// <summary>
    /// Defines configuration-related constants.
    /// </summary>
    internal static class ConfigurationConstants
    {
        /// <summary>
        /// The configuration key mapping to the value representing the application root path.
        /// </summary>
        public const string ApplicationRootKey = "applicationRoot";

        /// <summary>
        /// The configuration key mapping to the value representing the bot root path.
        /// </summary>
        public const string BotKey = "bot";

        /// <summary>
        /// The configuration key mapping to the value representing the default resource identifier
        /// of the dialog to be used as the root dialog of the bot.
        /// </summary>
        public const string RootDialogKey = "defaultRootDialog";

        /// <summary>
        /// The configuration key mapping to the value representing the default resource identifier
        /// of the LanguageGenerator to be used by the bot.
        /// </summary>
        public const string LanguageGeneratorKey = "defaultLg";

        /// <summary>
        /// Default configuration location for runtime settings.
        /// </summary>
        public const string RuntimeSettingsKey = "runtimeSettings";

        /// <summary>
        /// The configuration key mapping to the value representing the default locale.
        /// </summary>
        public const string DefaultLocaleKey = "defaultLocale";
    }

    /// <summary>
    /// Settings for runtime features.
    /// </summary>
    internal class FeatureSettings
    {
        /// <summary>
        /// Gets the configuration key for <see cref="FeatureSettings"/>.
        /// </summary>
        /// <value>
        /// Configuration key for <see cref="FeatureSettings"/>.
        /// </value>
        public static string FeaturesSettingsKey => $"{ConfigurationConstants.RuntimeSettingsKey}:features";

        /// <summary>
        /// Gets or sets a value indicating whether the runtime should remove recipient mentions.
        /// </summary>
        /// <value>
        /// A value indicating whether the runtime should remove recipient mentions.
        /// </value>
        public bool RemoveRecipientMentions { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the runtime should send typing activities.
        /// </summary>
        /// <value>
        /// A value indicating whether the runtime should send typing activities.
        /// </value>
        public bool ShowTyping { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to use inspection middleware.
        /// </summary>
        /// <value>
        /// A value indicating whether to use inspection middleware.
        /// </value>
        public bool UseInspection { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to use traces for transcripts.
        /// </summary>
        /// <value>
        /// A value indicating whether to use traces for transcripts.
        /// </value>
        public bool TraceTranscript { get; set; } = false;

        /// <summary>
        /// Gets or sets the blob transcript store settings.
        /// </summary>
        /// <value>
        /// The blob transcript store settings.
        /// </value>
        public BlobsStorageSettings BlobTranscript { get; set; }

        /// <summary>
        /// Gets or sets the SetSpeakMiddleware settings.
        /// </summary>
        /// <value>
        /// The SetSpeakMiddleware settings.
        /// </value>
        public SpeakSettings SetSpeak { get; set; }
    }

    /// <summary>
    /// Settings for blob storage.
    /// </summary>
    internal class BlobsStorageSettings
    {
        /// <summary>
        /// Gets or sets the blob connection string.
        /// </summary>
        /// <value>
        /// The blob connection string.
        /// </value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the blob container name.
        /// </summary>
        /// <value>
        /// The blob container name.
        /// </value>
        public string ContainerName { get; set; }
    }

    /// <summary>
    /// Speak settings for the runtim.  This is used by SetSpeakMiddleware.
    /// </summary>
    internal class SpeakSettings
    {
        /// <summary>
        /// Gets or sets the SSML voice name attribute value.
        /// </summary>
        /// <value>
        /// The SSML voice name attribute value.
        /// </value>
        public string VoiceFontName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the behavior to set an empty Activity.Speak 
        /// property with the value from Activity.Text.
        /// </summary>
        /// <value>
        /// true to indicates empty Activity.Speak should be set with Activity.Text.
        /// </value>
        public bool FallbackToTextForSpeechIfEmpty { get; set; }

        /// <summary>
        /// Gets or sets the xml:lang value for a SSML speak element.
        /// </summary>
        /// <value>
        /// The xml:lang value.
        /// </value>
        public string Lang { get; set; }
    }
}