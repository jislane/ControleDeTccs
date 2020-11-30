using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using SistemaDeControleDeTCCs.Models;

namespace SistemaDeControleDeTCCs.Controllers
{
    public class RelatorioAtividades : Report
    {
        public RelatorioAtividades()
        {
            Paisagem = false;
        }

        public override void MontaCorpoDados(List<Tcc> tccs, List<Banca> bancas, string semestre)
        {
            base.MontaCorpoDados(tccs, bancas, semestre);

            BaseColor preto = new BaseColor(0, 0, 0);
            BaseColor fundo = new BaseColor(200, 200, 200);
            Font font = FontFactory.GetFont("Verdana", 8, Font.NORMAL, preto);
            Font titulo = FontFactory.GetFont("Verdana", 8, Font.BOLD, preto);
            Font titulo2 = FontFactory.GetFont("Verdana", 12, Font.BOLD, preto);

            PdfPTable micros = new PdfPTable(1);
            micros.TotalWidth = doc.PageSize.Width - (doc.LeftMargin + doc.RightMargin);
            PdfPCell cell2 = new PdfPCell(new Phrase("BANCAS DE TCC " + semestre, titulo2));
            cell2.Border = 0;
            cell2.HorizontalAlignment = Element.ALIGN_CENTER;
            cell2.PaddingBottom = 20f;
            cell2.PaddingTop = 30f;
            micros.AddCell(cell2);

            doc.Add(micros);

            if(tccs != null && tccs.Count > 0)
            {
                foreach (Tcc tcc in tccs)
                {
                    PdfPTable dadosDiscente = new PdfPTable(1);
                    dadosDiscente.WidthPercentage = 100f;
                    float[] sizes = new float[] { 12f };
                    dadosDiscente.SetWidths(sizes);
                    dadosDiscente.HorizontalAlignment = Element.ALIGN_LEFT;

                    cell2 = new PdfPCell(new Phrase("DISCENTE: " + tcc.Usuario.Nome.ToUpper() + " " + tcc.Usuario.Sobrenome.ToUpper(), font));
                    cell2.Colspan = 2;
                    cell2.Border = 0;
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    dadosDiscente.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase("ORIENTADOR(A): " + bancas.Where(x => x.TccId == tcc.TccId).FirstOrDefault().Usuario.Nome.ToUpper() + " " + bancas.Where(x => x.TccId == tcc.TccId).FirstOrDefault().Usuario.Sobrenome.ToUpper(), font));
                    cell2.Border = 0;
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    dadosDiscente.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase("DATA DA APRESENTAÇÃO: " + tcc.DataApresentacao.Value.ToString("dd/MM/yyyy HH:mm"), font));
                    cell2.Colspan = 2;
                    cell2.Border = 0;
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    dadosDiscente.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase("LOCAL DE APRESENTAÇÃO: " + tcc.LocalApresentacao, font));
                    cell2.Border = 0;
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    dadosDiscente.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase("TEMA: " + tcc.Tema, font));
                    cell2.Border = 0;
                    cell2.Colspan = 2;
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    dadosDiscente.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase("RESUMO: " + tcc.Resumo, font));
                    cell2.Border = 0;
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell2.PaddingBottom = 10f;
                    dadosDiscente.AddCell(cell2);

                    doc.Add(dadosDiscente);
                }
            }
            else
            {
                PdfPTable dadosDiscente = new PdfPTable(1);
                dadosDiscente.WidthPercentage = 100f;
                float[] sizes = new float[] { 12f };
                dadosDiscente.SetWidths(sizes);
                dadosDiscente.HorizontalAlignment = Element.ALIGN_LEFT;

                cell2 = new PdfPCell(new Phrase("Nenhuma banca de TCC encontrada", font));
                cell2.Colspan = 2;
                cell2.Border = 0;
                cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                dadosDiscente.AddCell(cell2);

                doc.Add(dadosDiscente);
            }
        }
    }
}
