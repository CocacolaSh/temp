using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Framework.Mvc.PostTokens
{
    public interface IPostToken
    {
        /// <summary>
        /// Generates the Post token.
        /// </summary>
        string GeneratePostToken();

        /// <summary>
        /// Gets the get last Post token from Form
        /// </summary>
        string GetLastPostToken { get; }

        /// <summary>
        /// Gets a value indicating whether [tokens match].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [tokens match]; otherwise, <c>false</c>.
        /// </value>
        bool TokensMatch { get; }
    }
}
