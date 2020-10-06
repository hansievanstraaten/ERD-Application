using System;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;

namespace REPORT.Builder.ReportComponents
{
    public class PageSetupOptions
    {
        public static PageMediaSize GetPageMediaSize(PaperKind paperKind)
        {
            PaperSize pageSize = GetPaperSize(paperKind);

            if (pageSize == null)
            {
                throw new ApplicationException($"Paper kind {paperKind} not supported.");
            }

            double pageWidth = ConvertPaperSizeToPx(pageSize.Width);

            double pageHeight = ConvertPaperSizeToPx(pageSize.Height);

            PageMediaSize result = new PageMediaSize(ConvertPaperKindToPageMediaSize(paperKind), pageWidth, pageHeight);

            return result;
        }

        //internal static Margins GetPageMargins(PaperKind paperKind)
        //{
        //    PaperSize pageSize = GetPaperSize(paperKind);

        //    PrintDocument document = new PrintDocument();

        //    document.DefaultPageSettings.PaperSize = pageSize;

        //    return document.DefaultPageSettings.Margins;
        //}

        public static double ConvertPaperSizeToCm(int distance)
        {
            // PaperSize is defined in 1/100th inch. To convert to any other unit you must apply a factor.
            return distance * 0.0254;
        }

        private static double ConvertPaperSizeToPx(double distance)
        {
            // PaperSize is defined in 1/100th inch. To convert to any other unit you must apply a factor.
            return (distance / 100) * 96;
        }

        private static PageMediaSizeName ConvertPaperKindToPageMediaSize(PaperKind paperKind)
        {
            switch (paperKind)
            {
                case PaperKind.Custom:
                    return PageMediaSizeName.Unknown;
                case PaperKind.Letter:
                    return PageMediaSizeName.NorthAmericaLetter;
                case PaperKind.Legal:
                    return PageMediaSizeName.NorthAmericaLegal;
                case PaperKind.A4:
                    return PageMediaSizeName.ISOA4;
                case PaperKind.CSheet:
                    return PageMediaSizeName.NorthAmericaCSheet;
                case PaperKind.DSheet:
                    return PageMediaSizeName.NorthAmericaDSheet;
                case PaperKind.ESheet:
                    return PageMediaSizeName.NorthAmericaESheet;
                case PaperKind.LetterSmall:
                    throw new NotImplementedException();
                case PaperKind.Tabloid:
                    return PageMediaSizeName.NorthAmericaTabloid;
                case PaperKind.Ledger:
                    throw new NotImplementedException();
                case PaperKind.Statement:
                    return PageMediaSizeName.NorthAmericaStatement;
                case PaperKind.Executive:
                    return PageMediaSizeName.NorthAmericaExecutive;
                case PaperKind.A3:
                    return PageMediaSizeName.ISOA3;
                case PaperKind.A4Small:
                    throw new NotImplementedException();
                case PaperKind.A5:
                    return PageMediaSizeName.ISOA5;
                case PaperKind.B4:
                    return PageMediaSizeName.ISOB4;
                case PaperKind.B5:
                    throw new NotImplementedException();
                case PaperKind.Folio:
                    return PageMediaSizeName.OtherMetricFolio;
                case PaperKind.Quarto:
                    return PageMediaSizeName.NorthAmericaQuarto;
                case PaperKind.Standard10x14:
                    throw new NotImplementedException();
                case PaperKind.Standard11x17:
                    throw new NotImplementedException();
                case PaperKind.Note:
                    return PageMediaSizeName.NorthAmericaNote;
                case PaperKind.Number9Envelope:
                    return PageMediaSizeName.NorthAmericaNumber9Envelope;
                case PaperKind.Number10Envelope:
                    return PageMediaSizeName.NorthAmericaNumber10Envelope;
                case PaperKind.Number11Envelope:
                    return PageMediaSizeName.NorthAmericaNumber11Envelope;
                case PaperKind.Number12Envelope:
                    return PageMediaSizeName.NorthAmericaNumber12Envelope;
                case PaperKind.Number14Envelope:
                    return PageMediaSizeName.NorthAmericaNumber14Envelope;
                case PaperKind.DLEnvelope:
                    return PageMediaSizeName.ISODLEnvelope;
                case PaperKind.C5Envelope:
                    return PageMediaSizeName.ISOC5Envelope;
                case PaperKind.C3Envelope:
                    return PageMediaSizeName.ISOC3Envelope;
                case PaperKind.C4Envelope:
                    return PageMediaSizeName.ISOC4Envelope;
                case PaperKind.C6Envelope:
                    return PageMediaSizeName.ISOC6Envelope;
                case PaperKind.C65Envelope:
                    return PageMediaSizeName.ISOC6C5Envelope;
                case PaperKind.B4Envelope:
                    return PageMediaSizeName.ISOB4Envelope;
                case PaperKind.B5Envelope:
                    return PageMediaSizeName.ISOB5Envelope;
                case PaperKind.B6Envelope:
                    throw new NotImplementedException();
                case PaperKind.ItalyEnvelope:
                    return PageMediaSizeName.OtherMetricItalianEnvelope;
                case PaperKind.MonarchEnvelope:
                    return PageMediaSizeName.NorthAmericaMonarchEnvelope;
                case PaperKind.PersonalEnvelope:
                    return PageMediaSizeName.NorthAmericaPersonalEnvelope;
                case PaperKind.USStandardFanfold:
                    throw new NotImplementedException();
                case PaperKind.GermanStandardFanfold:
                    return PageMediaSizeName.NorthAmericaGermanStandardFanfold;
                case PaperKind.GermanLegalFanfold:
                    return PageMediaSizeName.NorthAmericaGermanLegalFanfold;
                case PaperKind.IsoB4:
                    return PageMediaSizeName.ISOB4;
                case PaperKind.JapanesePostcard:
                    return PageMediaSizeName.JapanHagakiPostcard;
                case PaperKind.Standard9x11:
                    throw new NotImplementedException();
                case PaperKind.Standard10x11:
                    throw new NotImplementedException();
                case PaperKind.Standard15x11:
                    throw new NotImplementedException();
                case PaperKind.InviteEnvelope:
                    return PageMediaSizeName.OtherMetricInviteEnvelope;
                case PaperKind.LetterExtra:
                    return PageMediaSizeName.NorthAmericaLetterExtra;
                case PaperKind.LegalExtra:
                    return PageMediaSizeName.NorthAmericaLegalExtra;
                case PaperKind.TabloidExtra:
                    return PageMediaSizeName.NorthAmericaTabloidExtra;
                case PaperKind.A4Extra:
                    return PageMediaSizeName.ISOA4Extra;
                case PaperKind.LetterTransverse:
                    throw new NotImplementedException();
                case PaperKind.A4Transverse:
                    throw new NotImplementedException();
                case PaperKind.LetterExtraTransverse:
                    throw new NotImplementedException();
                case PaperKind.APlus:
                    throw new NotImplementedException();
                case PaperKind.BPlus:
                    throw new NotImplementedException();
                case PaperKind.LetterPlus:
                    return PageMediaSizeName.NorthAmericaLetterPlus;
                case PaperKind.A4Plus:
                    return PageMediaSizeName.OtherMetricA4Plus;
                case PaperKind.A5Transverse:
                    throw new NotImplementedException();
                case PaperKind.B5Transverse:
                    throw new NotImplementedException();
                case PaperKind.A3Extra:
                    return PageMediaSizeName.ISOA3Extra;
                case PaperKind.A5Extra:
                    return PageMediaSizeName.ISOA5Extra;
                case PaperKind.B5Extra:
                    return PageMediaSizeName.ISOB5Extra;
                case PaperKind.A2:
                    return PageMediaSizeName.ISOA2;
                case PaperKind.A3Transverse:
                    throw new NotImplementedException();
                case PaperKind.A3ExtraTransverse:
                    throw new NotImplementedException();
                case PaperKind.JapaneseDoublePostcard:
                    return PageMediaSizeName.JapanDoubleHagakiPostcard;
                case PaperKind.A6:
                    return PageMediaSizeName.ISOA6;
                case PaperKind.JapaneseEnvelopeKakuNumber2:
                    return PageMediaSizeName.JapanKaku2Envelope;
                case PaperKind.JapaneseEnvelopeKakuNumber3:
                    return PageMediaSizeName.JapanKaku3Envelope;
                case PaperKind.JapaneseEnvelopeChouNumber3:
                    return PageMediaSizeName.JapanChou3Envelope;
                case PaperKind.JapaneseEnvelopeChouNumber4:
                    return PageMediaSizeName.JapanChou4Envelope;
                case PaperKind.LetterRotated:
                    return PageMediaSizeName.NorthAmericaLetterRotated;
                case PaperKind.A3Rotated:
                    return PageMediaSizeName.ISOA3Rotated;
                case PaperKind.A4Rotated:
                    return PageMediaSizeName.ISOA4Rotated;
                case PaperKind.A5Rotated:
                    return PageMediaSizeName.ISOA5Rotated;
                case PaperKind.B4JisRotated:
                    return PageMediaSizeName.JISB4Rotated;
                case PaperKind.B5JisRotated:
                    return PageMediaSizeName.JISB5Rotated;
                case PaperKind.JapanesePostcardRotated:
                    return PageMediaSizeName.JapanHagakiPostcardRotated;
                case PaperKind.JapaneseDoublePostcardRotated:
                    return PageMediaSizeName.JapanHagakiPostcardRotated;
                case PaperKind.A6Rotated:
                    return PageMediaSizeName.ISOA6Rotated;
                case PaperKind.JapaneseEnvelopeKakuNumber2Rotated:
                    return PageMediaSizeName.JapanKaku2EnvelopeRotated;
                case PaperKind.JapaneseEnvelopeKakuNumber3Rotated:
                    return PageMediaSizeName.JapanKaku3EnvelopeRotated;
                case PaperKind.JapaneseEnvelopeChouNumber3Rotated:
                    return PageMediaSizeName.JapanChou3EnvelopeRotated;
                case PaperKind.JapaneseEnvelopeChouNumber4Rotated:
                    return PageMediaSizeName.JapanChou4EnvelopeRotated;
                case PaperKind.B6Jis:
                    return PageMediaSizeName.JISB6;
                case PaperKind.B6JisRotated:
                    return PageMediaSizeName.JISB6Rotated;
                case PaperKind.Standard12x11:
                    throw new NotImplementedException();
                case PaperKind.JapaneseEnvelopeYouNumber4:
                    return PageMediaSizeName.JapanYou4Envelope;
                case PaperKind.JapaneseEnvelopeYouNumber4Rotated:
                    return PageMediaSizeName.JapanYou4EnvelopeRotated;
                case PaperKind.Prc16K:
                    return PageMediaSizeName.PRC16K;
                case PaperKind.Prc32K:
                    return PageMediaSizeName.PRC32K;
                case PaperKind.Prc32KBig:
                    return PageMediaSizeName.PRC32KBig;
                case PaperKind.PrcEnvelopeNumber1:
                    return PageMediaSizeName.PRC1Envelope;
                case PaperKind.PrcEnvelopeNumber2:
                    return PageMediaSizeName.PRC2Envelope;
                case PaperKind.PrcEnvelopeNumber3:
                    return PageMediaSizeName.PRC3Envelope;
                case PaperKind.PrcEnvelopeNumber4:
                    return PageMediaSizeName.PRC4Envelope;
                case PaperKind.PrcEnvelopeNumber5:
                    return PageMediaSizeName.PRC5Envelope;
                case PaperKind.PrcEnvelopeNumber6:
                    return PageMediaSizeName.PRC6Envelope;
                case PaperKind.PrcEnvelopeNumber7:
                    return PageMediaSizeName.PRC7Envelope;
                case PaperKind.PrcEnvelopeNumber8:
                    return PageMediaSizeName.PRC8Envelope;
                case PaperKind.PrcEnvelopeNumber9:
                    return PageMediaSizeName.PRC9Envelope;
                case PaperKind.PrcEnvelopeNumber10:
                    return PageMediaSizeName.PRC10Envelope;
                case PaperKind.Prc16KRotated:
                    return PageMediaSizeName.PRC16KRotated;
                case PaperKind.Prc32KRotated:
                    return PageMediaSizeName.PRC32KRotated;
                case PaperKind.Prc32KBigRotated:
                    throw new NotImplementedException();
                case PaperKind.PrcEnvelopeNumber1Rotated:
                    return PageMediaSizeName.PRC1EnvelopeRotated;
                case PaperKind.PrcEnvelopeNumber2Rotated:
                    return PageMediaSizeName.PRC2EnvelopeRotated;
                case PaperKind.PrcEnvelopeNumber3Rotated:
                    return PageMediaSizeName.PRC3EnvelopeRotated;
                case PaperKind.PrcEnvelopeNumber4Rotated:
                    return PageMediaSizeName.PRC4EnvelopeRotated;
                case PaperKind.PrcEnvelopeNumber5Rotated:
                    return PageMediaSizeName.PRC5EnvelopeRotated;
                case PaperKind.PrcEnvelopeNumber6Rotated:
                    return PageMediaSizeName.PRC6EnvelopeRotated;
                case PaperKind.PrcEnvelopeNumber7Rotated:
                    return PageMediaSizeName.PRC7EnvelopeRotated;
                case PaperKind.PrcEnvelopeNumber8Rotated:
                    return PageMediaSizeName.PRC8EnvelopeRotated;
                case PaperKind.PrcEnvelopeNumber9Rotated:
                    return PageMediaSizeName.PRC9EnvelopeRotated;
                case PaperKind.PrcEnvelopeNumber10Rotated:
                    return PageMediaSizeName.PRC10EnvelopeRotated;
                default:
                    throw new ArgumentOutOfRangeException("paperKind");
            }
        }
    
        private static PaperSize GetPaperSize(PaperKind paperKind)
        {
            PrinterSettings printerSettings = new PrinterSettings();

            IQueryable<PaperSize> paperSizes = printerSettings.PaperSizes.Cast<PaperSize>().AsQueryable();

            PaperSize result = paperSizes.Where(paperSize => paperSize.Kind == paperKind).FirstOrDefault();

            return result;
        }
    }
}
