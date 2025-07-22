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
    public class CarroConfiguration : IEntityTypeConfiguration<Carro>
    {
        public void Configure(EntityTypeBuilder<Carro> builder)
        {
       
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Placa).IsRequired().HasMaxLength(10);
            builder.Property(c => c.Ano).IsRequired();
            builder.Property(c => c.Cor).IsRequired().HasMaxLength(30);
            builder.Property(c => c.DataFabricacao).IsRequired();
            builder.Property(c => c.Status).IsRequired();
            builder.HasOne(c => c.Modelo)
                   .WithMany()
                   .HasForeignKey(c => c.ModeloId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(c => c.Alugueis)
                   .WithOne(a => a.Carro)
                   .HasForeignKey(a => a.CarroId);
        }
    }
}
