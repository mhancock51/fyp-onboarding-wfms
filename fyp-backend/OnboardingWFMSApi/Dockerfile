# Use the official .NET SDK image as the base image for building the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy the solution file
COPY OnboardingWFMSApi/OnboardingWFMSApi.sln . 

# Copy each .csproj file and restore as distinct layers
COPY OnboardingWFMSApi/OnboardingWFMSApi.BusinessLogic/OnboardingWFMSApi.BusinessLogic.csproj ./OnboardingWFMSApi.BusinessLogic/
COPY OnboardingWFMSApi/OnboardingWFMSApi.DataAccess/OnboardingWFMSApi.DataAccess.csproj ./OnboardingWFMSApi.DataAccess/
COPY OnboardingWFMSApi/OnboardingWFMSApi.DataModels/OnboardingWFMSApi.DataModels.csproj ./OnboardingWFMSApi.DataModels/
COPY OnboardingWFMSApi/OnboardingWFMSApi.Presentation/OnboardingWFMSApi.Presentation.csproj ./OnboardingWFMSApi.Presentation/

# Restore dependencies
RUN dotnet restore

# Copy everything else and build the app
COPY OnboardingWFMSApi/. ./OnboardingWFMSApi/
WORKDIR /source/OnboardingWFMSApi
RUN dotnet publish -c release -o /app --no-restore

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app ./ 
ENTRYPOINT ["dotnet", "OnboardingWFMSApi.dll"]  
