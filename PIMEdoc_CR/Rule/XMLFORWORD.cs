using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.CustomProperties;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using PIMEdoc_CR.Default.Rule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Web;
using System.Xml.Linq;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace PIMEdoc_CR.Default.XMLFORWORD
{
    public class XMLFORWORD
    {
        public static string errorMessage = "";

        #region Method For OpenXML

        #region Header Part
        public static Table findHeadTable(WordprocessingDocument wDoc)
        {
            Table tb = new Table();
            var head = wDoc.MainDocumentPart.HeaderParts;

            foreach (var headPart in head)
            {
                Table tbHeader = headPart.RootElement.Descendants<Table>().FirstOrDefault();
                if (tbHeader != null)
                {
                    tb = tbHeader;
                    return tb;
                    break;
                }
            }
            return tb;
        }
        #endregion
        public static void replaceText(WordprocessingDocument wDoc, string FindText, string newText)
        {
            try
            {
                //step to find the text and replace the old text with new text, after that. break this function
                foreach (var item in wDoc.MainDocumentPart.Document.Body.Descendants<Text>())
                {
                    if (item.Text.Contains(FindText))
                    {
                        item.Text = item.Text.Replace(FindText, newText);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void replaceAllText(WordprocessingDocument wDoc, string FindText, string newText)
        {
            try
            {
                //step to find the text and replace all text with new text
                foreach (var item in wDoc.MainDocumentPart.Document.Body.Descendants<Text>())
                {
                    if (item.Text.Contains(FindText))
                    {
                        item.Text = item.Text.Replace(FindText, newText);
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static int bodyFindTable(WordprocessingDocument wDoc, int rowNum = 0, int cellNum = 0, string findText = null)
        {
            int indexTable = 0;
            try
            {
                List<Table> listTb = wDoc.MainDocumentPart.Document.Body.Elements<Table>().ToList();
                if (listTb != null)
                {
                    for (int i = 0; i < listTb.Count; i++)
                    {
                        if (listTb[i].Descendants<TableRow>().ElementAt(rowNum).Descendants<TableCell>().ElementAt(cellNum).InnerText.Contains(findText))
                        {
                            return indexTable;
                        }
                        else
                        {
                            indexTable++;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
            return -1;
        }
        public static void setCheckBox(WordprocessingDocument wDoc, int tableNum, int rowNum, int colNum, string text = null)
        {
            try
            {
                //create Run && adding runPropetytype for create fontstyle 

                Run run = new Run();
                run.Append(setRunProperty("Wingdings", "Wingdings", "Wingdings", "24"));

                DocumentFormat.OpenXml.Wordprocessing.Table tb = wDoc.MainDocumentPart.Document.Body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>().ElementAt(tableNum);

                if (text.ToLower() == "checkbox")
                {
                    int unicode = 254;
                    tb.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableRow>()
                    .ElementAt(rowNum).Descendants<DocumentFormat.OpenXml.Wordprocessing.TableCell>()
                    .ElementAt(colNum).Descendants<Run>().FirstOrDefault().Remove();
                    //adding Run that we create on the top
                    tb.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableRow>()
                    .ElementAt(rowNum).Descendants<DocumentFormat.OpenXml.Wordprocessing.TableCell>()
                    .ElementAt(colNum).Descendants<Paragraph>().FirstOrDefault().Append(run);

                    //adding the word that we want in this case we use char to generate to the symbol
                    tb.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableRow>()
                    .ElementAt(rowNum).Descendants<DocumentFormat.OpenXml.Wordprocessing.TableCell>()
                    .ElementAt(colNum).Descendants<Run>().FirstOrDefault().Append(new Text() { Text = ((char)unicode).ToString() });

                }
                else if (text.ToLower() == "uncheckbox")
                {
                    int unicode = 111;

                    //FindThe Parargraph with the row column index, then Remove all childrennode
                    tb.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableRow>()
                    .ElementAt(rowNum).Descendants<DocumentFormat.OpenXml.Wordprocessing.TableCell>()
                    .ElementAt(colNum).Descendants<Paragraph>().FirstOrDefault().RemoveAllChildren();

                    //adding Run that we create on the top
                    tb.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableRow>()
                    .ElementAt(rowNum).Descendants<DocumentFormat.OpenXml.Wordprocessing.TableCell>()
                    .ElementAt(colNum).Descendants<Paragraph>().FirstOrDefault().PrependChild<Run>(run);

                    //adding the word that we want in this case we use char to generate to the symbol
                    tb.Descendants<DocumentFormat.OpenXml.Wordprocessing.TableRow>()
                    .ElementAt(rowNum).Descendants<DocumentFormat.OpenXml.Wordprocessing.TableCell>()
                    .ElementAt(colNum).Descendants<Run>().FirstOrDefault().Append(new Text() { Text = ((char)unicode).ToString() });

                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static Table bodyFindTableByBookMark(WordprocessingDocument wDoc, string bookmarkText)
        {
            BookmarkStart bookmark = wDoc.MainDocumentPart.Document.Body.Descendants<BookmarkStart>().FirstOrDefault();
            Table table = new Table();

            try
            {
                //IEnumerable<TableProperties> tableProperties = wDoc.MainDocumentPart.Document.Body.Descendants<TableProperties>().Where(tp => tp.TableCaption != null);
                //foreach (TableProperties tProp in tableProperties)
                //{
                //    if (tProp.TableCaption.Val.Equals(bookmarkText))
                //    {
                //        table = (Table)tProp.Parent;
                //        return table;
                //    }
                //}
                //return table;
                var listBookmark = wDoc.MainDocumentPart.Document.Body.Descendants<BookmarkStart>().ToList();
                //step to find the text and replace all text with new text
                foreach (var item in listBookmark)
                {
                    if (item != null)
                    {
                        if (item.Name.Value.Contains(bookmarkText))
                        {
                            if (item.Parent != null)
                            {
                                OpenXmlElement elem = item;

                                while (!(elem is DocumentFormat.OpenXml.Wordprocessing.Table) && elem.Parent != null)
                                    elem = elem.Parent;
                                table = (Table)elem;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
            return table;
        }
        public static int bodyFindTableString(WordprocessingDocument wDoc, string findText = null)
        {
            int rowTable = 0;

            try
            {
                Body bod = wDoc.MainDocumentPart.Document.Body;
                for (int i = 0; i < (int)(bod.Descendants<DocumentFormat.OpenXml.Wordprocessing.Table>().LongCount()); i++)
                {
                    if (bod.Descendants<DocumentFormat.OpenXml.Wordprocessing.Table>().ElementAt(i).InnerText.Contains(findText))
                    {
                        rowTable = i;
                        return rowTable;
                    }
                }
                //foreach (DocumentFormat.OpenXml.Wordprocessing.Table t in bod.Descendants<DocumentFormat.OpenXml.Wordprocessing.Table>().Where(tbl => tbl.InnerText.Contains(findText)))
                //{


                //}
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
            return rowTable;
        }
        public static void bodyFillTable(WordprocessingDocument wDoc, int tableNum, int rowNum, int colNum, string text)
        {
            try
            {
                //getting the tablecell
                TableCell tc = wDoc.MainDocumentPart.Document.Body.Descendants<Table>().ElementAt(tableNum).Descendants<TableRow>().ElementAt(rowNum - 1).Descendants<TableCell>().ElementAt(colNum - 1);
                // DocumentFormat.OpenXml.Wordprocessing.FontSize size = new DocumentFormat.OpenXml.Wordprocessing.FontSize();
                //  size.Val = new StringValue("13");
                //check text in tablecell
                if (tc.Descendants<Text>().FirstOrDefault() != null && tc.Descendants<Text>().FirstOrDefault().InnerText != "")
                {
                    foreach (var item in tc.Descendants<Text>())
                    {
                        tc.Descendants<Text>().FirstOrDefault().Remove();
                    }
                    tc.Descendants<Run>().FirstOrDefault().Append(new Text(text));
                }
                else
                {
                    if (tc.Descendants<Run>().FirstOrDefault() != null)
                    {
                        tc.Descendants<Run>().FirstOrDefault().RemoveAllChildren();
                        tc.Descendants<Run>().FirstOrDefault().Append(new Text(text));
                    }
                    else
                    {
                        tc.RemoveAllChildren();
                        tc.Append(new Paragraph(new Run(new Text(text))));
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void bodyFillTable(WordprocessingDocument wDoc, Table table, int rowNum, int colNum, string text)
        {
            try
            {
                //getting the tablecell
                TableRow tr = table.Elements<TableRow>().ElementAt(rowNum - 1);
                TableCell tc = tr.Elements<TableCell>().ElementAt(colNum - 1);

                RunProperties runProp = new RunProperties();
                RunFonts runFont = new RunFonts();
                runFont.Ascii = "TH SarabunPSK";
                runFont.EastAsia = "TH SarabunPSK";
                runFont.HighAnsi = "TH SarabunPSK";

                FontSize size = new FontSize() { Val = "32" };

                runProp.Append(runFont);
                runProp.Append(size);

                var t = new Text(text);
                t.Space = SpaceProcessingModeValues.Preserve;

                //check text in tablecell
                if (tc.Descendants<Text>().FirstOrDefault() != null && tc.Descendants<Text>().FirstOrDefault().InnerText != "")
                {
                    foreach (var item in tc.Descendants<Text>())
                    {
                        tc.Descendants<Text>().FirstOrDefault().Remove();
                    }
                    tc.Descendants<Run>().FirstOrDefault().Append(runProp);
                    tc.Descendants<Run>().FirstOrDefault().Append(t);
                }
                else
                {
                    if (tc.Descendants<Run>().FirstOrDefault() != null)
                    {
                        tc.Descendants<Run>().FirstOrDefault().RemoveAllChildren();
                        tc.Descendants<Run>().FirstOrDefault().Append(runProp);
                        tc.Descendants<Run>().FirstOrDefault().Append(t);
                    }
                    else
                    {
                        if (tc.Descendants<Paragraph>().FirstOrDefault() != null)
                        {
                            Paragraph p = tc.Descendants<Paragraph>().FirstOrDefault();
                            ParagraphProperties pProp = p.ParagraphProperties;
                            TableCellProperties tcp = tc.TableCellProperties;
                            p.RemoveAllChildren();
                            p.Append(new Run(runProp, t));
                            tc.Append(p);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void bodyFillTableWithTextAlign(WordprocessingDocument wDoc, Table table, int rowNum, int colNum, string text, string textAlign)
        {
            try
            {
                ParagraphProperties pragProp = new ParagraphProperties();
                pragProp.AppendChild<Justification>(SetPargParagraphProperties(textAlign));

                //getting the tablecell
                TableCell tc = table.Descendants<TableRow>().ElementAt(rowNum - 1).Descendants<TableCell>().ElementAt(colNum - 1);

               
                RunProperties runProp = new RunProperties();
                RunFonts runFont = new RunFonts();
                runFont.Ascii = "TH SarabunPSK";
                runFont.EastAsia = "TH SarabunPSK";
                runFont.HighAnsi = "TH SarabunPSK";

                FontSize size = new FontSize() { Val = "32" };
                runProp.Append(runFont);
                runProp.Append(size);

                //check text in tablecell
                if (tc.Descendants<Text>().FirstOrDefault() != null &&
                    tc.Descendants<Text>().FirstOrDefault().InnerText != "")
                {
                    foreach (var item in tc.Descendants<Text>())
                    {
                        tc.Descendants<Text>().FirstOrDefault().Remove();
                    }
                    tc.Descendants<Run>().FirstOrDefault().Append(runProp);
                    tc.Descendants<Run>().FirstOrDefault().Append(new Text(text));
                }
                else
                {
                    if (tc.Descendants<Run>().FirstOrDefault() != null)
                    {
                        tc.Descendants<Run>().FirstOrDefault().RemoveAllChildren();
                        tc.Descendants<Run>().FirstOrDefault().Append(runProp);
                        tc.Descendants<Run>().FirstOrDefault().Append(new Text(text));
                    }
                    else
                    {
                        if (tc.Descendants<Paragraph>().FirstOrDefault() != null)
                        {
                            Paragraph p = tc.Descendants<Paragraph>().FirstOrDefault();
                            TableCellProperties tcp = tc.TableCellProperties;
                            p.RemoveAllChildren();
                            p.Append(new Run(runProp, new Text(text)));
                            p.Append(pragProp);
                            tc.Append(p);
                        }
                    }
                }

                //Paragraph firstOrDefault = tc.Descendants<Paragraph>().FirstOrDefault();
                //if (firstOrDefault != null)
                //{
                //    firstOrDefault.AppendChild<ParagraphProperties>(pragProp);
                //}

            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static int getRowCount(WordprocessingDocument wDoc, int tableNum)
        {
            int rowCount = 0;
            try
            {
                DocumentFormat.OpenXml.Wordprocessing.Table tb = wDoc.MainDocumentPart.Document.Body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>().ElementAt(tableNum);
                rowCount = (int)(tb.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().LongCount());
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
            return rowCount;
        }
        public static void setNewCellWidth(WordprocessingDocument wDoc, Table table, int rowNum, int cellNum, float cellWidth)
        {
            try
            {
                if (table.Elements<TableRow>().ElementAt(rowNum - 1).Elements<TableCell>().ElementAt(cellNum - 1) != null)
                {
                    TableCell tc = table.Elements<TableRow>().ElementAt(rowNum - 1).Elements<TableCell>().ElementAt(cellNum - 1);
                    TableCellProperties tcp = tc.TableCellProperties;
                    tcp.TableCellWidth = new TableCellWidth() { Width = cellWidth.ToString() };
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void insertRow(WordprocessingDocument wDoc, int tableNum, int rowNum, int cellNum)
        {
            try
            {
                Table tb = wDoc.MainDocumentPart.Document.Body.Descendants<Table>().ElementAt(tableNum);

                for (int r = 0; r < rowNum; r++)
                {
                    TableRow tr = new TableRow();
                    TableCell tc = new TableCell();
                    int totalRow = tb.Elements<TableRow>().Count() - 1;

                    for (int c = 0; c < cellNum; c++)
                    {
                        TableCellProperties tcProp = (TableCellProperties)getTableCellPropertyByFirstRow(wDoc, tableNum, totalRow, c).CloneNode(true);
                        tc = new TableCell(tcProp);
                        tc.Append(new Paragraph(new Run(new Text())));
                        tr.Append(tc);
                    }
                    tb.Append(tr);
                }
                //                wDoc.MainDocumentPart.Document.Body.Append(tb);
                wDoc.MainDocumentPart.Document.Save();
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void insertRow(WordprocessingDocument wDoc, Table table, int rowNum, int cellNum)
        {
            try
            {

                for (int r = 0; r < rowNum; r++)
                {
                    TableRow tr = new TableRow();
                    TableCell tc = new TableCell();
                    int totalRow = table.Elements<TableRow>().Count() - 1;

                    for (int c = 0; c < cellNum; c++)
                    {
                        TableCellProperties tcProp = (TableCellProperties)getTableCellPropertyByFirstRow(wDoc, table, totalRow, c).CloneNode(true);
                        tc = new TableCell(tcProp);
                        tc.Append(new Paragraph(new Run(new Text())));
                        tr.Append(tc);
                    }
                    table.Append(tr);
                }
                wDoc.MainDocumentPart.Document.Save();
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void DuplicateRow(WordprocessingDocument wDoc, Table table, int srcRowNum)
        {
            try
            {
                TableRow srcTr = table.Elements<TableRow>().ElementAt(srcRowNum - 1);
                TableRowProperties srcTrPtop = srcTr.Elements<TableRowProperties>().First();

                TableRow newTr = (TableRow)srcTr.CloneNode(true);               
                table.Append(newTr);
                
                wDoc.MainDocumentPart.Document.Save();
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void insertRowNoSpace(WordprocessingDocument wDoc, Table table, int rowNum, int cellNum)
        {
            try
            {
                Table tb = table;
                for (int r = 0; r < rowNum; r++)
                {
                    TableRow tr = new TableRow();
                    TableCell tc = new TableCell();
                    for (int c = 0; c < cellNum; c++)
                    {
                        TableCellProperties tcProp = (TableCellProperties)getTableCellPropertyByFirstRow(wDoc, table, 0, c).CloneNode(true);
                        tc = new TableCell(tcProp);
                        tc.Append(new Paragraph(new Run(new Text())));
                        tr.Append(tc);
                    }
                    tb.Append(tr);
                }
                wDoc.MainDocumentPart.Document.Save();
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void insertRowNoSpace(WordprocessingDocument wDoc, int tableNum, int rowNum, int cellNum)
        {
            try
            {
                Table tb = wDoc.MainDocumentPart.Document.Body.Descendants<Table>().ElementAt(tableNum);
                for (int r = 0; r < rowNum; r++)
                {
                    TableRow tr = new TableRow();
                    TableCell tc = new TableCell();

                    for (int c = 0; c < cellNum; c++)
                    {
                        TableCellProperties tcProp = (TableCellProperties)getTableCellPropertyByFirstRow(wDoc, tableNum, 0, c).CloneNode(true);
                        tc = new TableCell(tcProp);
                        tc.Append(new Paragraph(new Run(new Text())));
                        tr.Append(tc);
                    }
                    tb.Append(tr);
                }
                //                wDoc.MainDocumentPart.Document.Body.Append(tb);
                wDoc.MainDocumentPart.Document.Save();
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void insertRowNoSpace(WordprocessingDocument wDoc, int tableNum, int rowNum, int cellNum, string textAlign)
        {
            try
            {
                Table tb = wDoc.MainDocumentPart.Document.Body.Descendants<Table>().ElementAt(tableNum);

                for (int r = 0; r < rowNum; r++)
                {
                    TableRow tr = new TableRow();
                    TableCell tc = new TableCell();

                    for (int c = 0; c < cellNum; c++)
                    {
                        TableCellProperties tcProp = (TableCellProperties)getTableCellPropertyByFirstRow(wDoc, tableNum, 0, c).CloneNode(true);
                        tc = new TableCell(tcProp);
                        Paragraph prag = new Paragraph(new Run(new Text()));
                        ParagraphProperties pragProp = new ParagraphProperties();
                        pragProp.AppendChild<Justification>(SetPargParagraphProperties(textAlign));

                        tc.AppendChild<ParagraphProperties>(pragProp);
                        tc.AppendChild<Paragraph>(prag);

                        tr.Append(tc);
                    }
                    tb.Append(tr);
                }
                //                wDoc.MainDocumentPart.Document.Body.Append(tb);
                wDoc.MainDocumentPart.Document.Save();
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void RemoveRowUntil(WordprocessingDocument wDoc, int tableNum, int rowNum)
        {
            try
            {
                Table tb = wDoc.MainDocumentPart.Document.Body.Descendants<Table>().ElementAt(tableNum);
                int getAllRow = tb.Descendants<TableRow>().Count();
                for (int r = 0; r < getAllRow - rowNum; r++)
                {
                    TableRow tr = tb.Descendants<TableRow>().LastOrDefault();
                    tb.RemoveChild(tr);
                }
                wDoc.MainDocumentPart.Document.Save();
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void RemoveRowUntil(WordprocessingDocument wDoc, Table table, int rowNum)
        {
            try
            {
                Table tb = table;
                int getAllRow = tb.Descendants<TableRow>().Count();
                for (int r = 0; r < getAllRow - rowNum; r++)
                {
                    TableRow tr = tb.Descendants<TableRow>().LastOrDefault();
                    tb.RemoveChild(tr);
                }
                wDoc.MainDocumentPart.Document.Save();
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void hideBorderTable(WordprocessingDocument wDoc, int firstIndexTable, int lastIndexTable)
        {
            try
            {
                List<Table> listTB = wDoc.MainDocumentPart.Document.Body.Descendants<Table>().Take(lastIndexTable).ToList();
                foreach (var itemInTB in listTB)
                {
                    if (itemInTB.Descendants<TableProperties>().FirstOrDefault() != null)
                    {
                        itemInTB.Descendants<TableProperties>().FirstOrDefault().TableBorders = setTableBorder(BorderValues.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static DocumentFormat.OpenXml.Wordprocessing.Table createTable(WordprocessingDocument wDoc, Table tb, int rowNum = 1, int cellNum = 1)
        {
            // Create an empty table.
            DocumentFormat.OpenXml.Wordprocessing.Table table = tb;

            try
            {
                // Append the TableProperties object to the empty table.
                table.AppendChild<TableProperties>(setTableProperty(BorderValues.Single));

                // Create a row.
                for (int r = 0; r < rowNum; r++)
                {
                    DocumentFormat.OpenXml.Wordprocessing.TableRow tr = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
                    // Create a Cell meaning to Column
                    for (int c = 0; c < cellNum; c++)
                    {
                        DocumentFormat.OpenXml.Wordprocessing.TableCell tc = new DocumentFormat.OpenXml.Wordprocessing.TableCell();
                        if (r == 0)
                        {
                            tc.Append(new TableCellProperties(
                                        new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" }));
                            tc.Append(new Paragraph(new Run(new Text("Columns " + c.ToString()))));

                        }
                        else
                        {
                            tc.Append(new TableCellProperties(
                                        new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" }));
                            tc.Append(new Paragraph(new Run(new Text("Row " + c.ToString()))));

                        }
                        tr.Append(tc);
                    }
                    // adding Table
                    table.Append(tr);
                }

                // adding new Line for make space
                TableRow trnewLine = new TableRow();
                TableCell tcnewLine = new TableCell();
                tcnewLine.Append(new TableCellProperties(
                                  new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" }));
                tcnewLine.Append(new Paragraph(new Run(new Break())));

                trnewLine.Append(tcnewLine);

                table.Append(trnewLine);
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
            //Return the table
            return table;
        }

        /// <summary>
        /// For add image to Table Cell.
        /// If image call from SharePoint, it have to call under SP Privilage
        /// </summary>
        /// <param name="wDoc"></param>
        /// <param name="table"></param>
        /// <param name="rowNum"></param>
        /// <param name="colNum"></param>
        /// <param name="filePath"></param>
        public static void AddImageToTable(WordprocessingDocument wDoc, Table table, int rowNum, int colNum, string filePath)
        {
            try
            {
                var mainPart = wDoc.MainDocumentPart;

                //getting the tablecell
                TableRow tr = table.Elements<TableRow>().ElementAt(rowNum - 1);
                TableCell tc = tr.Elements<TableCell>().ElementAt(colNum - 1);

                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Gif);
                using (MemoryStream ms = new MemoryStream(SharedRules.GetSPFile(filePath)))
                {
                    imagePart.FeedData(ms);
                }

                tc.RemoveAllChildren();
                AddImageToCell(tc, mainPart.GetIdOfPart(imagePart));

                mainPart.Document.Save();
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
        }
        public static void AddImageToCell(TableCell cell, string relationshipId)
        {
            var element =
              new Drawing(
                new DW.Inline(
                  //new DW.Extent() { Cx = 990000L, Cy = 792000L },
                  new DW.Extent() { Cx = 1600000L, Cy = 800000L },
                  new DW.EffectExtent()
                  {
                      LeftEdge = 0L,
                      TopEdge = 0L,
                      RightEdge = 0L,
                      BottomEdge = 0L
                  },
                  new DW.DocProperties()
                  {
                      Id = (UInt32Value)1U,
                      Name = "Picture 1"
                  },
                  new DW.NonVisualGraphicFrameDrawingProperties(
                      new A.GraphicFrameLocks() { NoChangeAspect = true }),
                  new A.Graphic(
                    new A.GraphicData(
                      new PIC.Picture(
                        new PIC.NonVisualPictureProperties(
                          new PIC.NonVisualDrawingProperties()
                          {
                              Id = (UInt32Value)0U,
                              Name = "New Bitmap Image.gif"
                          },
                          new PIC.NonVisualPictureDrawingProperties()),
                        new PIC.BlipFill(
                          new A.Blip(
                            new A.BlipExtensionList(
                              new A.BlipExtension()
                              {
                                  Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
                              })
                           )
                          {
                              Embed = relationshipId,
                              CompressionState =
                                A.BlipCompressionValues.Print
                          },
                          new A.Stretch(
                            new A.FillRectangle())),
                          new PIC.ShapeProperties(
                            new A.Transform2D(
                              new A.Offset() { X = 0L, Y = 0L },
                              new A.Extents() { Cx = 1600000L, Cy = 800000L }),
                              //new A.Extents() { Cx = 990000L, Cy = 792000L }),
                            new A.PresetGeometry(
                              new A.AdjustValueList()
                            ) { Preset = A.ShapeTypeValues.Rectangle }))
                    ) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                )
                {
                    DistanceFromTop = (UInt32Value)0U,
                    DistanceFromBottom = (UInt32Value)0U,
                    DistanceFromLeft = (UInt32Value)0U,
                    DistanceFromRight = (UInt32Value)0U
                });

            ParagraphProperties pragProp = new ParagraphProperties();
            pragProp.AppendChild<Justification>(SetPargParagraphProperties("centre"));
            cell.AppendChild(new Paragraph(pragProp, new Run(element)));
        }
        #endregion

        #region Setting Property  for XML

        public static TableCellProperties getTableCellPropertyByFirstRow(WordprocessingDocument wDoc, int tableNum, int rowNum, int cellNum)
        {
            //create new TableCellProperty
            TableCellProperties tcProp = new TableCellProperties();

            try
            {
                // find the old TableCell from Column ( Mean first row) of Table
                Table tb = wDoc.MainDocumentPart.Document.Body.Descendants<Table>().ElementAt(tableNum);

                var getTableCellProp = tb.Descendants<TableRow>().ElementAt(rowNum).Descendants<TableCell>().ElementAt(cellNum).Descendants<TableCellProperties>().FirstOrDefault();

                if (getTableCellProp != null)
                {
                    //adding old table cell property to new table cell property;
                    tcProp = getTableCellProp;
                }
                else
                {
                    //adding new TableCellProp
                    tcProp.TableCellWidth = new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" };
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
            return tcProp;
        }
        public static TableCellProperties getTableCellPropertyByFirstRow(WordprocessingDocument wDoc, Table table, int rowNum, int cellNum)
        {
            //create new TableCellProperty
            TableCellProperties tcProp = new TableCellProperties();

            try
            {
                // find the old TableCell from Column ( Mean first row) of Table
                Table tb = table;

                var getTableCellProp = tb.Descendants<TableRow>().ElementAt(rowNum).Descendants<TableCell>().ElementAt(cellNum).Descendants<TableCellProperties>().FirstOrDefault();

                if (getTableCellProp != null)
                {
                    //adding old table cell property to new table cell property;
                    tcProp = getTableCellProp;
                }
                else
                {
                    //adding new TableCellProp
                    tcProp.TableCellWidth = new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" };
                }
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }
            return tcProp;
        }
        public static ParagraphProperties getParagraphPropertyByFirstRow(WordprocessingDocument wDoc, int tableNum, int rowNum, int cellNum)
        {
            ParagraphProperties paragrap = new ParagraphProperties();

            return paragrap;
        }
        public static RunProperties setRunProperty(string AsciiType, string EastType, string HighAnsitype, string FontSizes)
        {
            //Create RunProperty with Font and Size
            RunProperties runProp = new RunProperties();

            try
            {
                RunFonts runFont = new RunFonts();
                runFont.Ascii = AsciiType;
                runFont.EastAsia = EastType;
                runFont.HighAnsi = HighAnsitype;

                DocumentFormat.OpenXml.Wordprocessing.FontSize size = new DocumentFormat.OpenXml.Wordprocessing.FontSize();
                size.Val = new StringValue(FontSizes);

                //For example fixcode
                //runFont.Ascii = "Wingdings";
                //runFont.EastAsia = "Wingdings";
                //runFont.HighAnsi = "Wingdings";
                //size.Val = new StringValue("24");

                //adding property to runProp
                runProp.Append(runFont);
                runProp.Append(size);
            }
            catch (Exception ex)
            {
                Extension.LogWriter.Write(ex);
            }

            return runProp;
        }
        public static TableProperties setTableProperty(BorderValues borderVal)
        {
            // Create a TableProperties object and specify its border information.
            TableProperties tblProp = new TableProperties(
                new TableBorders(
                new TopBorder()
                {
                    Val = new EnumValue<BorderValues>(borderVal),
                    Size = 24
                },
                new BottomBorder()
                {
                    Val =
                    new EnumValue<BorderValues>(borderVal),
                    Size = 24
                },
                new LeftBorder()
                {
                    Val =
                    new EnumValue<BorderValues>(borderVal),
                    Size = 24
                },
                new RightBorder()
                {
                    Val =
                    new EnumValue<BorderValues>(borderVal),
                    Size = 24
                },
                new InsideHorizontalBorder()
                {
                    Val =
                    new EnumValue<BorderValues>(borderVal),
                    Size = 24
                },
                new InsideVerticalBorder()
                {
                    Val =
                    new EnumValue<BorderValues>(borderVal),
                    Size = 24
                }
                    )
                );
            return tblProp;
        }
        public static TableBorders setTableBorder(BorderValues borderVal)
        {
            TableBorders tbBorder = new TableBorders();

            tbBorder =
              new TableBorders(
              new TopBorder()
              {
                  Val = new EnumValue<BorderValues>(borderVal),
                  Size = 24
              },
              new BottomBorder()
              {
                  Val =
                  new EnumValue<BorderValues>(borderVal),
                  Size = 24
              },
              new LeftBorder()
              {
                  Val =
                  new EnumValue<BorderValues>(borderVal),
                  Size = 24
              },
              new RightBorder()
              {
                  Val =
                  new EnumValue<BorderValues>(borderVal),
                  Size = 24
              },
              new InsideHorizontalBorder()
              {
                  Val =
                  new EnumValue<BorderValues>(borderVal),
                  Size = 24
              },
              new InsideVerticalBorder()
              {
                  Val =
                  new EnumValue<BorderValues>(borderVal),
                  Size = 24
              }
                  );
            return tbBorder;
        }
        public static Justification SetPargParagraphProperties(string textalignment)
        {
            if (string.IsNullOrEmpty(textalignment))
                throw new ArgumentException("Value cannot be null or empty.", "textalignment");

            JustificationValues? justification = GetJustificationFromString(textalignment);

            return justification.HasValue ? (new Justification() { Val = justification.Value }) : new Justification();
        }
        public static JustificationValues? GetJustificationFromString(string alignment)
        {
            switch (alignment.ToLower())
            {
                case "centre": return JustificationValues.Center;
                case "droite": return JustificationValues.Right;
                case "gauche": return JustificationValues.Left;
                default: return null;
            }
        }
        public static string getTitleProperty(WordprocessingDocument wDoc)
        {
            string title = "";

            //var getfileProp = wDoc.CoreFilePropertiesPart.OpenXmlPackage.PackageProperties;

            //title = getfileProp.Title;

            return title;
        }
        public static void settingPropertyFile(WordprocessingDocument wDoc, string title = "", string description = "", string username = "")
        {
            //var getfileProp = wDoc.CoreFilePropertiesPart.OpenXmlPackage.PackageProperties;

            //getfileProp.Title = title;
            //getfileProp.Created = DateTime.Now;
            //getfileProp.Description = description;
            //getfileProp.LastModifiedBy = username;
        }
        public enum PropertyTypes : int
        {
            YesNo,
            Text,
            DateTime,
            NumberInteger,
            NumberDouble
        }
        public static string SetCustomProperty(
               string fileName,
               string propertyName,
               object propertyValue,
               PropertyTypes propertyType, WordprocessingDocument wDoc)
        {
            // Given a document name, a property name/value, and the property type, 
            // add a custom property to a document. The method returns the original
            // value, if it existed.

            string returnValue = null;

            var newProp = new CustomDocumentProperty();
            bool propSet = false;

            // Calculate the correct type.
            switch (propertyType)
            {
                case PropertyTypes.DateTime:

                    // Be sure you were passed a real date, 
                    // and if so, format in the correct way. 
                    // The date/time value passed in should 
                    // represent a UTC date/time.
                    if ((propertyValue) is DateTime)
                    {
                        newProp.VTFileTime =
                            new VTFileTime(string.Format("{0:s}Z",
                                Convert.ToDateTime(propertyValue)));
                        propSet = true;
                    }

                    break;

                case PropertyTypes.NumberInteger:
                    if ((propertyValue) is int)
                    {
                        newProp.VTInt32 = new VTInt32(propertyValue.ToString());
                        propSet = true;
                    }

                    break;

                case PropertyTypes.NumberDouble:
                    if (propertyValue is double)
                    {
                        newProp.VTFloat = new VTFloat(propertyValue.ToString());
                        propSet = true;
                    }

                    break;

                case PropertyTypes.Text:
                    newProp.VTLPWSTR = new VTLPWSTR(propertyValue.ToString());
                    propSet = true;

                    break;

                case PropertyTypes.YesNo:
                    if (propertyValue is bool)
                    {
                        // Must be lowercase.
                        newProp.VTBool = new VTBool(
                          Convert.ToBoolean(propertyValue).ToString().ToLower());
                        propSet = true;
                    }
                    break;
            }

            if (!propSet)
            {
                // If the code was not able to convert the 
                // property to a valid value, throw an exception.
                throw new InvalidDataException("propertyValue");
            }

            // Now that you have handled the parameters, start
            // working on the document.
            newProp.FormatId = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}";
            newProp.Name = propertyName;


            var customProps = wDoc.CustomFilePropertiesPart;
            if (customProps == null)
            {
                // No custom properties? Add the part, and the
                // collection of properties now.
                customProps = wDoc.AddCustomFilePropertiesPart();
                customProps.Properties =
                    new DocumentFormat.OpenXml.CustomProperties.Properties();
            }

            var props = customProps.Properties;
            if (props != null)
            {
                // This will trigger an exception if the property's Name 
                // property is null, but if that happens, the property is damaged, 
                // and probably should raise an exception.
                var prop =
                    props.Where(
                    p => ((CustomDocumentProperty)p).Name.Value
                        == propertyName).FirstOrDefault();

                // Does the property exist? If so, get the return value, 
                // and then delete the property.
                if (prop != null)
                {
                    returnValue = prop.InnerText;
                    prop.Remove();
                }

                // Append the new property, and 
                // fix up all the property ID values. 
                // The PropertyId value must start at 2.
                props.AppendChild(newProp);
                int pid = 2;
                foreach (CustomDocumentProperty item in props)
                {
                    item.PropertyId = pid++;
                }
                props.Save();
            }

            return returnValue;
        }
        #endregion
    }
}