namespace tomenglertde.ProjectConfigurationManager.Model
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using Equatable;

    using JetBrains.Annotations;

    [ImplementsEquatable]
    public sealed class SolutionContext : INotifyPropertyChanged
    {
        [NotNull]
        private readonly Solution _solution;
        [NotNull, Equals]
        private readonly EnvDTE.SolutionContext _context;

        public SolutionContext([NotNull] Solution solution, [NotNull] SolutionConfiguration solutionConfiguration, [NotNull] EnvDTE.SolutionContext context)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Contract.Requires(solution != null);
            Contract.Requires(solutionConfiguration != null);
            Contract.Requires(context != null);

            _solution = solution;
            SolutionConfiguration = solutionConfiguration;
            _context = context;

            ConfigurationName = context.ConfigurationName;
            PlatformName = context.PlatformName;
            ProjectName = context.ProjectName;
        }

        [CanBeNull]
        public string ConfigurationName { get; private set; }

        [CanBeNull]
        public string PlatformName { get; private set; }

        public bool SetConfiguration([NotNull] ProjectConfiguration configuration)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Contract.Requires(configuration != null);

            if ((ConfigurationName == configuration.Configuration) && (PlatformName == configuration.Platform))
                return false;

            if (!ContextIsValid())
                return false;

            _context.ConfigurationName = configuration.Configuration + "|" + configuration.Platform;

            ConfigurationName = _context.ConfigurationName;
            PlatformName = _context.PlatformName;

            return true;
        }

        [CanBeNull]
        public string ProjectName { get; }

        public bool ShouldBuild
        {
            get => ContextIsValid() && _context.ShouldBuild;
            set
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                if (!ContextIsValid())
                    return;

                _context.ShouldBuild = value;
            }
        }

        [NotNull]
        public SolutionConfiguration SolutionConfiguration { get; }

        private bool ContextIsValid()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            // Check if the owning collection is valid - accessing other properties would throw an AccessViolationException!
            if (_context.Collection?.Count != 0)
                return true;

            // This context is no longer valid, schedule a solution update and return false...
            _solution.Update();
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
