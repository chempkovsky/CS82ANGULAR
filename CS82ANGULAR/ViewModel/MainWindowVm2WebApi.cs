using CS82ANGULAR.Helpers;
using CS82ANGULAR.Model;
using CS82ANGULAR.Model.Serializable;
using CS82ANGULAR.View;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;

namespace CS82ANGULAR.ViewModel
{
    #pragma warning disable VSTHRD010
    public class MainWindowVm2WebApi : MainWindowBase
    {
        #region Fieds
        UserControlGenerate GenerateUC = null;
        UserControlT4Editor T4EditorUC = null;
        UserControlSelectSource SelectDbContextUC = null;
        UserControlCreateWebApi CreateWebApiUC = null;
        GenTypeItem gti = null;
        #endregion

        public MainWindowVm2WebApi(DTE2 dte, ITextTemplating textTemplating, IVsThreadedWaitDialogFactory dialogFactory) : base(dte, textTemplating, dialogFactory)
        {
            GenTypeItems.Add(new GenTypeItem() { GtDisplayName = "1 IRepository(Interface)", GtType = "IRepo", GtItmPath = "WebApiRepoInterfaceTmplst", GtLstPath = "" });
            GenTypeItems.Add(new GenTypeItem() { GtDisplayName = "2 Repository(Class)", GtType = "Repo", GtItmPath = "WebApiRepoClassTmplst", GtLstPath = "" });
            GenTypeItems.Add(new GenTypeItem() { GtDisplayName = "3 Manager(Class)", GtType = "Manager", GtItmPath = "WebApiManagerClassTmplst", GtLstPath = "" });
            GenTypeItems.Add(new GenTypeItem() { GtDisplayName = "4 IAppService(Interface)", GtType = "IService", GtItmPath = "WebApiAppServiceInterfaceTmplst", GtLstPath = "" });
            GenTypeItems.Add(new GenTypeItem() { GtDisplayName = "5 AppService(Class)", GtType = "Service", GtItmPath = "WebApiAppServiceClassTmplst", GtLstPath = "" });
            GenTypeItems.Add(new GenTypeItem() { GtDisplayName = "6 WebApi(Class)", GtType = "WebApi", GtItmPath = "WebApiControllerTmplst", GtLstPath = "" });
            GenTypeComboSelectedItem = GenTypeItems[5];
            gti = GenTypeItems[5];

            InvitationViewModel InvitationVM = new InvitationViewModel();
            InvitationVM.WizardName = "#3 WebApi Wizard";
            InvitationVM.IsReady.IsReadyEvent += InvitationViewModel_IsReady;
            this.InvitationUC = new UserControlInvitation(InvitationVM);
            this.CurrentUserControl = this.InvitationUC;
            InvitationVM.DoAnalise(dte);
        }

        #region PrevBtnCommand
        public override void PrevBtnCommandAction(Object param)
        {
            switch (CurrentUiStepId)
            {
                case 1:
                    CurrentUiStepId = 0;
                    PrevBtnEnabled = false;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    GenTypeComboVisibility = Visibility.Collapsed;
                    GenTypeComboEnabled = false;

                    this.CurrentUserControl = InvitationUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 2:
                    CurrentUiStepId = 1;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    GenTypeComboVisibility = Visibility.Collapsed;
                    GenTypeComboEnabled = false;
                    (SelectDbContextUC.DataContext as SelectDbContextViewModel).CheckIsReady();
                    this.CurrentUserControl = SelectDbContextUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 3:
                    CurrentUiStepId = 2;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    GenTypeComboVisibility = Visibility.Visible;
                    GenTypeComboEnabled = true;
                    (CreateWebApiUC.DataContext as CreateWebApiViewModel).CheckIsReady();
                    this.CurrentUserControl = CreateWebApiUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 4:
                    CurrentUiStepId = 3;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    SaveBtnEnabled = false;
                    GenTypeComboVisibility = Visibility.Visible;
                    GenTypeComboEnabled = false;
                    (T4EditorUC.DataContext as T4EditorViewModel).CheckIsReady();
                    this.CurrentUserControl = T4EditorUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region NextBtnCommand
        [SuppressMessage("", "VSTHRD100")]
        public override async void NextBtnCommandAction(Object param)
        {
            switch (CurrentUiStepId)
            {
                case 0:
                    CurrentUiStepId = 1;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    GenTypeComboVisibility = Visibility.Collapsed;
                    GenTypeComboEnabled = false;
                    if (SelectDbContextUC == null)
                    {
                        SelectDbContextViewModel dataContext = new SelectDbContextViewModel(Dte);
                        dataContext.UiCommandButtonVisibility = Visibility.Collapsed;
                        dataContext.UiCommandCaption3 = "NameSpace: " + (InvitationUC.DataContext as InvitationViewModel).DefaultProjectNameSpace;
                        string folder = (InvitationUC.DataContext as InvitationViewModel).DestinationFolder;
                        if (!string.IsNullOrEmpty(folder))
                        {
                            dataContext.UiCommandCaption3 = dataContext.UiCommandCaption3 + "." + folder.Replace("\\", ".");
                        }
                        SelectDbContextUC = new UserControlSelectSource(dataContext);
                        dataContext.IsReady.IsReadyEvent += CallBack_IsReady;
                    }
                    (SelectDbContextUC.DataContext as SelectDbContextViewModel).DoAnaliseDbContext();
                    this.CurrentUserControl = SelectDbContextUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 1:
                    CurrentUiStepId = 2;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    GenTypeComboVisibility = Visibility.Visible;
                    GenTypeComboEnabled = true;
                    if (CreateWebApiUC == null)
                    {
                        CreateWebApiViewModel dataContext = new CreateWebApiViewModel(Dte);
                        dataContext.IsReady.IsReadyEvent += CallBack_IsReady;
                        dataContext.DestinationProject = (InvitationUC.DataContext as InvitationViewModel).DestinationProject;
                        dataContext.DefaultProjectNameSpace = (InvitationUC.DataContext as InvitationViewModel).DefaultProjectNameSpace;
                        dataContext.DestinationFolder = (InvitationUC.DataContext as InvitationViewModel).DestinationFolder;
                        CreateWebApiUC = new UserControlCreateWebApi(dataContext);
                    }
                    (CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedDbContext =
                        (SelectDbContextUC.DataContext as SelectDbContextViewModel).SelectedCodeElement;
                    (CreateWebApiUC.DataContext as CreateWebApiViewModel).CheckIsReady();
                    this.CurrentUserControl = CreateWebApiUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 2:
                    CurrentUiStepId = 3;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    GenTypeComboVisibility = Visibility.Visible;
                    GenTypeComboEnabled = false;
                    if (T4EditorUC == null)
                    {
                        //string templatePath = Path.Combine("Templates", "ViewModel.cs.t4");
                        string TemplatesFld = TemplatePathHelper.GetTemplatePath();
                        string templatePath = Path.Combine(TemplatesFld, "WebApiControllerTmplst");
                        T4EditorViewModel dataContext = new T4EditorViewModel(templatePath);
                        dataContext.IsReady.IsReadyEvent += CallBack_IsReady;
                        T4EditorUC = new UserControlT4Editor(dataContext);
                    }
                    gti = (GenTypeComboSelectedItem as GenTypeItem);
                    (T4EditorUC.DataContext as T4EditorViewModel).T4TemplateFolder = Path.Combine(TemplatePathHelper.GetTemplatePath(), gti.GtItmPath);
                    (T4EditorUC.DataContext as T4EditorViewModel).CheckIsReady();
                    this.CurrentUserControl = T4EditorUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                case 3:
                    CurrentUiStepId = 4;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = true;
                    GenTypeComboVisibility = Visibility.Visible;
                    GenTypeComboEnabled = false;
                    (T4EditorUC.DataContext as T4EditorViewModel).T4TemplateFolder = Path.Combine(TemplatePathHelper.GetTemplatePath(), gti.GtItmPath);
                    IVsThreadedWaitDialog2 aDialog = null;
                    bool aDialogStarted = false;
                    if (this.DialogFactory != null)
                    {
                        this.DialogFactory.CreateInstance(out aDialog);
                        if (aDialog != null)
                        {
                            aDialogStarted = aDialog.StartWaitDialog("Generation started", "VS is Busy", "Please wait", null, "Generation started", 0, false, true) == VSConstants.S_OK;
                        }
                    }
                    if (GenerateUC == null)
                    {
                        GenerateCommonStaffViewModel dataContext = new GenerateCommonStaffViewModel();
                        dataContext.IsReady.IsReadyEvent += GenerateWebApiViewModel_IsReady;
                        GenerateUC = new UserControlGenerate(dataContext);
                    }

                    (GenerateUC.DataContext as GenerateCommonStaffViewModel).GenText = (T4EditorUC.DataContext as T4EditorViewModel).T4TempateText;

                    try
                    {
                        await (GenerateUC.DataContext as GenerateCommonStaffViewModel)
                            .DoGenerateViewModelAsync(Dte, TextTemplating,
                            (T4EditorUC.DataContext as T4EditorViewModel).T4TempatePath,
                            (CreateWebApiUC.DataContext as CreateWebApiViewModel).SerializableDbContext,
                            (CreateWebApiUC.DataContext as CreateWebApiViewModel).GetSelectedModelShallowCopy());
                        if (aDialogStarted)
                        {
                            int iOut;
                            aDialog.EndWaitDialog(out iOut);
                        }
                    }
                    catch (Exception e)
                    {
                        if (aDialogStarted)
                        {
                            int iOut;
                            aDialog.EndWaitDialog(out iOut);
                        }
                        MessageBox.Show("Error: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        this.CurrentUserControl = GenerateUC;
                        this.OnPropertyChanged("CurrentUserControl");
                    }
                    break;
                case 4:
                    CurrentUiStepId = 2;
                    PrevBtnEnabled = true;
                    NextBtnEnabled = false;
                    SaveBtnEnabled = false;
                    GenTypeComboVisibility = Visibility.Visible;
                    GenTypeComboEnabled = true;
                    (CreateWebApiUC.DataContext as CreateWebApiViewModel).CheckIsReady();
                    this.CurrentUserControl = CreateWebApiUC;
                    this.OnPropertyChanged("CurrentUserControl");
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region SaveBtnCommand
        public override void SaveBtnCommandAction(Object param)
        {
            DbContextSerializable localDbContext = (CreateWebApiUC.DataContext as CreateWebApiViewModel).SerializableDbContext;
            ModelViewSerializable modelViewSerializable = (GenerateUC.DataContext as GenerateCommonStaffViewModel).GeneratedModelView;
            ModelViewSerializable existedModelViewSerializable =
                localDbContext.ModelViews.FirstOrDefault(mv => mv.ViewName == modelViewSerializable.ViewName);

            GenTypeItem si = (this.GenTypeComboSelectedItem as GenTypeItem);
            string GtType = "WebApi";
            if (si != null)
            {
                GtType = si.GtType;
            }
            if(modelViewSerializable.GeneratedServices == null)
            {
                modelViewSerializable.GeneratedServices = new List<GeneratedServiceSerializable>();
            }
            GeneratedServiceSerializable gss = modelViewSerializable.GeneratedServices.FirstOrDefault(d => d.SrvType == GtType);
            if (gss == null)
            {
                gss = new GeneratedServiceSerializable();
                modelViewSerializable.GeneratedServices.Add(gss);
            }
            gss.SrvDefaultProjectNameSpace = modelViewSerializable.WebApiServiceDefaultProjectNameSpace;
            gss.SrvFolder = modelViewSerializable.WebApiServiceFolder;
            gss.SrvType = GtType;
            switch(GtType)
            {
                case "IRepo":
                    gss.SrvClassName = "I" + modelViewSerializable.ViewName + "Repo";
                    break;
                case "Repo":
                    gss.SrvClassName = modelViewSerializable.ViewName + "Repo";
                    break;
                case "Manager":
                    gss.SrvClassName = modelViewSerializable.ViewName + "Manager";
                    break;
                case "IService":
                    gss.SrvClassName = "I" + modelViewSerializable.ViewName + "Service";
                    break;
                case "Service":
                    gss.SrvClassName = modelViewSerializable.ViewName + "Service";
                    break;
                default:
                    gss.SrvClassName = modelViewSerializable.WebApiServiceName;
                    break;
            }


            if (existedModelViewSerializable != null)
            {
                existedModelViewSerializable.ScalarProperties = modelViewSerializable.ScalarProperties;
                existedModelViewSerializable.WebApiRoutePrefix = modelViewSerializable.WebApiRoutePrefix;
                existedModelViewSerializable.WebApiServiceName = modelViewSerializable.WebApiServiceName;
                existedModelViewSerializable.IsWebApiSelectAll = modelViewSerializable.IsWebApiSelectAll;
                existedModelViewSerializable.IsWebApiSelectManyWithPagination = modelViewSerializable.IsWebApiSelectManyWithPagination;
                existedModelViewSerializable.IsWebApiSelectOneByPrimarykey = modelViewSerializable.IsWebApiSelectOneByPrimarykey;
                existedModelViewSerializable.IsWebApiAdd = modelViewSerializable.IsWebApiAdd;
                existedModelViewSerializable.IsWebApiUpdate = modelViewSerializable.IsWebApiUpdate;
                existedModelViewSerializable.IsWebApiDelete = modelViewSerializable.IsWebApiDelete;
                existedModelViewSerializable.IsStandalone = modelViewSerializable.IsStandalone;

                existedModelViewSerializable.WebApiServiceProject = modelViewSerializable.WebApiServiceProject;
                existedModelViewSerializable.WebApiServiceDefaultProjectNameSpace = modelViewSerializable.WebApiServiceDefaultProjectNameSpace;
                existedModelViewSerializable.WebApiServiceFolder = modelViewSerializable.WebApiServiceFolder;

                existedModelViewSerializable.UIFormProperties = modelViewSerializable.UIFormProperties;
                existedModelViewSerializable.UIListProperties = modelViewSerializable.UIListProperties;
                existedModelViewSerializable.UniqueKeys = modelViewSerializable.UniqueKeys;
                existedModelViewSerializable.GeneratedServices = modelViewSerializable.GeneratedServices;

            }
            else
            {
                localDbContext.ModelViews.Add(modelViewSerializable);
            }

            string projectName = "";
            if ((CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedDbContext.CodeElementRef != null)
            {
                if ((CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedDbContext.CodeElementRef.ProjectItem != null)
                {
                    projectName =
                        (CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedDbContext.CodeElementRef.ProjectItem.ContainingProject.UniqueName;
                }
            }
            if (!string.IsNullOrEmpty(projectName))
            {
                string locFileName = Path.Combine(projectName, (CreateWebApiUC.DataContext as CreateWebApiViewModel).SelectedDbContext.CodeElementFullName, JsonExtension);
                locFileName = locFileName.Replace("\\", ".");
                SolutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                locFileName = Path.Combine(SolutionDirectory, locFileName);
                string jsonString = JsonConvert.SerializeObject(localDbContext);
                File.WriteAllText(locFileName, jsonString);
            }
            {
                string FlNm = "";
                try
                {
                    SolutionDirectory = System.IO.Path.GetDirectoryName(Dte.Solution.FullName);
                    FlNm = Path.Combine(
                        SolutionDirectory,
                        Path.GetDirectoryName(modelViewSerializable.WebApiServiceProject),
                        modelViewSerializable.WebApiServiceFolder,
                        //modelViewSerializable.WebApiServiceName
                        gss.SrvClassName
                        + (GenerateUC.DataContext as GenerateCommonStaffViewModel).FileExtension);
                    File.WriteAllText(FlNm, (GenerateUC.DataContext as GenerateCommonStaffViewModel).GenerateText);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: Error: Could not save generated file. This type of exception can be thrown when EXISTING project added to the solution and developer tries to update VS project repository.  Original Error: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    DestinationProject.ProjectItems.AddFromFile(FlNm);
                    MessageBox.Show(SuccessNotification, "Done", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: Could not update VS Solution repository file. This type of exception can be thrown when EXISTING project added to the solution and developer tries to update VS project repository.  Original Error: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
        private void CallBack_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            OnPropertyChanged("NextBtnEnabled");
        }
        private void GenerateWebApiViewModel_IsReady(Object sender, bool ready)
        {
            NextBtnEnabled = ready;
            SaveBtnEnabled = ready;
            OnPropertyChanged("SaveBtnEnabled");
        }
    }
}
