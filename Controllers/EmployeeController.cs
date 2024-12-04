using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CasaToro.Novasoft.Fotos.Data;
using CasaToro.Novasoft.Fotos.Models;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;

namespace CasaToro.Novasoft.Fotos.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly NovasoftDbContext context;
        private readonly LogDbContext logContext;


        public EmployeeController(NovasoftDbContext context, LogDbContext logContext)
        {
            this.context = context;
            this.logContext = logContext;
        }

        // Acción para mostrar la vista principal de empleados
        public IActionResult Index()
        {
            try
            {
                // Obtener las unidades de negocio excluyendo ciertos códigos
                var businessUnits = context.BusinessUnits
                    .Where(bu => bu.code != "0" && bu.code != "23")
                    .OrderBy(bu => bu.name)
                    .ToList();
                ViewBag.BusinessUnits = businessUnits;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        // Acción para filtrar la lista de empleados
        public IActionResult Filter(string? nameCc, DateTime? fromDate, DateTime? toDate, string businessUnit, int hasCard)
        {
            try
            {
                // Filtrar la lista de empleados según los criterios proporcionados
                var filteredEmployees = context.Employees.AsQueryable();
                var cards = logContext.Cards.AsQueryable();

                var businessUnits = context.BusinessUnits
                    .Where(bu => bu.code != "0" && bu.code != "23")
                    .OrderBy(bu => bu.name)
                    .ToList();
                ViewBag.BusinessUnits = businessUnits;

                DateTime currentDate = DateTime.Now;

                // Filtrar por nombre o CC
                if (!string.IsNullOrEmpty(nameCc))
                {
                    var nameParts = nameCc.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in nameParts)
                    {
                        filteredEmployees = filteredEmployees.Where(e =>
                            (e.idEmployee.Contains(part) ||
                            e.nameEmployee.Contains(part) ||
                            e.surname1Employee.Contains(part) ||
                            e.surname2Employee.Contains(part)) &&
                            (e.dischargeDate >= currentDate || e.dischargeDate == null));
                    }
                }

                // Filtrar por rango de fechas
                if (fromDate.HasValue && toDate.HasValue)
                {
                    filteredEmployees = filteredEmployees.Where(e => e.entryDate >= fromDate && e.entryDate <= toDate && (e.dischargeDate >= currentDate || e.dischargeDate == null));
                }

                // Filtrar por unidad de negocio
                if (!string.IsNullOrEmpty(businessUnit) && !businessUnit.Equals("93"))
                {
                    filteredEmployees = filteredEmployees.Where(e => e.codeBusinessUnit == businessUnit && (e.dischargeDate >= currentDate || e.dischargeDate == null));
                }

                var employeesList = filteredEmployees.ToList();
                // Filtrar por empleados con o sin carné
                if (hasCard == 1)
                {
                    employeesList = employeesList.Where(e => cards.Any(c => c.idEmployee == e.idEmployee)).ToList();
                }
                else if (hasCard == 2)
                {
                    employeesList = employeesList.Where(e => !cards.Any(c => c.idEmployee == e.idEmployee)).ToList();
                }

                // Ordenar la lista de empleados por nombre
                var employees = employeesList.OrderBy(e => e.nameEmployee).ToList();

                // Pasar los valores de los filtros a la vista
                ViewBag.NameCc = nameCc;
                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
                ViewBag.SelectedBusinessUnit = businessUnit;
                ViewBag.SelectedHasCard = hasCard;

                return View("Index", employees);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error al aplicar filtros: {ex.Message}";
                return View("Error");
            }
        }

        // Acción para descargar la foto de un empleado
        public IActionResult DownloadPhoto(string idEmployee)
        {
            try
            {
                var employee = context.Employees.FirstOrDefault(e => e.idEmployee == idEmployee);
                if (employee == null || employee.photoEmployee == null)
                {
                    return View();
                }

                var photoBytes = employee.photoEmployee;
                var fileName = $"{employee.idEmployee}-{employee.nameEmployee.Replace(" ", "-")}-{employee.surname1Employee}-{employee.surname2Employee}.jpg";
                fileName = fileName.Replace(" ", "");
                //Registrar la descarga en el log
                string username = HttpContext.Session.GetString("Username");
                string name = HttpContext.Session.GetString("Name");

                LogDownload(employee.idEmployee, employee.nameEmployee, employee.surname1Employee, employee.surname2Employee, username, name);
                return File(photoBytes, "image/jpeg", fileName);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error al descargar: {ex.Message}";
                return View("Error");
            }
        }

        // Acción para descargar las fotos de los empleados seleccionados
        public IActionResult DownloadSelectedPhotos(string[] selectedEmployees)
        {
            try
            {
                var employees = context.Employees
                    .Where(e => selectedEmployees.Contains(e.idEmployee) && e.photoEmployee != null)
                    .ToList();

                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var employee in employees)
                        {

                            var fileName = $"{employee.idEmployee}-{employee.nameEmployee.Replace(" ", "-")}-{employee.surname1Employee}-{employee.surname2Employee}.jpg";
                            fileName = fileName.Replace(" ", "");
                            var photoEntry = archive.CreateEntry(fileName);
                            using (var entryStream = photoEntry.Open())
                            using (var photoStream = new MemoryStream(employee.photoEmployee))
                            {
                                photoStream.CopyTo(entryStream);
                            }
                            //Registrar la descarga en el log
                            string username = HttpContext.Session.GetString("Username");
                            string name = HttpContext.Session.GetString("Name");
                            LogDownload(employee.idEmployee, employee.nameEmployee, employee.surname1Employee, employee.surname2Employee, username, name);
                        }
                    }
                    return File(memoryStream.ToArray(), "application/zip", "FotosSeleccionadas.zip");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error al descargar fotos: {ex.Message}";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var businessUnits = context.BusinessUnits
                    .Where(bu => bu.code != "0" && bu.code != "23")
                    .OrderBy(bu => bu.name)
                    .ToList();
            ViewBag.BusinessUnits = businessUnits;
            if (file == null || file.Length == 0)
            {
                ViewBag.ErrorMessage = "Por favor, seleccione un archivo";
                return View("Index");
            }

            try
            {   
                ViewBag.Filename = file.FileName;
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                var processedIds = new HashSet<string>();

                if (fileExtension == ".csv")
                {
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    using (var csv = new CsvHelper.CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
                    {
                        csv.Context.RegisterClassMap<CardCsvMap>();
                        var records = csv.GetRecords<CardCsv>().ToList();

                        foreach (var record in records)
                        {
                            if (processedIds.Contains(record.idEmployee))
                            {
                                continue; // Saltar duplicados en el archivo
                            }

                            processedIds.Add(record.idEmployee);

                            var card = new Card
                            {
                                idEmployee = record.idEmployee,
                                nameEmployee = string.IsNullOrWhiteSpace(record.nameEmployee) ? "Nombre Desconocido" : record.nameEmployee,
                                deliveryDate = record.deliveryDate ?? DateTime.Now,
                                registrationDate = DateTime.Now
                            };

                            // Verificar si la entidad ya existe en el contexto
                            var existingCard = logContext.Cards.SingleOrDefault(c => c.idEmployee == card.idEmployee);
                            if (existingCard != null)
                            {
                                // Actualizar las propiedades de la entidad existente
                                existingCard.nameEmployee = card.nameEmployee;
                                existingCard.deliveryDate = card.deliveryDate;
                                existingCard.registrationDate = DateTime.Now;
                            }
                            else
                            {
                                // Agregar una nueva entidad
                                logContext.Cards.Add(card);
                            }
                        }

                        await logContext.SaveChangesAsync();
                    }
                }
                else if (fileExtension == ".xlsx" || fileExtension == ".xls")
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage(stream))
                        {
                            if (package.Workbook.Worksheets.Count == 0)
                            {
                                ViewBag.ErrorMessage = "Archvio no compatible o corrupto.";
                                return View("Index");
                            }

                            var worksheet = package.Workbook.Worksheets[0];
                            var rowCount = worksheet.Dimension.Rows;
                            var colCount = worksheet.Dimension.Columns;

                            if (rowCount == 0 || colCount == 0 )
                            {
                                ViewBag.ErrorMessage = "La hoja de cálculo está vacía.";
                                return View("Index");
                            }

                            var idEmployeeCol = -1;
                            var nameEmployeeCol = -1;
                            var deliveryDateCol = -1;

                            for (int col = 1; col <= colCount; col++)
                            {
                                var header = worksheet.Cells[1, col].Text.Trim();
                                if (header.Equals("Numero de Identificación", StringComparison.OrdinalIgnoreCase))
                                {
                                    idEmployeeCol = col;
                                }
                                else if (header.Equals("Nombres y Apellidos", StringComparison.OrdinalIgnoreCase))
                                {
                                    nameEmployeeCol = col;
                                }
                                else if (header.Equals("Fecha de Entrega", StringComparison.OrdinalIgnoreCase))
                                {
                                    deliveryDateCol = col;
                                }
                            }

                            if (idEmployeeCol == -1 || nameEmployeeCol == -1 || deliveryDateCol == -1)
                            {
                                ViewBag.ErrorMessage = "El archivo no contiene los encabezados requeridos.";
                                return View("Index");
                            }

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var idEmployee = worksheet.Cells[row, idEmployeeCol].Text;
                                var nameEmployee = worksheet.Cells[row, nameEmployeeCol].Text;
                                var deliveryDateText = worksheet.Cells[row, deliveryDateCol].Text;

                                // Valores por defecto
                                var defaultName = "Nombre Desconocido";
                                var defaultDate = DateTime.Now;
                                var defaultId = "0";

                                // Validar y asignar valores por defecto si es necesario
                                if (string.IsNullOrWhiteSpace(nameEmployee))
                                {
                                    nameEmployee = defaultName;
                                }

                                if (string.IsNullOrWhiteSpace(idEmployee))
                                {
                                    idEmployee = defaultId;
                                }

                                DateTime deliveryDate;
                                if (!DateTime.TryParse(deliveryDateText, out deliveryDate))
                                {
                                    deliveryDate = defaultDate;
                                }

                                if (processedIds.Contains(idEmployee))
                                {
                                    continue; // Saltar duplicados en el archivo
                                }

                                processedIds.Add(idEmployee);

                                // Verificar si la entidad ya existe en el contexto
                                var existingCard = logContext.Cards.SingleOrDefault(c => c.idEmployee == idEmployee);
                                if (existingCard != null)
                                {
                                    // Actualizar las propiedades de la entidad existente
                                    existingCard.nameEmployee = nameEmployee;
                                    existingCard.deliveryDate = deliveryDate;
                                    existingCard.registrationDate = DateTime.Now;
                                }
                                else
                                {
                                    // Agregar una nueva entidad
                                    var card = new Card
                                    {
                                        idEmployee = idEmployee,
                                        nameEmployee = nameEmployee,
                                        deliveryDate = deliveryDate,
                                        registrationDate = DateTime.Now
                                    };
                                    logContext.Cards.Add(card);
                                }
                            }

                            await logContext.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Formato de archivo no soportado.";
                    return View("Index");
                }
                ViewBag.SuccessMessage = "Archivo subido y procesado exitosamente.";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error al procesar el archivo: {ex.Message}";
                return View("Error");
            }
        }

        // Método para registrar las descargas en la table de logs
        private void LogDownload(string employeeId, string employeeName, string employeeSurname1, string employeeSurname2, string usrId, string usrName)
        {
            try
            {
                var log = new Log
                {
                    idUser = usrId,
                    nameUser = usrName.ToUpper(),
                    idEmployee = employeeId,
                    nameEmployee = $"{employeeName} {employeeSurname1} {employeeSurname2}",
                    downloadDate = DateTime.Now
                };

                logContext.Logs.Add(log);
                logContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guarda el log: " + ex.ToString());
            }

        }

    }

    public class CardCsvMap : ClassMap<Card>
    {
        public CardCsvMap()
        {
            Map(m => m.idEmployee).Name("Numero de Identificación");
            Map(m => m.nameEmployee).Name("Nombres y Apellidos");
            Map(m => m.deliveryDate).Name("Fecha de Entrega");
        }
    }

    public class CardCsv
    {
        public string idEmployee { get; set; }
        public string nameEmployee { get; set; }
        public DateTime? deliveryDate { get; set; }

    }
}