using iTextSharp.text;
using iTextSharp.text.pdf;
using SistemaDeControleDeTCCs.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace SistemaDeControleDeTCCs.Controllers
{
    public abstract class Report
    {
        protected Document doc;
        PdfWriter writer;
        MemoryStream output;

        public string PageTitle { get; set; }
        public string PageSubTitle { get; set; }
        public string PageSubLogo { get; set; }
        public string BasePath { get; set; }
        public bool ImprimirCabecalhoPadrao { get; set; }
        public bool ImprimirRodapePadrao { get; set; }
        public bool Paisagem { get; set; }

        public Report()
        {
            InicializaVariaveis();
        }

        private void InicializaVariaveis()
        {
            ImprimirCabecalhoPadrao = true;
            ImprimirRodapePadrao = true;
            PageTitle = string.Empty;
            PageSubTitle = string.Empty;
            BasePath = string.Empty;
            Paisagem = false;
        }

        public MemoryStream GetOutput(List<Tcc> tccs, List<Banca> bancas, string semestre)
        {
            MontaCorpoDados(tccs, bancas, semestre);

            if (output == null || output.Length == 0)
            {
                throw new Exception("Sem dados para exibir.");
            }

            try
            {
                writer.Flush();

                if (writer.PageEmpty)
                {
                    doc.Add(new Paragraph("Nenhum registro para listar."));
                }

                doc.Close();
            }
            catch(Exception e)

            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                doc = null;
                writer = null;
            }

            return output;
        }

        public virtual void MontaCorpoDados(List<Tcc> tccs, List<Banca> bancas, string semestre)
        {
            if (!Paisagem)
            {
                doc = new Document(PageSize.A4, 20, 20, 70, 70);
            }
            else
            {
                doc = new Document(PageSize.A4.Rotate(), 20, 20, 80, 80);
            }
            output = new MemoryStream();
            writer = PdfWriter.GetInstance(doc, output);

            doc.AddAuthor("CBSI");
            doc.AddTitle(PageTitle);
            doc.AddSubject(PageTitle);

            var footer = new MSPDFFooter();
            footer.PageTitle = PageTitle;
            footer.PageSubTitle = PageSubTitle;
            footer.BasePath = BasePath;
            footer.ImprimirCabecalhoPadrao = ImprimirCabecalhoPadrao;
            footer.ImprimirRodapePadrao = ImprimirRodapePadrao;

            writer.PageEvent = footer;

            doc.Open();

            return;
        }

        protected PdfPCell getNewCell(string Texto, Font Fonte, int Alinhamento, float Espacamento, int Borda, BaseColor CorBorda, BaseColor CorFundo)
        {
            var cell = new PdfPCell(new Phrase(Texto, Fonte));
            cell.HorizontalAlignment = Alinhamento;
            cell.Padding = Espacamento;
            cell.Border = Borda;
            cell.BorderColor = CorBorda;
            cell.BackgroundColor = CorFundo;

            return cell;
        }

        protected PdfPCell getNewCell(string Texto, Font Fonte, int Alinhamento, float Espacamento, int Borda, BaseColor CorBorda)
        {
            return getNewCell(Texto, Fonte, Alinhamento, Espacamento, Borda, CorBorda, new BaseColor(255, 255, 255));
        }
        protected PdfPCell getNewCell(string Texto, Font Fonte, int Alinhamento = 0, float Espacamento = 5, int Borda = 0)
        {
            return getNewCell(Texto, Fonte, Alinhamento, Espacamento, Borda, new BaseColor(0, 0, 0), new BaseColor(255, 255, 255));
        }
    }

    public class MSPDFFooter : PdfPageEventHelper
    {
        public string PageTitle { get; set; }
        public string PageSubTitle { get; set; }
        public string PageSubLogo { get; set; }
        public string BasePath { get; set; }
        public bool ImprimirCabecalhoPadrao { get; set; }
        public bool ImprimirRodapePadrao { get; set; }

        public override void OnOpenDocument(PdfWriter writer, Document doc)
        {
            base.OnOpenDocument(writer, doc);
        }

        public override void OnStartPage(PdfWriter writer, Document doc)
        {
            base.OnStartPage(writer, doc);

            ImprimeCabecalho(writer, doc);
        }

        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            base.OnEndPage(writer, doc);

            ImprimeRodape(writer, doc);
        }

        private void ImprimeRodape(PdfWriter writer, Document doc)
        {
            #region Dados do Rodapé
            if (ImprimirRodapePadrao)
            {
                BaseColor preto = new BaseColor(0, 0, 0);
                Font font = FontFactory.GetFont("Verdana", 8, Font.NORMAL, preto);
                Font negrito = FontFactory.GetFont("Verdana", 8, Font.BOLD, preto);
                float[] sizes = new float[] { 1.0f, 5.5f };

                PdfPTable table = new PdfPTable(2);
                table.TotalWidth = doc.PageSize.Width - (doc.LeftMargin + doc.RightMargin);
                table.SetWidths(sizes);

                #region Coluna TNE
                Image foot = Image.GetInstance(BasePath + @"\wwwroot\img\ControleTCC-IFS-Menu.png");
                foot.ScalePercent(60);

                PdfPCell cell = new PdfPCell(foot);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.BorderWidthTop = 1.5f;
                cell.PaddingLeft = 10f;
                cell.PaddingTop = 10f;
                table.AddCell(cell);
                #endregion

                #region Página
                PdfPTable micros = new PdfPTable(1);
                micros = new PdfPTable(1);
                cell = new PdfPCell(new Phrase("Relatório emitido pelo Sistema de Controle de TCC - CBSI - IFS", font));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                micros.AddCell(cell);

                cell = new PdfPCell(new Phrase("Data/Hora: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), font));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                micros.AddCell(cell);

                cell = new PdfPCell(micros);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.BorderWidthTop = 1.5f;
                cell.PaddingTop = 10f;
                table.AddCell(cell);
                #endregion

                table.WriteSelectedRows(0, -1, doc.LeftMargin, 70, writer.DirectContent);
            }
            #endregion 
        }

        private void ImprimeCabecalho(PdfWriter writer, Document doc)
        {
            #region Dados do Cabeçalho
            if (ImprimirCabecalhoPadrao)
            {
                BaseColor preto = new BaseColor(0, 0, 0);
                Font font = FontFactory.GetFont("Verdana", 8, Font.NORMAL, preto);
                Font titulo = FontFactory.GetFont("Verdana", 12, Font.BOLD, preto);
                float[] sizes = new float[] { 1f, 4f };

                PdfPTable table = new PdfPTable(2);
                table.TotalWidth = doc.PageSize.Width - (doc.LeftMargin + doc.RightMargin);
                table.SetWidths(sizes);

                #region Logo
                Image foot;
                if (File.Exists(BasePath + @"\PublicResources\" + PageSubLogo))
                {
                    foot = Image.GetInstance(BasePath + @"\PublicResources\" + PageSubLogo);
                }
                else
                {
                    foot = Image.GetInstance(BasePath + @"\wwwroot\img\logoCBSI.png");
                }
                foot.ScalePercent(60);

                PdfPCell cell = new PdfPCell(foot);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                cell.BorderWidthTop = 1.5f;
                //cell.BorderWidthBottom = 1.5f;
                cell.PaddingTop = 10f;
                //cell.PaddingBottom = 80f;
                table.AddCell(cell);

                PdfPTable micros = new PdfPTable(1);

                cell = new PdfPCell(new Phrase(PageTitle, titulo));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                micros.AddCell(cell);
                cell = new PdfPCell(new Phrase(PageSubTitle, font));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cell.PaddingBottom = 80f;
                micros.AddCell(cell);

                cell = new PdfPCell(micros);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.BorderWidthTop = 1.5f;
                //cell.BorderWidthBottom = 1.5f;
                cell.PaddingTop = 10f;
                //cell.PaddingBottom = 80f;
                table.AddCell(cell);
                #endregion

                #region Página
                /*
                micros = new PdfPTable(1);
                cell = new PdfPCell(new Phrase("Página: " + (doc.PageNumber).ToString(), font));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                micros.AddCell(cell);
                
                cell = new PdfPCell(micros);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.BorderWidthTop = 1.5f;
                cell.BorderWidthBottom = 1.5f;
                cell.PaddingTop = 10f;              
                table.AddCell(cell);
                */
                #endregion

                table.WriteSelectedRows(0, -1, doc.LeftMargin, (doc.PageSize.Height - 20), writer.DirectContent);
            }
            #endregion
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
        }
    }
}
