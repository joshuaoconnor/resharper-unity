using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Unity
{
    [SolutionComponent]
    public class UnityApi
    {
        private readonly Lazy<List<UnityType>> myTypes = Lazy.Of(() =>
        {
            var apiXml = new ApiXml();
            return apiXml.LoadTypes();
        }, true);

        [NotNull]
        public IEnumerable<UnityType> GetBaseUnityTypes([NotNull] ITypeElement type)
        {
            return myTypes.Value.Where(c => type.IsDescendantOf(c.GetType(type.Module)));
        }

        public bool IsUnityType([NotNull] ITypeElement type)
        {
            return GetBaseUnityTypes(type).Any();
        }

        public bool IsUnityMessage([NotNull] IMethod method)
        {
            var containingType = method.GetContainingType();
            if (containingType != null)
            {
                return GetBaseUnityTypes(containingType).Any(type => type.Contains(method));
            }
            return false;
        }

        public UnityMessage GetUnityMessage([NotNull] IMethod method)
        {
            var containingType = method.GetContainingType();
            if (containingType != null)
            {
                var messages = from t in GetBaseUnityTypes(containingType)
                    from m in t.Messages
                    where m.Match(method)
                    select m;
                return messages.FirstOrDefault();
            }
            return null;
        }
    }
}