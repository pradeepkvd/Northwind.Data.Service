using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.Data.Service
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter" />
    public class FileUploadOperation : IOperationFilter
    {
        /// <summary>
        /// Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The context.</param>
        public void Apply(Swashbuckle.AspNetCore.Swagger.Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId.ToLower() == "apiinboundpost")
            {
                var file = operation.Parameters.Where(q => q.Name.ToLower() == "file").FirstOrDefault();
                if (file != null)
                {
                    operation.Parameters.Remove(file);
                    //operation.Parameters.Clear();
                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "file",
                        In = "formData",
                        Description = "Upload File",
                        Required = true,
                        Type = "file"
                    });
                    operation.Consumes.Add("multipart/form-data");
                }
            }
        }
    }
}
