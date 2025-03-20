var builder = WebApplication.CreateBuilder(args);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
        .WithMethods("GET", "POST", "DELETE", "PUT", "OPTIONS")// Permite HTTP y HTTPS
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Si usas autenticación (cookies, tokens, etc.)
    });
});


// Añadir controladores al contenedor de servicios
builder.Services.AddControllers();

// para el swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Usar CORS
app.UseCors("AllowReactApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usar controladores
app.MapControllers();

app.Run();
