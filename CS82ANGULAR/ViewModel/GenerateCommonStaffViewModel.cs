using CS82ANGULAR.Model.Serializable;
using CS82ANGULAR.TemplateProcessingHelpers;
using System.IO;
using EnvDTE80;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using CS82ANGULAR.Helpers;

namespace CS82ANGULAR.ViewModel
{
    #pragma warning disable VSTHRD010
    public class GenerateCommonStaffViewModel : BaseGenerateViewModel
    {
        public GenerateCommonStaffViewModel() : base()
        {

        }



        public void DoGenerateViewModel(DTE2 Dte, ITextTemplating textTemplating, string T4TempatePath, DbContextSerializable SerializableDbContext, ModelViewSerializable model, string defaultProjectNameSpace = null)
        {

            this.GenerateText = "";
            this.GenerateError = "";
            IsReady.DoNotify(this, false);
            if ((model == null) || (SerializableDbContext == null)) return;
            GeneratedModelView = model;

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
            IsReady.DoNotify(this, string.IsNullOrEmpty(this.GenerateError));
        }
        public void DoGenerateFeature(DTE2 Dte, ITextTemplating textTemplating, string T4TempatePath, DbContextSerializable SerializableDbContext, FeatureContextSerializable SerializableFeatureContext, FeatureSerializable feature, AllowedFileTypesSerializable AllowedFileTypes, string defaultProjectNameSpace = null)
        {

            this.GenerateText = "";
            this.GenerateError = "";
            IsReady.DoNotify(this, false);
            if ((feature == null) || (SerializableDbContext == null) || (SerializableFeatureContext == null)) return;
            GeneratedFeature = feature;

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
            IsReady.DoNotify(this, string.IsNullOrEmpty(this.GenerateError));
        }

    }
}
