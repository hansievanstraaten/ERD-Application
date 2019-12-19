using System;
using WPF.Tools.Attributes;
using GeneralExtensions;

namespace ERD.Viewer.Models.BuildModels.EntityFrameworkModels
{
  [ModelName("Entity Framework Setup")]
  public class EntityFrameworkSetup : ModelBase
  {
    //private bool seperateMappingOutput;
    //private bool overideExistingFiles;
    private bool buildContextForeachCanvas;
    private bool useFriendlyNames;
    private string serverOutputBaseDirectory;
    private string clientModelOutputDirectory;
    private string dataContextUsings;
    private string dataContextNamspace;
    private string dataContextConstructor;
    private string mappingClassUsing;
    private string mappingClassNamespace;
    private string modelClassBaseUsing;
    private string modelClassNamespace;
    private string modelClassBaseString;
    private string modelClassString;
    private string modelPropertyString;
    private string repositoryUsing;
    private string repositoryNamespace;
    //private string repositoryClassString;
    //private string repositoryConstructorString;

    private string dataContextConstructorDefault = "public {0}(string connectionString) : base(connectionString)";
    private string modelClassBaseStringDefault = "public abstract class {0}_Base";
    private string modelClassStringDefault = "public class {0} : {0}_Base";
    //private string repositoryClassStringDefault = "public class {0} : I{0}";
    //private string repositoryConstructorStringDefault = "public {0}()";
    private string modelPropertyStringDefault = "        /// <summary>" + Environment.NewLine +
                                                "        /// {0}" + Environment.NewLine +
                                                "        /// </summary>" + Environment.NewLine +
                                                "        public {1} {2} { get; set; }";

    [FieldInformation("Server Base Directory", IsRequired = true)]
    [BrowseButtonAttribute("ServerOutputFolder", "...", "SearchFolder")]
    public string ServerOutputBaseDirectory
    {
      get
      {
        return this.serverOutputBaseDirectory;
      }

      set
      {
        base.OnPropertyChanged("ServerOutputBaseDirectory", ref this.serverOutputBaseDirectory, value);
      }
    }

    [FieldInformation("Client Model Directory")]
    [BrowseButtonAttribute("ClientOutputFolder", "...", "SearchFolder")]
    public string ClientModelOutputDirectory
    {
      get
      {
        return this.clientModelOutputDirectory;
      }

      set
      {
        base.OnPropertyChanged("ClientModelOutputDirectory", ref this.clientModelOutputDirectory, value);
      }
    }

    //[FieldInformation("Separate Mapping Output Folders")]
    //public bool SeperateMappingOutput
    //{
    //  get
    //  {
    //    return this.seperateMappingOutput;
    //  }

    //  set
    //  {
    //    base.OnPropertyChanged("SeperateTableOutput", ref this.seperateMappingOutput, value);
    //  }
    //}


    [FieldInformation("Build Context file for each Canvas")]
    public bool BuildContextForeachCanvas
    {
      get
      {
        return this.buildContextForeachCanvas;
      }

      set
      {
        base.OnPropertyChanged("BuildContextForeachCanvas", ref this.buildContextForeachCanvas, value);
      }
    }

    [FieldInformation("Use Friendly Names")]
    public bool UseFriendlyNames
    {
      get
      {
        return this.useFriendlyNames;
      }

      set
      {
        base.OnPropertyChanged("UseFriendlyNames", ref this.useFriendlyNames, value);
      }
    }

    [FieldInformation("Data Context Usings", DisableSpellChecker = true, IsRequired = true)]
    [BrowseButtonAttribute("DataContextUsings", "...", "")]
    public string DataContextUsing
    {
      get
      {
        return this.dataContextUsings;
      }

      set
      {
        base.OnPropertyChanged("DataContextUsing", ref this.dataContextUsings, value);
      }
    }

    [FieldInformation("Data Context Namespace", DisableSpellChecker = true, IsRequired = true)]
    public string DataContextNamespace
    {
      get
      {
        return this.dataContextNamspace;
      }

      set
      {
        base.OnPropertyChanged("DataContextNamespace", ref this.dataContextNamspace, value);
      }
    }

    [FieldInformation("Data Context Constructor", DisableSpellChecker = true, IsRequired = true)]
    public string DataContextConstructor
    {
      get
      {
        if (this.dataContextConstructor.IsNullEmptyOrWhiteSpace())
        {
          return this.dataContextConstructorDefault;
        }

        return this.dataContextConstructor;
      }

      set
      {
        base.OnPropertyChanged("DataContextConstructor", ref this.dataContextConstructor, value);
      }
    }

    [FieldInformation("Mapping Class Using's", DisableSpellChecker = true)]
    [BrowseButtonAttribute("MappingClassUsing", "...", "")]
    public string MappingClassUsing
    {
      get
      {
        return this.mappingClassUsing;
      }

      set
      {
        base.OnPropertyChanged("MappingClassUsing", ref this.mappingClassUsing, value);
      }
    }

    [FieldInformation("Mapping Class Namespace", DisableSpellChecker = true)]
    public string MappingClassNamespace
    {
      get
      {
        return this.mappingClassNamespace;
      }

      set
      {
        base.OnPropertyChanged("MappingClassNamespace", ref this.mappingClassNamespace, value);
      }
    }
    
    [FieldInformation("Class Model Base Using's", DisableSpellChecker = true)]
    [BrowseButtonAttribute("ModelClassBaseUsing", "...", "")]
    public string ModelClassBaseUsing
    {
      get
      {
        return this.modelClassBaseUsing;
      }

      set
      {
        base.OnPropertyChanged("ModelClassBaseUsing", ref this.modelClassBaseUsing, value);
      }
    }

    [FieldInformation("Class Model Namespace", DisableSpellChecker = true)]
    public string ModelClassNamespace
    {
      get
      {
        return this.modelClassNamespace;
      }

      set
      {
        base.OnPropertyChanged("ModelClassNamespace", ref this.modelClassNamespace, value);
      }
    }

    [FieldInformation("Class Model Base Format", DisableSpellChecker = true, IsRequired = true)]
    [BrowseButtonAttribute("ModelClassBaseString", "...", "")]
    public string ModelClassBaseString
    {
      get
      {
        if (this.modelClassBaseString.IsNullEmptyOrWhiteSpace())
        {
          return this.modelClassBaseStringDefault;
        }

        return this.modelClassBaseString;
      }

      set
      {
        base.OnPropertyChanged("ModelClassString", ref this.modelClassBaseString, value);
      }
    }

    /// <summary>
    /// Default: public class {Table Name}
    /// </summary>
    [FieldInformation("Class Model Format", DisableSpellChecker = true, IsRequired = true)]
    [BrowseButtonAttribute("ModelClassString", "...", "")]
    public string ModelClassString
    {
      get
      {
        if (this.modelClassString.IsNullEmptyOrWhiteSpace())
        {
          return this.modelClassStringDefault;
        }

        return this.modelClassString;
      }

      set
      {
        base.OnPropertyChanged("ModelClassString", ref this.modelClassString, value);
      }
    }

    /// <summary>
    /// Default: public {DataType} {Column Name} { get; set; }
    /// </summary>
    [FieldInformation("Model Property Format", DisableSpellChecker = true, IsRequired = true)]
    [BrowseButtonAttribute("ModelPropertyString", "...", "")]
    public string ModelPropertyString
    {
      get
      {
        if (this.modelPropertyString.IsNullEmptyOrWhiteSpace())
        {
          return this.modelPropertyStringDefault;
        }

        return this.modelPropertyString;
      }

      set
      {
        base.OnPropertyChanged("ModelPropertyString", ref this.modelPropertyString, value);
      }
    }

    [FieldInformation("Repository Using's", DisableSpellChecker = true, IsRequired = true)]
    [BrowseButtonAttribute("RepositoryUsing", "...", "")]
    public string RepositoryUsing
    {
      get
      {
        return this.repositoryUsing;
      }

      set
      {
        base.OnPropertyChanged("RepositoryUsing", ref this.repositoryUsing, value);
      }
    }

    [FieldInformation("Repository Namespace", DisableSpellChecker = true, IsRequired = true)]
    public string RepositoryNamespace
    {
      get
      {
        return this.repositoryNamespace;
      }

      set
      {
        base.OnPropertyChanged("RepositoryNamespace", ref this.repositoryNamespace, value);
      }
    }

    ///// <summary>
    ///// Default: public class {Table Name} : I{Table Name};
    ///// </summary>
    //[FieldInformation("Repository Class Format", DisableSpellChecker = true, IsRequired = true)]
    //[BrowseButtonAttribute("RepositoryClassString", "...", "")]
    //public string RepositoryClassString
    //{
    //  get
    //  {
    //    if (this.repositoryClassString.IsNullEmptyOrWhiteSpace())
    //    {
    //      return this.repositoryClassStringDefault;
    //    }

    //    return this.repositoryClassString;
    //  }

    //  set
    //  {
    //    base.OnPropertyChanged("RepositoryClassString", ref this.repositoryClassString, value);
    //  }
    //}

    ///// <summary>
    ///// Default: public {Table Name}()
    ///// </summary>
    //[FieldInformation("Repository Constructor Format", DisableSpellChecker = true, IsRequired = true)]
    //[BrowseButtonAttribute("RepositoryConstructorString", "...", "")]
    //public string RepositoryConstructorString
    //{
    //  get
    //  {
    //    if (this.repositoryConstructorString.IsNullEmptyOrWhiteSpace())
    //    {
    //      return this.repositoryConstructorStringDefault;
    //    }

    //    return this.repositoryConstructorString;
    //  }

    //  set
    //  {
    //    base.OnPropertyChanged("RepositoryConstructorString", ref this.repositoryConstructorString, value);
    //  }
    //}
  }
}


//private string repositoryUsing;
//private string repositoryNamespace;