using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GeneralExtensions;
using WPF.Tools.Dictionaries;

namespace WPF.Tools.Indicators
{
  /// <summary>
  /// Interaction logic for ValueMeter.xaml
  /// </summary>
  public partial class ValueMeter : UserControl
  {
    private double minimum = 0;
    private double meterValue = 50;
    private double safeRange = 50;
    private double warningRange = 30;
    private double dangerRange = 20;
    private MeterUnitTypeEnum unitType;

    public ValueMeter()
    {
      this.InitializeComponent();

      this.Loaded += this.ValueMeter_Loaded;
    }

    public double Maximum
    {
      get
      {
        return this.SafeRangeValue + this.WarningRangevalue + this.DangerRangeValue;
      }
    }

    public double Minimum
    {
      get
      {
        return this.minimum;
      }

      set
      {
        this.minimum = value;

        if (this.UnitType == MeterUnitTypeEnum.Percentage)
        {
          this.uxStateMin.Content = "0%";
        }
        else
        {
          this.uxStateMin.Content = value;
        }
      }
    }

    public double Value
    {
      get
      {
        return this.meterValue;
      }

      set
      {
        double renderValue = this.meterValue = value;

        if (this.UnitType == MeterUnitTypeEnum.Percentage)
        {
          // for this value is %. So we need to see how much is the % value of the actual width
          renderValue = (value / 100) * this.uxRangeGrid.ActualWidth;

          renderValue = this.uxRangeGrid.ActualWidth - renderValue;
        }
        else
        {
          renderValue = this.uxRangeGrid.ActualWidth - this.meterValue;
        }

        if (renderValue <= 0)
        {
          this.uxRangeOverlay.Width = this.uxRangeGrid.ActualWidth;
        }
        else if (renderValue > this.uxRangeGrid.ActualWidth)
        {
          this.uxRangeOverlay.Width = 0;
        }
        else
        {
          this.uxRangeOverlay.Width = renderValue;
        }

        if (this.UnitType == MeterUnitTypeEnum.Percentage)
        {
          this.uxStateValue.Content = $"{value.ToString("#.00")}% of 100%";
        }
        else
        {
          this.uxStateValue.Content = $"{value.ToString().ToInt32()} of {this.Maximum}";
        }
      }
    }

    public double SafeRangeValue
    {
      get
      {
        return this.safeRange;
      }

      set
      {
        this.safeRange = value;

        this.SetRanges();

        this.SetStateLables();
      }
    }

    public double WarningRangevalue
    {
      get
      {
        return this.warningRange;
      }

      set
      {
        this.warningRange = value;

        this.SetRanges();

        this.SetStateLables();
      }
    }

    public double DangerRangeValue
    {
      get
      {
        return dangerRange;
      }

      set
      {
        this.dangerRange = value;

        this.SetRanges();

        this.SetStateLables();
      }
    }
    
    public string Title
    {
      set
      {
        this.uxTitle.Content = value;
      }
    }

    new public Brush Background
    {
      get
      {
        return base.Background;
      }

      set
      {
        base.Background = value;

        this.uxRangeOverlay.Background = value;
      }
    }

    public MeterUnitTypeEnum UnitType
    {
      get
      {
        return this.unitType;
      }

      set
      {
        this.unitType = value;

        this.SetStateLables();
      }
    }

    private void ValueMeter_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        this.SetRanges();

        this.Value = 0;
      }
      catch (Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }

    private void SetStateLables()
    {
      if (this.UnitType == MeterUnitTypeEnum.Percentage)
      {
        this.uxStateMin.Content = "0%";

        this.uxStateMax.Content = "100%";
      }
      else
      {
        this.uxStateMin.Content = "0";

        this.uxStateMax.Content = this.Maximum;
      }
    }
    
    private void SetRanges()
    {
      double startUpWidth = this.uxRangeGrid.ActualWidth;
      
      double safe = (this.SafeRangeValue * startUpWidth) / 100;

      double warn = (this.WarningRangevalue * startUpWidth) / 100;

      double danger = (this.DangerRangeValue * startUpWidth) / 100;

      this.uxSafeRange.Width = new GridLength(safe, GridUnitType.Star);

      this.uxWarningRange.Width = new GridLength(warn, GridUnitType.Star);

      this.uxDangerRange.Width = new GridLength(danger, GridUnitType.Star);
    }
  }
}
