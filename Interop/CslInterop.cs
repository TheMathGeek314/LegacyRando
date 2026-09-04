using System;
using System.Collections.Generic;
using MonoMod.ModInterop;
using RandomizerMod.Logging;

namespace LegacyRando {
    internal static class CondensedSpoilerLogger {
        [ModImportName("CondensedSpoilerLogger")]
        private static class CslInterop {
            public static Action<string, Func<LogArguments, bool>, List<string>> AddCategory = null;
        }
        static CondensedSpoilerLogger() {
            typeof(CslInterop).ModInterop();
        }
        public static void AddCategory(string categoryName, Func<LogArguments, bool> test, List<string> entries) => CslInterop.AddCategory?.Invoke(categoryName, test, entries);
    }
}
