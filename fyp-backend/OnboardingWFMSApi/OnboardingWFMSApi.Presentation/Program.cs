using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.BusinessLogic.AccountLogic;
using OnboardingWFMSApi.BusinessLogic.Factories;
using OnboardingWFMSApi.BusinessLogic.Handlers.CustomAuthHandlers;
using OnboardingWFMSApi.BusinessLogic.Handlers.MediatRHandlers;
using OnboardingWFMSApi.BusinessLogic.Handlers.TaskInstanceHandlers;
using OnboardingWFMSApi.BusinessLogic.Handlers.TaskTemplateHandlers;
using OnboardingWFMSApi.BusinessLogic.NotificationLogic;
using OnboardingWFMSApi.BusinessLogic.ReportedIssuesLogic;
using OnboardingWFMSApi.BusinessLogic.TaskInstanceLogic;
using OnboardingWFMSApi.BusinessLogic.TaskTemplateLogic;
using OnboardingWFMSApi.BusinessLogic.WorkflowInstanceLogic;
using OnboardingWFMSApi.BusinessLogic.WorkflowTemplateLogic;
using OnboardingWFMSApi.DataAccess;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IOrganisationRepository, OrganisationRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<ITaskTypeRepository, TaskTypeRepository>();
builder.Services.AddScoped<ITaskTemplateRepository, TaskTemplateRepository>();

builder.Services.AddScoped<IFileUploadTaskTemplateRepository, FileUploadTaskTemplateRepository>();
builder.Services.AddScoped<IReadDocumentTaskTemplateRepository, ReadDocumentTaskTemplateRepository>();
builder.Services.AddScoped<IChecklistTaskTemplateRepository, ChecklistTaskTemplateRepository>();
builder.Services.AddScoped<IProjectTaskTemplateRepository, ProjectTaskTemplateRepository>();
builder.Services.AddScoped<IFeedbackTaskTemplateRepository, FeedbackTaskTemplateRepository>();

builder.Services.AddScoped<ITaskInstanceRepository, TaskInstanceRepository>();

builder.Services.AddScoped<IChecklistTaskInstanceRepository, ChecklistTaskInstanceRepository>();
builder.Services.AddScoped<IReadDocumentTaskInstanceRepository, ReadDocumentTaskInstanceRepository>();
builder.Services.AddScoped<IFileUploadTaskInstanceRepository, FileUploadTaskInstanceRepository>();
builder.Services.AddScoped<IProjectTaskInstanceRepository, ProjectTaskInstanceRepository>();
builder.Services.AddScoped<IFeedbackTaskInstanceRepository, FeedbackTaskInstanceRepository>();

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentAccessLinkRepository, DocumentAccessLinkRepository>();

builder.Services.AddScoped<IWorkflowTemplateRepository, WorkflowTemplateRepository>();
builder.Services.AddScoped<IWorkflowTemplateNodeRepository, WorkflowTemplateNodeRepository>();
builder.Services.AddScoped<INodeTaskDependencyRepository, NodeTaskDependencyRepository>();

builder.Services.AddScoped<IWorkflowInstanceRepository, WorkflowInstanceRepository>();
builder.Services.AddScoped<IWorkflowNodeInstanceRepository, WorkflowNodeInstanceRepository>();

builder.Services.AddScoped<IOnboardingEmployeeDetailsRepository, OnboardingEmployeeDetailsRepository>();

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IReportedIssueRepository, ReportedIssueRepository>();

builder.Services.AddScoped<IWorkflowInstanceAuditLogRepository, WorkflowInstanceAuditLogRepository>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

builder.Services.AddScoped<IChecklistTaskTemplateHandler, ChecklistTaskTemplateHandler>();
builder.Services.AddScoped<IUploadDocumentTaskTemplateHandler, UploadDocumentTaskTemplateHandler>();
builder.Services.AddScoped<IReadDocumentTaskTemplateHandler, ReadDocumentTaskTemplateHandler>();
builder.Services.AddScoped<IProjectTaskTemplateHandler, ProjectTaskTemplateHandler>();
builder.Services.AddScoped<IFeedbackTaskTemplateHandler, FeedbackTaskTemplateHandler>();

builder.Services.AddScoped<ITaskTemplateHandlerFactory, TaskTemplateHandlerFactory>();

builder.Services.AddScoped<IChecklistTaskInstanceHandler, ChecklistTaskInstanceHandler>();
builder.Services.AddScoped<IReadDocumentTaskInstanceHandler, ReadDocumentTaskInstanceHandler>();
builder.Services.AddScoped<IUploadDocumentInstanceHandler, UploadDocumentInstanceHandler>();
builder.Services.AddScoped<IProjectTaskInstanceHandler, ProjectTaskInstanceHandler>();
builder.Services.AddScoped<IFeedbackTaskInstanceHandler, FeedbackTaskInstanceHandler>();

builder.Services.AddScoped<ITaskInstanceHandlerFactory, TaskInstanceHandlerFactory>();

builder.Services.AddScoped<IAccountUtility, OnboardingWFMSApi.BusinessLogic.AccountUtility>();


builder.Services.AddScoped<IOrganisationLogic, OrganisationLogic>();
builder.Services.AddScoped<IDepartmentLogic, DepartmentLogic>();
builder.Services.AddScoped<IAccountLogic, AccountLogic>();
builder.Services.AddScoped<IAuthLogic, AuthLogic>();
builder.Services.AddScoped<ITaskTemplateLogic, TaskTemplateLogic>();
builder.Services.AddScoped<ITaskInstanceLogic, TaskInstanceLogic>();
builder.Services.AddScoped<IDocumentLogic, DocumentLogic>();
builder.Services.AddScoped<IAnalyticsLogic, AnalyticsLogic>();

builder.Services.AddScoped<IWorkflowTemplateLogic, WorkflowTemplateLogic>();
builder.Services.AddScoped<IWorkflowInstanceLogic, WorkflowInstanceLogic>();

builder.Services.AddScoped<ICommentLogic, CommentLogic>();

builder.Services.AddScoped<IReportedIssuesLogic, ReportedIssuesLogic>();
builder.Services.AddScoped<IWorkflowInstanceAuditLogic, WorkflowInstanceAuditLogic>();

builder.Services.AddScoped<INotificationLogic, NotificationLogic>();
builder.Services.AddScoped<IFeedbackLogic, FeedbackLogic>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
// register mediatR and register all services from assemblies
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(TaskCompletedRequest).Assembly));
builder.Services.AddScoped<IRequestHandler<TaskCompletedRequest, HTTPResponse<string, string>>, TaskCompletionHandler>();
builder.Services.AddScoped<IRequestHandler<AccountRegistrationRequest, HTTPResponse<string, string>>, AccountRegistrationHandler>();
builder.Services.AddScoped<IRequestHandler<UploadDocumentRequest, HTTPResponse<DocumentDTO, string>>, UploadDocumentHandler>();
builder.Services.AddScoped<IRequestHandler<InviteAccountRequest, HTTPResponse<string, string>>, InviteAccountHandler>();
builder.Services.AddScoped<IRequestHandler<CreateWorkflowInstanceAuditLogRequest, ServerResponse<string, string>>, CreateWorkflowInstanceAuditLogHandler>();
builder.Services.AddScoped<IRequestHandler<RetrieveWorkflowInstanceRequest, ServerResponse<WorkflowInstanceDTO, string>>, RetrieveWorkflowInstanceHandler>();
builder.Services.AddScoped<IRequestHandler<CreateTaskInstanceRequest, ServerResponse<string, string>>, CreateTaskInstanceHandler>();
builder.Services.AddScoped<IRequestHandler<RetrieveAccountDirectoryRequest, AccountDirectoryDTO>, RetrieveAccountDirectoryHandler>();
builder.Services.AddScoped<IRequestHandler<RetrieveAccountsWorkflowInstancesRequest, List<WorkflowInstanceDTO>>, RetrieveAccountsWorkflowInstancesHandler>();

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
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SupervisorRoleClaim", policy =>
    {
        policy.Requirements.Add(new SupervisorUserRequirement());
    });
});

// register custom auth handlers
builder.Services.AddScoped<IAuthorizationHandler, SupervisorUserHandler>();

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