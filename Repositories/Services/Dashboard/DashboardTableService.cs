using System.Linq.Expressions;
using System.Reflection;
using DataLibrary;
using Microsoft.EntityFrameworkCore;
using Repositories.Services.Dashboard.Interface;
using Repositories.Services.Report.Common.interfaces;
using ViewModels.Dashboard.Common.Table;
using ViewModels.Report.PendingOrder;
using System.Linq.Dynamic.Core;

namespace Repositories.Services.Dashboard
{
    public class DashboardTableService : IDashboardTableService
    {
        private readonly IReportServiceQueries _reportServiceQueries;
        private readonly ApplicationDbContext _db;

        public DashboardTableService(IReportServiceQueries reportServiceQueries, ApplicationDbContext db)
        {
            _reportServiceQueries = reportServiceQueries;
            _db = db;
        }

        public async Task<DashboardTableDataViewModel<PendingOrderReportViewModel>> GetPendingOrderTableData()
        {
            try
            {
                var response = new DashboardTableDataViewModel<PendingOrderReportViewModel>();
                var queryable = await _reportServiceQueries.FilterPendingOrderReportQuery<PendingOrderReportViewModel>(new PendingOrderReportSearchViewModel());
                response.TableData = await queryable.OrderByDescending(x => x.Date).Take(10).ToListAsync();
                return response;
            }
            catch (Exception ex)
            {
            }
            return new DashboardTableDataViewModel<PendingOrderReportViewModel>();
        }

        public async Task<bool> SaveDatatableCell(string propertyName, string propertyValue, string entityId, string entityName)
        {
            try
            {
                //// Get the DbSet dynamically using reflection
               
                var modelsAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("Models")).FirstOrDefault();
                Type entityType = modelsAssembly?.GetTypes()?.FirstOrDefault(t => t.Name == entityName); // Change entityName to "Asset"
                if (entityType == null)
                {
                    throw new ArgumentException("Invalid entityName");
                }

                // Get the DbSet dynamically
                MethodInfo setMethod = typeof(ApplicationDbContext)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == nameof(ApplicationDbContext.Set) && m.GetParameters().Length == 0);

                MethodInfo genericSetMethod = setMethod.MakeGenericMethod(entityType);
                object dbSet = genericSetMethod.Invoke(_db, null);

                // Create a parameter expression for the LINQ expression
                var record = await ((IQueryable<object>)dbSet).Where($"Id == {entityId}").FirstOrDefaultAsync();

                if (record == null)
                {
                    throw new ArgumentException("Record not found");
                }

                // Use reflection to set the property value
                PropertyInfo propertyInfo = record.GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    // Convert propertyValue to the appropriate type
                    var targetType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                    if (targetType == null) targetType = propertyInfo.PropertyType;

                    object safeValue = (propertyValue == null || string.IsNullOrWhiteSpace(propertyValue.ToString()))
                                        ? null
                                        : Convert.ChangeType(propertyValue, targetType);

                    propertyInfo.SetValue(record, safeValue);
                }
                else
                {
                    throw new ArgumentException("Invalid propertyName");
                }

                // Save changes to the database
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return false;
            }
        }


    }
}
