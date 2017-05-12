using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Core.Presentation;
using System.Reflection;
using System.Activities.Presentation.Metadata;
using System.Resources;
using System.Activities;
using System.IO;
using System.Drawing;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.Activities.Presentation;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
//using Workflow.Activity;
namespace Workflow.CustomerDesigner
{
    public class MainViewModel2 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public MainViewModel2()
        {
            //这句话的意思是注册控件的外观，如if控件是两个分支，但是不注册这个外观，在设计框哪里看到的就是一个框，
            (new DesignerMetadata()).Register();
            //工具箱里加载样式，否则看到的都是齿轮
            LoadToolboxIconsForBuiltInActivities();
            this.NewCommand = new ReplayCommand(ExecuteNew, null);
            this.OpenCommand = new ReplayCommand(this.ExecuteOpen, null);
            this.SaveCommand = new ReplayCommand(this.ExecuteSave, null);
            this.ToolboxPanel = CreateToolbox();
            this.ExecuteNew(null);
        }
        public ICommand NewCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public object ToolboxPanel { get; private set; }
        public WorkflowDesigner workflowDesigner;
        public WorkflowDesigner WorkflowDesignerObj
        {
            get
            {
                return workflowDesigner;
            }
            private set
            {
                workflowDesigner = value;

                this.NotifyChanged("WorkflowDesignerPanel");
                this.NotifyChanged("WorkflowPropertyPanel");
            }
        }
        /// <summary>
        ///   Gets WorkflowDesignerPanel.
        /// </summary>
        public object WorkflowDesignerPanel
        {
            get
            {
                return this.WorkflowDesignerObj.View;
            }
        }

        /// <summary>
        ///   Gets WorkflowPropertyPanel.
        /// </summary>
        public object WorkflowPropertyPanel
        {
            get
            {
                return this.WorkflowDesignerObj.PropertyInspectorView;
            }
        }

        /// <summary>
        ///   Gets XAML.
        /// </summary>
        public string XAML
        {
            get
            {
                if (this.WorkflowDesignerObj.Text != null)
                {
                    this.WorkflowDesignerObj.Flush();
                    return this.WorkflowDesignerObj.Text;
                }

                return null;
            }
        }

        private const string TemplateXaml = "template.xaml";

        public void ExecuteNew(object parameter)
        {
            WorkflowDesignerObj = new WorkflowDesigner();

            WorkflowDesignerObj.ModelChanged += new EventHandler(workflowDesigner_ModelChanged);

            if (File.Exists(TemplateXaml))
            {
                WorkflowDesignerObj.Load(TemplateXaml);
            }
            else
            {
                WorkflowDesignerObj.Load(new Sequence());
            }

            WorkflowDesignerObj.Flush();
        }

        private void ExecuteOpen(object obj)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog(Application.Current.MainWindow).Value)
            {
                this.LoadWorkflow(openFileDialog.FileName);
            }
        }
        private void ExecuteSave(object obj)
        {
            var saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "xaml",

                Filter = "xaml files (*.xaml) | *.xaml;*.xamlx| All Files | *.*"
            };

            if (saveFileDialog.ShowDialog().Value)
            {

                this.workflowDesigner.Save(saveFileDialog.FileName);
            }
        }

        private void LoadWorkflow(string name)
        {
            //this.ResolveImportedAssemblies(name);
            this.WorkflowDesignerObj = new WorkflowDesigner();
            this.WorkflowDesignerObj.ModelChanged += this.WorkflowDesignerModelChanged;
            this.WorkflowDesignerObj.Load(name);
        }

        private void ResolveImportedAssemblies(string name)
        {
            var references = XamlClrReferences.Load(name);

            var query = from reference in references.References where !reference.Loaded select reference;
            foreach (var xamlClrRef in query)
            {
                this.Locate(xamlClrRef);
            }
        }
        private void Locate(XamlClrRef xamlClrRef)
        {
            //this.Status = string.Format("Locate referenced assembly {0}", xamlClrRef.CodeBase);
            var openFileDialog = new OpenFileDialog
            {
                FileName = xamlClrRef.CodeBase,
                CheckFileExists = true,
                Filter = "Assemblies (*.dll;*.exe)|*.dll;*.exe|All Files|*.*",
                //Title = this.Status
            };

            if (openFileDialog.ShowDialog(Application.Current.MainWindow).Value)
            {
                if (!xamlClrRef.Load(openFileDialog.FileName))
                {
                    MessageBox.Show("Error loading assembly");
                }
            }
        }
        private void WorkflowDesignerModelChanged(object sender, EventArgs e)
        {
            this.NotifyChanged("XAML");
        }

        //这个方法是为了加载控件库
        private static ToolboxControl CreateToolbox()
        {
            var toolboxControl = new ToolboxControl();

            toolboxControl.Categories.Add(
                new ToolboxCategory("控制流")
                    {
                        new ToolboxItemWrapper(typeof(DoWhile)), 
                        new ToolboxItemWrapper(typeof(ForEach<>)), 
                        new ToolboxItemWrapper(typeof(If)), 
                        new ToolboxItemWrapper(typeof(Parallel)), 
                        new ToolboxItemWrapper(typeof(ParallelForEach<>)), 
                        new ToolboxItemWrapper(typeof(Pick)), 
                        new ToolboxItemWrapper(typeof(PickBranch)), 
                        new ToolboxItemWrapper(typeof(Sequence)), 
                        new ToolboxItemWrapper(typeof(Switch<>)), 
                        new ToolboxItemWrapper(typeof(While)), 
                    });
            toolboxControl.Categories.Add(
                 new ToolboxCategory("流程图")
                    {
                        new ToolboxItemWrapper(typeof(Flowchart)), 
                        new ToolboxItemWrapper(typeof(FlowDecision)), 
                        new ToolboxItemWrapper(typeof(FlowSwitch<>)),                
                    });
            toolboxControl.Categories.Add(
                new ToolboxCategory("运行时")
                    {
                        new ToolboxItemWrapper(typeof(Persist)),               
                    });
            toolboxControl.Categories.Add(
                new ToolboxCategory("基元")
                    {
                        new ToolboxItemWrapper(typeof(Assign)), 
                        new ToolboxItemWrapper(typeof(Delay)), 
                        new ToolboxItemWrapper(typeof(InvokeMethod)), 
                        new ToolboxItemWrapper(typeof(WriteLine)), 
                    });
            toolboxControl.Categories.Add(
                new ToolboxCategory("事务")
                    {
                        new ToolboxItemWrapper(typeof(CancellationScope)), 
                        new ToolboxItemWrapper(typeof(CompensableActivity)), 
                        new ToolboxItemWrapper(typeof(Compensate)), 
                        new ToolboxItemWrapper(typeof(Confirm)), 
                        new ToolboxItemWrapper(typeof(TransactionScope)), 
                    });
            toolboxControl.Categories.Add(
                new ToolboxCategory("集合")
                    {
                        new ToolboxItemWrapper(typeof(AddToCollection<>)), 
                        new ToolboxItemWrapper(typeof(ClearCollection<>)), 
                        new ToolboxItemWrapper(typeof(ExistsInCollection<>)), 
                        new ToolboxItemWrapper(typeof(RemoveFromCollection<>)), 
                    });

            toolboxControl.Categories.Add(
                new ToolboxCategory("错误处理")
                    {
                        new ToolboxItemWrapper(typeof(Rethrow)), 
                        new ToolboxItemWrapper(typeof(Throw)), 
                        new ToolboxItemWrapper(typeof(TryCatch)), 
                    });
            //一下为自定义的工作量活动
            //     toolboxControl.Categories.Add(
            //         new ToolboxCategory("MyActivityLibrary") { new ToolboxItemWrapper(typeof(AgreeAuth)), });

            return toolboxControl;
        }

        //这个方法是为了加载左边控件库的图标
        private static void LoadToolboxIconsForBuiltInActivities()
        {
            try
            {
                var sourceAssembly = Assembly.LoadFrom(@"Lib\Microsoft.VisualStudio.Activities.dll");

                var builder = new AttributeTableBuilder();

                if (sourceAssembly != null)
                {
                    var stream =
                        sourceAssembly.GetManifestResourceStream(
                            "Microsoft.VisualStudio.Activities.Resources.resources");
                    if (stream != null)
                    {
                        var resourceReader = new ResourceReader(stream);

                        foreach (var type in
                            typeof(System.Activities.Activity).Assembly.GetTypes().Where(
                                t => t.Namespace == "System.Activities.Statements"))
                        {
                            CreateToolboxBitmapAttributeForActivity(builder, resourceReader, type);
                        }
                    }
                }

                MetadataStore.AddAttributeTable(builder.CreateTable());
            }
            catch (FileNotFoundException)
            {
                // Ignore - will use default icons
            }
        }

        private static void CreateToolboxBitmapAttributeForActivity(
          AttributeTableBuilder builder, ResourceReader resourceReader, Type builtInActivityType)
        {
            var bitmap = ExtractBitmapResource(
                resourceReader,
                builtInActivityType.IsGenericType ? builtInActivityType.Name.Split('`')[0] : builtInActivityType.Name);

            if (bitmap == null)
            {
                return;
            }

            var tbaType = typeof(ToolboxBitmapAttribute);

            var imageType = typeof(Image);

            var constructor = tbaType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { imageType, imageType }, null);

            var tba = constructor.Invoke(new object[] { bitmap, bitmap }) as ToolboxBitmapAttribute;

            builder.AddCustomAttributes(builtInActivityType, tba);
        }
        private static Bitmap ExtractBitmapResource(ResourceReader resourceReader, string bitmapName)
        {
            var dictEnum = resourceReader.GetEnumerator();

            Bitmap bitmap = null;

            while (dictEnum.MoveNext())
            {
                if (Equals(dictEnum.Key, bitmapName))
                {
                    bitmap = dictEnum.Value as Bitmap;

                    if (bitmap != null)
                    {
                        var pixel = bitmap.GetPixel(bitmap.Width - 1, 0);

                        bitmap.MakeTransparent(pixel);
                    }

                    break;
                }
            }

            return bitmap;
        }
        void workflowDesigner_ModelChanged(object sender, EventArgs e)
        {
            this.NotifyChanged("XAML");
        }
        internal void NotifyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
