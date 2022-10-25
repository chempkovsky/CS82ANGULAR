using CS82ANGULAR.Helpers;
using CS82ANGULAR.Model;
using CS82ANGULAR.Model.Serializable;
using CS82ANGULAR.TemplateProcessingHelpers;
using EnvDTE;
using EnvDTE80;
using System.IO;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Threading.Tasks;

namespace CS82ANGULAR.ViewModel
{
    #pragma warning disable VSTHRD010
    public class GenerateViewModel : BaseGenerateViewModel
    {
        public GenerateViewModel() : base()
        {

        }
        public void DoGenerateViewModel(DTE2 Dte, ITextTemplating textTemplating, SelectedItem DestinationSelectedItem, string T4TempatePath, ModelView modelView)
        {
            this.GenerateText = "";
            this.GenerateError = "";


            GeneratedModelView = new ModelViewSerializable();
            modelView.ModelViewAssingTo(GeneratedModelView);
            //try
            //{
            //    await AngularJsonHelper.GetAngularJson().ReadPublicApiTsAndWebpackConfigJsAsync();
            //}
            //catch (Exception e)
            //{
            //    this.GenerateError = e.Message;
            //    IsReady.DoNotify(this, string.IsNullOrEmpty(this.GenerateError));
            //    return;
            //}
            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)textTemplating;
            textTemplatingSessionHost.Session = textTemplatingSessionHost.CreateSession();
            TPCallback tpCallback = new TPCallback();
//            textTemplatingSessionHost.Session["AngularJsonFile"] = AngularJsonHelper.GetAngularJson();
            textTemplatingSessionHost.Session["Model"] = GeneratedModelView;
            textTemplatingSessionHost.Session["AngularJsonFile"] = AngularJsonHelper.GetAngularJson();
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
                    this.GenerateError = tpError.ToString() + "\n";
                }
            }
            IsReady.DoNotify(this, string.IsNullOrEmpty(this.GenerateError));
        }
    }
}
