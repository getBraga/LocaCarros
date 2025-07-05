using LocaCarros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Infra.Data.EntitiesConfiguration
{
    public class MarcaConfiguration : IEntityTypeConfiguration<Marca>
    {
        public void Configure(EntityTypeBuilder<Marca> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Nome).IsRequired().HasMaxLength(100).IsRequired();
            builder.HasData(
             new { Id = 1, Nome = "Renault" },

              new { Id = 2, Nome = "Honda" },

              new { Id = 3, Nome = "Ford" },

              new { Id = 4, Nome = "Chevrolet" },

               new { Id = 5, Nome = "Volkswagen" }

            );




        }





    }
}
