using PIMEdoc_CR.Default.Rule;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIMEdoc_CR.Rule
{
    public static class PDFHelper
    {

        public static PdfDocument MergeSelectPDF(List<PdfDocument> PDFs)
        {
            try
            {
                PdfDocument mergeDoc = new PdfDocument();
                foreach (var pdf in PDFs)
                {
                    mergeDoc.Append(pdf);
                }
                return mergeDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static PdfDocument SelectPDFAddPageNumber(PdfDocument PDFs, bool isAddToTop)
        {
            try
            {
                // create a new pdf document
                PdfDocument doc = PDFs;
                doc.Margins = new PdfMargins(10, 10, 0, 0);

                // create a new pdf font
                var fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\THSarabun.ttf";
                PdfFont font = doc.AddFont(fontPath);
                //PdfFont font = doc.AddFont(PdfStandardFont.TimesRoman);

                font.Size = 14;
                for (int i = 1; i < PDFs.Pages.Count; i++)
                {
                    PdfPage page = doc.Pages[i];
                    page.DisplayHeader = true;

                    float width = page.PageSize.Width;
                    float height = page.PageSize.Height;
                    bool isPortrait = width > height;

                    float yPosition = isAddToTop ? 15 : height - 15;

                    PdfTemplate customHeader = doc.AddTemplate(page.ClientRectangle);
                    PdfTextElement text1 = new PdfTextElement(0, yPosition,
                        "{page_number}", font);
                    text1.HorizontalAlign = PdfTextHorizontalAlign.Center;
                    text1.ForeColor = System.Drawing.Color.Black;
                    customHeader.DisplayOnFirstPage = false;
                    customHeader.Add(text1);
                }

                return doc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static PdfDocument SelectPDFAddWaterMark(PdfDocument PDFs, string WaterText, bool isAdditional)
        {
            try
            {
                PDFs.Margins = new PdfMargins(10, 10, 0, 0);

                // create a new pdf font
                var fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\THSarabun.ttf";
                PdfFont font = PDFs.AddFont(fontPath);
                font.Size = 14;
                for (int i = 0; i < PDFs.Pages.Count; i++)
                {
                    PdfPage page = PDFs.Pages[i];

                    float width = page.PageSize.Width;
                    float height = page.PageSize.Height;
                    float additionalHeight = isAdditional ? 45 : 0;
                    bool isPortrait = width < height;

                    PdfTemplate customTemplate = PDFs.AddTemplate(width, height);//;
                    PdfTextElement textElement = new PdfTextElement(-60, (height / 2) + additionalHeight, width, height, WaterText, font)
                    {
                        ForeColor = System.Drawing.Color.FromArgb(50, 0, 0, 0),
                        Direction = 45

                    };
                    textElement.Font.Size = 40;
                    textElement.Transparency = 50;
                    textElement.HorizontalAlign = PdfTextHorizontalAlign.Center;
                    textElement.VerticalAlign = PdfTextVerticalAlign.Middle;

                    customTemplate.Add(textElement);
                }

                return PDFs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static PdfDocument SelectPDFAddSignature(PdfDocument PDFs, List<KeyValuePair<string, string>> ListApproval)
        {
            try
            {
                PdfDocument firstPage = SelectPDFGetFirstPage(PDFs);

                // create a new pdf font
                var fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\THSarabun Italic.ttf";
                PdfFont font = firstPage.AddFont(fontPath);
                font.Size = 14;

                PdfPage page = firstPage.Pages[0];

                float pageWidth = page.PageSize.Width;
                float pageHeight = page.PageSize.Height;
                bool isPortrait = pageWidth < pageHeight;

                PdfTemplate customTemplate = firstPage.AddTemplate(page.ClientRectangle);
                customTemplate.DisplayOnFirstPage = true;
                customTemplate.Background = false;

                for (int i = 0; i < ListApproval.Count; i++)
                {
                    var approval = ListApproval[i];
                    bool isImage = approval.Value.Equals("image");
                    float xPosition = (((isPortrait ? pageWidth - 40 : pageHeight - 40) / 3) * (i)) + 10;
                    float yPosition = pageHeight - 150;

                    if (isImage)
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(SharedRules.GetSPFile(approval.Key)));

                        float scaledHeight = 180f;
                        float scaledWidth = (image.Height * scaledHeight) / image.Width;

                        PdfImageElement imageElm = new PdfImageElement(xPosition, yPosition, 180, image);
                        imageElm.TransparentRendering = true;

                        customTemplate.Add(imageElm);
                    }
                    else
                    {
                        PdfTextElement textElement = new PdfTextElement(xPosition, yPosition + 100, 180, approval.Key, font)
                        {
                            HorizontalAlign = PdfTextHorizontalAlign.Center,
                            ForeColor = System.Drawing.Color.Black,
                            Direction = 45

                        };
                        textElement.Font.Size = 30;
                        customTemplate.Add(textElement);
                    }
                }

                return SelectPDFReplaceFirstPage(PDFs, firstPage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static PdfDocument SelectPDFGetFirstPage(PdfDocument PDFs)
        {
            try
            {
                // load the pdf document
                PdfDocument doc1 = PDFs;
                // create a new pdf document
                PdfDocument doc = new PdfDocument();

                doc.AddPage(doc1.Pages[0]);
                return doc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static PdfDocument SelectPDFReplaceFirstPage(PdfDocument MainPDFs, PdfDocument NewFirstPage)
        {
            try
            {
                // load the pdf document
                PdfDocument doc1 = NewFirstPage;
                PdfDocument doc2 = MainPDFs;
                // create a new pdf document
                PdfDocument doc = new PdfDocument();

                doc.AddPage(doc1.Pages[0]);
                for (int i = 1; i < doc2.Pages.Count; i++)
                {
                    doc.AddPage(doc2.Pages[i]);
                }
                return doc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
