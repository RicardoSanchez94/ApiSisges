using OfficeOpenXml;
using OfficeOpenXml.Style;
using ServicioApi.Model.Clases;
using ServicioApi.Model.SisGes;
using System.ComponentModel;
using System.Globalization;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace ServicioApi.Model.Metodos
{
    public class LibroExcel
    {
        public ExcelPackage ReporteUsuario(List<Persona> lst)
        {
            //*************************************************************
            //FileInfo newFile = new FileInfo("C:\\excel\\Centros de costos.xlsx");
            //BORRAMOS EL ARCHIVO SI EXISTE, SI NO CREA UNO NUEVO.
            //newFile.Delete();
            
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();
            //Propiedades Hoja de excel
            //var zona = lst.Select(x => x.nombreZona).Distinct();


            var sheet = excelPackage.Workbook.Worksheets.Add("Lista Personas");
            //sheet.Name = item;

            //********************************************************************************************************
            //encabezados tabla de datos
            var rowindex = 1;

            sheet.Cells[rowindex, 1].Value = "Id";
            sheet.Cells[rowindex, 2].Value = "Rut";
            sheet.Cells[rowindex, 3].Value = "Nombre";
            sheet.Cells[rowindex, 4].Value = "Apellido Paterno";
            sheet.Cells[rowindex, 5].Value = "Apellido Materno";
            

            sheet.Column(1).AutoFit(20);
            sheet.Column(2).AutoFit(18);
            sheet.Column(3).AutoFit(13);
            sheet.Column(4).AutoFit(13);
            sheet.Column(5).AutoFit(10);
            //sheet.Column(6).AutoFit(30);
            //sheet.Column(7).AutoFit(13);
            //sheet.Column(8).AutoFit(30);
            //sheet.Column(9).AutoFit(30);
            //sheet.Column(10).AutoFit(30);
            //sheet.Column(11).AutoFit(30);
            //sheet.Column(12).AutoFit(30);
            //sheet.Column(13).AutoFit(30);


            sheet.Cells[rowindex, 1, rowindex, 13].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 13].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 13].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            //var datosPlan = lst;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;

            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in lst)
            {

                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.Id;
                sheet.Cells[rowindex, col++].Value = itemDatos.Run;
                sheet.Cells[rowindex, col++].Value = itemDatos.Nombre;
                sheet.Cells[rowindex, col++].Value = itemDatos.ApellidoPaterno;
                sheet.Cells[rowindex, col++].Value = itemDatos.ApellidoMaterno;                
                
                rowindex++;
            }


            //rowindex = 2;
            sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            var bordes = sheet.Cells[2, 1, rowindex + 1, 13].Style.Border;
            bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;

            return excelPackage;
        }

        public ExcelPackage FI(List<FIMP> fi)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();

            var sheet = excelPackage.Workbook.Worksheets.Add("FI");
            //sheet.Name = item.ToString();
            var rowindex = 1;

            //sheet.Cells[rowindex, 1].Value = "Autorizador";
            //sheet.Cells[rowindex, 2].Value = "Fecha Transanccion";
            //sheet.Cells[rowindex, 3].Value = "CodigoLocal";
            //sheet.Cells[rowindex, 4].Value = "Tienda";
            //sheet.Cells[rowindex, 5].Value = "Monto MP";
            //sheet.Cells[rowindex, 6].Value = "Monto TBK";
            //sheet.Cells[rowindex, 7].Value = "Fecha Conciliacion";
            //sheet.Cells[rowindex, 8].Value = "Tarjeta MP";
            //sheet.Cells[rowindex, 9].Value = "Tarjeta TBK";

            //sheet.Column(1).AutoFit(20);
            //sheet.Column(2).AutoFit(12);
            //sheet.Column(3).AutoFit(13);
            //sheet.Column(4).AutoFit(7);
            //sheet.Column(5).AutoFit(13);
            //sheet.Column(6).AutoFit(13);
            //sheet.Column(7).AutoFit(15);
            //sheet.Column(8).AutoFit(20);
            //sheet.Column(9).AutoFit(20);

            //sheet.Cells[rowindex, 1, rowindex, 9].Style.Font.Bold = true;
            //sheet.Cells[rowindex, 1, rowindex, 9].AutoFilter = true;

            //var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 9].Style.Border;
            //bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;


            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = fi;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 1;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan.Where(x => x.WRBTR != 0))
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.MANDT;
                sheet.Cells[rowindex, col++].Value = itemDatos.INTERFAZ;
                sheet.Cells[rowindex, col++].Value = itemDatos.FECHAINT;
                sheet.Cells[rowindex, col++].Value = itemDatos.CORRELAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.NITEM;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUKRS;
                sheet.Cells[rowindex, col++].Value = itemDatos.BLART;
                sheet.Cells[rowindex, col++].Value = itemDatos.WAERS;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUDAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.BLDAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.MONAT;
                sheet.Cells[rowindex, col++].Value = itemDatos.XBLNR;
                sheet.Cells[rowindex, col++].Value = itemDatos.BKTXT;
                sheet.Cells[rowindex, col++].Value = itemDatos.BUPLA;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBS;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWKO;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWUM;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBK;
                sheet.Cells[rowindex, col++].Value = itemDatos.WRBTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.FWBAS;
                sheet.Cells[rowindex, col++].Value = itemDatos.MWSKZ;
                sheet.Cells[rowindex, col++].Value = itemDatos.GSBER;
                sheet.Cells[rowindex, col++].Value = itemDatos.KOSTL_PRCTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.AUFNR;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZTERM;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZUONR;
                sheet.Cells[rowindex, col++].Value = itemDatos.SGTXT;
                sheet.Cells[rowindex, col++].Value = itemDatos.VBUND;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF1;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF2;
                sheet.Cells[rowindex, col++].Value = itemDatos.XREF3;
                sheet.Cells[rowindex, col++].Value = itemDatos.VALUT;
                sheet.Cells[rowindex, col++].Value = itemDatos.XMWST;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZLSPR;
                sheet.Cells[rowindex, col++].Value = itemDatos.ZFBDT;
                sheet.Cells[rowindex, col++].Value = itemDatos.MANSP;
                sheet.Cells[rowindex, col++].Value = itemDatos.NEWBW;
                sheet.Cells[rowindex, col++].Value = itemDatos.MENGE;
                sheet.Cells[rowindex, col++].Value = itemDatos.KURSF;
                sheet.Cells[rowindex, col++].Value = itemDatos.WWERT;
                sheet.Cells[rowindex, col++].Value = itemDatos.PRCTR;
                sheet.Cells[rowindex, col++].Value = itemDatos.SKFBT;
                sheet.Cells[rowindex, col++].Value = itemDatos.NAME1;
                sheet.Cells[rowindex, col++].Value = itemDatos.ORT01;
                sheet.Cells[rowindex, col++].Value = itemDatos.STCD1;
                sheet.Cells[rowindex, col++].Value = itemDatos.EBELN;
                sheet.Cells[rowindex, col++].Value = itemDatos.WERKS;
                rowindex++;
            }

            //sheet.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            //sheet.Column(7).Style.Numberformat.Format = "dd-MM-yyyy hh:mm";
            //var bordes = sheet.Cells[2, 1, rowindex - 1, 9].Style.Border;
            //bordes.Top.Style = bordes.Right.Style = bordes.Bottom.Style = bordes.Left.Style = ExcelBorderStyle.Thin;
            //var bordesTotal = sheet.Cells[rowindex, 4, rowindex, 6].Style.Border;
            //bordesTotal.Top.Style = bordesTotal.Right.Style = bordesTotal.Bottom.Style = bordesTotal.Left.Style = ExcelBorderStyle.Thin;
            return excelPackage;
        }

        
    }
}
