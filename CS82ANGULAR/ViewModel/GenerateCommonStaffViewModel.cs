using CS82ANGULAR.Model.Serializable;
using CS82ANGULAR.TemplateProcessingHelpers;
using System.IO;
using EnvDTE80;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using CS82ANGULAR.Helpers;
using System;
using System.Threading.Tasks;

namespace CS82ANGULAR.ViewModel
{
    #pragma warning disable VSTHRD010
    public class GenerateCommonStaffViewModel : BaseGenerateViewModel
    {
        public GenerateCommonStaffViewModel() : base()
        {

        }



        public async Task DoGenerateViewModelAsync(DTE2 Dte, ITextTemplating textTemplating, string T4TempatePath, DbContextSerializable SerializableDbContext, ModelViewSerializable model, string defaultProjectNameSpace = null)
        {

            this.GenerateText = "";
            this.GenerateError = "";
            IsReady.DoNotify(this, false);
            if ((model == null) || (SerializableDbContext == null)) return;
            GeneratedModelView = model;
            try
            {
                await AngularJsonHelper.GetAngularJson().ReadPublicApiTsAndWebpackConfigJsAsync();
            }
            catch (Exception e)
            {
                this.GenerateError = e.Message;
                Exception innerExcpt = e.InnerException;
                while (innerExcpt != null)
                {
                    this.GenerateError += "\n" + innerExcpt;
                    innerExcpt = innerExcpt.InnerException;
                }
                IsReady.DoNotify(this, string.IsNullOrEmpty(this.GenerateError));
                return;
            }
            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)textTemplating;
            textTemplatingSessionHost.Session = textTemplatingSessionHost.CreateSession();
            TPCallback tpCallback = new TPCallback();
            textTemplatingSessionHost.Session["AngularJsonFile"] = AngularJsonHelper.GetAngularJson();
            textTemplatingSessionHost.Session["Model"] = GeneratedModelView;
            textTemplatingSessionHost.Session["Context"] = SerializableDbContext;
            textTemplatingSessionHost.Session["DefaultProjectNameSpace"] = string.IsNullOrEmpty(defaultProjectNameSpace) ? "" : defaultProjectNameSpace;

            if (string.IsNullOrEmpty(GenText))
            {
                this.GenerateText = textTemplating.ProcessTemplate(T4TempatePath, File.ReadAllText(T4TempatePath), tpCallback);
            }
            else
            {
                this.GenerateText = textTemplating.ProcessTemplate(T4TempatePath, GenText, tpCallback);
            }
            FileExtension = tpCallback.FileExtension;
            if (tpCallback.ProcessingErrors != null)
            {
                foreach (TPError tpError in tpCallback.ProcessingErrors)
                {
                    this.GenerateError += tpError.ToString() + "\n";
                }
            }
            if (string.IsNullOrEmpty(this.GenerateError))
            {
                if (string.Compare(this.FileExtension, ".jsonefm2txt", true) == 0)
                {
                    this.FileExtension = ".txt";
                    this.GenerateText = await ExportFileModifier.ExecuteJsonScriptEFMAsync(AngularJsonHelper.GetAngularJson(), model, this.GenerateText);
                }
            }

            IsReady.DoNotify(this, string.IsNullOrEmpty(this.GenerateError));
        }
        public async Task DoGenerateFeatureAsync(DTE2 Dte, ITextTemplating textTemplating, string T4TempatePath, DbContextSerializable SerializableDbContext, FeatureContextSerializable SerializableFeatureContext, FeatureSerializable feature, AllowedFileTypesSerializable AllowedFileTypes, string defaultProjectNameSpace = null)
        {

            this.GenerateText = "";
            this.GenerateError = "";
            IsReady.DoNotify(this, false);
            if ((feature == null) || (SerializableDbContext == null) || (SerializableFeatureContext == null)) return;
            GeneratedFeature = feature;
            try
            {
                await AngularJsonHelper.GetAngularJson().ReadPublicApiTsAndWebpackConfigJsAsync();
            }
            catch (Exception e)
            {
                this.GenerateError = e.Message;
                Exception innerExcpt = e.InnerException;
                while (innerExcpt != null)
                {
                    this.GenerateError += "\n" + innerExcpt;
                    innerExcpt = innerExcpt.InnerException;
                }
                IsReady.DoNotify(this, string.IsNullOrEmpty(this.GenerateError));
                return;
            }

            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)textTemplating;
            textTemplatingSessionHost.Session = textTemplatingSessionHost.CreateSession();
            TPCallback tpCallback = new TPCallback();
            textTemplatingSessionHost.Session["AngularJsonFile"] = AngularJsonHelper.GetAngularJson();
            textTemplatingSessionHost.Session["AllowedFileTypes"] = AllowedFileTypes;
            textTemplatingSessionHost.Session["Feature"] = GeneratedFeature;
            textTemplatingSessionHost.Session["FeatureContext"] = SerializableFeatureContext;
            textTemplatingSessionHost.Session["Context"] = SerializableDbContext;
            textTemplatingSessionHost.Session["DefaultProjectNameSpace"] = string.IsNullOrEmpty(defaultProjectNameSpace) ? "" : defaultProjectNameSpace;

            if (string.IsNullOrEmpty(GenText))
            {
                this.GenerateText = textTemplating.ProcessTemplate(T4TempatePath, File.ReadAllText(T4TempatePath), tpCallback);
            }
            else
            {
                this.GenerateText = textTemplating.ProcessTemplate(T4TempatePath, GenText, tpCallback);
            }
            FileExtension = tpCallback.FileExtension;
            if (tpCallback.ProcessingErrors != null)
            {
                foreach (TPError tpError in tpCallback.ProcessingErrors)
                {
                    this.GenerateError += tpError.ToString() + "\n";
                }
            }
            if (string.IsNullOrEmpty(this.GenerateError))
            {
                if (string.Compare(this.FileExtension, ".jsonefm2txt", true) == 0)
                {
                    this.FileExtension = ".txt";
                    this.GenerateText = await ExportFileModifier.ExecuteJsonScriptFeatureEFMAsync(AngularJsonHelper.GetAngularJson(), GeneratedFeature, this.GenerateText);
                    // throw new Exception("Not implemented yet");
                }
            }
            IsReady.DoNotify(this, string.IsNullOrEmpty(this.GenerateError));
        }

    }
}
