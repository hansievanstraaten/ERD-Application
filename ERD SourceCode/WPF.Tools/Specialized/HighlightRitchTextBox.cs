using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using GeneralExtensions;

namespace WPF.Tools.Specialized
{
  public class HighlightRitchTextBox : RichTextBox
  {
    private Brush highlightColour = Brushes.Yellow;

    public string Text
    {
      get
      {
        TextRange result = new TextRange(base.Document.ContentStart, base.Document.ContentEnd);

        return result.Text;
      }

      set
      {
        base.Document.Blocks.Clear();

        if (value.IsNullEmptyOrWhiteSpace())
        {
          return;
        }

        string[] valueSplit = value.TrimEnd(new char[] {' ', '\n', '\r'}).Split(new string[] {Environment.NewLine}, StringSplitOptions.None);

        Paragraph runParagraph = new Paragraph();

        foreach (string item in valueSplit)
        {

          Run itemRun = new Run(item);

          runParagraph.Inlines.Add(itemRun);
        }

        base.Document.Blocks.Add(runParagraph);
      }
    }

    public Brush HighlightColour
    {
      get
      {
        return this.highlightColour;
      }

      set
      {
        this.highlightColour = value;
      }
    }

    public int GetSelectionStartIndex()
    {
      //return this.CaretPositionIndex() - this.GetSelectedTextLength();
      TextPointer documentStart = base.Document.ContentStart;

      TextPointer textStart = this.Selection.Start;

      TextRange range = new TextRange(documentStart, textStart);

      int result = range.Text.Length;

      return result;
    }

    public int GetSelectedTextLength()
    {
      return base.Selection.Text.Length;
    }

    public void HighlightText(int start, int length, Brush colour)
    {
      if (start <= 0)
      {
        start = 0;
      }

      start += 2;

      TextPointer startPointer = base.Document.ContentStart.GetPositionAtOffset(start);

      TextPointer endPointer = startPointer.GetPositionAtOffset(length);

      TextRange range = new TextRange(startPointer, endPointer);

      // Programmatically change the selection in the RichTextBox.
      range.ApplyPropertyValue(TextElement.BackgroundProperty, colour);
    }

    public void HighlightText(string[] phraseArray)
    {
      this.Text = this.Text; // This will reset the previous serch

      foreach (string phrase in phraseArray)
      {
        this.Highlite(phrase);
      }
    }

    public void HighlightText(string phrase)
    {
      this.Text = this.Text; // This will reset the previous serch

      this.Highlite(phrase);
    }

    private void Highlite(string phrase)
    {
      phrase = phrase.ToUpperInvariant();

      for (TextPointer position = base.Document.ContentStart;
        position != null && position.CompareTo(base.Document.ContentEnd) <= 0;
        position = position.GetNextContextPosition(LogicalDirection.Forward))
      {
        if (position.CompareTo(base.Document.ContentEnd) == 0)
        {
          break;
        }

        String textRun = position.GetTextInRun(LogicalDirection.Forward);

        StringComparison stringComparison = StringComparison.CurrentCulture;

        Int32 indexInRun = textRun.ToUpperInvariant().IndexOf(phrase, stringComparison);

        if (indexInRun >= 0)
        {
          position = position.GetPositionAtOffset(indexInRun);

          if (position != null)
          {
            TextPointer nextPointer = position.GetPositionAtOffset(phrase.Length);

            TextRange textRange = new TextRange(position, nextPointer);

            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, this.HighlightColour);
          }
        }
      }
    }
  }
}
