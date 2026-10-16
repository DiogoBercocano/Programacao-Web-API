
using Acervo.Data;
using Acervo.DTOS.Validation;
using Acervo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var erros = context.ModelState
        .Where(x => x.Value?.Errors.Count > 0)
        .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors
                    .Select(e =>
                        string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? "Valor inválido."
                            : e.ErrorMessage)
                    .ToArray()
            );

        var resposta = new ErroResponseDTO
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Mensagem = "Um ou mais campos são inválidos.",
            Erros = erros,
        };

        return new BadRequestObjectResult(resposta);
    };
});









// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Adição do Contexto
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"
        )
    )
);


//Adiciona o Service ao Conteiner de Injeção
builder.Services.AddScoped<
    ILivroService, 
    LivroService>();

builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
