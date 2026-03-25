# Step 1: Build the .NET application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FlexFit.csproj", "."]
RUN dotnet restore "FlexFit.csproj"
COPY . .
RUN dotnet publish "FlexFit.csproj" -c Release -o /app/publish

# Step 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Use environment variable to expose proper port
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

EXPOSE 8080

# Run the app DLL
CMD ["dotnet", "FlexFit.dll", "serve"]
