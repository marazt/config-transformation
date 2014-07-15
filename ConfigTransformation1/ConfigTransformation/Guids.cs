// Guids.cs
// MUST match guids.h
using System;

namespace marazt.ConfigTransformation
{
    static class GuidList
    {
        public const string guidConfigTransformationPkgString = "7cf867d9-725e-43b9-a3b2-9a61ca19da4d";
        public const string guidConfigTransformationCmdSetString = "18ad250f-f0b8-4d2a-a2c9-0fc831b2488d";

        public static readonly Guid guidConfigTransformationCmdSet = new Guid(guidConfigTransformationCmdSetString);
    };
}