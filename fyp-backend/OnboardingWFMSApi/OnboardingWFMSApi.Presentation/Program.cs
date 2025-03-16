using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataAccess;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IOrganisationRepository, OrganisationRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IOrganisationAdminLinkRepository, OrganisationAdminLinkRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<ITaskTypeRepository, TaskTypeRepository>();
builder.Services.AddScoped<ITaskTemplateRepository, TaskTemplateRepository>();

builder.Services.AddScoped<IFileUploadTaskTemplateRepository, FileUploadTaskTemplateRepository>();
builder.Services.AddScoped<IReadDocumentTaskTemplateRepository, ReadDocumentTaskTemplateRepository>();
builder.Services.AddScoped<IChecklistTaskTemplateRepository, ChecklistTaskTemplateRepository>();

builder.Services.AddScoped<ITaskInstanceRepository, TaskInstanceRepository>();

builder.Services.AddScoped<IChecklistTaskInstanceRepository, ChecklistTaskInstanceRepository>();
builder.Services.AddScoped<IReadDocumentTaskInstanceRepository, ReadDocumentTaskInstanceRepository>();
builder.Services.AddScoped<IFileUploadTaskInstanceRepository, FileUploadTaskInstanceRepository>();

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

builder.Services.AddScoped<IWorkflowTemplateRepository, WorkflowTemplateRepository>();
builder.Services.AddScoped<IWorkflowTemplateNodeRepository, WorkflowTemplateNodeRepository>();
builder.Services.AddScoped<INodeTaskDependencyRepository, NodeTaskDependencyRepository>();

builder.Services.AddScoped<IWorkflowInstanceRepository, WorkflowInstanceRepository>();

builder.Services.AddScoped<IOrganisationLogic, OrganisationLogic>();
builder.Services.AddScoped<IDepartmentLogic, DepartmentLogic>();
builder.Services.AddScoped<IAccountLogic, AccountLogic>();
builder.Services.AddScoped<IAuthLogic, AuthLogic>();
builder.Services.AddScoped<ITaskTemplateLogic, TaskTemplateLogic>();
builder.Services.AddScoped<ITaskInstanceLogic, TaskInstanceLogic>();
builder.Services.AddScoped<IDocumentLogic, DocumentLogic>();


builder.Services.AddScoped<IWorkflowTemplateLogic, WorkflowTemplateLogic>();
builder.Services.AddScoped<IWorkflowInstanceLogic, WorkflowInstanceLogic>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var jwtKey = builder.Configuration["Auth:Key"];
var jwtIssuer = builder.Configuration["Auth:Issuer"];
var jwtAudience = builder.Configuration["Auth:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://your-identity-provider";
        options.Audience = jwtAudience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()   // Allow requests from anywhere
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
