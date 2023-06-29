using OfficeOpenXml.Style;
using OfficeOpenXml;
using ServicioApi.Model.SisGes;

namespace ServicioApi.Model.Clases
{
    public class Execelprueba
    {
        public ExcelPackage ConsumoAPI(List<ApiGuardar> Pago)
        {

            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage();
            var sheet = excelPackage.Workbook.Worksheets.Add("Conciliadas Automaticas Pago");
            //sheet.Name = item.ToString();
            var rowindex = 1;
            sheet.Cells[rowindex, 1].Value = "codigoAutorizacion";
            sheet.Cells[rowindex, 2].Value = "codigoLocal";
            sheet.Cells[rowindex, 3].Value = "fechaAbono";
            sheet.Cells[rowindex, 4].Value = "fechaTransaccion";
            sheet.Cells[rowindex, 5].Value = "montoCuotas";
            sheet.Cells[rowindex, 6].Value = "montoVenta";
            sheet.Cells[rowindex, 7].Value = "nombreLocal";
            sheet.Cells[rowindex, 8].Value = "numeroTarjeta";
            sheet.Cells[rowindex, 9].Value = "tipoProducto";
            sheet.Cells[rowindex, 10].Value = "tipoTransaccion";
            sheet.Cells[rowindex, 11].Value = "totalCuotas";
            sheet.Cells[rowindex, 12].Value = "tipoCuota";
            sheet.Cells[rowindex, 13].Value = "montoAfecto";
            sheet.Cells[rowindex, 14].Value = "montoExentoTotal";
            sheet.Cells[rowindex, 15].Value = "ordenPedido";
            sheet.Cells[rowindex, 16].Value = "Estado";

            sheet.Column(1).AutoFit(10);
            sheet.Column(2).AutoFit(10);
            sheet.Column(3).AutoFit(30);
            sheet.Column(4).AutoFit(11);
            sheet.Column(5).AutoFit(10);
            sheet.Column(6).AutoFit(3);
            sheet.Column(7).AutoFit(10);
            sheet.Column(8).AutoFit(11);

            sheet.Cells[rowindex, 1, rowindex, 16].Style.Font.Bold = true;
            sheet.Cells[rowindex, 1, rowindex, 16].AutoFilter = true;

            var bordesEncabezados = sheet.Cells[rowindex, 1, rowindex, 16].Style.Border;
            bordesEncabezados.Top.Style = bordesEncabezados.Right.Style = bordesEncabezados.Bottom.Style = bordesEncabezados.Left.Style = ExcelBorderStyle.Medium;

            // SE CARGAN LOS DATOS DE TODO EL DOCUMENTO
            var datosPlan = Pago;
            var col = 0;
            //Empezamos a escribir sobre ella
            rowindex = 2;
            col = 1;
            //sheet.Cells[rowindex++, col].Value = item;
            foreach (var itemDatos in datosPlan)
            {
                col = 1;
                sheet.Cells[rowindex, col++].Value = itemDatos.codigoAutorizacion;
                sheet.Cells[rowindex, col++].Value = itemDatos.codigoLocal;
                sheet.Cells[rowindex, col++].Value = (itemDatos.fechaAbono.Value.Year == 0001 ? null : itemDatos.fechaAbono);
                sheet.Cells[rowindex, col++].Value = itemDatos.fechaTransaccion;
                sheet.Cells[rowindex, col++].Value = itemDatos.montoCuotas;
                sheet.Cells[rowindex, col++].Value = itemDatos.montoVenta;
                sheet.Cells[rowindex, col++].Value = itemDatos.nombreLocal.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
                sheet.Cells[rowindex, col++].Value = itemDatos.numeroTarjeta;
                sheet.Cells[rowindex, col++].Value = itemDatos.tipoProducto;
                sheet.Cells[rowindex, col++].Value = itemDatos.tipoTransaccion;
                sheet.Cells[rowindex, col++].Value = itemDatos.totalCuotas;
                sheet.Cells[rowindex, col++].Value = itemDatos.tipoCuota;
                sheet.Cells[rowindex, col++].Value = itemDatos.montoAfecto;
                sheet.Cells[rowindex, col++].Value = itemDatos.montoExentoTotal;
                sheet.Cells[rowindex, col++].Value = itemDatos.ordenPedido;



                rowindex++;
            }

            return excelPackage;
        }
    }
}
