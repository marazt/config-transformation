// Guids.cs
// MUST match guids.h
using System;

namespace Marazt.ConfigTransformation
{
    static class GuidList
    {
        public const string guidConfigTransformationPkgString = "a8ef38c4-8d82-4245-a6ac-775412dd55e7";
        public const string guidConfigTransformationCmdSetString = "0180f190-1c18-4c44-b021-9768701419d7";


        public static readonly Guid guidConfigTransformationCmdSet = new Guid(guidConfigTransformationCmdSetString);
    };
}