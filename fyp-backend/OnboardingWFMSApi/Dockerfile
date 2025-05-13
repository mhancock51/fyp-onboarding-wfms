# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the solution file and restore dependencies
COPY fyp-backend/OnboardingWFMSApi/*.sln ./
COPY fyp-backend/OnboardingWFMSApi/OnboardingWFMSApi.BusinessLogic/OnboardingWFMSApi.BusinessLogic.csproj ./OnboardingWFMSApi.BusinessLogic/
COPY fyp-backend/OnboardingWFMSApi/OnboardingWFMSApi.DataAccess/OnboardingWFMSApi.DataAccess.csproj ./OnboardingWFMSApi.DataAccess/
COPY fyp-backend/OnboardingWFMSApi/OnboardingWFMSApi.DataModels/OnboardingWFMSApi.DataModels.csproj ./OnboardingWFMSApi.DataModels/
COPY fyp-backend/OnboardingWFMSApi/OnboardingWFMSApi.Presentation/OnboardingWFMSApi.Presentation.csproj ./OnboardingWFMSApi.Presentation/

# Restore the dependencies
RUN dotnet restore OnboardingWFMSApi.sln

# Copy all the source files and build the solution
COPY fyp-backend/OnboardingWFMSApi/. ./
RUN dotnet build OnboardingWFMSApi.sln -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish OnboardingWFMSApi.sln -c Release -o /app/publish

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Copy the published application from the build container
COPY --from=publish /app/publish .

# Set the entry point to the application
ENTRYPOINT ["dotnet", "OnboardingWFMSApi.dll"]
