// Guids.cs
// MUST match guids.h
using System;

namespace Marazt.ConfigTransformation
{
    /// <summary>
    /// 
    /// </summary>
    static class GuidList
    {
        #region Constants

        /// <summary>
        /// The unique identifier configuration transformation PKG string
        /// </summary>
        public const string guidConfigTransformationPkgString = "a8ef38c4-8d82-4245-a6ac-775412dd55e7";

        /// <summary>
        /// The unique identifier configuration transformation command set string
        /// </summary>
        public const string guidConfigTransformationCmdSetString = "0180f190-1c18-4c44-b021-9768701419d7";

        /// <summary>
        /// The unique identifier configuration transformation command set
        /// </summary>
        public static readonly Guid guidConfigTransformationCmdSet = new Guid(guidConfigTransformationCmdSetString);

        #endregion Constants

    };
}