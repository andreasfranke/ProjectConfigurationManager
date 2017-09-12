﻿namespace tomenglertde.ProjectConfigurationManager.View
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Interaction logic for MultipleChoiceFilter.xaml
    /// </summary>
    public class MultipleChoiceFilter : MultipleChoiceFilterBase
    {
        static MultipleChoiceFilter()
        {
            // ReSharper disable once PossibleNullReferenceException
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultipleChoiceFilter), new FrameworkPropertyMetadata(typeof(MultipleChoiceFilterBase)));
        }

        protected override void OnSourceValuesChanged([CanBeNull] IEnumerable<string> newValue)
        {
            if (newValue == null)
                Values.Clear();
            else
                Values.SynchronizeWith(newValue.ToArray());
        }

        protected override MultipleChoiceContentFilterBase CreateFilter([CanBeNull] IEnumerable<string> items)
        {
            return new MultipleChoicesContentFilter(items);
        }

        private class MultipleChoicesContentFilter : MultipleChoiceContentFilterBase
        {
            public MultipleChoicesContentFilter([CanBeNull] IEnumerable<string> items)
                : base(items)
            {
            }

            public override bool IsMatch(object value)
            {
                var input = value as string;
                if (string.IsNullOrWhiteSpace(input))
                    return Items?.Contains(string.Empty) ?? true;

                return Items?.ContainsAny(input) ?? true;
            }
        }
    }
}
