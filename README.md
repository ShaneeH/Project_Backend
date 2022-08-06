# Project_Backend
Backend for my Online Store Project
This API contains the HTTP Methods to allow communication and Data Manipulation between the FrontEnd and DataBase of my Project

# Pre Requirements
This Project runs of localhost so therefore CORS Policy must be disabled 

*Add this to the Program.cs to disable CORS

```CS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowOrigin",
        builder =>      
        {
            builder.WithOrigins(Local Host URL).AllowAnyHeader().AllowAnyMethod();});            
            });
 
```

* The Code in this API is configured to work for MySQL Databases only

* Please find the Insomnia Collection(Compatible with Postman) added to the Project for easier Testing



# NuGet
For this Program to run properly ensure you have these NuGet packages installed
![image](https://user-images.githubusercontent.com/22625921/181421370-a1024d8d-27d2-48e2-9776-d913ca33fb55.png)

