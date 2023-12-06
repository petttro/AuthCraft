using Microsoft.Extensions.Options;

namespace AuthCraft.Api.Test.Common
{
    /// <summary>
    /// IOptions wrapper that returns the options instance.
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public class OptionsSnapshotWrapper<TOptions> : IOptionsSnapshot<TOptions> where TOptions : class, new()
    {
        /// <summary>
        /// Intializes the wrapper with the options instance to return.
        /// </summary>
        /// <param name="options">The options instance to return.</param>
        public OptionsSnapshotWrapper(TOptions options)
        {
            Value = options;
        }

        /// <summary>
        /// The options instance.
        /// </summary>
        public TOptions Value { get; }

        public TOptions Get(string name)
        {
            return Value;
        }
    }
}
