using System;
using WPF.Tools.ModelViewer;

namespace WPF.Tools.Attributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class FieldInformationAttribute : Attribute
  {
    private int sortOrder = 0;

    private int maxTextLength = 0;

    private bool isReadOnly = false;

    private bool visible = true;

    private bool isRequired = false;

    private bool disableSpellCheck = false;

    private string caption;

    public FieldInformationAttribute(
      string fieldCaption,
      bool defaultVisible = true,
      int sort = 0,
      int maxLength = 0,
      bool readOnly = false,
      bool isRequiredField = false,
      bool disableSpellChecker = false)
    {
      this.caption = fieldCaption;

      this.visible = defaultVisible;

      this.sortOrder = sort;

      this.maxTextLength = maxLength;

      this.isReadOnly = readOnly;

      this.isRequired = isRequiredField;

      this.disableSpellCheck = disableSpellChecker;
    }

    public int Sort
    {
      get
      {
        return this.sortOrder;
      }

      set
      {
        this.sortOrder = value;
      }
    }

    public int MaxTextLength
    {
      get
      {
        return this.maxTextLength;
      }

      set
      {
        this.maxTextLength = value;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return this.isReadOnly;
      }

      set
      {
        this.isReadOnly = value;
      }
    }

    public bool IsVisible
    {
      get
      {
        return this.visible;
      }

      set
      {
        this.visible = value;
      }
    }

    public bool IsRequired
    {
      get
      {
        return this.isRequired;
      }

      set
      {
        this.isRequired = value;
      }
    }

    public bool DisableSpellChecker
    {
      get
      {
        return this.disableSpellCheck;
      }

      set
      {
        this.disableSpellCheck = value;
      }
    }

    public string FieldCaption
    {
      get
      {
        return this.caption;
      }

      set
      {
        this.caption = value;
      }
    }
  }
}
