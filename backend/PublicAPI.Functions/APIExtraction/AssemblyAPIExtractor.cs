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

            var publicProperties = typeDefinition.Properties.Where(p => (p.GetMethod?.IsPublic ?? false) || (p.SetMethod?.IsPublic ?? false))
                .ToList();

            type.Properties = publicProperties.Select(p => ConvertPropertyDefinitionToProperty(p))
                .ToList();

            var gettersAndSetters = publicProperties.Where(p => p.GetMethod != null)
                .Select(p => p.GetMethod)
                .Concat(publicProperties.Where(p => p.SetMethod != null).Select(p => p.SetMethod))
                .Where(m => m.IsPublic)
                .ToList();

            type.Methods = typeDefinition.Methods
                .Where(m => m.IsPublic && !gettersAndSetters.Contains(m))
                .OrderBy(m => m.Name, StringComparer.Ordinal)
                .Select(m => ConvertMethodDefinitionToMethod(m))
                .ToList();

            type.Fields = typeDefinition.Fields
                .Where(f => f.IsPublic)
                .OrderBy(f => f.Name, StringComparer.Ordinal)
                .Select(f => ConvertFieldDefinitionToField(f))
                .ToList();

            return type;
        }

        Field ConvertFieldDefinitionToField(FieldDefinition fieldDefinition)
        {
            return new Field
            {
                Name = fieldDefinition.Name,
                Type = fieldDefinition.FieldType.FullName
            };
        }

        Property ConvertPropertyDefinitionToProperty(PropertyDefinition propertyDefinition)
        {
            var property = new Property
            {
                Name = propertyDefinition.Name,
                Type = propertyDefinition.PropertyType.FullName,
                HasGetter = propertyDefinition.GetMethod?.IsPublic ?? false,
                HasSetter = propertyDefinition.SetMethod?.IsPublic ?? false
            };

            return property;
        }

        Method ConvertMethodDefinitionToMethod(MethodDefinition methodDefinition)
        {
            var method = new Method
            {
                Name = methodDefinition.Name,
                ReturnType = methodDefinition.ReturnType.FullName
            };

            method.Parameters = methodDefinition.Parameters.Select(p => ConvertParameterDefinitionToParameter(p))
                .ToList();

            return method;
        }

        Parameter ConvertParameterDefinitionToParameter(ParameterDefinition parameterDefinition)
        {
            var parameter = new Parameter
            {
                Name = parameterDefinition.Name,
                Type = parameterDefinition.ParameterType.FullName,
                IsOptional = parameterDefinition.IsOptional
            };

            if (parameterDefinition.IsOut)
            {
                parameter.Modifier = "out";
            }
            else if (parameterDefinition.ParameterType.IsByReference)
            {
                parameter.Modifier = "ref";
            }
            else if (parameterDefinition.CustomAttributes.Any(attribute => attribute.AttributeType.FullName == typeof(ParamArrayAttribute).FullName))
            {
                parameter.Modifier = "params";
            }
            else
            {
                parameter.Modifier = "none";
            }

            return parameter;
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
