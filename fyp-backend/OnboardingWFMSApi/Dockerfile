# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the solution and project files
COPY ./OnboardingWFMSApi/*.sln ./
COPY ./OnboardingWFMSApi/OnboardingWFMSApi.BusinessLogic/OnboardingWFMSApi.BusinessLogic.csproj ./OnboardingWFMSApi.BusinessLogic/
COPY ./OnboardingWFMSApi/OnboardingWFMSApi.DataAccess/OnboardingWFMSApi.DataAccess.csproj ./OnboardingWFMSApi.DataAccess/
COPY ./OnboardingWFMSApi/OnboardingWFMSApi.DataModels/OnboardingWFMSApi.DataModels.csproj ./OnboardingWFMSApi.DataModels/
COPY ./OnboardingWFMSApi/OnboardingWFMSApi.Presentation/OnboardingWFMSApi.Presentation.csproj ./OnboardingWFMSApi.Presentation/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY ./OnboardingWFMSApi/. ./

# Build the solution
RUN dotnet build OnboardingWFMSApi.sln -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish ./OnboardingWFMSApi.Presentation/OnboardingWFMSApi.Presentation.csproj -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app
EXPOSE 80

# Copy published output
COPY --from=publish /app/publish .

# Set the entrypoint
ENTRYPOINT ["dotnet", "OnboardingWFMSApi.Presentation.dll"]
