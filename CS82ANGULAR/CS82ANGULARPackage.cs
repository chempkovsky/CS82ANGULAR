﻿using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace CS82ANGULAR
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(CS82ANGULARPackage.PackageGuidString)]
    [ProvideMenuResource("Menus1.ctmenu", 1)]
    public sealed class CS82ANGULARPackage : AsyncPackage
    {
        /// <summary>
        /// CS82ANGULARPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "5b1ef8e4-69a5-4cf9-856c-faf7ce76f34e";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);


            ITextTemplating textTemplating = null;
            EnvDTE80.DTE2 dte2 = null;
            IVsThreadedWaitDialogFactory dialogFactory = null;

            dte2 = await GetServiceAsync(typeof(SDTE)) as EnvDTE80.DTE2;
            textTemplating = await GetServiceAsync(typeof(STextTemplating)) as ITextTemplating;
            dialogFactory = await GetServiceAsync(typeof(SVsThreadedWaitDialogFactory)) as IVsThreadedWaitDialogFactory;


            await CS82ANGULAR.Commands.CrtDbContextCommand.InitializeAsync(this, dte2, textTemplating, dialogFactory);
            await CS82ANGULAR.Commands.CrtFeatureScriptsCommand.InitializeAsync(this, dte2, textTemplating, dialogFactory);
            await CS82ANGULAR.Commands.CrtJavaScriptsCommand.InitializeAsync(this, dte2, textTemplating, dialogFactory);
            await CS82ANGULAR.Commands.CrtViewModelCommand.InitializeAsync(this, dte2, textTemplating, dialogFactory);
            await CS82ANGULAR.Commands.CrtWebApiServiceCommand.InitializeAsync(this, dte2, textTemplating, dialogFactory);
        }

        #endregion
    }
}
