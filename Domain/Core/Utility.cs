using System;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Text;
using System.Xml;

using Rareform.Validation;

namespace SoL.Core
{
    public static class Utility
    {
        public static string TagCleaner(string uncleanTag)
        {
            if (uncleanTag == null)
            {
                Throw.ArgumentNullException(() => uncleanTag);
            }
            var buffer = new StringBuilder(uncleanTag.Length);
            foreach (char c in uncleanTag)
            {
                buffer.Append(XmlConvert.IsXmlChar(c) ? c : '_');
            }

            return buffer.ToString();
        }

        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/></exception>
        public static IObservable<NotifyCollectionChangedEventArgs> Changed<T>(this T source)
            where T : class, INotifyCollectionChanged
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    handler => source.CollectionChanged += handler,
                    handler => source.CollectionChanged -= handler)
                .Select(x => x.EventArgs);
        }

        /// <exception cref="Exception">Condition.</exception>
        public static void Retry(this Action block, int retries = 3)
        {
            while (true)
            {
                try
                {
                    block();
                    return;
                }

                catch (Exception)
                {
                    retries--;

                    if (retries == 0)
                    {
                        throw;
                    }
                }
            }
        }
    }
}