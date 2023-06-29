dotnet tool restore 

dotnet ef dbcontext scaffold "Name=StringConexionSisges" Microsoft.EntityFrameworkCore.SqlServer ^
-d -c SisgesDBContext --context-dir Model/SisGes/ -o Model/SisGes/   --force ^
--table [dbo].[VENTAS] ^
--table [dbo].[Usuario] ^
--table [dbo].[Persona] ^
--table [dbo].[Auditoria] ^
--table [dbo].[CENTROS_LOCAL] ^
--table [dbo].[InterfazPago] ^
--table [dbo].[IDOC_FI] ^
--table [dbo].[IDOC_PAGOS_FI] ^
--table [dbo].[TipoInterfaz] ^
--table [dbo].[InterfazAutorizacion] ^
--table [dbo].[EstadoSencillo] ^
--table [dbo].[DetalleSencillo] ^
--table [dbo].[EstadoSencillo] ^
--table [dbo].[MontoSencillo] ^
--table [dbo].[Remito] ^
--table [dbo].[Sencillos] ^
--table [dbo].[Sencillos_Tiendas] ^
--table [dbo].[SencillosSAP] ^
--table [dbo].[TipoTransaccionCAPA]

pause

exit
