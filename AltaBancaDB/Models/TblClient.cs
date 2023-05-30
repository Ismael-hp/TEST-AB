using System;
using System.Collections.Generic;

namespace AltaBancaApi.AltaBancaDB.Models;

public partial class TblClient
{
    public int Id { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public DateTime FechaDeNacimiento { get; set; }

    public string? Cuit { get; set; }

    public string? Domicilio { get; set; }

    public string? Celular { get; set; }

    public string Email { get; set; } = null!;
}
