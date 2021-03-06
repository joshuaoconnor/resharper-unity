using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.AspectLookupItems.BaseInfrastructure;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.AspectLookupItems.Info;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems.Impl;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Plugins.Unity.Psi.Resolve;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExpectedTypes;
using JetBrains.ReSharper.Psi.Resolve;

namespace JetBrains.ReSharper.Plugins.Unity.Feature.Services.CodeCompletion
{
    [Language(typeof(CSharpLanguage))]
    public class UnityMessageLiteralTransformationRule : ItemsProviderOfSpecificContext<CSharpCodeCompletionContext>
    {
        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            return IsApplicable(context.TerminatedContext.Reference);
        }

        private bool IsApplicable(IReference reference)
        {
            return reference is IUnityMessageReference;
        }

        protected override AutocompletionBehaviour GetAutocompletionBehaviour(CSharpCodeCompletionContext specificContext)
        {
            return AutocompletionBehaviour.AutocompleteWithReplace;
        }

        protected override void DecorateItems(CSharpCodeCompletionContext context, IEnumerable<ILookupItem> items)
        {
            var reference = context.TerminatedContext.Reference;
            if (!IsApplicable(reference))
                return;

            foreach (var item in GetLookupItems(items))
            {
                // Remove the trailing parentheses from the item
                item.EraseInsertText();
                item.EraseReplaceText();
                item.SetTailType(TailType.None);

                if (item is TextLookupItemBase)
                    context.Freeze(item);

                if (item is LookupItem<DeclaredElementInfo>)
                    context.Freeze(item);
            }
        }

        private IEnumerable<ILookupItem> GetLookupItems(IEnumerable<ILookupItem> items)
        {
            return items.Select(i => i is IWrappedLookupItem ? ((IWrappedLookupItem) i).Item : i)
                .Where(i => i.IsDeclaredElementLookupItem());
        }
    }
}