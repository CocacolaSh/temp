using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Framework.Mvc.PostTokens
{
    public abstract class PostTokenBase : IPostToken
    {
        public static readonly string TokenName = "hiddenToken";
        public static readonly string MyToken = "Token";

        /// <summary>
        /// Generates the Post token.
        /// </summary>
        /// <returns></returns>
        public abstract string GeneratePostToken();

        /// <summary>
        /// Gets the get last Post token from Form
        /// </summary>
        public abstract string GetLastPostToken { get; }

        /// <summary>
        /// Gets a value indicating whether [tokens match].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [tokens match]; otherwise, <c>false</c>.
        /// </value>
        public abstract bool TokensMatch { get; }

    }
}
