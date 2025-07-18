using OfficeOpenXml;
using DispersionMailerDesktop.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;

namespace DispersionMailerDesktop.Helpers
{
    public static class ExcelHelper
    {
        public static List<DestinatarioInfo> CargarDestinatariosDesdeExcel( string rutaArchivoExcel )
        {
            var lista = new List<DestinatarioInfo>( );

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using(var package = new ExcelPackage( new FileInfo( rutaArchivoExcel ) ))
            {
                var worksheet = package.Workbook.Worksheets.First( );
                int rowCount = worksheet.Dimension.Rows;

                for(int row = 2; row <= rowCount; row++)
                {
                    var contrato = worksheet.Cells[ row, 1 ].Text.Trim( );
                    var tipoEnvio = worksheet.Cells[ row, 2 ].Text.Trim( );
                    var correo = worksheet.Cells[ row, 3 ].Text.Trim( );

                    if(!string.IsNullOrEmpty( contrato ) && !string.IsNullOrEmpty( correo ))
                    {
                        lista.Add( new DestinatarioInfo
                        {
                            Contrato = contrato,
                            TipoDeEnvio = tipoEnvio,
                            Correo = correo
                        } );
                    }
                }
            }

            return lista;
        }
    }
}
