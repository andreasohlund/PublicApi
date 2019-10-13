namespace PublicAPI.APIExtraction
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class AssemblyAPIExtractor
    {
        public Task<List<PublicType>> ExtractFromStream(Stream stream)
        {

            using var assembly = AssemblyDefinition.ReadAssembly(stream);

            var publicTypes = assembly.Modules.SelectMany(m => m.GetTypes())
                .Where(t => !t.IsNested && ShouldIncludeType(t))
                .OrderBy(t => t.FullName, StringComparer.Ordinal)
                .Select(ti => ConvertTypeInfoToPublicType(ti))
                .ToList();

            return Task.FromResult(publicTypes);
        }

        PublicType ConvertTypeInfoToPublicType(TypeDefinition typeDefinition)
        {
            var type = new PublicType
            {
                Name = typeDefinition.Name,
                Namespace = typeDefinition.Namespace
            };

            type.Methods = typeDefinition.Methods
                .Where(m => m.IsPublic)
                .OrderBy(m => m.Name, StringComparer.Ordinal)
                .Select(m => ConvertMethodDefinitionToMethod(m))
                .ToList();

            return type;
        }

        Method ConvertMethodDefinitionToMethod(MethodDefinition methodDefinition)
        {
            return new Method
            {
                Name = methodDefinition.Name,
                ReturnType = methodDefinition.ReturnType.FullName
            };
        }

        static bool ShouldIncludeType(TypeDefinition t)
        {
            return (t.IsPublic || t.IsNestedPublic || t.IsNestedFamily) && !IsCompilerGenerated(t);
        }

        static bool IsCompilerGenerated(IMemberDefinition m)
        {
            return m.CustomAttributes.Any(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
        }
    }
}
