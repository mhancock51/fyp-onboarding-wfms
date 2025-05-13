# Use the official .NET 8.0 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the solution file and restore dependencies
COPY ./OnboardingWFMSApi/*.sln ./
COPY ./OnboardingWFMSApi/OnboardingWFMSApi.BusinessLogic/OnboardingWFMSApi.BusinessLogic.csproj ./OnboardingWFMSApi.BusinessLogic/
COPY ./OnboardingWFMSApi/OnboardingWFMSApi.DataAccess/OnboardingWFMSApi.DataAccess.csproj ./OnboardingWFMSApi.DataAccess/
COPY ./OnboardingWFMSApi/OnboardingWFMSApi.DataModels/OnboardingWFMSApi.DataModels.csproj ./OnboardingWFMSApi.DataModels/
COPY ./OnboardingWFMSApi/OnboardingWFMSApi.Presentation/OnboardingWFMSApi.Presentation.csproj ./OnboardingWFMSApi.Presentation/

# Restore the dependencies
RUN dotnet restore OnboardingWFMSApi.sln

# Copy all the source files and build the solution
COPY ./OnboardingWFMSApi/. ./
RUN dotnet build OnboardingWFMSApi.sln -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish OnboardingWFMSApi.sln -c Release -o /app/publish

# Use the official .NET 8.0 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Copy the published application from the build container
COPY --from=publish /app/publish .

# Set the entry point to the application
ENTRYPOINT ["dotnet", "OnboardingWFMSApi.dll"]
