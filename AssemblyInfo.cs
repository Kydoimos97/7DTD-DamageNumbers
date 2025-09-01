// AssemblyInfo.cs

using System.Reflection;
using System.Security;
using Utilities;

[assembly: AssemblyCompany("DamageNumbers")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion(ModMetaData.ModVersion)]
[assembly: AssemblyInformationalVersion(ModMetaData.ModVersion)]
[assembly: AssemblyProduct("DamageNumbers")]
[assembly: AssemblyTitle("DamageNumbers")]
[assembly: AssemblyVersion(ModMetaData.ModVersion)]

// Allow partially trusted callers (replaces obsolete SecurityPermission)
[assembly: AllowPartiallyTrustedCallers]