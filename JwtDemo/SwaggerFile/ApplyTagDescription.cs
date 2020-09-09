using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JwtDemo.SwaggerFile
{
    public class ApplyTagDescription : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new List<OpenApiTag>
            {
                new OpenApiTag {Name = "WeatherForecast", Description = "天气测试"},
                new OpenApiTag {Name = "Authentication", Description = "获取Token"}
            };
        }
    }
}