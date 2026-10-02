// Krafs.Publicizer publicises the reference assembly, which is what lets the component read
// pawn.filth.carriedFilth at all. In the real Assembly-CSharp that member is not public: the
// compiler emits a plain cross-assembly access either way, and the CLR allows that instruction
// only when this assembly declares the waiver below.
//
// Publicizer defines the attribute type for us and normally applies it through the SDK's
// generated AssemblyInfo — which this project switches off with GenerateAssemblyInfo=false. The
// type was therefore embedded and the waiver was not. Nothing said so: the build stayed clean,
// the patch applied, and every bath would have ended on a FieldAccessException thrown out of the
// hediff's removal, taking the last onlooker check with it. The one thing the mod does on the way
// out, dead behind a clean startup.
//
// Two checks, neither of which needs the game running. Whether the waiver is applied is read from
// this assembly's own attribute list, GetCustomAttributesData(), never from the bytes — the type
// name is in the file in both cases, so grepping proves nothing and is exactly the shape of the
// trap. Whether the waiver is needed at all is settled by deleting the Publicize line from the
// csproj and building: it stops compiling here, on this one member.

[assembly: System.Runtime.CompilerServices.IgnoresAccessChecksTo("Assembly-CSharp")]
